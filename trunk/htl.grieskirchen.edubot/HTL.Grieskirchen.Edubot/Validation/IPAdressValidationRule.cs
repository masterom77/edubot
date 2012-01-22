using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Net;

namespace HTL.Grieskirchen.Edubot.Validation
{
    public class IPAdressValidationRule : ValidationRule
    {
        

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if(value == null || value.ToString() == String.Empty)
                return new ValidationResult(false, "Bitte geben Sie eine IP-Adresse ein");

            string input = value.ToString();
            string[] octets = input.Split('.');
            int result;
            if (octets.Length != 4) {
                return new ValidationResult(false, "Die IP-Adresse muss aus 4 Oktets getrennt durch einen \".\" bestehen");
            }
            foreach (string octet in octets) { 
                if(!int.TryParse(octet, out result))
                    return new ValidationResult(false, "Jedes Oktet muss eine Zahl sein");
                if (!(result >= 0 && result <= 255)) {
                    return new ValidationResult(false, "Jedes Oktet muss eine Zahl zwischen 0 und 255 sein");
                }

            }
            
            return new ValidationResult(true, String.Empty);
        }
    }
}
