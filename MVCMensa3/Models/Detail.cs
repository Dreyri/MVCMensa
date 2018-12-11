using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCMensa3.Models
{
    public class Detail
    {
        public Mahlzeit Mahlzeit { get; set; }
        public List<Zutat> Zutaten { get; set; }
        public Bild Bild { get; set; }
        public decimal Preis { get; set; }
        public MensaSession Session { get; set; }


        public Detail(Mahlzeit mzeit, List<Zutat> zutaten, Bild b, decimal p, MensaSession s)
        {
            Mahlzeit = mzeit;
            Zutaten = zutaten;
            Bild = b;
            Preis = p;
            Session = s;
        }
    }
}