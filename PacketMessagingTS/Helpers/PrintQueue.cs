using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MetroLog;
using PacketMessagingTS.Services;
using PacketMessagingTS.Views;
using SharedCode;
using Windows.ApplicationModel.Background;
using Windows.Data.Html;
using Windows.Storage;

namespace PacketMessagingTS.Helpers.PrintHelpers
{
    public class PrintQueue
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<PrintQueue>();
        private static LogHelper _logHelper = new LogHelper(log);

        Dictionary<string, object> _properties = App.Properties;

        private const string SettingsKey = "PrintDestinations";

        private Dictionary<string, string[]> _printQueue = new Dictionary<string, string[]>();
        public Dictionary<string, string[]> PrintQueueDict => _printQueue;

        private ApplicationTrigger _backgroundPrintingTrigger;
        public ApplicationTrigger BackgroundPrintingTrigger
        {
            get
            {
                if (_backgroundPrintingTrigger == null)
                {
                    _backgroundPrintingTrigger = new ApplicationTrigger();
                }
                return _backgroundPrintingTrigger;
            }
        }

        public void AddToPrintQueue(string fileName, string[] destinations)
        {
            _printQueue.Add(fileName, destinations);
            SavePrintQueue();
        }

        public async Task RemoveFromPrintQueueAsync(string fileName)
        {
            _printQueue.Remove(fileName);
            SavePrintQueue();
            StorageFolder folder = SharedData.PrintMessagesFolder;
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            if (file == null)
            {
                _logHelper.Log(LogLevel.Error, $"File not found: {fileName}");
            }
            await file?.DeleteAsync();
        }

        public void SavePrintQueue()
        {
            _properties[SettingsKey] = _printQueue;
            //await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, _printQueue);
        }

        public void RestorePrintQueue()
        {
            _printQueue = Utilities.GetProperty<Dictionary<string, string[]>>(SettingsKey);
        }

        private void PrintCopy(string file, string destination)
        {
            _logHelper.Log(LogLevel.Info, $"print {file}, to {destination}");
            //ViewLifetimeControl viewLifetimeControl = await WindowManagerService.Current.TryShowAsStandaloneAsync("Print Message", typeof(PrintMessagePage));
            string folder = SharedData.PrintMessagesFolder.Path;
            string packetMessagePath = Path.Combine(folder, file);
            NavigationService.Navigate(typeof(PrintMsgTestPage), packetMessagePath);
        }

        public async Task PrintToDestinationsAsync()
        {
            List<string> processedFiles = new List<string>();

            RestorePrintQueue();
            foreach (string file in _printQueue.Keys)
            {
                for (int i = 0; i < _printQueue[file].Length; i++)
                {
                    PrintCopy(file, _printQueue[file][i]);
                }

                processedFiles.Add(file);
            }
            foreach (string file in processedFiles)
            {
                await RemoveFromPrintQueueAsync(file);
            }
            SavePrintQueue();
        }
    }
}
