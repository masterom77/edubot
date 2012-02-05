using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace HTL.Grieskirchen.Edubot.Validation
{
    public class LengthValidationRule : ValidationRule
    {

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            float result;
            if (value == null)
                return new ValidationResult(false, "Bitte geben Sie die Achsenlänge ein");
            if (!float.TryParse(value.ToString(), out result))
            {
                return new ValidationResult(false, "Die Länge muss eine Zahl sein");
            }
            if (result <= 0) {
                return new ValidationResult(false, "Die Länge muss größer als 0 sein");
            }
            return new ValidationResult(true, null);
        }
    }
}
