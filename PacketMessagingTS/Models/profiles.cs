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
using System.Linq;
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
	public partial class ProfileArray
	{
		private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<ProfileArray>();
        private static LogHelper _logHelper = new LogHelper(log);

        private static volatile ProfileArray _instance;
        private static object _syncRoot = new Object();

        static string profileFileName = "Profiles.xml";

		private Profile[] profileField;

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("Profile")]
		public Profile[] Profiles
		{
			get
			{
				return this.profileField;
			}
			set
			{
				this.profileField = value;
			}
		}

        private List<Profile> profileList;
        [System.Xml.Serialization.XmlIgnore]
        public List<Profile> ProfileList
        {
            get => profileList;
            set => profileList = value;
        }

        private ProfileArray()
        {
            profileField = new Profile[0];
        }

        public static ProfileArray Instance
        {
            get
            {
                if (_instance is null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance is null)
                            _instance = new ProfileArray();
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

                var storageItem = await localFolder.TryGetItemAsync(profileFileName);
                if (storageItem is null)
                {
                    // Create a new profile file with a default entry
                    Profile profile = new Profile()
                    {
                        Name = "Default",
                        BBS = BBSDefinitions.Instance.BBSDataArray[0].Name,
                        TNC = TNCDeviceArray.Instance.TNCDeviceList.ToArray()[0].Name,
                        Subject = "",
                    };

					_instance = new ProfileArray();
                    _instance.Profiles = new Profile[1];
                    _instance.Profiles.SetValue(profile, 0);
                    _instance.ProfileList = _instance.Profiles.ToList();

                    await _instance.SaveAsync();
				}

				file = await localFolder.GetFileAsync(profileFileName);

				using (FileStream reader = new FileStream(file.Path, FileMode.Open))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ProfileArray));
					_instance = (ProfileArray)serializer.Deserialize(reader);

                    _instance.profileList = _instance.profileField.ToList();
				}
			}
			catch (Exception e)
			{
                _logHelper.Log(LogLevel.Error, $"Error opening file {file?.Path + profileFileName}, {e}");
			}
		}


		public async Task SaveAsync()
		{
            StorageFile storageFile = null;

            Profiles = ProfileList.ToArray();
            try
			{
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                storageFile = await localFolder.CreateFileAsync(profileFileName, CreationCollisionOption.ReplaceExisting);
                if (storageFile != null)
                {

                    using (StreamWriter writer = new StreamWriter(new FileStream(storageFile.Path, FileMode.Create)))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(ProfileArray));
                        serializer.Serialize(writer, this);
                    }
                }
                else
                {
                    log.Error($"File not found {profileFileName}");
                }
            }
            catch (Exception e)
			{
				log.Error($"Error saving file {storageFile.Path}, {e}");
			}
		}

        public void AddItem(Profile profile)
        {
            profileList.Add(profile);
        }

        public void DeleteItem(Profile profile)
        {
            profileList.Remove(profile);
        }

    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
	//[System.SerializableAttribute()]
	//[System.Diagnostics.DebuggerStepThroughAttribute()]
	//[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class Profile
	{

		private string nameField;

		private string tNCField;

		private string bBSField;

		private string sendToField;

        private string subjectField;

        private string messageField;
        //private bool selectedField;

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
		public string TNC
		{
			get
			{
				return this.tNCField;
			}
			set
			{
				this.tNCField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string BBS
		{
			get
			{
				return this.bBSField;
			}
			set
			{
				this.bBSField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string SendTo
		{
			get
			{
				return this.sendToField;
			}
			set
			{
				this.sendToField = value;
			}
		}

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Subject
        {
            get
            {
                return this.subjectField;
            }
            set
            {
                this.subjectField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        //public bool Selected
        //{
        //	get
        //	{
        //		return this.selectedField;
        //	}
        //	set
        //	{
        //		this.selectedField = value;
        //	}
        //}

        public override string ToString() => Name;
	}
}
