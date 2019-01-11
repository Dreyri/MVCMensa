using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels;

namespace MVCMensa3.Models.LinqModels
{ 
    public class BestellungModel
    {
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string Nutzername { get; set; }

        public string EMail { get; set; }

        public List<Mahlzeitenbestellungen> Mahlzeiten { get; set; }
    }
}