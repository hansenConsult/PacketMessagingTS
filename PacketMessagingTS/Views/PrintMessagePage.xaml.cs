using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MessageFormControl;
using MetroLog;
using Microsoft.Toolkit.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Services;
using PacketMessagingTS.ViewModels;

using SharedCode;
using SharedCode.Helpers;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PacketMessagingTS.Views
{
    public sealed partial class PrintMessagePage : BasePrintFormsPage
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<PrintMessagePage>();
        private static LogHelper _logHelper = new LogHelper(log);

        public PrintMessageViewModel ViewModel { get; } = Singleton<PrintMessageViewModel>.Instance;

        public PrintMessagePage()
        {
            InitializeComponent();
        }



    }
}
