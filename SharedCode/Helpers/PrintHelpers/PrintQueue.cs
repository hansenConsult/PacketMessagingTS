using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments.AppointmentsProvider;

namespace SharedCode.Helpers.PrintHelpers
{
    public class PrintQueue
    {
        Dictionary<string, string[]> _printQueue = new Dictionary<string, string[]>();


        public void AddToPrintQueue(string fileName, string[] destinations)
        {
            _printQueue.Add(fileName, destinations);
        }

        public (string fileName, string[] destinations) RemoveFromPrintQueue()
        {
            string fileName = _printQueue.Keys.FirstOrDefault();
            string[] destinations = _printQueue[fileName];

            _printQueue.Remove(fileName);

            return (fileName, destinations);
        }
    }
}
