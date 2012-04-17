using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Utilities;
using System.Threading;
using AgFx;
using System;
using System.Globalization;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        private string CurrentDateForWiki
        {
            get
            {
                return DateTime.Now.ToString("MMMM_d", CultureInfo.InvariantCulture);
            }
        }

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

        private void ClearSavedDataButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                GlobalLoading.Instance.IsLoading = true;
                DataManager.Current.Clear<DayViewModel>(CurrentDateForWiki);
                DataManager.Current.DeleteCache();
                App.ReloadRequired = true;
            }
            finally
            {
                Thread.CurrentThread.Join(250);
                GlobalLoading.Instance.IsLoading = false;
            }
        }
    }
}