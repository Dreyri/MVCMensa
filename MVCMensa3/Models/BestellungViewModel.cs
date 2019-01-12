using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DataModels;

namespace MVCMensa3.Models
{
    public class BestellungViewModel : IValidatableObject
    {
        public class BestellungView
        {
            public uint ID { get; set; }
            public string Name { get; set; }
            public decimal Preis { get; set; }
            [Range(0, 10)]
            public int Anzahl { get; set; }

            public BestellungView(uint id, string name, decimal preis, int anz)
            {
                ID = id;
                Name = name;
                Preis = preis;
                Anzahl = anz;
            }
        }

        public string Message { get; set; }
        public IEnumerable<BestellungView> Bestellungen { get; set; }
        public DateTime? AbholZeit { get; set; }

        public BestellungViewModel(IEnumerable<BestellungView> bestellungen, DateTime? abholZeit)
        {
            Message = "";
            if (bestellungen == null)
                Bestellungen = new List<BestellungView>();
            else
                Bestellungen = bestellungen;
            AbholZeit = abholZeit;
        }

        public static BestellungViewModel FromDict(MensaSession.Role rolle, Dictionary<int, int> idAnz)
        {
            // when the dictionary is null
            if (idAnz == null)
            {
                return new BestellungViewModel(new List<BestellungView>(), null);
            }

            using (var db = new EmensaDB())
            {
                var entries = from mahlzeiten in db.Mahlzeitens
                              join preise in db.Preises
                              on mahlzeiten.PreisID equals preise.ID
                              select new BestellungView(mahlzeiten.ID, mahlzeiten.Name, MensaSession.DeterminePreis(rolle, preise), (int) mahlzeiten.Vorrat);

                var entriesList = entries.ToList();

                // remove entries smaller than 1
                var filteredEntries = from unfilteredWarenkorb in entriesList
                                      join dict in idAnz on (int)unfilteredWarenkorb.ID equals dict.Key
                                      where (dict.Value >= 1)
                                      select new BestellungView(unfilteredWarenkorb.ID, unfilteredWarenkorb.Name, unfilteredWarenkorb.Preis, dict.Value);

                // no agreed upon abholzeit yet
                return new BestellungViewModel(filteredEntries.ToList(), null);
            }
        }

        public static BestellungViewModel FromCookie(Dictionary<string, Dictionary<int, int>> cookie = null, String user = null)
        {
            Dictionary<int, int> warenkorb = null;
            Dictionary<string, Dictionary<int, int>> currCookie = null;
            Models.MensaSession session = MensaSession.FromCookie(HttpContext.Current.Request.Cookies);

            if(cookie != null)
            {
                currCookie = cookie;
                if (currCookie.Keys.Contains(user))
                {
                    warenkorb = currCookie[user];
                }
            }
            else
            {
                if (HttpContext.Current.Request.Cookies.Get("warenkorb") != null)
                {
                    HttpCookie warenkorbCookie = HttpContext.Current.Request.Cookies.Get("warenkorb");
                    currCookie = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, int>>>(warenkorbCookie.Value);

                    if (currCookie.Keys.Contains(user))
                    {
                        warenkorb = currCookie[user];
                    }
                }
                else
                {
                    return null;
                }
            }

            if(warenkorb != null)
            {
                using (var db = new EmensaDB())
                {
                    var entries = from mahlzeiten in db.Mahlzeitens
                                  join preise in db.Preises
                                  on mahlzeiten.PreisID equals preise.ID
                                  select new BestellungView(mahlzeiten.ID, mahlzeiten.Name, session.DeterminePreis(preise), 0);

                    var entriesList = entries.ToList();

                    var filteredList = from unfilteredWarenkorb in entriesList
                                       join dict in warenkorb on (int)unfilteredWarenkorb.ID equals dict.Key
                                       where (dict.Value >= 1)
                                       select new BestellungView(unfilteredWarenkorb.ID, unfilteredWarenkorb.Name, unfilteredWarenkorb.Preis, dict.Value);

                    entriesList = filteredList.ToList();

                    return new BestellungViewModel(entriesList, null);
                }
            }

            return null;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            using (var db = new EmensaDB())
            {
                var entries = from bestellung in this.Bestellungen
                              join mahlzeit in db.Mahlzeitens
                              on bestellung.ID equals mahlzeit.ID
                              where bestellung.Anzahl > mahlzeit.Vorrat
                              select new ValidationResult(string.Format("Nur noch {0} {1} vorhanden.", mahlzeit.Vorrat, bestellung.Name));

                return entries;
            }
        }   
    }
}