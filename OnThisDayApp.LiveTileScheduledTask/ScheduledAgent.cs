using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using AgFx;
using Microsoft.Phone.Info;
using Microsoft.Phone.Scheduler;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.LiveTileScheduledTask
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        #region Properties

        /// <summary>
        /// Date passed as a primary key for wiki
        /// </summary>
        private string CurrentDateForWiki
        {
            get
            {
                return DateTime.Now.ToString("MMMM_d", CultureInfo.InvariantCulture);
            }
        }

        #endregion

        #region ScheduledTaskAgent Implementation

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            // TODO: report error using bugsense
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            Debug.WriteLine("Current memory - initial: {0}", DeviceStatus.ApplicationCurrentMemoryUsage);

            DataManager.Current.Load<DayViewModel>(
                CurrentDateForWiki,
                vm =>
                {
                    Debug.WriteLine("Current memory - loaded: {0}", DeviceStatus.ApplicationCurrentMemoryUsage);
                    try
                    {
                        if (vm.Highlights.Count == 0)
                        {
                            return;
                        }

                        int index = new Random().Next(vm.Highlights.Count);
                        Entry entry = vm.Highlights[index];

                        string title = entry.Year;
                        string content = entry.Description;

                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        LiveTile.UpdateLiveTile(title, content);
                        Debug.WriteLine("Current memory - updated: {0}", DeviceStatus.ApplicationCurrentMemoryUsage);
                    }
                    finally
                    {
                        NotifyComplete();                        
                    }
                },
                ex =>
                {
                    // TODO: report error using bugsense
                    NotifyComplete();
                });
        }

        #endregion
    }
}
