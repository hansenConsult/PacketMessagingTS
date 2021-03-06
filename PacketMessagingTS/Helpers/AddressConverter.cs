﻿using System;

using Windows.UI.Xaml.Data;

namespace PacketMessagingTS.Helpers
{
    public sealed class AddressConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            parameter = value as bool?;
            int length = ((string)value).Length;
            if (length > 0)
                return Windows.UI.Xaml.Visibility.Visible;
            else
                return Windows.UI.Xaml.Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
