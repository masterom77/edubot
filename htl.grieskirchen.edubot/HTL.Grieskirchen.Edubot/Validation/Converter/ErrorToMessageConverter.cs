using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace HTL.Grieskirchen.Edubot.Validation.Converter
{
    class ErrorToMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StringBuilder builder = new StringBuilder();
            ReadOnlyCollection<ValidationError> errors = value as ReadOnlyCollection<ValidationError>;
          
                //if (errors != null)
            //{
            //    foreach (ValidationError error in errors.Where(e => e.ErrorContent != null))
            //    { builder.AppendLine(error.ErrorContent.ToString()); }
            //}

            return value == null ? String.Empty : value.ToString();//builder.ToString();
        }
    

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
