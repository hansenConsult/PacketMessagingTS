﻿using System.Linq;
using System.Threading.Tasks;

using MetroLog;

using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Helpers.PrintHelpers;

using SharedCode;

using Windows.ApplicationModel.Background;

namespace PacketMessagingTS.BackgroundTasks
{
    public sealed class BackgroundPrinting : BackgroundTask
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<BackgroundPrinting>();
        private static LogHelper _logHelper = new LogHelper(log);

        //private volatile bool _cancelRequested = false;
        private IBackgroundTaskInstance _taskInstance;
        private BackgroundTaskDeferral _deferral;


        //private ApplicationTrigger _backgroundPrintingTrigger;
        //public ApplicationTrigger BackgroundPrintingTrigger
        //{
        //    get
        //    {
        //        if (_backgroundPrintingTrigger == null)
        //        {
        //            _backgroundPrintingTrigger = new ApplicationTrigger();
        //        }
        //        return _backgroundPrintingTrigger;
        //    }
        //}

        public override void Register()
        {
            var taskName = GetType().Name;
            var taskRegistration = BackgroundTaskRegistration.AllTasks.FirstOrDefault(t => t.Value.Name == taskName).Value;

            if (taskRegistration == null)
            //if (!_isRegistrared)
            {
                var builder = new BackgroundTaskBuilder()
                {
                    Name = taskName
                };

                // TODO WTS: Define the trigger for your background task and set any (optional) conditions
                // More details at https://docs.microsoft.com/windows/uwp/launch-resume/create-and-register-an-inproc-background-task
                builder.SetTrigger(Singleton<PrintQueue>.Instance.BackgroundPrintingTrigger);
                builder.AddCondition(new SystemCondition(SystemConditionType.UserPresent));

                builder.Register();
                //_isRegistrared = true;
            }
        }

        public override Task RunAsyncInternal(IBackgroundTaskInstance taskInstance)
        {
            if (taskInstance == null)
            {
                return null;
            }

            _deferral = taskInstance.GetDeferral();

            return Task.Run(async () =>
            {
                //// TODO WTS: Insert the code that should be executed in the background task here.
                //// This sample initializes a timer that counts to 100 in steps of 10.  It updates Message each time.

                //// Documentation:
                ////      * General: https://docs.microsoft.com/en-us/windows/uwp/launch-resume/support-your-app-with-background-tasks
                ////      * Debug: https://docs.microsoft.com/en-us/windows/uwp/launch-resume/debug-a-background-task
                ////      * Monitoring: https://docs.microsoft.com/windows/uwp/launch-resume/monitor-background-task-progress-and-completion

                //// To show the background progress and message on any page in the application,
                //// subscribe to the Progress and Completed events.
                //// You can do this via "BackgroundTaskService.GetBackgroundTasksRegistration"

                _logHelper.Log(LogLevel.Info, "Entered background task");

                _taskInstance = taskInstance;

                //Singleton<PrintQueue>.Instance.RestorePrintQueue();

                await Singleton<PrintQueue>.Instance.PrintToDestinationsAsync();

                _deferral?.Complete();
            });
        }

        public override void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //_cancelRequested = true;

            // TODO WTS: Insert code to handle the cancelation request here.
            // Documentation: https://docs.microsoft.com/windows/uwp/launch-resume/handle-a-cancelled-background-task
        }

        //private void SampleTimerCallback(ThreadPoolTimer timer)
        //{
        //    if ((_cancelRequested == false) && (_taskInstance.Progress < 100))
        //    {
        //        _taskInstance.Progress += 10;
        //        //Message = $"Background Task {_taskInstance.Task.Name} running";
        //    }
        //    else
        //    {
        //        timer.Cancel();

        //        if (_cancelRequested)
        //        {
        //            //Message = $"Background Task {_taskInstance.Task.Name} canceled";
        //        }
        //        else
        //        {
        //            //Message = $"Background Task {_taskInstance.Task.Name} finished";
        //        }

        //        _deferral?.Complete();
        //    }
        //}

    }
}
