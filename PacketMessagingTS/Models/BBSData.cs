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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

using FormControlBaseClass;

using MetroLog;

using SharedCode;

using Windows.Storage;
using Windows.UI.Xaml.Data;

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
    public partial class BBSDefinitions : ICollectionViewFactory
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

        private List<BBSData> bbsDataList;
        [System.Xml.Serialization.XmlIgnore]
        public List<BBSData> BBSDataList
        {
            get
            {
                bbsDataList = new List<BBSData>();
                foreach (BBSData bbsData in bBSDataArrayField)
                {
                    bbsDataList.Add(bbsData);
                }
                return bbsDataList;
            }
        }

        private BBSDefinitions()
        {
            bBSDataArrayField = new BBSData[0];
            bbsDataList = new List<BBSData>();
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
				var storageItem = await localFolder.TryGetItemAsync(bbsFileName);
				if (storageItem is null)
				{
					// Copy the file from the install folder to the local folder
					var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
					var storageFile = await folder.GetFileAsync(bbsFileName);
					if (storageFile != null)
					{
						await storageFile.CopyAsync(localFolder, bbsFileName, Windows.Storage.NameCollisionOption.FailIfExists);
					}
				}

				StorageFile file = await localFolder.GetFileAsync(bbsFileName);

				using (FileStream reader = new FileStream(file.Path, FileMode.Open))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(BBSDefinitions));
                    _instance = (BBSDefinitions)serializer.Deserialize(reader);
				}
			}
			catch (FileNotFoundException e)
			{
				Debug.WriteLine($"Open BBSData file failed: {e.Message}");
                _logHelper.Log(LogLevel.Error, $"Open BBSData file failed: {e.Message}");
			}
			catch (Exception e)
			{
                _logHelper.Log(LogLevel.Error, $"Error opening {e.Message} {e}");
                Debug.WriteLine($"Error opening {e.Message} {e}");
			}
		}

		public async Task SaveAsync()
		{
            this.BBSDataArray = BBSDataList.ToArray();

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            try
            {
                var storageItem = await localFolder.CreateFileAsync(bbsFileName);
                if (storageItem != null)
                {
                    using (StreamWriter writer = new StreamWriter(new FileStream(storageItem.Path, FileMode.OpenOrCreate)))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(BBSDefinitions));
                        serializer.Serialize(writer, this);
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
        //public static BBSDefinitions CreateFromBulletin(ref PacketMessage bbsBulletin)
        //{
        //	string bulletin = bbsBulletin.MessageBody;

        //	if (bulletin is null)
        //		return null;

        //	int start = 0;
        //	int start2 = 0;
        //	bulletin = bulletin.Substring(start);

        //	start = bulletin.IndexOf("---------");
        //	start2 += start;
        //	string bbsInfo = bulletin.Substring(start);
        //	start = bbsInfo.IndexOf("\n");
        //	start2 += start;
        //	bbsInfo = bbsInfo.Substring(start + 1);
        //	int end = bbsInfo.IndexOf('*');
        //	bbsInfo = bbsInfo.Substring(0, end);

        //	BBSData bbsData;
        //	var lines = bbsInfo.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        //	int bbsCount = lines.Length;
        //	BBSData[] bbsdataArray = new BBSData[lines.Length + 1];
        //	int i = 0;
        //	for (; i < lines.Length; i++)
        //	{
        //		var callsign = lines[i].Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        //		bbsData = new BBSData();
        //		bbsData.Name = callsign[0];
        //		bbsData.ConnectName = callsign[1];
        //		bbsData.Frequency1 = callsign[2];
        //		bbsData.Frequency2 = callsign[3];
        //		bbsData.Selected = false;

        //		bbsdataArray[i] = bbsData;
        //	}
        //	// Location
        //	bbsInfo = bulletin.Substring(start2 + end);
        //	start = bbsInfo.IndexOf("---------");
        //	bbsInfo = bbsInfo.Substring(start);
        //	start = bbsInfo.IndexOf("\n");
        //	bbsInfo = bbsInfo.Substring(start + 1);
        //	string description = "Santa Clara County ARES/RACES PacketSystem. ";
        //	lines = bbsInfo.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        //	for (i = 0; i < bbsCount; i++)
        //	{
        //		var callsign = lines[i].Split(new string[] { ",", "      ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
        //		bbsdataArray[i].Description = description + callsign[1];
        //	}
        //	bbsdataArray[0].Selected = true;

        //	// Add training BBS
        //	bbsData = new BBSData();
        //	bbsData.Name = "W5XSC";
        //	bbsData.ConnectName = "W5XSC-1";
        //	bbsdataArray[i] = bbsData;

        //	BBSDefinitions bbsDefinitions = new BBSDefinitions();
        //	bbsDefinitions.RevisionTime = bbsBulletin.JNOSDate;
        //	bbsDefinitions.BBSDataArray = bbsdataArray;

        //	return bbsDefinitions;
        //}

        //ICollectionView CreateView()
        //{
        //	throw new NotImplementedException();
        //	//return new MyListCollectionView(this);
        //	//BBSDataArray.CreateView();
        //	//return (BBSData[]) CreateView();


        //}

        ICollectionView ICollectionViewFactory.CreateView()
		{
			throw new NotImplementedException();
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
