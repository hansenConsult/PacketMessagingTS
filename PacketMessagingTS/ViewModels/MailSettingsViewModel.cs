using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

namespace PacketMessagingTS.ViewModels
{
    public class MailSettingsViewModel : BaseViewModel
    {
        static bool _emailMailServerChanged = false;
        static bool _emailMailServerPortChanged = false;
        static bool _emailMailUserNameChanged = false;
        static bool _emailMailPasswordChanged = false;
        static bool _emailMailIsSSLChanged = false;



        private Int32 mailAccountSelectedIndex = -1;
        public Int32 MailAccountSelectedIndex
        {
            get => GetProperty(ref mailAccountSelectedIndex);
            set
            {
                SetProperty(ref mailAccountSelectedIndex, value, true);
            }
        }

        EmailAccount currentMailAccount;
        public EmailAccount CurrentMailAccount
        {
            get => GetProperty(ref currentMailAccount);
            //{
            //	int index = _settings.CurrentMailAccount;
            //MailServer = EmailAccountArray.Instance.EmailAccounts[index].MailServer;
            //MailPortString = EmailAccountArray.Instance.EmailAccounts[index].MailServerPort.ToString();
            //IsMailSSL = EmailAccountArray.Instance.EmailAccounts[index].MailIsSSLField;
            //MailUserName = EmailAccountArray.Instance.EmailAccounts[index].MailUserName;
            //MailPassword = EmailAccountArray.Instance.EmailAccounts[index].MailPassword;
            //	return _CurrentMailAccount;
            //}
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
                _emailMailServerChanged = CurrentMailAccount.MailServer != mailServer;

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
                if (Convert.ToInt32(MailPortString) != MailPort)
                    MailPort = Convert.ToInt32(mailPortString);
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
 //  TODO               Services.SMTPClient.SmtpClient.Instance.Port = MailPort;
            }
        }

        bool isMailSSL;
        public bool IsMailSSL
        {
            get => isMailSSL;
            //get => _settings.IsMailSSL;
            set
            {
                SetProperty(ref isMailSSL, value);


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

                _emailMailUserNameChanged = CurrentMailAccount.MailUserName != mailUserName;

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
 //  TODO               Services.SMTPClient.SmtpClient.Instance.Password = MailPassword;
            }
        }

        public bool IsSettingsSaveEnabled
        {
            get => _emailMailServerChanged | _emailMailServerPortChanged | _emailMailUserNameChanged
                                           | _emailMailPasswordChanged | _emailMailIsSSLChanged;
        }

    }
}
