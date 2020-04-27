using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FormControlBaseClass;
using MessageFormControl;
using MetroLog;
using Microsoft.Toolkit.Helpers;
using PacketMessagingTS.Services;
using PacketMessagingTS.ViewModels;
using SharedCode;
using SharedCode.Helpers;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;
using static SharedCode.Helpers.MessageOriginHelper;

namespace PacketMessagingTS.Helpers
{
    public class BasePrintFormsPage : Base0FormsPage
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<BasePrintFormsPage>();
        private static LogHelper _logHelper = new LogHelper(log);


        public virtual StackPanel ContentArea
        { get; }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    base.OnNavigatedTo(e);
        //    Singleton<PrintMessageViewModel>.Instance.Initialize(e.Parameter as ViewLifetimeControl);
        //}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PopulateMessagePage(e.Parameter as string);
        }

        public async void PopulateMessagePage(string packetMessagePath)
        {
            _packetMessage = PacketMessage.Open(packetMessagePath);

            _packetForm = CreateFormControlInstance(_packetMessage.PacFormName); // Should be PacketFormName, since there may be multiple files with same name
            if (_packetForm is null)
            {
                await ContentDialogs.ShowSingleButtonContentDialogAsync("Failed to find packet form.", "Close", "Packet Messaging Error");
                return;
            }
            Singleton<PrintMsgTestViewModel>.Instance.PacketForm = _packetForm;
            //_formsViewModel.PacketForm = _packetForm;

            _packetForm.FormPacketMessage = _packetMessage;

            StackPanel stackPanel = ContentArea;
            stackPanel.Margin = new Thickness(0, 0, 12, 0);

            stackPanel.Children.Clear();
            stackPanel.Children.Insert(0, _packetForm);

            if (_packetMessage.PacFormName == "SimpleMessage")
            {
                //_packetForm.MessageReceivedTime = DateTime.Now;
                switch (_packetMessage.MessageOrigin)
                {
                    case MessageOrigin.Received:
                        (_packetForm as MessageControl).InBoxHeaderVisibility = true;
                        break;
                    case MessageOrigin.Sent:
                        (_packetForm as MessageControl).SentHeaderVisibility = true;
                        break;
                    default:
                        (_packetForm as MessageControl).NewHeaderVisibility = true;
                        break;
                }
            }
            FillFormFromPacketMessage();

            Singleton<PrintMsgTestViewModel>.Instance.PrintForm();
        }

    }
}
