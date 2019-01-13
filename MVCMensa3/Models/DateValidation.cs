using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MVCMensa3.Models
{
    public class DateValidation : ValidationAttribute
    {
        public string Format { get; set; }

        public DateValidation()
        {
            Format = "t";
        }

        public DateValidation(string format)
        {
            Format = format;
        }

        public override bool IsValid(object value)
        {
            var dateString = value as string;
            CultureInfo provider = CultureInfo.InvariantCulture;
            try
            {
                var parsedDate = DateTime.ParseExact(dateString, Format, provider);
                return parsedDate < DateTime.Now.AddMinutes(210);
            }
            catch
            {
                return false;
            }
        }
    }
}