using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace HTL.Grieskirchen.Edubot.Validation
{
    public class RelationValidationRule : ValidationRule
    {

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            float result;
            if (value == null)
                return new ValidationResult(false, "Bitte geben Sie eine Relation ein");
            if (!float.TryParse(value.ToString(), out result))
            {
                return new ValidationResult(false, "Die Relation muss eine Zahl sein");
            }
            if (result <= 0) {
                return new ValidationResult(false, "Die Relation muss größer als 0 sein");
            }
            if (result > 10)
            {
                return new ValidationResult(false, "Die Relation muss kleiner gleich 10 sein");
            }
            return new ValidationResult(true, null);
        }
    }
}
