﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MetroLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Popups;

namespace FormControlBaseClass
{
	// 
	// This source code was auto-generated by xsd, Version=4.6.81.0.
	// 


	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public sealed partial class PacketMessage
    {
		private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<PacketMessage>();

		private static double gridWidthField = 218;

		private string fileNameField;

        private string areaField;

		private string pacFormNameField;

        private string pacFormTypeField;

        private string bBSNameField;

        private string tncNameField;

        private DateTime jNOSDateField;

		private string jNOSDateDisplayField;

		private DateTime? messageSentTimeField;

		private string messageSentTimeDisplayField;

		private string messageCreateTimeField = null;

        private DateTime? messageReceiveTimeField = null;

		private string messageReceiveTimeDisplayField;

		private string messageNumberField;

		private string receiverMessageNumberField;

		private string senderMessageNumberField;

		private string messageFromField;

        private string messageToField;

        private int? messageSizeField;

        private string messageSubjectField;

        private FormField[] arrayOfFormFieldField;

        private string messageBodyField;

        private bool messageOpenedField = false;

		private string mailUserNameField;

        /// <remarks/>
        public string FileName
        {
            get
            {
                return this.fileNameField;
            }
            set
            {
                this.fileNameField = value;
            }
        }

        /// <remarks/>
        public string Area
        {
            get
            {
                return this.areaField;
            }
            set
            {
                this.areaField = value;
            }
        }

		/// <remarks/>
		public string PacFormName
		{
			get
			{
				return this.pacFormNameField;
			}
			set
			{
				this.pacFormNameField = value;
			}
		}

        /// <remarks/>
        public string PacFormType
        {
            get
            {
                return this.pacFormTypeField;
            }
            set
            {
                this.pacFormTypeField = value;
            }
        }

        /// <remarks/>
        public string BBSName
        {
            get
            {
                return this.bBSNameField;
            }
            set
            {
                this.bBSNameField = value;
            }
        }

        /// <remarks/>
        public string TNCName
        {
            get
            {
                return this.tncNameField;
            }
            set
            {
                this.tncNameField = value;
            }
        }

        /// <remarks/>
        public DateTime JNOSDate
		{
            get
            {
				return this.jNOSDateField;
			}
            set
            {
                this.jNOSDateField = value;
            }
        }

		/// <remarks/>
		public string JNOSDateDisplay
		{
			get => jNOSDateDisplayField;
			set => jNOSDateDisplayField = value;
		}

		/// <remarks/>
		public DateTime? SentTime
        {
            get
            {
                return this.messageSentTimeField;
            }
            set
            {
                this.messageSentTimeField = value;
            }
        }

		/// <remarks/>
		public string SentTimeDisplay
		{
			get => messageSentTimeDisplayField;
			set => messageSentTimeDisplayField = value;
		}

		/// <remarks/>
		public string CreateTime
        {
            get
            {
                return this.messageCreateTimeField;
            }
            set
            {
                this.messageCreateTimeField = value;
            }
        }

        /// <remarks/>
        public DateTime? ReceivedTime
        {
            get
            {
                return this.messageReceiveTimeField;
            }
            set
            {
                this.messageReceiveTimeField = value;
				if (value != null)
				{
					messageReceiveTimeDisplayField = ConvertToDisplayTime((DateTime)this.messageReceiveTimeField);
				}

			}
        }

		/// <remarks/>
		public string ReceivedTimeDisplay
		{
			get => messageReceiveTimeDisplayField;
			set => messageReceiveTimeDisplayField = value;
		}

		/// <remarks/>
		public string MessageNumber
        {
            get
            {
                return this.messageNumberField;
            }
            set
            {
                this.messageNumberField = value;
            }
        }

		public string ReceiverMessageNumber
		{
			get => receiverMessageNumberField;
			set => receiverMessageNumberField = value;
		}

		public string SenderMessageNumber
		{
			get => senderMessageNumberField;
			set => senderMessageNumberField = value;
		}

		/// <remarks/>
		public string MessageFrom
        {
            get
            {
                return this.messageFromField;
            }
            set
            {
                this.messageFromField = value;
            }
        }

        /// <remarks/>
        public string MessageTo
        {
            get
            {
                return this.messageToField;
            }
            set
            {
                this.messageToField = value;
            }
        }

        /// <remarks/>
        public int? MessageSize
        {
            get
            {
                return this.messageSizeField;
            }
            set
            {
                this.messageSizeField = value;
            }
        }

        /// <remarks/>
        public string Subject
        {
            get
            {
                return this.messageSubjectField;
            }
            set
            {
                this.messageSubjectField = value;
            }
        }
		
		/// <remarks/>
		[System.Xml.Serialization.XmlArrayItemAttribute("FormField", IsNullable = false)]
        public FormField[] FormFieldArray
        {
            get
            {
                return this.arrayOfFormFieldField;
            }
            set
            {
                this.arrayOfFormFieldField = value;
            }
        }

        /// <remarks/>
        public string MessageBody
        {
            get
            {
                return this.messageBodyField;
            }
            set
            {
                this.messageBodyField = value;
            }
        }

        /// <remarks/>
        public bool MessageOpened
        {
            get
            {
                return this.messageOpenedField;
            }
            set
            {
                this.messageOpenedField = value;
            }
        }

		public string MailUserName
		{
			get => mailUserNameField;
			set => mailUserNameField = value;
		}

		/// <remarks/>
		//public bool MessageReadOnly
		//{
		//    get
		//    {
		//        return this.messageReadOnlyField;
		//    }
		//    set
		//    {
		//        this.messageReadOnlyField = value;
		//    }
		//}

		public double GridWidth
		{
			get { return gridWidthField; }
			set => gridWidthField = value;
		}

		[XmlIgnore]
		public int Size
		{
			get
			{
				int MessageBodyLength = string.IsNullOrEmpty(MessageBody) ? 0 : MessageBody.Length;
				int MessageSubjectLength = string.IsNullOrEmpty(Subject) ? 0 : Subject.Length;

				return MessageBodyLength + MessageSubjectLength;
			}
		}

		public override string ToString()
		{
			return Subject;
		}

		private string ConvertToDisplayTime(DateTime dateTime)
		{
			return $"{dateTime.Month:d2}/{dateTime.Day:d2}/{dateTime.Year - 2000:d2} {dateTime.Hour:d2}:{dateTime.Minute:d2}";
		}

		public async void CreateFileName()
		{
			if (MessageNumber?.Length != 0)
			{
				FileName = MessageNumber + "_" + PacFormName + ".xml";
			}
			else if (MessageNumber == null || MessageNumber?.Length == 0)
			{
				var messageDialog = new MessageDialog("Message number does not exist.\nContact support.");
				await messageDialog.ShowAsync();
			}
		}

        public static PacketMessage Open(string filePath)
		{
			if (!File.Exists(filePath))
				return null;

			StreamReader reader = null;
			//TextReader reader = null;
			PacketMessage packetMessage;
			try
			{
				using (var stream = new FileStream(filePath, FileMode.Open))
				{
					using (reader = new StreamReader(stream, System.Text.Encoding.UTF8))
					{
						var serializer = new XmlSerializer(typeof(PacketMessage));
						packetMessage = (PacketMessage)serializer.Deserialize(reader);
					}
				}
				return packetMessage;
			}
			catch (Exception e)
			{
                LogHelper.Log(LogLevel.Error, $"Failed to open {filePath}, {e}");
			}
			//finally
			//{
			//    reader?.Dispose();
			//}
			return null;
		}

		public static PacketMessage Open(StorageFile file)
        {
			StreamReader reader = null;
			//TextReader reader = null;
			PacketMessage packetMessage;
			try
			{
				//file = await storageFolder.GetFileAsync(fileName);
				using (var stream = new FileStream(file.Path, FileMode.Open))
				{
					using (reader = new StreamReader(stream, System.Text.Encoding.UTF8))
					{
						var serializer = new XmlSerializer(typeof(PacketMessage));
						packetMessage = (PacketMessage)serializer.Deserialize(reader);
					}
				}
				return packetMessage;
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Failed to open {file?.Path}, {e}");
			}
			//finally
			//{
			//    reader?.Dispose();
			//}
			return null;
		}

		public void Save(string fileFolder)
		{
			string filePath = fileFolder + @"\" + this.FileName;
			//StreamWriter writer = null;
			TextWriter writer = null;
			try
			{
				using (Stream stream = new FileStream(filePath, FileMode.OpenOrCreate))
				{
					using (writer = new StreamWriter(stream))
					{
						XmlSerializer serializer = new XmlSerializer(typeof(PacketMessage));
						serializer.Serialize(writer, this);
					}
				}
			}
			catch (Exception e)
			{
                LogHelper.Log(LogLevel.Error, $"Failed to save {filePath}", e.Message);
			}
			finally
			{
				writer?.Dispose();
			}

			// Noe replace tab characters with escaped tab character
			string fileBuffer = "";
			bool tabCharacterFound = false;
			try
			{   // Open the text file using a stream reader.
				using (StreamReader sr = new StreamReader(new FileStream(filePath, FileMode.Open)))
				{
					// Read the stream to a string, and write the string to the console.
					fileBuffer = sr.ReadToEnd();
					int index = fileBuffer.IndexOf('\t');
					if (index < 0)
						return;

					tabCharacterFound = true;
					fileBuffer = fileBuffer.Replace("\t", "&#x9;");
				}
			}
			catch (Exception e)
			{
                LogHelper.Log(LogLevel.Error, $"Failed to read {filePath} for substituting tab characters, {e}");
			}

			if (tabCharacterFound)
			{
				try
				{
					// Write xml file back with escaped tab characters
					using (var stream = new FileStream(filePath, FileMode.CreateNew))
					{
						using (StreamWriter outputFile = new StreamWriter(stream))
						{
							outputFile.Write(fileBuffer);
						}
					}
				}
				catch (Exception e)
				{
                    LogHelper.Log(LogLevel.Error, $"Failed to write {filePath} with escaped tab characters {e}");
				}
			}
		}


		public static async System.Threading.Tasks.Task<List<PacketMessage>> GetPacketMessages(StorageFolder storageFolder)
		{
			List<PacketMessage> packetMessages = new List<PacketMessage>();

			List<string> fileTypeFilter = new List<string>() { ".xml" };
			QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);

			var results = storageFolder.CreateFileQueryWithOptions(queryOptions);
            // Iterate over the results
            var files = await results.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                PacketMessage packetMessage = Open(file);
				if (packetMessage != null)
				{
					packetMessages.Add(packetMessage);
				}
			}
			return packetMessages;
		}

	}


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public sealed partial class FormField
    {

        private string controlNameField;

        private string controlContentField;

        private string misSpelsField;

        private int pacFormIndex;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ControlName
        {
            get
            {
                return this.controlNameField;
            }
            set
            {
                this.controlNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ControlContent
        {
            get
            {
                return this.controlContentField;
            }
            set
            {
                this.controlContentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MisSpells
        {
            get
            {
                return this.misSpelsField;
            }
            set
            {
                this.misSpelsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int PacFormIndex
        {
            get => this.pacFormIndex;
            set => this.pacFormIndex = value;
        }

    }
}