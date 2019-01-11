using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCMensa3.Models
{
    public class BestellungViewModel
    {
        public class BestellungView
        {
            public int ID { get; set; }
            public String Name { get; set; }
            public decimal Preis { get; set; }
            [Range(0, 10)]
            public int Anzahl { get; set; }

            public BestellungView(int id, String name, decimal preis, int anz)
            {
                ID = id;
                Name = name;
                Preis = preis;
                Anzahl = anz;
            }
        }

        public String Message { get; set; }
        public IEnumerable<BestellungView> Bestellungen { get; set; }
        public DateTime AbholZeit { get; set; }

        public BestellungViewModel(IEnumerable<BestellungView> bestellungen, DateTime abholZeit)
        {
            Message = "";
            Bestellungen = bestellungen;
            AbholZeit = abholZeit;
        }

        //public static BestellungViewModel FromCookie()
    }
}