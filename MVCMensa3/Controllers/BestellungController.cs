using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCMensa3.Controllers
{
    public class BestellungController : Controller
    {
        public ActionResult Index()
        {
            Models.Warenkorb.DoViewBag(ViewBag);

            var session = Models.MensaSession.FromContext();
            var warenkorb = Models.Warenkorb.FromContext();

            if (session == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var model = Models.BestellungViewModel.FromWarenkorb(session, warenkorb);

            return View(model);
        }

        // called with aendern
        [HttpPost]
        public ActionResult Index(Models.BestellungViewModel model)
        {
            if (ModelState.IsValid)
            {
                var korb = Models.Warenkorb.FromContext();
                korb.MergeViewModel(Models.MensaSession.FromContext(), model);
                korb.Commit();
            }
            return View(model);
            // nothing needs to be commited
        }

        [HttpPost]
        public ActionResult Complete(Models.BestellungViewModel model)
        {
            // if the state is valid we can submit to the database
            if (ModelState.IsValid)
            {
                Models.MensaSession session = Models.MensaSession.FromContext();
                var korb = Models.Warenkorb.FromContext();
                korb.MergeViewModel(Models.MensaSession.FromContext(), model);
                korb.Commit();

                // do our db transaction
                using (var db = new DataModels.EmensaDB())
                {
                    var nutzer = (from benutzer in db.Benutzers
                                  where benutzer.Nutzername == session.User
                                  select new
                                  {
                                      benutzer.Nutzername,
                                      ID = benutzer.Nummer
                                  }).First();

                    DataModels.Bestellungen bestellung = new DataModels.Bestellungen
                    {
                        AbholZeitpunkt = DateTime.ParseExact(model.AbholZeit, "t", CultureInfo.CurrentCulture),
                        BenutzerID = nutzer.ID,
                        Endpreis = model.TotalPreis(),
                        BestellZeitpunkt = DateTime.Now
                    };

                    try
                    {
                        var transaction = db.BeginTransaction();


                        bestellung.Nummer = (uint) db.InsertWithInt32Identity(bestellung);

                        foreach (var entry in model.Bestellungen)
                        {
                            DataModels.Mahlzeitenbestellungen mzBest = new DataModels.Mahlzeitenbestellungen
                            {
                                Anzahl = (uint)entry.Anzahl,
                                BestellungID = bestellung.Nummer,
                                MahlzeitID = entry.ID,
                            };

                            db.Insert(mzBest);
                        }

                        transaction.Commit();

                        // remove the order on success
                        korb.Dict.Remove(session.User);
                        korb.Commit();
                    }
                    catch (Exception e)
                    {
                        string message = string.Format("Fehler bei der Bestellung: {0}", e.Message);
                        return PartialView("ErrorBestellung", message);
                    }

                    return View();
                }
            }
            else
            {
                // back to action if the model is invalid
                return RedirectToAction("Index");
            }
        }

        public ActionResult Bestel(int? mahlzeitId)
        {
            var session = Models.MensaSession.FromContext();

            // fail conditions
            if (session == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (!mahlzeitId.HasValue)
            {
                return RedirectToAction("Index", "Produkt");
            }

            var warenkorb = Models.Warenkorb.FromContext();

            // add the mahlzeit to the bestellungscookie
            warenkorb.AddMahlzeit(mahlzeitId.Value, session.User);
            warenkorb.Commit();

            return RedirectToAction("Index");
        }

        public ActionResult Loeschen()
        {
            var session = Models.MensaSession.FromCookie(Request.Cookies);
            var warenkorbCookie = HttpContext.Request.Cookies.Get("warenkorb");

            if (warenkorbCookie != null && session != null)
            {
                Dictionary<string, Dictionary<int, int>> warenkorb = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, int>>>(warenkorbCookie.Value);
                warenkorb.Remove(session.User);
                warenkorbCookie.Value = JsonConvert.SerializeObject(warenkorb);
            }

            warenkorbCookie.Expires = DateTime.Now.AddHours(6);
            HttpContext.Response.Cookies.Set(warenkorbCookie);

            return RedirectToAction("Index");
        }


    }
}