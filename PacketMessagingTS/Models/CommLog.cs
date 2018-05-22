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
using Windows.Storage;
using MetroLog;
using FormControlBaseClass;
using System.Linq;
using System.Collections.Generic;

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
	public partial class CommLog
	{
		private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<CommLog>();

		private const string commLogFileName = "CommLog.xml";
		private static volatile CommLog _instance;
		private static object _syncRoot = new Object();

		private CommLogEntry[] logentryField;

		private List<CommLogEntry> commLogEntryListField;

		private string incidentNameField;

		private string activationNumberField;

		private string operationalPeriodFromField;

		private string operationalPeriodToField;

		private string radioNetNameField;

		private string operatorNameCallsignField;

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("LogEntry")]
		public CommLogEntry[] CommLogEntries
		{
			get
			{
				return this.logentryField;
			}
			set
			{
				this.logentryField = value;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public List<CommLogEntry> CommLogEntryList
		{
			get => commLogEntryListField == null ? commLogEntryListField = new List<CommLogEntry>() : commLogEntryListField;
			set
			{
				commLogEntryListField = value;
				CommLogEntries = commLogEntryListField.ToArray();
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string IncidentName
		{
			get
			{
				return this.incidentNameField;
			}
			set
			{
				this.incidentNameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string ActivationNumber
		{
			get
			{
				return this.activationNumberField;
			}
			set
			{
				this.activationNumberField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string OperationalPeriodFrom
		{
			get
			{
				return this.operationalPeriodFromField;
			}
			set
			{
				this.operationalPeriodFromField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string OperationalPeriodTo
		{
			get
			{
				return this.operationalPeriodToField;
			}
			set
			{
				this.operationalPeriodToField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string RadioNetName
		{
			get
			{
				return this.radioNetNameField;
			}
			set
			{
				this.radioNetNameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string OperatorNameCallsign
		{
			get
			{
				return this.operatorNameCallsignField;
			}
			set
			{
				this.operatorNameCallsignField = value;
			}
		}
		private CommLog()
		{
			//logentryField = new CommLogEntry[0];
			//_distributionListArrayField = new DistributionList[0];
			//_distributionListsDict = new Dictionary<string, string>();
		}

		public static CommLog Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_syncRoot)
					{
						if (_instance == null)
							_instance = new CommLog();
					}
				}
				return _instance;
			}
		}

		public async Task OpenAsync()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;

			StorageFile file = null;
			try
			{
				var storageItem = await localFolder.TryGetItemAsync(commLogFileName);
				if (storageItem == null)
					return;

				file = await localFolder.GetFileAsync(commLogFileName);
				using (FileStream reader = new FileStream(file.Path, FileMode.Open))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(CommLog));
					_instance = (CommLog)serializer.Deserialize(reader);
				}
			}
			catch (FileNotFoundException e)
			{
				log.Error($"Open E-Mail Accounts file failed: {e.Message}");
				return;
			}

			catch (Exception e)
			{
				log.Error($"Error opening file {file?.Path + commLogFileName}, {e}");
				return;
			}
			//UpdateDictionary();
		}

		public async Task SaveAsync()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			try
			{
				StorageFile file = await localFolder.CreateFileAsync(commLogFileName, CreationCollisionOption.ReplaceExisting);
				using (StreamWriter writer = new StreamWriter(new FileStream(file.Path, FileMode.OpenOrCreate)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(EmailAccount[]));
					serializer.Serialize(writer, _instance);
				}
			}
			catch (Exception e)
			{
				log.Error($"Error saving {commLogFileName}, {e}");
				return;
			}
			//UpdateDictionary();
			//_dataChanged = false;
		}

		public void AddCommLogEntry(PacketMessage packetMessage, DateTime startTime, DateTime endTime)
		{
			CommLogEntry commLogEntry = null;
			if (packetMessage.SentTime != null && (packetMessage.SentTime > startTime && packetMessage.SentTime < endTime))
			{
				// This message was sent
				string messageTo = packetMessage.MessageTo.Substring(0, (packetMessage.MessageTo.IndexOf('@') == -1 ? packetMessage.MessageTo.Length : packetMessage.MessageTo.IndexOf('@')));
				commLogEntry = new CommLogEntry()
				{
					Time = packetMessage.SentTime ,
					FromCallsign = "",
					FromMessageNumber = packetMessage.MessageNumber,
					ToCallsign = packetMessage.MessageTo.Substring(0, (packetMessage.MessageTo.IndexOf('@') == -1 ? packetMessage.MessageTo.Length : packetMessage.MessageTo.IndexOf('@'))),
					ToMessageNumber = packetMessage.ReceiverMessageNumber,
					Message = packetMessage.Subject,
				};
			}
			else if (packetMessage.ReceivedTime != null && (packetMessage.ReceivedTime > startTime && packetMessage.ReceivedTime < endTime))
			{
				string fromMessageNumber = packetMessage.Subject.Substring(0, (packetMessage.Subject.IndexOf('_') == -1 ? 0 : packetMessage.Subject.IndexOf('_')));
				// This message was received
				commLogEntry = new CommLogEntry()
				{
					Time = packetMessage.ReceivedTime,
					FromCallsign = packetMessage.MessageFrom.Substring(0, (packetMessage.MessageFrom.IndexOf('@') == -1 ? packetMessage.MessageFrom.Length : packetMessage.MessageFrom.IndexOf('@'))),
					FromMessageNumber = packetMessage.Subject.Substring(0, (packetMessage.Subject.IndexOf('_') == -1 ? 0 : packetMessage.Subject.IndexOf('_'))),
					ToCallsign = "",
					ToMessageNumber = packetMessage.MessageNumber,
					Message = packetMessage.Subject,
				};
			}
			if (commLogEntry != null)
			{
				commLogEntryListField.Add(commLogEntry);
				CommLogEntries = commLogEntryListField.ToArray();
			}
		}

	}

	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
	//[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	//[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class CommLogEntry
	{
		private DateTime? timeField;

		private string fromCallsignField;

		private string fromMessageNumberField;

		private string toCallsignField;

		private string toMessageNumberField;

		private string messageField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public DateTime? Time
		{
			get
			{
				return this.timeField;
			}
			set
			{
				this.timeField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string FromCallsign
		{
			get
			{
				return this.fromCallsignField;
			}
			set
			{
				this.fromCallsignField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string FromMessageNumber
		{
			get
			{
				return this.fromMessageNumberField;
			}
			set
			{
				this.fromMessageNumberField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string ToCallsign
		{
			get
			{
				return this.toCallsignField;
			}
			set
			{
				this.toCallsignField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string ToMessageNumber
		{
			get
			{
				return this.toMessageNumberField;
			}
			set
			{
				this.toMessageNumberField = value;
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
	}
}
