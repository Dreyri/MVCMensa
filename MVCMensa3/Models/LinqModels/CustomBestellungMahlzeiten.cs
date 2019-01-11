using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCMensa3.Models.LinqModels
{
    public class CustomBestellungMahlzeiten
    {
        public int Anzahl { get; set; }
        public string Kategorie { get; set; }
        public  string Name { get; set; }
        public int Vorrat { get; set; }
    }
}