using Microsoft.Phone.Controls;
using Utilities;

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
            BackgroundAgent agent = new BackgroundAgent();
            agent.Toggle();

            LiveTileToggle.IsChecked = AppSettings.LiveTileEnabled;
        }
    }
}