using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace HTL.Grieskirchen.Edubot.Validation
{
    class StringEmptyValidationRule : ValidationRule
    {
        string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string input = (string)value;
            if (input == String.Empty)
                return new ValidationResult(false, errorMessage);
            return new ValidationResult(true, null);
        }
    }
}
