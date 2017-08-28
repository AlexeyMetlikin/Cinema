using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cinema.Converters
{
    [ValueConversion(typeof(int), typeof(bool))]
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
           System.Globalization.CultureInfo culture)
        {
            try
            {
                return ((int)value) > 0 ? true : false;
            }
            catch (FormatException formatException)
            {
                throw new FormatException(formatException.Message);
            }
            catch (InvalidCastException invalidCastException)
            {
                throw new InvalidCastException(invalidCastException.Message);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
