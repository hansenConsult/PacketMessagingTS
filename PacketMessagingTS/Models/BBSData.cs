﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

using MetroLog;

using SharedCode;

using Windows.Storage;

namespace PacketMessagingTS.Models
{

    // 
    // This source code was auto-generated by xsd, Version=4.0.30319.33440.
    // 


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class BBSDefinitions
	{
		private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<BBSDefinitions>();
        private static LogHelper _logHelper = new LogHelper(log);


        public static string bbsFileName = "BBSData.xml";
        private static volatile BBSDefinitions _instance;
        private static object _syncRoot = new Object();


        private DateTime? revisionTimeField;

        private BBSData[] bBSDataArrayField;

		public DateTime? RevisionTime
		{
			get { return revisionTimeField; }
			set { revisionTimeField = value; }
		}

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BBS")]
        public BBSData[] BBSDataArray
        {
            get
            {
                return this.bBSDataArrayField;
            }
            set
            {
                this.bBSDataArrayField = value;
            }
        }

        private BBSDefinitions()
        {
            bBSDataArrayField = new BBSData[0];
        }

        public static BBSDefinitions Instance
        {
            get
            {
                if (_instance is null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance is null)
                            _instance = new BBSDefinitions();
                    }
                }
                return _instance;
            }
        }

        public async Task OpenAsync()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			try
			{
                StorageFile bbsDataFile;
                ulong size = 0;
                var storageItem = await localFolder.TryGetItemAsync(bbsFileName);
                if (storageItem != null)
                {
                    Windows.Storage.FileProperties.BasicProperties basicProperties = await storageItem.GetBasicPropertiesAsync();
                    size = basicProperties.Size;
                }
                if (storageItem is null || size == 0)
				{
					// Copy the file from the install folder to the local folder
					var assetsFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                    bbsDataFile = await assetsFolder.GetFileAsync(bbsFileName);
					if (bbsDataFile != null)
					{
						await bbsDataFile.CopyAsync(localFolder, bbsFileName, NameCollisionOption.ReplaceExisting);
					}
				}

                bbsDataFile = await localFolder.GetFileAsync(bbsFileName);

				using (FileStream reader = new FileStream(bbsDataFile.Path, FileMode.Open))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(BBSDefinitions));
                    _instance = (BBSDefinitions)serializer.Deserialize(reader);
				}
			}
			catch (FileNotFoundException e)
			{
                _logHelper.Log(LogLevel.Error, $"Open BBSData file failed: {e.Message}");
			}
			catch (Exception e)
			{
                _logHelper.Log(LogLevel.Error, $"Error opening {e.Message} {e}");
			}
		}

		//public async Task SaveAsync()
		//{
  //          this.BBSDataArray = BBSDataList.ToArray();

  //          StorageFolder localFolder = ApplicationData.Current.LocalFolder;

  //          try
  //          {
  //              var storageItem = await localFolder.CreateFileAsync(bbsFileName);
  //              if (storageItem != null)
  //              {
  //                  using (StreamWriter writer = new StreamWriter(new FileStream(storageItem.Path, FileMode.OpenOrCreate)))
  //                  {
  //                      XmlSerializer serializer = new XmlSerializer(typeof(BBSDefinitions));
  //                      serializer.Serialize(writer, this);
  //                  }
  //              }
  //              else
  //              {
  //                  _logHelper.Log(LogLevel.Error, $"File not found {bbsFileName}");

  //              }
  //          }
		//	catch (Exception e)
		//	{
  //              _logHelper.Log(LogLevel.Error, $"Error saving {bbsFileName} {e}");
		//	}
		//}

        public static async void SaveAsync(BBSDefinitions bBSDefinitions)
        {

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            try
            {
                var storageItem = await localFolder.CreateFileAsync(bbsFileName, CreationCollisionOption.ReplaceExisting);
                if (storageItem != null)
                {
                    using (StreamWriter writer = new StreamWriter(new FileStream(storageItem.Path, FileMode.Create)))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(BBSDefinitions));
                        serializer.Serialize(writer, bBSDefinitions);
                    }
                }
                else
                {
                    _logHelper.Log(LogLevel.Error, $"File not found {bbsFileName}");

                }
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"Error saving {bbsFileName} {e}");
            }
        }

        public BBSData GetBBSFromName(string bbsName)
        {
            BBSData retval = null;
            foreach (BBSData bbsData in BBSDataArray)
            {
                if (bbsData.Name == bbsName)
                {
                    retval = bbsData;
                    break;
                }
            }
            return retval;
        }

        public static async Task CreateFromBulletinAsync(PacketMessage bbsBulletin)
        {
            string[] subjectElements = bbsBulletin.Subject.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string version = subjectElements[3];
            string bulletin = bbsBulletin?.MessageBody;

            if (bulletin is null)
                return;
            try
            {
                int start = bulletin.IndexOf("W1XSC");
                int stop = bulletin.IndexOf('*', start);
                stop = bulletin.IndexOf('\r', stop);
                string bbsInfo = bulletin.Substring(start, stop - start);

                BBSData bbsData;
                var lines = bbsInfo.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                int bbsCount = lines.Length;
                BBSData[] bbsdataArray = new BBSData[lines.Length + 1];
                for (int i = 0; i < lines.Length; i++)
                {
                    var bbs = lines[i].Split(new char[] { ',', ' ', '\t', '*' }, StringSplitOptions.RemoveEmptyEntries);

                    bbsData = new BBSData();
                    bbsData.Name = bbs[0];
                    bbsData.ConnectName = bbs[1];
                    bbsData.Frequency1 = bbs[2];
                    bbsData.Frequency2 = bbs[3];
                    if (bbs.Length > 4)
                    {
                        bbsData.Frequency3 = bbs[4];
                    }

                    bbsdataArray[i] = bbsData;
                }
                // Add location
                start = bulletin.IndexOf("W1XSC", stop);
                bbsInfo = bulletin.Substring(start);

                lines = bbsInfo.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < bbsCount; i++)
                {
                    var callsign = lines[i].Split(new string[] { ",", "      ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    bbsdataArray[i].Description = callsign[1];
                }

                // Add training BBS
                bbsData = new BBSData();
                bbsData.Name = "W5XSC";
                bbsData.ConnectName = "W5XSC-1";
                bbsData.Description = "Used for training and testing";
                bbsData.Frequency1 = "144.910";
                bbsData.Frequency3 = "433.450";
                bbsdataArray[bbsCount] = bbsData;

                BBSDefinitions bbsDefinitions = new BBSDefinitions();
                bbsDefinitions.RevisionTime = bbsBulletin.JNOSDate;
                bbsDefinitions.BBSDataArray = bbsdataArray;

                BBSDefinitions.SaveAsync(bbsDefinitions);
                // Also save to Assets. Make a copy first
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile bbsDataFile = await localFolder.GetFileAsync(bbsFileName);
                if (bbsDataFile != null)
                {
                    StorageFolder assetsFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                    StorageFile storageFile = await assetsFolder.GetFileAsync(bbsFileName);
                    string bbsCopyFileName = bbsFileName + " - Copy";
                    await storageFile.CopyAsync(assetsFolder, bbsCopyFileName, NameCollisionOption.ReplaceExisting);
                    await bbsDataFile.CopyAsync(assetsFolder, bbsFileName, NameCollisionOption.ReplaceExisting);
                }
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"Error in CreateFromBulletinAsync(): {e.Message}");
            }
        }

	}

	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class BBSData
    {
        private string nameField;

        private string connectNameField;

        private string descriptionField;

        private string frequency1Field;

        private string frequency2Field;

        private string frequency3Field;

        private string secondaryField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ConnectName
        {
            get
            {
                return this.connectNameField;
            }
            set
            {
                this.connectNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Frequency1
        {
            get
            {
                return this.frequency1Field;
            }
            set
            {
                this.frequency1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Frequency2
        {
            get
            {
                return this.frequency2Field;
            }
            set
            {
                this.frequency2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Frequency3
        {
            get
            {
                return this.frequency3Field;
            }
            set
            {
                this.frequency3Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Secondary
        {
            get
            {
                return this.secondaryField;
            }
            set
            {
                this.secondaryField = value;
            }
        }

		//public override string ToString() => Name;

	}
}
