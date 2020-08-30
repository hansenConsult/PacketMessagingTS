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
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<CustomFoldersArray>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        private static volatile CustomFoldersArray _instance;
        private static readonly object _syncRoot = new Object();

        static StorageFolder _localFolder = ApplicationData.Current.LocalFolder;

        private const string customFoldersFileName = "CustomFolders.xml";


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

        //private List<TabViewItemData> _customFolderList = new List<TabViewItemData>();
        [System.Xml.Serialization.XmlIgnore]
        public List<TabViewItemData> CustomFolderDataList
        {
            get;
            set;
            //get => _customFolderList;
            //set => _customFolderList = value;
        }

        private List<StorageFolder> _customStorageFolderList = new List<StorageFolder>();
        [System.Xml.Serialization.XmlIgnore]
        public List<StorageFolder> CustomStorageFolderList
        {
            get => _customStorageFolderList;
        }

        private List<string> _customStorageFolderPathList = new List<string>();
        [System.Xml.Serialization.XmlIgnore]
        public List<string> CustomStorageFolderPathList => _customStorageFolderPathList;

        private CustomFoldersArray()
        {
            CustomFolderDataList = new List<TabViewItemData>();
            //customFoldersArrayField = new TabViewItemData[0];
            customFoldersArrayField = Array.Empty<TabViewItemData>();
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

        public async Task AddFolderAsync(TabViewItemData tabViewItemData)
        {
            StorageFolder storageFolder = await _localFolder.CreateFolderAsync(tabViewItemData.Folder, CreationCollisionOption.OpenIfExists);
            CustomStorageFolderList.Add(storageFolder);
            CustomStorageFolderPathList.Add(storageFolder.Path);
            CustomFolderDataList.Add(tabViewItemData);
        }

        // The Remove() api does not work for this list
        private bool RemoveStorageFolderFromList(StorageFolder storageFolder)
        {
            int i = 0;
            for (; i < CustomStorageFolderList.Count; i++)
            {
                if (CustomStorageFolderList[i].Path == storageFolder.Path)
                {
                    break;
                }
            }
            if (i < CustomStorageFolderList.Count)
            {
                CustomStorageFolderList.RemoveAt(i);
                return true;
            }
            return false;
        }

        public async Task RemoveFolderAsync(TabViewItemData tabViewItemData)
        {
            StorageFolder storageFolder = await _localFolder.CreateFolderAsync(tabViewItemData.Folder, CreationCollisionOption.OpenIfExists);
            bool success = RemoveStorageFolderFromList(storageFolder);
            success &= CustomStorageFolderPathList.Remove(storageFolder.Path);
            await storageFolder.DeleteAsync();
            success &= CustomFolderDataList.Remove(tabViewItemData);
            if (!success)
            {
                throw new Exception();
            }
        }

        public async Task RenameFolderAsync(TabViewItemData tabViewItemDataOld, TabViewItemData tabViewItemDataNew)
        {
            StorageFolder storageFolder = await _localFolder.CreateFolderAsync(tabViewItemDataOld.Folder, CreationCollisionOption.OpenIfExists);
            bool success = RemoveStorageFolderFromList(storageFolder);
            success &= CustomStorageFolderPathList.Remove(storageFolder.Path);
            success &= CustomFolderDataList.Remove(tabViewItemDataOld);
            try
            {
                await storageFolder.RenameAsync(tabViewItemDataNew.Folder);
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, "Renamed folder exist");
            }
            storageFolder = await _localFolder.CreateFolderAsync(tabViewItemDataNew.Folder, CreationCollisionOption.OpenIfExists);
            CustomStorageFolderList.Add(storageFolder);
            CustomStorageFolderPathList.Add(storageFolder.Path);
            CustomFolderDataList.Add(tabViewItemDataNew);
            if (!success)
            {
                throw new Exception();
            }
        }

        public static async Task OpenAsync()
        {
            StorageFile file = null;
            try
            {
                ulong size = 0;
                var storageItem = await _localFolder.TryGetItemAsync(customFoldersFileName);
                if (storageItem != null)
                {
                    Windows.Storage.FileProperties.BasicProperties basicProperties = await storageItem.GetBasicPropertiesAsync();
                    size = basicProperties.Size;
                }
                if (storageItem is null || size == 0)
                {
                    // Create a new Custom Folder file with two default entries
                    _instance = new CustomFoldersArray
                    {
                        CustomFolders = new TabViewItemData[2]
                    };

                    TabViewItemData tabViewItemData = new TabViewItemData()
                    {
                        Index = 1,
                        Header = "Folder 1",
                        Folder = "Folder 1",
                    };
                    await _instance.AddFolderAsync(tabViewItemData);

                    tabViewItemData = new TabViewItemData()
                    {
                        Index = 2,
                        Header = "Folder 2",
                        Folder = "Folder 2",
                    };
                    await _instance.AddFolderAsync(tabViewItemData);

                    await _instance.SaveAsync();                    
                }

                file = await _localFolder.GetFileAsync(customFoldersFileName);

                using (FileStream reader = new FileStream(file.Path, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CustomFoldersArray));
                    _instance = (CustomFoldersArray)serializer.Deserialize(reader);

                    _instance.CustomFolderDataList = _instance.customFoldersArrayField.ToList();
                    _instance.CustomStorageFolderList.Clear();
                    _instance.CustomStorageFolderPathList.Clear();
                    foreach (TabViewItemData tabViewItemData in _instance.CustomFolderDataList)
                    {
                        StorageFolder storageFolder = await _localFolder.CreateFolderAsync(tabViewItemData.Folder, CreationCollisionOption.OpenIfExists);
                        _instance.CustomStorageFolderList.Add(storageFolder);
                        _instance.CustomStorageFolderPathList.Add(storageFolder.Path);
                    }
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

            CustomFolders = CustomFolderDataList.ToArray();
            try
            {
                storageFile = await _localFolder.CreateFileAsync(customFoldersFileName, CreationCollisionOption.ReplaceExisting);
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

        //public StorageFolder[] GetStorageFolders()
        //{
        //    StorageFolder[] customFolders = new StorageFolder[CustomFoldersArray.Instance.CustomFolderList.Count];
        //    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        //    for (int i = 0; i < CustomFolderList.Count; i++) //TabViewItemData folderData in 
        //    {
        //        customFolders[i] = await localFolder.CreateFolderAsync(CustomFolderList[i].Folder, CreationCollisionOption.OpenIfExists);
        //    }
        //}
    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class TabViewItemData : IEquatable<TabViewItemData>
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

        public TabViewItemData(TabViewItemData tabViewItemData)
        {
            Index = tabViewItemData.Index;
            Header = tabViewItemData.Header;
            Folder = tabViewItemData.Folder;
        }

        public TabViewItemData()
        {
        }

        [System.Xml.Serialization.XmlIgnore]
        public ObservableCollection<PacketMessage> Content
        {
            get
            {
                return Singleton<CustomFoldersViewModel>.Instance.DataGridSource;
            }
        }

        public bool Equals(TabViewItemData tabViewItemData)
        {
            if (tabViewItemData == null)
                return false;

            if (Index == tabViewItemData.Index && Header == tabViewItemData.Header && Folder == tabViewItemData.Folder)
                return true;
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            TabViewItemData tabViewItemData = obj as TabViewItemData;

            if (tabViewItemData == null)
                return false;
            else
                return Equals(tabViewItemData);
        }
    }
}
