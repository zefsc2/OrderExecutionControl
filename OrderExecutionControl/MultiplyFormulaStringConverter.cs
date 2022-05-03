using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;

namespace OrderExecutionControl
{
    class MultiplyFormulaStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValues = values.Cast<System.DateTime>().ToArray();
            System.DateTime date1 = doubleValues[0];
            System.DateTime date2 = doubleValues[1];
            var number = System.Convert.ToInt32((date1 - date2).TotalDays);
            string result;
            if (number < 0)
            { 
                result = "просрочено на ";
                number = Math.Abs(number);
            }
            else
                result = "осталось ";
            number = number % 100;
            if (number >= 11 && number <= 19)
            {
                result += string.Format("{0} дней", number);
            }
            else
            {
                int i = number % 10;

                switch (i)
                {
                    case 1:
                        result += string.Format("{0} день", number);
                        break;
                    case 2:
                    case 3:
                    case 4:
                        result += string.Format("{0} дня", number);
                        break;
                    default:
                        result += string.Format("{0} дней", number);
                        break;
                }
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
