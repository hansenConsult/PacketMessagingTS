﻿using System;

using Windows.UI.Xaml.Data;

namespace PacketMessagingTS.Helpers
{
    public sealed class AreaConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && (((string)value).ToLower() == "xscperm" || ((string)value).ToLower() == "xscevent"))
            {
                //((PacketMessage)value).Area = "B";
                //return value;
                return "B";
            }
            else if (value is null)
            {
                //((PacketMessage)value).Area = "";
                //return value;
                return "";
            }
            else
            {
                //return ((PacketMessage)value).Area;
                return value;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class ComboBoxItemConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language) => value as object;

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }

    public sealed class VisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (string.IsNullOrEmpty(value as string))
                return Windows.UI.Xaml.Visibility.Collapsed;
            else
                return Windows.UI.Xaml.Visibility.Visible;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public sealed class StringIntConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            return ((int)value).ToString();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (string.IsNullOrEmpty(value as string))
                return value;
            else
                return Convert.ToUInt32((string)value);
        }
    }
}
