using System;

using Windows.UI.Xaml.Data;

namespace PacketMessagingTS.Helpers
{
    public sealed class DateConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null || (DateTime)value == DateTime.MinValue)
            {
                return "";
            }
            DateTime dateTime = (DateTime)value;
            string date = $"{dateTime.Month:d2}/{dateTime.Day:d2}/{dateTime.Year:d4}";
            return date;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
