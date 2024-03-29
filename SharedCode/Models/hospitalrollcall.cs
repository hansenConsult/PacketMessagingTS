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
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

using MetroLog;

using Windows.Storage;

// 
// This source code was auto-generated by xsd, Version=4.8.4084.0.
// 

namespace SharedCode.Models
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.4084.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class HospitalRollCall
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<HospitalRollCall>();
        private static LogHelper _logHelper = new LogHelper(log);

        private const string fileName = "HospitalRollCall.xml";
        private Hospital[] hospitalsField = new Hospital[15];

        private static volatile HospitalRollCall _instance;
        private static object _syncRoot = new Object();


        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Hospital")]
        public Hospital[] Hospitals
        {
            get
            {
                return this.hospitalsField;
            }
            set
            {
                this.hospitalsField = value;
            }
        }

        private List<Hospital> hospitalList;
        [System.Xml.Serialization.XmlIgnore]
        public List<Hospital> HospitalList
        {
            get
            {
                hospitalList = new List<Hospital>();
                for (int i = 0; i < _instance.Hospitals.Length; i++)
                {
                    hospitalList.Add(_instance.Hospitals[i]);
                }
                return hospitalList;
            }
            set => hospitalList = value;
        }

        private HospitalRollCall()
        {
            Hospitals = new Hospital[0];
        }

        public static HospitalRollCall Instance
        {
            get
            {
                if (_instance is null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance is null)
                            _instance = new HospitalRollCall();
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
                var storageItem = await localFolder.TryGetItemAsync(fileName);
                if (storageItem is null)
                {
                    // Copy the file from the install folder to the local folder
                    var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                    var storageFile = await folder.GetFileAsync(fileName);
                    if (storageFile != null)
                    {
                        await storageFile.CopyAsync(localFolder, fileName, Windows.Storage.NameCollisionOption.FailIfExists);
                    }
                }

                StorageFile file = await localFolder.GetFileAsync(fileName);

                using (FileStream reader = new FileStream(file.Path, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(HospitalRollCall));
                    _instance = (HospitalRollCall)serializer.Deserialize(reader);
                }
                //HospitalList = _instance.Hospitals.ToList();
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

        //public static void Save()
        //{
        //}

        //public static void Clear()
        //{
        //    //for (int i = 0; i < Hospitals.Length; i++)
        //    //{
        //    //    Hospitals[i].callSign = "";
        //    //}
        //}
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.4084.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class Hospital
    {
        private string hospitalNameField;

        private string hospitalPacketAddressField;

        private string callSignField;

        private string nameField;

        private bool trafficField;

        private bool packetField;

        private bool hosEqField;

        private bool printerField;

        private bool testedField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string HospitalName
        {
            get
            {
                return this.hospitalNameField;
            }
            set
            {
                this.hospitalNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string HospitalPacketAddress
        {
            get
            {
                return this.hospitalPacketAddressField;
            }
            set
            {
                this.hospitalPacketAddressField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CallSign
        {
            get
            {
                return this.callSignField;
            }
            set
            {
                this.callSignField = value;
            }
        }

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
        public bool Traffic
        {
            get
            {
                return this.trafficField;
            }
            set
            {
                this.trafficField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Packet
        {
            get
            {
                return this.packetField;
            }
            set
            {
                this.packetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool HosEq
        {
            get
            {
                return this.hosEqField;
            }
            set
            {
                this.hosEqField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Printer
        {
            get
            {
                return this.printerField;
            }
            set
            {
                this.printerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Tested
        {
            get
            {
                return this.testedField;
            }
            set
            {
                this.testedField = value;
            }
        }
    }
}