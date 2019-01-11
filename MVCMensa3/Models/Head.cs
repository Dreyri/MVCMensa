using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCMensa3.Models
{
    public class Head
    {
        public int? AnzahlBestellungen { get; set; }

        public Head()
        {
            AnzahlBestellungen = null;
        }

        public Head(int? anzBestellungen)
        {
            AnzahlBestellungen = anzBestellungen;
        }
    }
}