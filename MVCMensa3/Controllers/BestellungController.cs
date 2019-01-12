using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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