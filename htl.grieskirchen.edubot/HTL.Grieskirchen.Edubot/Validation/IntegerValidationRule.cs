using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace HTL.Grieskirchen.Edubot.Validation
{
    public class IntegerValidationRule : ValidationRule
    {
        string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int result;
            if (value == null)
                return new ValidationResult(false, errorMessage);
            if (int.TryParse(value.ToString(), out result))
            {
                return new ValidationResult(true, null);
            }
            return new ValidationResult(false, errorMessage);
        }
    }
}
