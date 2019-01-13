using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinqToDB;
using System.Web.Mvc;
using MVCMensa3.Models.LinqModels;
using System.Net;

namespace MVCMensa3.Controllers
{
    public class DispatchController : Controller
    {
        // GET: Dispatch
        public JsonResult Bestellungen()
        {
            string token = "";
            var tokens = Request.Headers.GetValues("X-Authorize");

            if (tokens != null)
            {
                token = tokens[0];
            }

            if (token != "besttoken")
            {
                Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                Response.SuppressFormsAuthenticationRedirect = true;
                return Json(new { error = "Unauthorized" }, JsonRequestBehavior.AllowGet);
            }

            using (var db = new EmensaDB())
            {
                var bestellungen_qry = from user in db.Benutzers
                                       join orders in db.Bestellungens
                                       on user.Nummer equals orders.BenutzerID
                                       where orders.AbholZeitpunkt <= DateTime.Now.AddHours(1)
                                       select new
                                       {
                                           User = new
                                           {
                                               Vorname = user.Vorname,
                                               Nachname = user.Nachname,
                                               Nutzername = user.Nutzername,
                                               EMail = user.EMail
                                           },
                                           Abholung = (orders.AbholZeitpunkt.ToLocalTime()).ToString(),
                                           Bestellnummer = orders.Nummer,
                                           Meals = new List<CustomBestellungMahlzeiten>()
                                       };

                var meal_qry = from meals in db.Mahlzeitens
                               join bestellungen in db.Mahlzeitenbestellungens
                               on meals.ID equals bestellungen.MahlzeitID
                               select new
                               {
                                   Anzahl = bestellungen.Anzahl,
                                   Kategorie = meals.Kategorie.Bezeichnung,
                                   Name = meals.Name,
                                   Vorrat = meals.Vorrat,
                                   BestellungID = bestellungen.BestellungID
                               };

                var receivedOrders = bestellungen_qry.ToList();
                var receivedMeals = meal_qry.ToList();
                foreach (var order in receivedOrders)
                {
                    foreach (var meal in receivedMeals)
                    {
                        if (order.Bestellnummer == meal.BestellungID)
                        {
                            order.Meals.Add(new CustomBestellungMahlzeiten
                            {
                                Anzahl = (int)meal.Anzahl,
                                Kategorie = meal.Kategorie,
                                Name = meal.Name,
                                Vorrat = (int)meal.Vorrat
                            });
                        }
                    }
                }
                return Json(receivedOrders, JsonRequestBehavior.AllowGet);
            }

        }
    }
}