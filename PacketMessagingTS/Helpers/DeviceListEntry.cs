//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.Devices.Enumeration;

namespace PacketMessagingTS.Helpers
{
    /// <summary>
    /// The class will only expose properties from DeviceInformation that are going to be used
    /// in this sample. Each instance of this class provides information about a single device.
    ///
    /// This class is used by the UI to display device specific information so that
    /// the user can identify which device to use.
    /// </summary>
    public class DeviceListEntry
    {
        /// <summary>
        /// The class is mainly used as a DeviceInformation wrapper so that the UI can bind to a list of these.
        /// </summary>
        /// <param name="deviceInformation"></param>
        /// <param name="deviceSelector">The AQS used to find this device</param>
        public DeviceListEntry(DeviceInformation deviceInformation, string deviceSelector, string portName = "")
        {
            device = deviceInformation;
            this.deviceSelector = deviceSelector;
            comPort = portName;
        }

        private readonly string deviceSelector;

        public string InstanceId
        {
            get
            {
                return device.Properties[DeviceProperties.DeviceInstanceId] as string;
            }
        }

        private DeviceInformation device;
        public DeviceInformation DeviceInformation
        {
            get
            {
                return device;
            }
        }

        public string DeviceSelector
        {
            get
            {
                return deviceSelector;
            }
        }

        private string comPort;
        public string ComPort
        {
            get => comPort;
            set => comPort = value;
        }

    }
}
