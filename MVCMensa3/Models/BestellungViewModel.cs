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

            public BestellungView()
            {
                ID = 0;
                Name = "Unbekannt";
                Anzahl = 0;
            }

            public BestellungView(uint id, string name, decimal preis, int anz)
            {
                ID = id;
                Name = name;
                Preis = preis;
                Anzahl = anz;
            }
        }

        public string Message { get; set; }
        public List<BestellungView> Bestellungen { get; set; }
        public DateTime? AbholZeit { get; set; }

        public BestellungViewModel()
            : this(null, null)
        { }

        public BestellungViewModel(List<BestellungView> bestellungen = null, DateTime? abholZeit = null)
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

        public static BestellungViewModel FromWarenkorb(MensaSession session, Warenkorb korb)
        {
            if (session == null || korb == null || !korb.Dict.ContainsKey(session.User))
            {
                return new BestellungViewModel();
            }

            return FromDict(session.Rolle, korb.Dict[session.User]);
        }

        public static BestellungViewModel FromCookie(Dictionary<string, Dictionary<int, int>> cookie = null, string user = null)
        {
            Dictionary<int, int> warenkorb = null;
            Dictionary<string, Dictionary<int, int>> currCookie = null;
            MensaSession session = MensaSession.FromCookie(HttpContext.Current.Request.Cookies);

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

            return FromDict(session.Rolle, warenkorb);
        }

        public decimal TotalPreis()
        {
            decimal preis = 0m;

            foreach(var bestellung in Bestellungen)
            {
                preis += (bestellung.Preis * bestellung.Anzahl);
            }

            return preis;
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

                return entries.ToList();
            }
        }   

        public static List<string> GenerateTimes()
        {
            //do roundup
            DateTime inHalfHour = DateTime.Now.AddMinutes(30);
            TimeSpan intervals = TimeSpan.FromMinutes(15);
            var modTicks = inHalfHour.Ticks % intervals.Ticks;
            var delta = modTicks != 0 ? intervals.Ticks - modTicks : 0;
            DateTime time = new DateTime(inHalfHour.Ticks + delta, inHalfHour.Kind);

            var timeList = new List<string>();

            for(int i = 0; i < 12; ++i)
            {
                timeList.Add(time.ToString("t"));
                time = time.AddMinutes(15);
            }

            return timeList;
        }
    }
}