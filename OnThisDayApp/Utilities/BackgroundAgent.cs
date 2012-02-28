using System;
using System.Linq;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace Utilities
{
    class BackgroundAgent
    {
        private const string SettingString = "LiveTileDisabled";
        private const string TaskName = "OnThisDayApp.LiveTileScheduledTask";
        private readonly IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        public bool LiveTileDisabled
        {
            get
            {
                if (settings.Contains(SettingString))
                {
                    return bool.Parse(SettingString);
                }

                return false;
            }
            set
            {
                settings[SettingString] = value;
            }
        }

        public void Toggle()
        {
            LiveTileDisabled = !LiveTileDisabled;

            if (LiveTileDisabled)
            {
                Stop();
                ResetTileToDefault();
            }
            else
            {
                Start();
            }
        }

        public void StartIfEnabled()
        {
            if (!LiveTileDisabled)
            {
                ScheduledAction action = ScheduledActionService.Find(TaskName);
                if (action == null)
                {
                    Start();
                }
                else if (action != null && !action.IsEnabled)
                {
                    Stop();
                    Start();
                }
            }
        }

        private void Start()
        {
            PeriodicTask task = new PeriodicTask(TaskName);
            task.Description = "Service to update On This Day... live tile";
            ScheduledActionService.Add(task);

#if DEBUG
            // If we're debugging, attempt to start the task immediately 
            ScheduledActionService.LaunchForTest(TaskName, new TimeSpan(0, 0, 1));
#endif
        }

        private void Stop()
        {
            if (ScheduledActionService.Find(TaskName) != null)
            {
                ScheduledActionService.Remove(TaskName);
            }
        }

        private void ResetTileToDefault()
        {
            StandardTileData tileData = new StandardTileData();
            tileData.BackgroundImage = new Uri("appdata:/icons/Application_Icon_173.png");

            ShellTile tile = ShellTile.ActiveTiles.First();
            tile.Update(tileData);
        }
    }
}
