using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Cinema.Converters
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            try
            {
                return ((bool)value) ? Brushes.GreenYellow : Brushes.White;
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
