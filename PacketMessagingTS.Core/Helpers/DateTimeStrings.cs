using System;
using System.Collections.Generic;
using System.Text;

namespace PacketMessagingTS.Core.Helpers;

public static class DateTimeStrings
{
    public static string DateString(DateTime dateTime)
    {
        return $"{dateTime.Month:d2}/{dateTime.Day:d2}/{dateTime.Year:d4}";
    }

    public static string TimeString(DateTime dateTime)
    {
        return $"{dateTime.Hour:d2}:{dateTime.Minute:d2}";
    }

    public static string DateTimeString(DateTime dateTime)
    {
        return $"{DateString(dateTime)} {TimeString(dateTime)}";
    }

    public static string DateTimeStringShortYear(DateTime dateTime)
    {
        return $"{dateTime.Month:d2}/{dateTime.Day:d2}/{dateTime.Year - 2000:d2} {TimeString(dateTime)}";
    }

}
