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

using static PacketMessagingTS.Core.Helpers.MessageOriginHelper;
using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using SharedCode.Helpers;

using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml.Media;

using Windows.UI;
using PacketMessagingTS.Core.Helpers;

namespace SharedCode
{
    // 
    // This source code was auto-generated by xsd, Version=4.6.81.0.
    // 

    public enum MessageState
    {
        Locked,         // Received or sent message or message in outbox
        Edit,           // Message in outbox opened for editing or draft message
        None,           // New message
        ResendSameID,   // Resend message with same ID
        ResendNewID,    // Resend message with new ID
    }

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
        private static LogHelper _logHelper = new LogHelper(log);


        private string fileNameField;

        private string areaField;

        private FormProviders formProviderField;

		private string pacFormNameField;

        private string pacFormTypeField;

        private string bBSNameField;

        private string tncNameField;

        private DateTime? jNOSDateField;

		private DateTime? messageSentTimeField;

		private DateTime? messageCreateTimeField = null;

        private DateTime? messageReceiveTimeField = null;

		private string messageNumberField;

		private string receiverMessageNumberField;

		private string senderMessageNumberField;

		private string messageFromField;

        private string messageToField;

        private int? messageSizeField;

        private string messageSubjectField;

        private bool messageOpenedField = false;

        private FormField[] arrayOfFormFieldsField;

        private string messageBodyField;

		private string mailUserNameField;

        private MessageState messageStateField;

        private FormControlAttribute.FormType formControlTypeField;

        private string movedFromFolderField;

        private MessageOrigin messageOriginField;


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
        public FormProviders FormProvider
        {
            get
            {
                return this.formProviderField;
            }
            set
            {
                this.formProviderField = value;
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
        public DateTime? JNOSDate
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
        public DateTime? CreateTime
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
				//if (value != null)
				//{
				//	messageReceiveTimeDisplayField = ConvertToDisplayTime((DateTime)this.messageReceiveTimeField);
				//}

			}
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

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("FormField", IsNullable = false)]
        public FormField[] FormFieldArray
        {
            get
            {
                return this.arrayOfFormFieldsField;
            }
            set
            {
                this.arrayOfFormFieldsField = value;
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
        public string MailUserName
		{
			get => mailUserNameField;
			set => mailUserNameField = value;
		}

        /// <remarks/>
        public MessageState MessageState
        {
            get => messageStateField;
            set => messageStateField = value;
        }

        /// <remarks/>
        public FormControlAttribute.FormType FormControlType
        {
            get => formControlTypeField;
            set => formControlTypeField = value;
        }

        public MessageOrigin MessageOrigin
        {
            get => messageOriginField;
            set => messageOriginField = value;
        }

        public string MovedFromFolder
        {
            get => movedFromFolderField;
            set => movedFromFolderField = value;
        }

        //[XmlIgnore]
        //public bool IsStillActive { get; set; } = true;

        //[XmlIgnore]
        //public string ActiveDescription => IsStillActive ? "Active" : "Retired";

        public void UpdateMessageSize()
        {
            int MessageBodyLength = string.IsNullOrEmpty(MessageBody) ? 0 : MessageBody.Length;
            int MessageSubjectLength = string.IsNullOrEmpty(Subject) ? 0 : Subject.Length;

            MessageSize = MessageBodyLength + MessageSubjectLength;
        }

        public override string ToString()
		{
			return Subject;
		}

		public bool CreateFileName()
		{
            bool success = false;

			if (!string.IsNullOrEmpty(MessageNumber) && !string.IsNullOrEmpty(PacFormType))
			{
				FileName = MessageNumber + "_" + PacFormType + ".xml";
                success = true;

            }
            return success;
        }

        public static PacketMessage Open(string filePath)
		{
            if (!File.Exists(filePath))
            {
                _logHelper.Log(LogLevel.Error, $"Failed to open {filePath}, file does not exist");
                return null;
            }
            //string fileName = Path.GetFileName(filePath);
            //string directory = Path.GetDirectoryName(filePath);
            ////StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            //StorageFolder storageFolder = await StorageFolder.GetFolderFromPathAsync(directory);
            //var storageItem = await storageFolder.TryGetItemAsync(fileName);
            //if (storageItem != null)
            //{
            //    Windows.Storage.FileProperties.BasicProperties basicProperties = await storageItem.GetBasicPropertiesAsync();
            //    var size = basicProperties.Size;
            //    if (size == 0)
            //    {
            //        _logHelper.Log(LogLevel.Error, $"Failed to open {filePath}, file size = 0");
            //        return null;
            //    }
            //}

            PacketMessage packetMessage;
			try
			{
				using (var stream = new FileStream(filePath, FileMode.Open))
				{
					using (TextReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
					{
						var serializer = new XmlSerializer(typeof(PacketMessage));
						packetMessage = (PacketMessage)serializer.Deserialize(reader);
					}
				}
				return packetMessage;
			}
			catch (Exception e)
			{
                _logHelper.Log(LogLevel.Error, $"Failed to open {filePath}, {e.Message}");
			}
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
                _logHelper.Log(LogLevel.Error, $"Failed to open {file?.Path}, {e}");
            }
            return null;
        }

        public void Save(string fileFolder)
		{
            string filePath = Path.Combine(fileFolder, this.FileName);
			//StreamWriter writer = null;
			//TextWriter writer = null;
			try
			{
				using (Stream stream = new FileStream(filePath, FileMode.Create))
				{
					using (TextWriter writer = new StreamWriter(stream))
					{
						XmlSerializer serializer = new XmlSerializer(typeof(PacketMessage));
						serializer.Serialize(writer, this);
					}
				}
			}
			catch (Exception e)
			{
                _logHelper.Log(LogLevel.Error, $"Failed to save {filePath}, {e.Message}");
			}

			// Now replace tab characters with escaped tab character
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
                _logHelper.Log(LogLevel.Error, $"Failed to read {filePath} for substituting tab characters, {e.Message}");
			}

			if (tabCharacterFound)
			{
				try
				{
					// Write xml file back with escaped tab characters
					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						using (StreamWriter outputFile = new StreamWriter(stream))
						{
							outputFile.Write(fileBuffer);
						}
					}
				}
				catch (Exception e)
				{
                    _logHelper.Log(LogLevel.Error, $"Failed to write {filePath} with escaped tab characters {e.Message}");
				}
			}
		}

        public static async Task<List<PacketMessage>> GetPacketMessages(string fileFolder)
        {
            List<PacketMessage> packetMessages = new List<PacketMessage>();

            if (!string.IsNullOrEmpty(fileFolder) && Directory.Exists(fileFolder))
            {
                await Task.Run(() =>
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(fileFolder);
                    FileInfo[] info = dirInfo.GetFiles("*.xml", SearchOption.TopDirectoryOnly);

                    foreach (FileInfo file in info)
                    {
                        string filePath = Path.Combine(fileFolder, file.Name);
                        PacketMessage packetMessage = Open(filePath);
                        if (packetMessage != null)
                        {
                            packetMessages?.Add(packetMessage);
                        }
                        else
                        {

                        }
                    }
                });
            }
            return packetMessages;
        }

        public static async Task<List<PacketMessage>> GetPacketMessages(StorageFolder storageFolder)
        {
            if (storageFolder is null)
                return null;

            List<PacketMessage> packetMessages = new List<PacketMessage>();

            List<string> fileTypeFilter = new List<string>() { ".xml" };
            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);

            var results = storageFolder.CreateFileQueryWithOptions(queryOptions);
            // Iterate over the results
            var files = await results.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                PacketMessage packetMessage = Open(file.Path);
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
        //private Control inputControlField;

        private string controlNameField;

        private string controlContentField;

        private ComboBoxPackItItem controlComboxContentField;

        private string misSpelsField;

        private string controlIndexField;

        ///// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        //public Control InputControl
        //{
        //    get
        //    {
        //        return this.inputControlField;
        //    }
        //    set
        //    {
        //        this.inputControlField = value;
        //    }
        //}

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
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        public ComboBoxPackItItem ControlComboxContent
        {
            get
            {
                return this.controlComboxContentField;
            }
            set
            {
                this.controlComboxContentField = value;
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
        public string ControlIndex
        {
            get => this.controlIndexField;
            set => this.controlIndexField = value;
        }
        
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public sealed partial class ComboBoxPackItItem
    {
        static Brush _backgroundBrush = new SolidColorBrush(Colors.White);
        static Brush _foregroundBrush = new SolidColorBrush(Colors.Black);

        private string itemField;

        private string packetDataField;

        private int selectedIndexField;

        private string backgroundColorField;

        private Brush backgroundBrushField;

        private Brush foregroundBrushField;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Item
        {
            get => this.itemField;
            set => this.itemField = value;
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PacketData
        {
            get => this.packetDataField;
            set => this.packetDataField = value;
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int SelectedIndex
        {
            get => selectedIndexField;
            set => selectedIndexField = value;
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BackgroundColor
        {
            get => this.backgroundColorField;
            set => this.backgroundColorField = value;
        }

        [System.Xml.Serialization.XmlIgnore]
        public Brush BackgroundBrush
        {
            get => this.backgroundBrushField;
            set => this.backgroundBrushField = value;
        }

        [System.Xml.Serialization.XmlIgnore]
        public Brush ForegroundBrush
        {
            get => this.foregroundBrushField;
            set => this.foregroundBrushField = value;
        }

        public ComboBoxPackItItem()
        { }

        public ComboBoxPackItItem(string item)
        {
            //throw new Exception("Obsolete");

            Item = item;
            PacketData = item;
            BackgroundBrush = _backgroundBrush;
            ForegroundBrush = _foregroundBrush;
        }

        //public ComboBoxPackItItem(string item, Color color)
        //{
        //    Item = item;
        //    Data = item;
        //    BackgroundColor = color.ToString();
        //}

        public ComboBoxPackItItem(string item, Brush brush)
        {
            //throw new Exception("Obsolete");

            Item = item;
            PacketData = item;
            BackgroundBrush = brush;
            ForegroundBrush = _foregroundBrush;
        }

        public ComboBoxPackItItem(string item, Brush brush, Brush foregroundBrush)
        {
            //throw new Exception("Obsolete");

            Item = item;
            PacketData = item;
            BackgroundBrush = brush;
            ForegroundBrush = foregroundBrush;
        }

        public ComboBoxPackItItem(string item, string packetData)
        {
            //throw new Exception("Obsolete");

            Item = item;
            PacketData = packetData;
        }

        public ComboBoxPackItItem(string item, string packetData, int index)
        {
            //throw new Exception("Obsolete");

            Item = item;
            PacketData = packetData;
            SelectedIndex = index;
            BackgroundBrush = _backgroundBrush;
            ForegroundBrush = _foregroundBrush;
        }

        //public Brush GetBackgroundBrush()
        //{
        //    byte a = Convert.ToByte(BackgroundColor.Substring(1, 2), 16);
        //    byte r = Convert.ToByte(BackgroundColor.Substring(3, 2), 16);
        //    byte g = Convert.ToByte(BackgroundColor.Substring(5, 2), 16);
        //    byte b = Convert.ToByte(BackgroundColor.Substring(7, 2), 16);
        //    Color color = ColorHelper.FromArgb(a, r, g, b);
        //    return new SolidColorBrush(color);
        //}

        public override string ToString()
        {
            //throw new Exception("Obsolete");

            return Item;
        }

    }
}
