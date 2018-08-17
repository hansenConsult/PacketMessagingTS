using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace PacketMessagingTS.Helpers
{
    public class UpdatePacFormsFiles
    {
        private static async Task<DateTimeOffset> GetFileModifiedTimeAsync(StorageFile file)
        {
            List<string> propertiesName = new List<string>();
            propertiesName.Add("System.DateModified");

            // Get the specified properties through StorageFile.Properties
            IDictionary<string, object> extraProperties = await file.Properties.RetrievePropertiesAsync(propertiesName);
            var propValue = extraProperties["System.DateModified"];
            if (propValue != null)
            {
                string s = "Date accessed: " + propValue;
            }
            return (DateTimeOffset)propValue;
        }

        private static async Task SyncFilesAsync(string assetsFolder, string localFolder)
        {
            StorageFolder folder = await Package.Current.InstalledLocation.GetFolderAsync(assetsFolder);
            IReadOnlyList<StorageFile> pacForms = await folder.GetFilesAsync();
            foreach (StorageFile file in pacForms)
            {
                StorageFolder localPacFormsFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(localFolder);
                IReadOnlyList<StorageFile> localPacForms = await localPacFormsFolder.GetFilesAsync();

                foreach (StorageFile localFile in localPacForms)
                {
                    IStorageItem fileInlocalFolder = await localPacFormsFolder.TryGetItemAsync(localFile.Name);
                    if (fileInlocalFolder == null)
                    {
                        await file.CopyAsync(localPacFormsFolder, file.Name, NameCollisionOption.ReplaceExisting);
                        break;
                    }
                    else if (file.Name == localFile.Name)
                    {
                        DateTimeOffset fileModifiedTime = await GetFileModifiedTimeAsync(file);
                        DateTimeOffset localFileModifiedTime = await GetFileModifiedTimeAsync(localFile);
                        if (localFileModifiedTime < fileModifiedTime)
                        {
                            await file.CopyAsync(localPacFormsFolder, file.Name, NameCollisionOption.ReplaceExisting);
                        }
                        break;
                    }
                }
            }
        }

        public static async Task SyncPacFormFoldersAsync()
        {
            await SyncFilesAsync("Assets\\Pacforms", "Pacforms");

            await SyncFilesAsync("Assets\\Pacforms\\js", "Pacforms\\js");
        }
    }
}
