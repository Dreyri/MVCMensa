using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCMensa3.Models;

namespace MVCMensa3.Controllers
{
    public class ProduktController : Controller
    {
        // GET: Produkt
        public ActionResult Index(int? kategorie, bool? verfuegbar, bool? vegan, bool? vegetarisch)
        {
            Kategorie selected = null;
            List<Mahlzeit> mahlzeiten = null;

            if (kategorie.HasValue)
            {
                selected = Kategorie.GetKategorie(kategorie.Value);

                if (kategorie.Value == 0)
                {
                    mahlzeiten = Mahlzeit.GetMahlzeitenList(8);
                }
                else if (selected == null)
                {
                    return RedirectToAction("Index", "Produkt", null);
                }

                mahlzeiten = Mahlzeit.GetMahlzeitenWithFilter(selected == null ? 0 : selected.ID, vegan == null ? false : vegan.Value, vegetarisch == null ? false : vegetarisch.Value, verfuegbar == null ? false : verfuegbar.Value, 8);
                
            }
            else
            {
                mahlzeiten = Mahlzeit.GetMahlzeitenList(8);
            }

            var kategorien = Kategorie.GetKategorieList();

            var mahlzeitbilder = new List<KeyValuePair<Mahlzeit, Bild>>();

            foreach (var mzeit in mahlzeiten)
            {
                var bild = Bild.GetFirstBildForMahlzeit(mzeit.ID);

                mahlzeitbilder.Add(new KeyValuePair<Mahlzeit, Bild>(mzeit, bild));
            }

            return View(Tuple.Create(mahlzeitbilder, kategorien, selected));
        }

        public ActionResult Detail(int id)
        {
            var mahlzeit = Mahlzeit.GetMahlzeit(id);

            if (mahlzeit == null)
            {
                return RedirectToAction("Index", "Produkt");
            }

            var zutaten = Zutat.GetZutatenListForMahlzeit(mahlzeit.ID);
            var bild = Bild.GetFirstBildForMahlzeit(id);
            var preis = Preis.GetPreis(mahlzeit.PreisId);
            var session = new MensaSession(Request);

            decimal realPreis = preis.GastPreis;

            if (session != null)
            {
                realPreis = preis.GetForRole(session.Rolle);
            }

            return View(new Detail(mahlzeit, zutaten, bild, realPreis, session));
        }

        public ActionResult Zutaten()
        {
            ViewBag.Title = "Zutaten";
            var zutaten = Zutat.GetSortedZutaten();
            return View(zutaten);
        }
    }
}