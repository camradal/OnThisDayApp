using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Windows;
using Microsoft.Phone.Info;
using Microsoft.Phone.Scheduler;
using OnThisDayApp.Parsers;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.LiveTileScheduledTask
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;
        private const string sourceUriFormat = @"http://en.wikipedia.org/wiki/Wikipedia:Selected_anniversaries/{0}";

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
            var loaded = new ManualResetEvent(false);

            try
            {
                Debug.WriteLine("Current memory - initial: {0}", DeviceStatus.ApplicationCurrentMemoryUsage);

                string uriString = string.Format(sourceUriFormat, CurrentDateForWiki);
                var uri = new Uri(uriString, UriKind.Absolute);

                var client = new WebClient();
                client.DownloadStringAsync(uri);
                client.DownloadStringCompleted += client_DownloadStringCompleted;

                loaded.WaitOne(25 * 1000);
            }
            finally
            {
                NotifyComplete();
            }
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
                Debug.WriteLine("Current memory - request complete: {0}", DeviceStatus.ApplicationCurrentMemoryUsage);

                List<Entry> entries = PageParser.ExtractHighlightEntriesFromHtml(e.Result);
                if (entries.Count == 0)
                {
                    return;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Debug.WriteLine("Current memory - loaded: {0}", DeviceStatus.ApplicationCurrentMemoryUsage);

                int index = new Random().Next(entries.Count);
                Entry entry = entries[index];

                string title = entry.Year;
                string content = entry.Description;

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    LiveTile.UpdateLiveTile(title, content);
                    Debug.WriteLine("Current memory - updated: {0}", DeviceStatus.ApplicationCurrentMemoryUsage);
                    NotifyComplete();
                });
        }

        #endregion
    }
}
