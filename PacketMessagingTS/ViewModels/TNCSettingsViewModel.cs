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

    

        public TNCSettingsViewModel()
        {
            
        }

        public override void ResetChangedProperty()
        {
            base.ResetChangedProperty();
            IsAppBarSaveEnabled = false;
        }

        private int tncDeviceSelectedIndex;
        public int TNCDeviceSelectedIndex
        {
            get => GetProperty(ref tncDeviceSelectedIndex);
            set
            { 
                SetProperty(ref tncDeviceSelectedIndex, value, true);
                //if (!TNCDeviceArray.Instance.TNCDeviceList[tncDeviceSelectedIndex].Name.Contains("E-Mail"))
                {
                    CurrentTNCDevice = TNCDeviceArray.Instance.TNCDeviceList[tncDeviceSelectedIndex];
                }
            }
        }

        private TNCDevice currentTNCDevice;
        public TNCDevice CurrentTNCDevice
        {
            get => currentTNCDevice;
            set
            {
                currentTNCDevice = value;

                TNCInitCommandsPre = currentTNCDevice.InitCommands.Precommands;
                TNCInitCommandsPost = currentTNCDevice.InitCommands.Postcommands;
                IsToggleSwitchOn = currentTNCDevice.CommPort.IsBluetooth;
                TNCComPort = currentTNCDevice.CommPort.Comport;
                TNCComName = currentTNCDevice.CommPort.BluetoothName;
                TNCComBaudRate = currentTNCDevice.CommPort.Baudrate;
                TNCComDatabits = currentTNCDevice.CommPort.Databits;
                TNCComStopbits = currentTNCDevice.CommPort.Stopbits;
                TNCComParity = currentTNCDevice.CommPort.Parity;
                TNCComHandshake = currentTNCDevice.CommPort.Flowcontrol;
                TNCCommandsMyCall = currentTNCDevice.Commands.MyCall;
                TNCCommandsConnect = currentTNCDevice.Commands.Connect;
                TNCCommandsRetry = currentTNCDevice.Commands.Retry;
                TNCCommandsConversMode = currentTNCDevice.Commands.Conversmode;
                TNCCommandsDateTime = currentTNCDevice.Commands.Datetime;
                TNCPromptsCommand = currentTNCDevice.Prompts.Command;
                TNCPromptsTimeout = currentTNCDevice.Prompts.Timeout;
                TNCPromptsConnected = currentTNCDevice.Prompts.Connected;
                TNCPromptsDisconnected = currentTNCDevice.Prompts.Disconnected;

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

                bool changed = CurrentTNCDevice.InitCommands.Precommands != tncInitCommandsPre;

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

                bool changed = CurrentTNCDevice.InitCommands.Precommands != tncInitCommandsPre;

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
                SetProperty(ref tncComPort, value, false, true);

                bool changed = CurrentTNCDevice.CommPort.Comport != tncComPort;

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

                bool changed = CurrentTNCDevice.CommPort.BluetoothName != tncComName;

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

                bool changed = CurrentTNCDevice.CommPort.Baudrate != tncComBaudRate;

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

                bool changed = CurrentTNCDevice.CommPort.Databits != tncComDatabits;

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

                bool changed = CurrentTNCDevice.CommPort.Stopbits != tncComStopbits;

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

                bool changed = CurrentTNCDevice.CommPort.Parity != tncComParity;

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

                bool changed = CurrentTNCDevice.CommPort.Flowcontrol != tncComHandshake;

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
                bool changed = CurrentTNCDevice.Commands.MyCall != tncCommandsMyCall;

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
                bool changed = CurrentTNCDevice.Commands.Connect != value;

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
                bool changed = CurrentTNCDevice.Commands.Retry != tncCommandsRetry;

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
                bool changed = CurrentTNCDevice.Commands.Retry != tncCommandsConversMode;

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
                bool changed = CurrentTNCDevice.Commands.Retry != tncCommandsDateTime;

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

                bool changed = CurrentTNCDevice.Prompts.Command != tncPromptsCommand;

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
                bool changed = CurrentTNCDevice.Prompts.Timeout != tncPromptsTimeout;

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

                bool changed = CurrentTNCDevice.Prompts.Connected != tncPromptsConnected;

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

                bool changed = CurrentTNCDevice.Prompts.Disconnected != tncPromptsDisconnected;

                IsAppBarSaveEnabled = SaveEnabled(changed);

            }
        }
        #region Mail Settings
        private int mailAccountSelectedIndex;
        public int MailAccountSelectedIndex
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
            set
            {
                SetProperty(ref currentMailAccount, value);

                MailServer = currentMailAccount.MailServer;
                MailPort = currentMailAccount.MailServerPort;
                IsMailSSL = currentMailAccount.MailIsSSLField;
                MailUserName = currentMailAccount.MailUserName;
                MailPassword = currentMailAccount.MailPassword;
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

                Services.SMTPClient.SmtpClient.Instance.Server = MailServer;
            }
        }

        //private string mailPortString;
        //public string MailPortString
        //{
        //    get => mailPortString;
        //    set
        //    {
        //        SetProperty(ref mailPortString, value);
        //        bool changed = (Convert.ToInt32(MailPortString) != CurrentMailAccount.MailServerPort);
        //        if (changed)
        //            MailPort = Convert.ToInt32(mailPortString);

        //        IsAppBarSaveEnabled = SaveEnabled(changed);
        //    }
        //}

        private Int32 mailPort;
        public Int32 MailPort
        {
            get => mailPort;
            set
            {
                SetProperty(ref mailPort, value);
                //if (MailPortString != MailPort.ToString())
                //    MailPortString = MailPort.ToString();

                bool changed = CurrentMailAccount.MailServerPort != mailPort;
                IsAppBarSaveEnabled = SaveEnabled(changed);

                Services.SMTPClient.SmtpClient.Instance.Port = MailPort;
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
                Services.SMTPClient.SmtpClient.Instance.IsSsl = IsMailSSL;
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

                Services.SMTPClient.SmtpClient.Instance.UserName = MailUserName;
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

                Services.SMTPClient.SmtpClient.Instance.Password = MailPassword;
            }
        }
        #endregion Mail Settings
        private bool isAppBarAddEnabled = true;
        public bool IsAppBarAddEnabled
        {
            get => isAppBarAddEnabled;
            set => SetProperty(ref isAppBarAddEnabled, value);
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
