using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml.Data;

namespace SharedCode
{
    public sealed class DateTimeConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
            {
                return "";
            }
            DateTime dateTime = (DateTime)value;
            string date = $"{dateTime.Month:d2}/{dateTime.Day:d2}/{dateTime.Year - 2000:d2} {dateTime.Hour:d2}:{dateTime.Minute:d2}";
            return date;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();

            //bool success = DateTime.TryParse((string)value, out DateTime dateTime);
            //return success ? dateTime : (DateTime?)null;
        }
    }
}
