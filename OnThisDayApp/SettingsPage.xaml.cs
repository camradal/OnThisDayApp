using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Utilities;
using System.Threading;

namespace OnThisDayApp
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void LiveTileToggle_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                GlobalLoading.Instance.IsLoading = true;
                BackgroundAgent agent = new BackgroundAgent();
                agent.Toggle();

                LiveTileToggle.IsChecked = AppSettings.LiveTileEnabled;
            }
            finally
            {
                Thread.CurrentThread.Join(250);
                GlobalLoading.Instance.IsLoading = false;
            }
        }

        private void BuyAddFreeVersionButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                MarketplaceDetailTask task = new MarketplaceDetailTask();
                task.ContentIdentifier = "60070dfd-ac08-4018-b6cf-9ccda9806158";
                task.Show();
            }
            catch
            {
                // double clicking might cause an error
            }
        }
    }
}