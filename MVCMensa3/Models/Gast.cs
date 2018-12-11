using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCMensa3.Models
{
    public class Gast : Benutzer
    {
        [Required(ErrorMessage = "Grund ist noetig")]
        [StringLength(255)]
        string Grund { get; set; }
        
        public Gast(int num, string nutzer, string hash, string salt, string vorname, string nachname, string grund)
          : base(num, nutzer, hash, salt, vorname, nachname)
        {
            Grund = grund;
        }
    }
}