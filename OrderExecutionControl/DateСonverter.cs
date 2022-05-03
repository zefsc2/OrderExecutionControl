using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;

namespace OrderExecutionControl
{

    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateConverter : IValueConverter
    {
        DateTime date;
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                date = (DateTime)value;
                return date.ToString("yyyy.MM.dd HH:mm");
            }
            else if (value == null)
                return new DateTime().ToString("yyyy.MM.dd HH:mm");
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime result;
            if (DateTime.TryParseExact((string)value, "yyyy.MM.dd HH:mm", null, DateTimeStyles.AllowLeadingWhite, out result))
                return result;
            else
                return value;
        }
    }
}
