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
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

using MetroLog;

using Windows.Storage;
using Windows.Storage.FileProperties;

// 
// This source code was auto-generated by xsd, Version=4.6.1055.0.
// 

namespace PacketMessagingTS.Models
{
	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
	//[System.SerializableAttribute()]
	//[System.Diagnostics.DebuggerStepThroughAttribute()]
	//[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
	public partial class EmailAccountArray
	{
		private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<DistributionListArray>();

		//private Dictionary<string, string> _distributionListsDict;
		private const string emailAccountsFileName = "EmailAccounts.xml";
		private static volatile EmailAccountArray _instance;
		private static object _syncRoot = new Object();


		private EmailAccount[] emailAccountsArrayField;

		/// <remarks/>
		[System.Xml.Serialization.XmlArrayItemAttribute("emailAccount", IsNullable = false)]
		public EmailAccount[] EmailAccounts
		{
			get
			{
				return this.emailAccountsArrayField;
			}
			set
			{
				this.emailAccountsArrayField = value;
			}
		}

        private List<EmailAccount> emailAccountList;
        [System.Xml.Serialization.XmlIgnore]
        public List<EmailAccount> EmailAccountList
        {
            get => emailAccountList;
            set => emailAccountList = value;
        }

        private EmailAccountArray()
		{
			emailAccountsArrayField = new EmailAccount[0];
		}

		public static EmailAccountArray Instance
		{
			get
			{
				if (_instance is null)
				{
					lock (_syncRoot)
					{
						if (_instance is null)
							_instance = new EmailAccountArray();
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
                StorageFile storageItem = await localFolder.GetFileAsync(emailAccountsFileName);
                BasicProperties basicProperties = null;
                StorageFile emailAccountsFile;
                if (storageItem != null)
                {
                    basicProperties = await storageItem.GetBasicPropertiesAsync();
                }
                if (storageItem is null || (basicProperties != null && basicProperties.Size == 0))
                {
                    // Copy the file from the install folder to the local folder
                    StorageFolder assetsFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                    emailAccountsFile = await assetsFolder.GetFileAsync(emailAccountsFileName);
					if (emailAccountsFile != null)
					{
						await emailAccountsFile.CopyAsync(localFolder, emailAccountsFileName, NameCollisionOption.ReplaceExisting);
					}
				}

                emailAccountsFile = await localFolder.GetFileAsync(emailAccountsFileName);
				using (FileStream reader = new FileStream(emailAccountsFile.Path, FileMode.Open))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(EmailAccount[]));
					EmailAccounts = (EmailAccount[])serializer.Deserialize(reader);
				}
                EmailAccountList = EmailAccounts.ToList();

            }
			catch (FileNotFoundException e)
			{
				log.Error($"Open E-Mail Accounts file failed: {e.Message}");
				return;
			}

			catch (Exception e)
			{
				log.Error($"Error opening file {e.Message}, {e}");
				return;
			}
			//UpdateDictionary();
		}

		public async Task SaveAsync()
		{
			if (EmailAccountList is null || EmailAccountList.Count == 0)
				return;

            EmailAccounts = EmailAccountList.ToArray();

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			try
			{
				StorageFile file = await localFolder.CreateFileAsync(emailAccountsFileName, CreationCollisionOption.ReplaceExisting);
				using (StreamWriter writer = new StreamWriter(new FileStream(file.Path, FileMode.Create)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(EmailAccount[]));
					serializer.Serialize(writer, EmailAccounts);
				}
			}
			catch (Exception e)
			{
				log.Error($"Error saving {emailAccountsFileName}, {e}");
				return;
			}
			//UpdateDictionary();
			//_dataChanged = false;
		}

        public int GetSelectedIndexFromEmailUserName(string emailUserName)
        {
            int i = 0;
            for (; i < EmailAccountList.Count; i++)
            {
                if (emailUserName == EmailAccountList[i].MailUserName)
                {
                    return i;
                }
            }
            return -1;
        }

        public List<string> GetMailServers(string partialName = null)
		{
			if (EmailAccountList is null || EmailAccountList.Count == 0)
				return null;

			List<string> matches = new List<string>();

			foreach (EmailAccount item in EmailAccountList)
			{
				if (string.IsNullOrEmpty(partialName) || item.MailServer.StartsWith(partialName.ToUpper()))
				{
					matches.Add(item.MailServer);
				}
			}
			return matches;
		}

		public List<EmailAccount> GetMailAccounts(string partialServerName = null)
		{
			if (EmailAccountList is null || EmailAccountList.Count == 0)
				return null;

			List<EmailAccount> matches = new List<EmailAccount>();

			foreach (EmailAccount item in EmailAccountList)
			{
				if (string.IsNullOrEmpty(partialServerName) || item.MailServer.StartsWith(partialServerName.ToUpper()))
				{
					matches.Add(item);
				}
			}
			return matches;
		}

	}

	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
	//[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	//[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class EmailAccount
	{
		private string mailServerField;

		private ushort mailServerPortField;

		private string mailUserNameField;

		private string mailPasswordField;

		private bool mailIsSSLField;


		/// <remarks/>
		public string MailServer
		{
			get
			{
				return this.mailServerField;
			}
			set
			{
				this.mailServerField = value;
			}
		}

		/// <remarks/>
		public ushort MailServerPort
		{
			get
			{
				return this.mailServerPortField;
			}
			set
			{
				this.mailServerPortField = value;
			}
		}

		/// <remarks/>
		public string MailUserName
		{
			get
			{
				return this.mailUserNameField;
			}
			set
			{
				this.mailUserNameField = value;
			}
		}

		/// <remarks/>
		public string MailPassword
		{
			get
			{
				return this.mailPasswordField;
			}
			set
			{
				this.mailPasswordField = value;
			}
		}

		public bool MailIsSSLField
		{
			get => mailIsSSLField;
			set => mailIsSSLField = value;
		}

		public override bool Equals(Object obj)
		{
			//Check for null and compare run-time types.
			if ((obj is null) || !this.GetType().Equals(obj.GetType()))
			{
				return false;
			}
			else
			{
				EmailAccount emailAccount = (EmailAccount)obj;
				return (MailServer == emailAccount.MailServer
					&& MailServerPort == emailAccount.MailServerPort
					&& MailUserName == emailAccount.MailUserName
					&& MailPassword == emailAccount.MailPassword
					&& MailIsSSLField == emailAccount.MailIsSSLField);
			}
		}

        public override int GetHashCode()
        {
            var hashCode = -2123422976;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MailServer);
            hashCode = hashCode * -1521134295 + MailServerPort.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MailUserName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MailPassword);
            hashCode = hashCode * -1521134295 + MailIsSSLField.GetHashCode();
            return hashCode;
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        //public string Name
        //{
        //	get
        //	{
        //		return this.nameField;
        //	}
        //	set
        //	{
        //		this.nameField = value;
        //	}
        //}
        public override string ToString()
		{
			return MailUserName;
		}

        public static bool IsEMailAccountsEqual(EmailAccount account1, EmailAccount account2)
        {
            if (account1.MailServer == account2.MailServer
                && account1.MailServerPort == account2.MailServerPort
                && account1.MailUserName == account2.MailUserName
                && account1.MailPassword == account2.MailPassword                
                && account1.MailIsSSLField == account2.MailIsSSLField)
                return true;
            else
                return false;

        }
    }

}
