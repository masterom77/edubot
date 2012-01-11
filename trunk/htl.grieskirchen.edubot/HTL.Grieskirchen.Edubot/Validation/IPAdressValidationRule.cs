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
        string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string input = value.ToString();
            IPAddress result;
            
            if (input.Split('.').Count() == 4 &&IPAddress.TryParse(input.ToString(), out result))
            {
                return new ValidationResult(true, null);
            }
            return new ValidationResult(false, errorMessage);
        }
    }
}
