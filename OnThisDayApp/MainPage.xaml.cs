using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using OnThisDayApp.ViewModels;
using Microsoft.Phone.Tasks;
using AgFx;

namespace OnThisDayApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            // load a new ViewModel based on the date
            // this will either fetch from disk or from internet

            string date = "January 18";
            this.DataContext = DataManager.Current.Load<DayViewModel>(
                date,
                (vm) => {},
                (ex) =>
                {
                    MessageBox.Show("Failed to get data for " + date);
                }
             );
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            // use dispatcher to prevent jumping elements on the screen
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
            });
        }

        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // if selected index is -1 (no selection) do nothing
            if (MainListBox.SelectedIndex == -1)
            {
                return;
            }

            // navigate to the new page

            WebBrowserTask task = new WebBrowserTask();
            //task.Uri = new Uri(@"http://en.wikipedia.org" + ((Entry)MainListBox.SelectedItem).Link);
            task.Show();
            //NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + MainListBox.SelectedIndex, UriKind.Relative));

            // reset selected index to -1 (no selection)
            MainListBox.SelectedIndex = -1;
        }
    }
}