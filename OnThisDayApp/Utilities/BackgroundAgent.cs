using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using OnThisDayApp.Resources;

namespace Utilities
{
    internal sealed class BackgroundAgent
    {
        private const string SettingString = "LiveTileDisabled";
        private const string TaskName = "OnThisDayApp.LiveTileScheduledTask";
        private readonly IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        #region Properties

        public bool LiveTileDisabled
        {
            get
            {
                if (settings.Contains(SettingString))
                {
                    return bool.Parse(settings[SettingString].ToString());
                }

                return false;
            }
            set
            {
                settings[SettingString] = value;
            }
        }

        #endregion

        #region Public Methods

        public void Toggle()
        {
            bool result = false;
            LiveTileDisabled = !LiveTileDisabled;

            if (LiveTileDisabled)
            {
                Stop();
                ResetTileToDefault();
                result = true;
            }
            else
            {
                result = StartIfEnabledInternal();
            }

            // do not switch values if not succesful
            if (!result)
            {
                LiveTileDisabled = !LiveTileDisabled;
            }
        }

        public bool StartIfEnabled()
        {
            bool result = true;
            if (!LiveTileDisabled)
            {
                result = StartIfEnabledInternal();
            }
            return result;
        }

        #endregion

        #region Helper Methods

        private bool StartIfEnabledInternal()
        {
            bool result = false;

            ScheduledAction action = ScheduledActionService.Find(TaskName);
            if (action == null)
            {
                result = Start();
            }
            else if (action != null && !action.IsEnabled)
            {
                Stop();
                result = Start();
            }

#if DEBUG
            // If we're debugging, attempt to start the task immediately 
            ScheduledActionService.LaunchForTest(TaskName, new TimeSpan(0, 0, 1));
#endif

            return result;
        }

        private bool Start()
        {
            bool result = false;
            try
            {
                PeriodicTask task = new PeriodicTask(TaskName);
                task.Description = "Service to update On This Day... live tile";
                ScheduledActionService.Add(task);
                result = true;
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(
                    Strings.ErrorCouldNotEnableLiveTileDescription,
                    Strings.ErrorCouldNotEnableLiveTileTitle,
                    MessageBoxButton.OK);
            }
            catch (Exception)
            {
                // still show the main UI
            }

            return result;
        }

        private void Stop()
        {
            try
            {
                if (ScheduledActionService.Find(TaskName) != null)
                {
                    ScheduledActionService.Remove(TaskName);
                }
            }
            catch (Exception)
            {
                // ignore, best effort cleanup
            }
        }

        private bool ResetTileToDefault()
        {
            bool result = false;

            try
            {
                StandardTileData tileData = new StandardTileData()
                {
                    Title = "On This Day...",
                    BackgroundImage = new Uri("/icons/Application_Icon_173.png", UriKind.Relative),
                    BackBackgroundImage = new Uri("NONESUCH.png", UriKind.Relative),
                    BackTitle = string.Empty,
                    BackContent = string.Empty
                };

                ShellTile tile = ShellTile.ActiveTiles.First();
                tile.Update(tileData);
                result = true;
            }
            catch (Exception)
            {
                // ignore, best effort cleanup
            }

            return result;
        }

        #endregion
    }
}
