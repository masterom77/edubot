using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace HTL.Grieskirchen.Edubot.Validation
{
    public class PortValidationRule : ValidationRule
    {

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
       {
            int result;
            if (value == null || value.ToString() == String.Empty)
                return new ValidationResult(false, "Bitte geben Sie einen Port ein");
            if (!int.TryParse(value.ToString(), out result))
            {
                return new ValidationResult(false, "Port muss eine Zahl sein");
            }
            if(!(result >= 0 &&  result <= 65535))
                return new ValidationResult(false, "Port muss eine Zahl zwischen 0 und 65535 sein");
            return new ValidationResult(true, null);
           
        }
    }
}
