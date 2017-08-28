using System;
using System.Windows.Data;

namespace Cinema.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class StringToTimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            try
            {
                string[] time = (value as string).Split(':');
                return string.Format("Начало показа: {0}:{1}", time[0], time[1]);
            }
            catch (InvalidCastException invalidCastException)
            {
                throw new InvalidCastException(invalidCastException.Message);
            }
            catch (NullReferenceException nullReferenceException)
            {
                throw new NullReferenceException(nullReferenceException.Message);
            }
            catch (IndexOutOfRangeException indexOutOfRangeException)
            {
                throw new IndexOutOfRangeException(indexOutOfRangeException.Message);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
