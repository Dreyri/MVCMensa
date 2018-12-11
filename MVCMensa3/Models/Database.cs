using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace MVCMensa3.Models
{
    public class Database
    {
        public static string ConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EMensaWeb"].ConnectionString;
        }
    }
}