﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

using MetroLog;

using SharedCode;

using Windows.Devices.SerialCommunication;
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
    public partial class TNCDeviceArray
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<TNCDeviceArray>();
        private static readonly LogHelper _logHelper = new LogHelper(log);


        public static string tncFileName = "TNCData.xml";
        private static volatile TNCDeviceArray _instance;
        private static object _syncRoot = new Object();
   
		private TNCDevice[] deviceField;


        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Device")]
        public TNCDevice[] TNCDevices
        {
            get
            {
                return this.deviceField;
            }
            set
            {
                this.deviceField = value;
            }
        }

        private List<TNCDevice> tncDeviceList;
        [System.Xml.Serialization.XmlIgnore]
        public List<TNCDevice> TNCDeviceList
        {
            get => tncDeviceList;
            set => tncDeviceList = value;
        }

        public void TNCDeviceListUpdate(int index, TNCDevice tncDevice)
        {
            tncDeviceList.RemoveAt(index);
            tncDeviceList.Insert(index, tncDevice);
        }

        private TNCDeviceArray()
        {
            deviceField = new TNCDevice[0];
            tncDeviceList = new List<TNCDevice>();
        }

        public static TNCDeviceArray Instance
        {
            get
            {
                if (_instance is null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance is null)
                            _instance = new TNCDeviceArray();
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
                ulong size = 0;
                var storageItem = await localFolder.TryGetItemAsync(tncFileName);
                if (storageItem != null)
                {
                    Windows.Storage.FileProperties.BasicProperties basicProperties = await storageItem.GetBasicPropertiesAsync();
                    size = basicProperties.Size;
                }
                if (storageItem is null || size == 0)
                {
					// Copy the file from the install folder to the local folder
					var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
					var storageFile = await folder.GetFileAsync(tncFileName);
					if (storageFile != null)
					{
						await storageFile.CopyAsync(localFolder, tncFileName, NameCollisionOption.ReplaceExisting);
					}
				}

				StorageFile file = await localFolder.GetFileAsync(tncFileName);

				using (FileStream stream = new FileStream(file.Path, FileMode.Open))
				{
					using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
					{
						XmlSerializer serializer = new XmlSerializer(typeof(TNCDeviceArray));
                        _instance = (TNCDeviceArray)serializer.Deserialize(reader);

                        _instance.TNCDeviceList = _instance.TNCDevices.ToList();
                    }
				}
			}
			catch (FileNotFoundException e)
			{
                _logHelper.Log(LogLevel.Error, $"Error opening {e.Message} {e}");
			}
			catch (Exception e)
			{
                _logHelper.Log(LogLevel.Error, $"Failed to read TNC data file {e}");
			}
			if (_instance.TNCDevices is null || _instance.TNCDevices.Length == 0)
			{
                //System.Windows.MessageDialog.Show(tncFileName + " missing");
                _logHelper.Log(LogLevel.Error, $"{tncFileName} missing");
			}
		}

        public async Task SaveAsync()
        {
            //TNCDevice[] deviceList = TNCDeviceList.ToArray();
            //deviceField = new TNCDevice[TNCDeviceList.Count];
            //deviceList.CopyTo(deviceField, 0);
            TNCDevices = TNCDeviceList.ToArray();

            if (TNCDevices.Length == 0)
                return;

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            try
            {

                var storageItem = await localFolder.CreateFileAsync(tncFileName, CreationCollisionOption.ReplaceExisting);
                if (storageItem != null)
                {
                    using (Stream writer = new FileStream(storageItem.Path, FileMode.Create))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(TNCDeviceArray));           
                        serializer.Serialize(writer, this);
                    }
                }
                else
                {
                    _logHelper.Log(LogLevel.Error, $"Folder not found {localFolder.Path}");
                }
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error,$"Error saving file {tncFileName}, {e}");
            }
        }

        public void AddTNCDevice(TNCDevice tncDevice)
        {
            TNCDeviceList.Add(tncDevice);
        }

        public void DeleteTNCDevice(TNCDevice tncDevice)
        {
            TNCDeviceList.Remove(tncDevice);
        }

        public int GetSelectedIndexFromDeviceName(string deviceName)
        {
            for (int i = 0; i < TNCDeviceList.Count; i++)
            {
                if (deviceName == TNCDeviceList[i].Name)
                {
                    return i;
                }
            }
            return -1;
        }

        //public static void CreateBackupFile()
        //{
        //    //Create a backup
        //    string backupFilePath = tncFilePath + ".bak";
        //    File.Copy(tncFilePath, backupFilePath, true);
        //    File.Delete(tncFilePath);
        //}
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class TNCDevice
    {
        private TNCDevicePrompts promptsField;

        private TNCDeviceCommands commandsField;

        private TNCDeviceInitCommands initCommandsField;

        private TNCDeviceCommPort commPortField;

        private string mailUserNameField;

        private string nameField;


        /// <remarks/>
        public TNCDevicePrompts Prompts
        {
            get
            {
                return this.promptsField;
            }
            set
            {
                this.promptsField = value;
            }
        }

        /// <remarks/>
        public TNCDeviceCommands Commands
        {
            get
            {
                return this.commandsField;
            }
            set
            {
                this.commandsField = value;
            }
        }

        /// <remarks/>
        public TNCDeviceInitCommands InitCommands
        {
            get
            {
                return this.initCommandsField;
            }
            set
            {
                this.initCommandsField = value;
            }
        }

        /// <remarks/>
        public TNCDeviceCommPort CommPort
        {
            get
            {
                return this.commPortField;
            }
            set
            {
                this.commPortField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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

		public TNCDevice()
		{
			Name = "";
			Prompts = new TNCDevicePrompts()
			{
				Command = "",
				Connected = "",
				Disconnected = "",
				Timeout = ""
			};
			Commands = new TNCDeviceCommands()
			{
				Connect = "",
				Conversmode = "",
				Datetime = "",
				MyCall = "",
				Retry = ""
			};
			InitCommands = new TNCDeviceInitCommands()
			{
				Precommands = "",
				Postcommands = ""
			};
			CommPort = new TNCDeviceCommPort()
			{
				IsBluetooth = false,
				BluetoothName = "",
				DeviceId = "",
				Comport = "",
				Baudrate = 9600,
				Databits = 8,
				Parity = Parity.None,
				Stopbits = StopBits.One,
				Flowcontrol = Handshake.RequestToSend
			};
		}

		public TNCDevice(TNCDevice tncDevice)
		{
			Prompts = new TNCDevicePrompts()
			{
				Command = tncDevice.Prompts?.Command,
				Connected = tncDevice.Prompts?.Connected,
				Disconnected = tncDevice.Prompts?.Disconnected,
				Timeout = tncDevice.Prompts?.Timeout
			};
			Commands = new TNCDeviceCommands()
			{
				Connect = tncDevice.Commands?.Connect,
				Conversmode = tncDevice.Commands?.Conversmode,
				Datetime = tncDevice.Commands?.Datetime,
				MyCall = tncDevice.Commands?.MyCall,
				Retry = tncDevice.Commands?.Retry
			};
			InitCommands = new TNCDeviceInitCommands()
			{
				Precommands = tncDevice.InitCommands?.Precommands,
				Postcommands = tncDevice.InitCommands?.Postcommands
			};
			CommPort = new TNCDeviceCommPort()
			{
				IsBluetooth = tncDevice.CommPort.IsBluetooth,
				BluetoothName = tncDevice.CommPort?.BluetoothName,
				DeviceId = tncDevice.CommPort?.DeviceId,
				Comport = tncDevice.CommPort?.Comport,
				Baudrate = tncDevice.CommPort.Baudrate,
				Databits = tncDevice.CommPort.Databits,
				Parity = tncDevice.CommPort.Parity,
				Stopbits = tncDevice.CommPort.Stopbits,
				Flowcontrol = tncDevice.CommPort.Flowcontrol
			};
			Name = tncDevice.Name;
		}

		public override string ToString() => Name;
	}

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class TNCDevicePrompts
    {

        private string commandField;

        private string timeoutField;

		private string connectedField;

		private string disconnectedField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Command
        {
            get
            {
                return this.commandField;
            }
            set
            {
                this.commandField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Timeout
        {
            get
            {
                return this.timeoutField;
            }
            set
            {
                this.timeoutField = value;
            }
        }

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Connected
		{
			get
			{
				return this.connectedField;
			}
			set
			{
				this.connectedField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
        public string Disconnected
        {
            get
            {
                return this.disconnectedField;
            }
            set
            {
                this.disconnectedField = value;
            }
        }

		public static bool IsTNCDevicePromptsEqual(TNCDevicePrompts prompts1, TNCDevicePrompts prompts2)
		{
            if (prompts1.Command == prompts2.Command
                && prompts1.Connected == prompts2.Connected
                && prompts1.Disconnected == prompts2.Disconnected
                && prompts1.Timeout == prompts2.Timeout)
                return true;
            else
                return false;
        }
	}

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class TNCDeviceCommands
    {

        private string myCallField;

        private string connectField;

        private string retryField;

        private string conversmodeField;

        private string datetimeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MyCall
        {
            get
            {
                return this.myCallField;
            }
            set
            {
                this.myCallField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Connect
        {
            get
            {
                return this.connectField;
            }
            set
            {
                this.connectField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Retry
        {
            get
            {
                return this.retryField;
            }
            set
            {
                this.retryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Conversmode
        {
            get
            {
                return this.conversmodeField;
            }
            set
            {
                this.conversmodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Datetime
        {
            get
            {
                return this.datetimeField;
            }
            set
            {
                this.datetimeField = value;
            }
        }

        public static bool IsTNCDeviceCommandsEqual(TNCDeviceCommands commands1, TNCDeviceCommands commands2)
        {
            if (commands1.Retry == commands2.Retry
                && commands1.MyCall == commands2.MyCall
                && commands1.Datetime == commands2.Datetime
                && commands1.Conversmode == commands2.Conversmode
                && commands1.Connect == commands2.Connect)
                return true;
            else
                return false;
        }

    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class TNCDeviceInitCommands
    {

        private string precommandsField;

        private string postcommandsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Precommands
        {
            get
            {
                return this.precommandsField;
            }
            set
            {
                this.precommandsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Postcommands
        {
            get
            {
                return this.postcommandsField;
            }
            set
            {
                this.postcommandsField = value;
            }
        }

        public static bool IsTNCDeviceInitCommandsEqual(TNCDeviceInitCommands initCommands1, TNCDeviceInitCommands initCommands2)
        {
            if (initCommands1.Precommands == initCommands2.Precommands
                && initCommands1.Postcommands == initCommands2.Postcommands)
                return true;
            else
                return false;
        }

    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class TNCDeviceCommPort
    {
		private bool isBluetoothField;

		private string bluetoothNameField;

		private string deviceIdField;

		private string comportField;

        private ushort baudrateField;

        private ushort databitsField;

        private Parity parityField;

        private StopBits stopbitsField;

        private Handshake flowcontrolField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public bool IsBluetooth
		{
			get
			{
				return this.isBluetoothField;
			}
			set
			{
				this.isBluetoothField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string BluetoothName
		{
			get
			{
				return this.bluetoothNameField;
			}
			set
			{
				this.bluetoothNameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string DeviceId
		{
			get
			{
				return this.deviceIdField;
			}
			set
			{
				this.deviceIdField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
        public string Comport
        {
            get
            {
                return this.comportField;
            }
            set
            {
                this.comportField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort Baudrate
        {
            get
            {
                return this.baudrateField;
            }
            set
            {
                this.baudrateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort Databits
        {
            get
            {
                return this.databitsField;
            }
            set
            {
                this.databitsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public Parity Parity
        {
            get
            {
                return this.parityField;
            }
            set
            {
                this.parityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public StopBits Stopbits
        {
            get
            {
                return this.stopbitsField;
            }
            set
            {
                this.stopbitsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public Handshake Flowcontrol
        {
            get
            {
                return this.flowcontrolField;
            }
            set
            {
                this.flowcontrolField = value;
            }
        }

        public static bool IsTNCDeviceComportsEqual(TNCDeviceCommPort comport1, TNCDeviceCommPort comport2)
        {
            if (comport1.IsBluetooth == comport2.IsBluetooth
                && comport1.BluetoothName == comport2.BluetoothName
                && comport1.Comport == comport2.Comport
                && comport1.Parity == comport2.Parity
                && comport1.Baudrate == comport2.Baudrate
                && comport1.Databits == comport2.Databits
                && comport1.Stopbits == comport2.Stopbits
                && comport1.Flowcontrol == comport2.Flowcontrol)
                return true;
            else
                return false;
        }

    }
}
