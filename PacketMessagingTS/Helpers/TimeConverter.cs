using System;

using Windows.UI.Xaml.Data;

namespace PacketMessagingTS.Helpers
{
    public sealed class TimeConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is null)
			{
				return "";
			}
			DateTime dateTime = (DateTime)value;
			string date = $"{dateTime.Hour:d2}{dateTime.Minute:d2}";
			return date;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
