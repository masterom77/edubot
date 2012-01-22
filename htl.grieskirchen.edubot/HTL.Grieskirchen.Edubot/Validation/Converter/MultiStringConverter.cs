using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace HTL.Grieskirchen.Edubot.Validation.Converter
{
    class MultiStringConverter : IMultiValueConverter
    {
        public const string UNSET_VALUE = "{DependencyProperty.UnsetValue}";

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            StringBuilder builder = new StringBuilder();
            foreach(object msg in values){
                if (msg.ToString() != "" && msg.ToString() != UNSET_VALUE)
                {
                    builder.AppendLine(msg.ToString());
                }
            }
            return builder.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
