using System.Collections.Generic;

namespace PacketMessagingTS.Helpers
{
    public class ComportComparer : IComparer<string>
    {
        int IComparer<string>.Compare(string x, string y)
        {
            // Only compare the port number
            string comString = "COM";
            string x1 = x.Substring(comString.Length);
            string y1 = y.Substring(comString.Length);

            if (x is null)
            {
                if (y is null)
                {
                    // If x is null and y is null, they're
                    // equal. 
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y
                    // is greater. 
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                if (y is null)
                // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {
                    // ...and y is not null, compare the 
                    // lengths of the two strings.
                    int retval = x1.Length.CompareTo(y1.Length);

                    if (retval != 0)
                    {
                        // If the strings are not of equal length,
                        // the longer string is greater.
                        return retval;
                    }
                    else
                    {
                        // If the strings are of equal length,
                        // sort them with ordinary string comparison.
                        return x1.CompareTo(y1);
                    }
                }
            }
        }
    }
}
