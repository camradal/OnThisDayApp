using System;
using System.Linq;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

namespace OnThisDayApp.Utilities
{
    public class BackgroundAgent
    {
        private const string TaskName = "OnThisDayApp.LiveTileScheduledTask";

        public void StopAndStart()
        {
            StopAgentIfStarted();
            StartAgent();
        }

        public void StartAgent()
        {
            PeriodicTask task = new PeriodicTask(TaskName);
            task.Description = "Service to update On This Day... live tile";
            ScheduledActionService.Add(task);

#if DEBUG
            // If we're debugging, attempt to start the task immediately 
            ScheduledActionService.LaunchForTest(TaskName, new TimeSpan(0, 0, 1));
#endif
        }

        public void StopAgentIfStarted()
        {
            if (ScheduledActionService.Find(TaskName) != null)
            {
                ScheduledActionService.Remove(TaskName);
            }
        }

        public void ResetTileToDefault()
        {
            StandardTileData newTileData = new StandardTileData();
            newTileData.BackgroundImage = new Uri("appdata:/icons/Application_Icon_173.png");

            ShellTile appTile = ShellTile.ActiveTiles.First();
            appTile.Update(newTileData);
        }
    }
}
