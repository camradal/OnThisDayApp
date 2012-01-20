using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using AgFx;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Primitives;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            // load a new ViewModel based on the date
            // this will either fetch from disk or from internet

            string date = DateTime.Now.ToString("MMMM_d", CultureInfo.InvariantCulture); ;
            this.DataContext = DataManager.Current.Load<DayViewModel>(
                date,
                (vm) => {},
                (ex) =>
                {
                    MessageBox.Show("Failed to get data for " + date);
                }
             );
        }

        #region ListBox Handlers

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;

            // if selected index is -1 (no selection) do nothing
            if (listBox.SelectedIndex == -1)
            {
                return;
            }

            // navigate to the new page
            var selectedItem = (EntryViewModel)listBox.SelectedItem;
            string encodedUri = HttpUtility.HtmlEncode(selectedItem.Link);
            Uri uri = new Uri("/DetailsPage.xaml?uri=" + encodedUri, UriKind.Relative);
            NavigationService.Navigate(uri);

            // reset selected index to -1 (no selection)
            listBox.SelectedIndex = -1;
        }

        #endregion

        #region Application Bar Handlers

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            // use dispatcher to prevent jumping elements on the screen
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
            });
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            // use dispatcher to prevent jumping elements on the screen
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/Microsoft.Phone.Controls.Toolkit;component/DateTimePickers/DatePickerPage.xaml", UriKind.Relative));
            });
        }

        #endregion

        #region Navigation handlers

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            var page = e.Content as IDateTimePickerPage;
            if (page != null)
            {
                page.Value = DateTime.Now;
            }
        }

        #endregion
    }
}