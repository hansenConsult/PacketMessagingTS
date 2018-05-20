using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using Windows.Devices.SerialCommunication;

namespace PacketMessagingTS.ViewModels
{
    public class TNCSettingsViewModel : BaseViewModel
    {

        Dictionary<string, bool> SaveEnabledDictionary;
    

        public TNCSettingsViewModel()
        {
            SaveEnabledDictionary = new Dictionary<string, bool>();
        }

        public void ResetChangedProperty()
        {
            string[] keyArray = new string[SaveEnabledDictionary.Count];

            int i = 0;
            foreach (string key in SaveEnabledDictionary.Keys)
            {
                keyArray[i++] = key;
            }
            for (i = 0; i < SaveEnabledDictionary.Count; i++)
            {
                SaveEnabledDictionary[keyArray[i]] = false;
            }
            IsAppBarSaveEnabled = false;
        }

        private bool SaveEnabled(bool propertyChanged, [CallerMemberName]string propertyName = "")
        {
            SaveEnabledDictionary[propertyName] = propertyChanged;
            bool saveEnabled = false;
            foreach (bool value in SaveEnabledDictionary.Values)
            {
                saveEnabled |= value;
            }
            return saveEnabled;
        }

        private Int64 tncDeviceSelectedIndex;
        public Int64 TNCDeviceSelectedIndex
        {
            get
            {
                GetProperty(ref tncDeviceSelectedIndex);
                if (SavedTNCDevice == null && !TNCDeviceArray.Instance.TNCDeviceList[Convert.ToInt32(tncDeviceSelectedIndex)].Name.Contains("E-Mail"))
                {
                    SavedTNCDevice = TNCDeviceArray.Instance.TNCDeviceList[Convert.ToInt32(tncDeviceSelectedIndex)];
                }

                return tncDeviceSelectedIndex;
            }
            set
            {
                SetProperty(ref tncDeviceSelectedIndex, value, true);
                if (!TNCDeviceArray.Instance.TNCDeviceList[Convert.ToInt32(tncDeviceSelectedIndex)].Name.Contains("E-Mail"))
                {
                    SavedTNCDevice = TNCDeviceArray.Instance.TNCDeviceList[Convert.ToInt32(tncDeviceSelectedIndex)];
                }
            }
        }

        private TNCDevice savedTNCDevice;
        public TNCDevice SavedTNCDevice
        {
            get => savedTNCDevice;
            set
            {
                savedTNCDevice = value;

                TNCInitCommandsPre = savedTNCDevice.InitCommands.Precommands;
                TNCInitCommandsPost = savedTNCDevice.InitCommands.Postcommands;
                IsToggleSwitchOn = savedTNCDevice.CommPort.IsBluetooth;
                TNCComPort = savedTNCDevice.CommPort.Comport;
                TNCComName = savedTNCDevice.CommPort.BluetoothName;
                TNCComBaudRate = savedTNCDevice.CommPort.Baudrate;
                TNCComDatabits = savedTNCDevice.CommPort.Databits;
                TNCComStopbits = savedTNCDevice.CommPort.Stopbits;
                TNCComParity = savedTNCDevice.CommPort.Parity;
                TNCComHandshake = savedTNCDevice.CommPort.Flowcontrol;
                TNCCommandsMyCall = savedTNCDevice.Commands.MyCall;
                TNCCommandsConnect = savedTNCDevice.Commands.Connect;
                TNCCommandsRetry = savedTNCDevice.Commands.Retry;
                TNCCommandsConversMode = savedTNCDevice.Commands.Conversmode;
                TNCCommandsDateTime = savedTNCDevice.Commands.Datetime;
                TNCPromptsCommand = savedTNCDevice.Prompts.Command;
                TNCPromptsTimeout = savedTNCDevice.Prompts.Timeout;
                TNCPromptsConnected = savedTNCDevice.Prompts.Connected;
                TNCPromptsDisconnected = savedTNCDevice.Prompts.Disconnected;

                ResetChangedProperty();
            }
        }

        private string tncInitCommandsPre;
        public string TNCInitCommandsPre
        {
            get => tncInitCommandsPre;
            set
            {
                SetProperty(ref tncInitCommandsPre, value);

                bool changed = SavedTNCDevice.InitCommands.Precommands != tncInitCommandsPre;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncInitCommandsPost;
        public string TNCInitCommandsPost
        {
            get => tncInitCommandsPost;
            set
            {
                SetProperty(ref tncInitCommandsPost, value);

                bool changed = SavedTNCDevice.InitCommands.Precommands != tncInitCommandsPre;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private bool isToggleSwitchOn;
        public bool IsToggleSwitchOn
        {
            get => isToggleSwitchOn;
            set
            {
                SetProperty(ref isToggleSwitchOn, value);

                if (isToggleSwitchOn)
                {
                    TNCComPortVisible = Windows.UI.Xaml.Visibility.Collapsed;
                    TNCComNameVisible = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    TNCComPortVisible = Windows.UI.Xaml.Visibility.Visible;
                    TNCComNameVisible = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
        }

        private string tncComPort;
        public string TNCComPort
        {
            get => tncComPort;
            set
            {
                SetProperty(ref tncComPort, value);

                bool changed = SavedTNCDevice.CommPort.Comport != tncComPort;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private Windows.UI.Xaml.Visibility tncComPortVisible;
        public Windows.UI.Xaml.Visibility TNCComPortVisible
        {
            get => tncComPortVisible;
            set
            {
                SetProperty(ref tncComPortVisible, value);
            }
        }

        public IEnumerable<SerialParity> SerialParityValues
        {
            get
            {
                return Enum.GetValues(typeof(SerialParity))
                    .Cast<SerialParity>();
            }
        }

        public IEnumerable<SerialStopBitCount> SerialStopBitCountValues
        {
            get
            {
                return Enum.GetValues(typeof(SerialStopBitCount))
                    .Cast<SerialStopBitCount>();
            }
        }

        public IEnumerable<SerialHandshake> MyEnumTypeValues
        {
            get
            {
                return Enum.GetValues(typeof(SerialHandshake))
                    .Cast<SerialHandshake>();
            }
        }
        private string tncComName;
        public string TNCComName
        {
            get => tncComName;
            set
            {
                SetProperty(ref tncComName, value);

                bool changed = SavedTNCDevice.CommPort.BluetoothName != tncComName;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private Windows.UI.Xaml.Visibility tncComNameVisible;
        public Windows.UI.Xaml.Visibility TNCComNameVisible
        {
            get => tncComNameVisible;
            set
            {
                SetProperty(ref tncComNameVisible, value);
            }
        }

        private uint tncComBaudRate;
        public uint TNCComBaudRate
        {
            get => tncComBaudRate;
            set
            {
                SetProperty(ref tncComBaudRate, value);

                bool changed = SavedTNCDevice.CommPort.Baudrate != tncComBaudRate;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private ushort tncComDatabits;
        public ushort TNCComDatabits
        {
            get => tncComDatabits;
            set
            {
                SetProperty(ref tncComDatabits, value);

                bool changed = SavedTNCDevice.CommPort.Databits != tncComDatabits;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private SerialStopBitCount tncComStopbits;
        public SerialStopBitCount TNCComStopbits
        {
            get => tncComStopbits;
            set
            {
                SetProperty(ref tncComStopbits, value);

                bool changed = SavedTNCDevice.CommPort.Stopbits != tncComStopbits;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private SerialParity tncComParity;
        public SerialParity TNCComParity
        {
            get => tncComParity;
            set
            {
                SetProperty(ref tncComParity, value);

                bool changed = SavedTNCDevice.CommPort.Parity != tncComParity;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private SerialHandshake tncComHandshake;
        public SerialHandshake TNCComHandshake
        {
            get { return tncComHandshake; }
            set
            {
                SetProperty(ref tncComHandshake, value);

                bool changed = SavedTNCDevice.CommPort.Flowcontrol != tncComHandshake;

                IsAppBarSaveEnabled = SaveEnabled(changed);

            }
        }

        private string tncCommandsMyCall;
        public string TNCCommandsMyCall
        {
            get => tncCommandsMyCall;
            set
            {
                SetProperty(ref tncCommandsMyCall, value);
                bool changed = SavedTNCDevice.Commands.MyCall != tncCommandsMyCall;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncCommandsConnect;
        public string TNCCommandsConnect
        {
            get => tncCommandsConnect;
            set
            {
                SetProperty(ref tncCommandsConnect, value);
                bool changed = SavedTNCDevice.Commands.Connect != value;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncCommandsRetry;
        public string TNCCommandsRetry
        {
            get => tncCommandsRetry;
            set
            {
                SetProperty(ref tncCommandsRetry, value);
                bool changed = SavedTNCDevice.Commands.Retry != tncCommandsRetry;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncCommandsConversMode;
        public string TNCCommandsConversMode
        {
            get => tncCommandsConversMode;
            set
            {
                SetProperty(ref tncCommandsConversMode, value);
                bool changed = SavedTNCDevice.Commands.Retry != tncCommandsConversMode;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncCommandsDateTime;
        public string TNCCommandsDateTime
        {
            get => tncCommandsDateTime;
            set
            {
                SetProperty(ref tncCommandsDateTime, value);
                bool changed = SavedTNCDevice.Commands.Retry != tncCommandsDateTime;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncPromptsCommand;
        public string TNCPromptsCommand
        {
            get => GetProperty(ref tncPromptsCommand);
            set
            {
                SetProperty(ref tncPromptsCommand, value);

                bool changed = SavedTNCDevice.Prompts.Command != tncPromptsCommand;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncPromptsTimeout;
        public string TNCPromptsTimeout
        {
            get => GetProperty(ref tncPromptsTimeout);
            set
            {
                SetProperty(ref tncPromptsTimeout, value);
                bool changed = SavedTNCDevice.Prompts.Timeout != tncPromptsTimeout;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncPromptsConnected;
        public string TNCPromptsConnected
        {
            get => GetProperty(ref tncPromptsConnected);
            set
            {
                SetProperty(ref tncPromptsConnected, value);

                bool changed = SavedTNCDevice.Prompts.Connected != tncPromptsConnected;

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncPromptsDisconnected;
        public string TNCPromptsDisconnected
        {
            get => GetProperty(ref tncPromptsDisconnected);
            set
            {
                SetProperty(ref tncPromptsDisconnected, value);

                bool changed = SavedTNCDevice.Prompts.Disconnected != tncPromptsDisconnected;

                IsAppBarSaveEnabled = SaveEnabled(changed);

            }
        }
        #region Mail Settings
        private Int64 mailAccountSelectedIndex;
        public Int64 MailAccountSelectedIndex
        {
            get
            {
                GetProperty(ref mailAccountSelectedIndex);
                if (mailAccountSelectedIndex >= 0)
                {
                    CurrentMailAccount = EmailAccountArray.Instance.EmailAccounts[mailAccountSelectedIndex];
                }
                return mailAccountSelectedIndex;
            }
            set
            {
                SetProperty(ref mailAccountSelectedIndex, value, true);
                CurrentMailAccount = EmailAccountArray.Instance.EmailAccounts[mailAccountSelectedIndex];
            }
        }

        EmailAccount currentMailAccount;
        public EmailAccount CurrentMailAccount
        {
            get => GetProperty(ref currentMailAccount);
            //MailServer = EmailAccountArray.Instance.EmailAccounts [index].MailServer;
            //MailPortString = EmailAccountArray.Instance.EmailAccounts [index].MailServerPort.ToString();
            //IsMailSSL = EmailAccountArray.Instance.EmailAccounts[index].MailIsSSLField;
            //MailUserName = EmailAccountArray.Instance.EmailAccounts[index].MailUserName;
            //MailPassword = EmailAccountArray.Instance.EmailAccounts[index].MailPassword;
            set
            {
                SetProperty(ref currentMailAccount, value);

                MailServer = CurrentMailAccount.MailServer;
                MailPort = CurrentMailAccount.MailServerPort;
                IsMailSSL = CurrentMailAccount.MailIsSSLField;
                MailUserName = CurrentMailAccount.MailUserName;
                MailPassword = CurrentMailAccount.MailPassword;
            }
        }

        private string mailServer;
        public string MailServer
        {
            get => GetProperty(ref mailServer);
            set
            {
                SetProperty(ref mailServer, value);
                //foreach (EmailAccount account in EmailAccountArray.Instance.EmailAccounts)
                //{
                //	if (account.MailServer == MailServer)
                //	{
                //		MailPortString = account.MailServerPort.ToString();
                //		IsMailSSL = account.MailIsSSLField;
                //		MailUserName = account.MailUserName;
                //		MailPassword = account.MailPassword;
                //		break;
                //	}
                //}
                bool changed = CurrentMailAccount.MailServer != mailServer;

                IsAppBarSaveEnabled = SaveEnabled(changed);

                //        TODO       Services.SMTPClient.SmtpClient.Instance.Server = MailServer;
            }
        }

        private string mailPortString;
        public string MailPortString
        {
            get => mailPortString;
            set
            {
                SetProperty(ref mailPortString, value);
                bool changed = (Convert.ToInt32(MailPortString) != CurrentMailAccount.MailServerPort);
                if (changed)
                    MailPort = Convert.ToInt32(mailPortString);

                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private Int32 mailPort;
        public Int32 MailPort
        {
            get => mailPort;
            set
            {
                SetProperty(ref mailPort, value);
                if (MailPortString != MailPort.ToString())
                    MailPortString = MailPort.ToString();

                bool changed = CurrentMailAccount.MailServerPort != mailPort;

                IsAppBarSaveEnabled = SaveEnabled(changed);

                //  TODO               Services.SMTPClient.SmtpClient.Instance.Port = MailPort;
            }
        }

        bool isMailSSL;
        public bool IsMailSSL
        {
            get => isMailSSL;
            set
            {
                SetProperty(ref isMailSSL, value);

                bool changed = CurrentMailAccount.MailIsSSLField != isMailSSL;

                IsAppBarSaveEnabled = SaveEnabled(changed);
                // TODO                Services.SMTPClient.SmtpClient.Instance.IsSsl = IsMailSSL;
            }
        }

        private string mailUserName;
        public string MailUserName
        {
            get => mailUserName;
            set
            {
                SetProperty(ref mailUserName, value);

                bool changed = CurrentMailAccount.MailUserName != mailUserName;

                IsAppBarSaveEnabled = SaveEnabled(changed);


                //  TODO                Services.SMTPClient.SmtpClient.Instance.UserName = MailUserName;
            }
        }

        private string mailPassword;
        public string MailPassword
        {
            get => mailPassword;
            set
            {
                SetProperty(ref mailPassword, value);

                bool changed = CurrentMailAccount.MailPassword != mailPassword;

                IsAppBarSaveEnabled = SaveEnabled(changed);

                //  TODO               Services.SMTPClient.SmtpClient.Instance.Password = MailPassword;
            }
        }
        #endregion Mail Settings
        private bool isAppBarAddEnabled = true;
        public bool IsAppBarAddEnabled
        {
            get => isAppBarAddEnabled;
            set => SetProperty(ref isAppBarAddEnabled, value);
        }

        private bool isAppBarSaveEnabled = false;
        public bool IsAppBarSaveEnabled
        {
            get => isAppBarSaveEnabled;
            set => SetProperty(ref isAppBarSaveEnabled, value);
        }

        private bool isAppBarDeleteEnabled = true;
        public bool IsAppBarDeleteEnabled
        {
            get => isAppBarDeleteEnabled;
            set => SetProperty(ref isAppBarDeleteEnabled, value);
        }

        private bool isAppBarEditEnabled = true;
        public bool IsAppBarEditEnabled
        {
            get => isAppBarEditEnabled;
            set => SetProperty(ref isAppBarEditEnabled, value);
        }
    }
}
