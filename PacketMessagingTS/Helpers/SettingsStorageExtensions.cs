﻿using System;
using System.IO;
using System.Threading.Tasks;

using MetroLog;

using PacketMessagingTS.Core.Helpers;
using SharedCode;
using Windows.Storage;
using Windows.Storage.Streams;

namespace PacketMessagingTS.Helpers
{
    // Use these extension methods to store and retrieve local and roaming app data
    // More details regarding storing and retrieving app data at https://docs.microsoft.com/windows/uwp/app-settings/store-and-retrieve-app-data
    public static class SettingsStorageExtensions
    {
        //private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<SettingsStorageExtensions>();
        //private static readonly LogHelper _logHelper = new LogHelper(log);

        private const string FileExtension = ".json";

        public static bool IsRoamingStorageAvailable(this ApplicationData appData)
        {
            return appData.RoamingStorageQuota == 0;
        }

        public static async Task SaveAsync<T>(this StorageFolder folder, string name, T content)
        {
            try
            {
                StorageFile file = await folder.CreateFileAsync(GetFileName(name), CreationCollisionOption.ReplaceExisting);
                string fileContent = await Json.StringifyAsync(content);

                await FileIO.WriteTextAsync(file, fileContent);
                //_logHelper.Log(LogLevel.Info, $"Saved file, size {fileContent.Length} bytes, File Name {name}");
            }
            catch 
            {
                //_logHelper.Log(LogLevel.Error, $"Error saving {name}. {e.Message}");
            }
            finally
            {

            }
        }

        public static async Task<T> ReadAsync<T>(this StorageFolder folder, string name)
        {
            if (!File.Exists(Path.Combine(folder.Path, GetFileName(name))))
            {
                return default(T);
            }

            var file = await folder.GetFileAsync($"{name}.json");
            var fileContent = await FileIO.ReadTextAsync(file);

            return await Json.ToObjectAsync<T>(fileContent);
        }

        public static async Task SaveAsync<T>(this ApplicationDataContainer settings, string key, T value)
        {
            settings.SaveString(key, await Json.StringifyAsync(value));
        }

        public static void SaveString(this ApplicationDataContainer settings, string key, string value)
        {
            settings.Values[key] = value;
        }

        public static async Task<T> ReadAsync<T>(this ApplicationDataContainer settings, string key)
        {
            if (settings.Values.TryGetValue(key, out object obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }

            return default;
        }

        public static async Task<StorageFile> SaveFileAsync(this StorageFolder folder, byte[] content, string fileName, CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("ExceptionSettingsStorageExtensionsFileNameIsNullOrEmpty".GetLocalized(), nameof(fileName));
            }

            var storageFile = await folder.CreateFileAsync(fileName, options);
            await FileIO.WriteBytesAsync(storageFile, content);
            return storageFile;
        }

        public static async Task<byte[]> ReadFileAsync(this StorageFolder folder, string fileName)
        {
            var item = await folder.TryGetItemAsync(fileName).AsTask().ConfigureAwait(false);

            if ((item != null) && item.IsOfType(StorageItemTypes.File))
            {
                var storageFile = await folder.GetFileAsync(fileName);
                byte[] content = await storageFile.ReadBytesAsync();
                return content;
            }

            return null;
        }

        public static async Task<byte[]> ReadBytesAsync(this StorageFile file)
        {
            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    using (var reader = new DataReader(stream.GetInputStreamAt(0)))
                    {
                        await reader.LoadAsync((uint)stream.Size);
                        var bytes = new byte[stream.Size];
                        reader.ReadBytes(bytes);
                        return bytes;
                    }
                }
            }

            return null;
        }

        private static string GetFileName(string name)
        {
            return string.Concat(name, FileExtension);
        }
    }
}
