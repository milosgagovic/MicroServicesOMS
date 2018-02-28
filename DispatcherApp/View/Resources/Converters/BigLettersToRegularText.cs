using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DispatcherApp.View.Resources.Converters
{
    public class BigLettersToRegularText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = string.Empty;

            string[] parts = value.ToString().Split('_');

            foreach (string part in parts)
            {
                text += part[0] + part.Substring(1).ToLower() + " ";
            }

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
