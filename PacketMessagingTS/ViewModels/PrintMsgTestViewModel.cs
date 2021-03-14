using System;
using System.Windows.Input;
using FormControlBaseClass;
using PacketMessagingTS.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class PrintMsgTestViewModel : BaseViewModel
    {
        public static PrintMsgTestViewModel Instance { get; } = new PrintMsgTestViewModel();

        public PrintMsgTestViewModel()
        {
        }

        public FormControlBase PacketForm
        { get; set; }

        private ICommand _PrintFormCommand;
        public ICommand PrintFormCommand => _PrintFormCommand ?? (_PrintFormCommand = new RelayCommand(PrintForm));

        public void PrintForm()
        {
            PacketForm.PrintForm();
        }

    }
}
