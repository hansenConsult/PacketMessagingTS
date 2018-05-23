﻿using MetroLog;
using PacketMessagingTS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PacketMessagingTS.Helpers
{
    public static class Utilities
    { 

        public static async Task<string> GetMessageNumberPacketAsync(bool markAsUsed = false) => await GetMessageNumberAsync(markAsUsed) + "P";

        public static async Task<string> GetMessageNumberAsync(bool reserveMessageNumber = false)
        {
            string messageNumberString;

            int messageNumber = await SettingsStorageExtensions.ReadAsync<int>(SharedData.SettingsContainer, "MessageNumber");
            if (messageNumber == default(int))
            {
                messageNumber = 100;
            }

            if (Singleton<IdentityViewModel>.Instance.UseTacticalCallsign)
            {
                messageNumberString = Singleton<IdentityViewModel>.Instance.TacticalMsgPrefix + "-" + messageNumber.ToString();
            }
            else
            {
                messageNumberString = Singleton<IdentityViewModel>.Instance.UserMsgPrefix + "-" + messageNumber.ToString();
            }
            if (reserveMessageNumber)
            {
                messageNumber++;
                await SettingsStorageExtensions.SaveAsync(SharedData.SettingsContainer, "MessageNumber", messageNumber);
            }

            return messageNumberString;
        }

        public static async Task MarkMessageNumberAsUsed(int startMessageNumber = -1)
        {
            int messageNumber;
            if (startMessageNumber < 0)
            {
                messageNumber = await SettingsStorageExtensions.ReadAsync<int>(SharedData.SettingsContainer, "MessageNumber");
                messageNumber++;
            }
            else
            {
                messageNumber = startMessageNumber;
            }
            await SettingsStorageExtensions.SaveAsync(SharedData.SettingsContainer, "MessageNumber", messageNumber);
        }

        public static async Task ReturnMessageNumberAsync()
        {
            // Only remove the last message number created
            //if (!_activeMessageNumber)
            //    return;

            int messageNumber = await SettingsStorageExtensions.ReadAsync<int>(SharedData.SettingsContainer, "MessageNumber");
            if (messageNumber == default(int))
            {
                return;
            }
            messageNumber--;
            await SettingsStorageExtensions.SaveAsync(SharedData.SettingsContainer, "MessageNumber", messageNumber);
        }

    }

    public class LogHelper
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<LogHelper>();

        // Log
        public static void Log(LogLevel logLevel, string message, [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    log.Trace($"{message}, Line = {sourceLineNumber}");
                    break;
                case LogLevel.Debug:
                    log.Debug($"{message}, Line = {sourceLineNumber}");
                    break;
                case LogLevel.Info:
                    log.Info($"{message}, Line = {sourceLineNumber}");
                    break;
                case LogLevel.Warn:
                    log.Warn($"{message}, Line = {sourceLineNumber}");
                    break;
                case LogLevel.Error:
                    log.Error($"{message}, Line = {sourceLineNumber}");
                    break;
                case LogLevel.Fatal:
                    log.Fatal($"{message}, Line = {sourceLineNumber}");
                    break;
            }
        }

    }

}
