using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using MetroLog;
using Microsoft.Toolkit.Uwp.UI.Controls;
using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;

using SharedCode;

using Windows.Media.ContentRestrictions;
using Windows.Storage;

namespace PacketMessagingTS.Models
{
    public class CustomFoldersArray
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<CustomFoldersArray>();
        private static LogHelper _logHelper = new LogHelper(log);

        private static volatile CustomFoldersArray _instance;
        private static object _syncRoot = new Object();

        static string customFoldersFileName = "CustomFolders.xml";

        private TabViewItemData[] customFoldersArrayField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("TabViewItemData", IsNullable = false)]
        public TabViewItemData[] CustomFolders
        {
            get
            {
                return customFoldersArrayField;
            }
            set
            {
                customFoldersArrayField = value;
            }
        }

        private List<TabViewItemData> customFolderList;
        [System.Xml.Serialization.XmlIgnore]
        public List<TabViewItemData> CustomFolderList
        {
            get => customFolderList;
            set => customFolderList = value;
        }

        private CustomFoldersArray()
        {
            customFoldersArrayField = new TabViewItemData[0];
        }

        public static CustomFoldersArray Instance
        {
            get
            {
                if (_instance is null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance is null)
                            _instance = new CustomFoldersArray();
                    }
                }
                return _instance;
            }
        }

        public async Task OpenAsync()
        {
            StorageFile file = null;
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                ulong size = 0;
                var storageItem = await localFolder.TryGetItemAsync(customFoldersFileName);
                if (storageItem != null)
                {
                    Windows.Storage.FileProperties.BasicProperties basicProperties = await storageItem.GetBasicPropertiesAsync();
                    size = basicProperties.Size;
                }
                if (storageItem is null || size == 0)
                {
                    // Create a new Custom Folder file with two default entries
                    _instance = new CustomFoldersArray();
                    _instance.CustomFolders = new TabViewItemData[2];

                    TabViewItemData tabViewItemData = new TabViewItemData()
                    {
                        Index = 1,
                        Header = "Folder 1",
                        Folder = "Folder 1",
                    };
                    await localFolder.CreateFolderAsync(tabViewItemData.Folder, CreationCollisionOption.OpenIfExists);
                    _instance.CustomFolders.SetValue(tabViewItemData, 0);

                    tabViewItemData = new TabViewItemData()
                    {
                        Index = 2,
                        Header = "Folder 2",
                        Folder = "Folder 2",
                    };
                    await localFolder.CreateFolderAsync(tabViewItemData.Folder, CreationCollisionOption.OpenIfExists);
                    _instance.CustomFolders.SetValue(tabViewItemData, 1);

                    _instance.CustomFolderList = _instance.CustomFolders.ToList();

                    await _instance.SaveAsync();                    
                }

                file = await localFolder.GetFileAsync(customFoldersFileName);

                using (FileStream reader = new FileStream(file.Path, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CustomFoldersArray));
                    _instance = (CustomFoldersArray)serializer.Deserialize(reader);

                    _instance.CustomFolderList = _instance.customFoldersArrayField.ToList();
                }
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"Error opening file {file?.Path + customFoldersFileName}, {e}");
            }
        }


        public async Task SaveAsync()
        {
            StorageFile storageFile = null;

            CustomFolders = CustomFolderList.ToArray();
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                storageFile = await localFolder.CreateFileAsync(customFoldersFileName, CreationCollisionOption.ReplaceExisting);
                if (storageFile != null)
                {
                    using (StreamWriter writer = new StreamWriter(new FileStream(storageFile.Path, FileMode.Create)))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(CustomFoldersArray));
                        serializer.Serialize(writer, this);
                    }
                }
                else
                {
                    log.Error($"File not found {customFoldersFileName}");
                }
            }
            catch (Exception e)
            {
                log.Error($"Error saving file {storageFile.Path}, {e}");
            }
        }

    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class TabViewItemData
    {
        private int indexField;

        private string headerField;

        private string folderField;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Index
        {
            get => indexField;
            set => indexField = value;
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Header
        {
            get => headerField;
            set => headerField = value;
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Folder
        {
            get => folderField;
            set => folderField = value;
        }

        [System.Xml.Serialization.XmlIgnore]
        public ObservableCollection<PacketMessage> Content
        {
            get
            {
                return Singleton<CustomFoldersViewModel>.Instance.DataGridSource;
            }
        }        
    }
}
