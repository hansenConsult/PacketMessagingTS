using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PacketMessagingTS.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class TNCSettingsViewModel : BaseViewModel
    {
        bool _bluetoothOnChanged = false;
        bool _comPortChanged = false;
        bool _comNameChanged = false;
        bool _comportBaudRateChanged = false;
        bool _databitsChanged = false;
        bool _stopbitsChanged = false;
        bool _parityChanged = false;
        bool _handshakeChanged = false;
        bool _comportSettingsChanged = false;
        bool _initCommandsPreChanged = false;
        bool _initCommandsPostChanged = false;
        bool _initCommandsChanged = false;
        bool _commandsConnectChanged = false;
        bool _commandsChanged = false;
        bool _promptsCommandChanged = false;
        bool _promptsTimeoutChanged = false;
        bool _promptsConnectedChanged = false;
        bool _promptsDisconnectedChanged = false;
        bool _promptsChanged = false;
        bool _emailMailServerChanged = false;
        bool _emailMailServerPortChanged = false;
        bool _emailMailUserNameChanged = false;
        bool _emailMailPasswordChanged = false;
        bool _emailMailIsSSLChanged = false;

        public TNCSettingsViewModel()
        {

        }

        private string tncPromptsCommand;
        public string TNCPromptsCommand
        {
            get => GetProperty(ref tncPromptsCommand);
            //get { return SharedData.CurrentTNCDevice.Prompts.Command; }
            set
            {
                SetProperty(ref tncPromptsCommand, value);
                SharedData.CurrentTNCDevice.Prompts.Command = value;

                _promptsCommandChanged = SharedData.SavedTNCDevice.Prompts.Command != value;

                _promptsChanged = _promptsCommandChanged | _promptsTimeoutChanged | _promptsConnectedChanged
                        | _promptsDisconnectedChanged;

                //appBarSettingsSave.IsEnabled = _comportSettingsChanged | _initCommandsChanged
                //        | _commandsChanged | _promptsChanged;
            }
        }

        public string TNCPromptsTimeout
        {
            get => SharedData.CurrentTNCDevice.Prompts.Timeout;
            set
            {
                SharedData.CurrentTNCDevice.Prompts.Timeout = value;

                _promptsTimeoutChanged = SharedData.SavedTNCDevice.Prompts.Timeout != value;

                _promptsChanged = _promptsCommandChanged | _promptsTimeoutChanged | _promptsConnectedChanged
                        | _promptsDisconnectedChanged;
                //appBarSettingsSave.IsEnabled = _comportSettingsChanged | _initCommandsChanged
                //        | _commandsChanged | _promptsChanged;
            }
        }

        private string tncCommandsConnect;
        public string TNCCommandsConnect
        {
            get => GetProperty(ref tncPromptsCommand);
            //get => sharedData.CurrentTNCDevice.Commands.Connect;
            set
            {
                SharedData.CurrentTNCDevice.Commands.Connect = value;
                if (SharedData.SavedTNCDevice.Commands.Connect != value)
                    _commandsConnectChanged = true;
                else
                    _commandsConnectChanged = false;

                _commandsChanged = _commandsConnectChanged;
                //appBarSaveTNC.IsEnabled = _bluetoothOnChanged | _comportSettingsChanged | _initCommandsChanged
                //	| _commandsChanged | _promptsChanged;
            }
        }

    }
}
