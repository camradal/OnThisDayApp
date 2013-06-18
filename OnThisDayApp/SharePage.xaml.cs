using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using OnThisDayApp.Resources;

namespace OnThisDayApp
{
    public partial class SharePage : PhoneApplicationPage
    {
        public List<string> ShareSources = new List<string>
        {
            Strings.ShareEmail,
            Strings.ShareSocialNetwork,
            Strings.ShareTextMessaging,
            Strings.ShareClipboard
        };

        public SharePage()
        {
            InitializeComponent();
            ShareListBox.ItemsSource = ShareSources;
        }

        private void ShareListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ShareListBox.SelectedItem as string;
            
            if (selectedItem == null)
                return;

            var item = App.ShareViewModel;
            if (selectedItem == Strings.ShareEmail)
                ShareHelper.ShareViaEmail(item);
            else if (selectedItem == Strings.ShareSocialNetwork)
                ShareHelper.ShareViaSocial(item);
            else if (selectedItem == Strings.ShareTextMessaging)
                ShareHelper.ShareViaSms(item);
            else if (selectedItem == Strings.ShareClipboard)
                ShareHelper.ShareViaClipBoard(item);

            ShareListBox.SelectedIndex = -1;
        }
    }
}