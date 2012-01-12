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
using OnThisDayApp.ViewModel;

namespace OnThisDayApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        private DayViewModel viewModel;

        public DayViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {
                    viewModel = new DayViewModel();
                }
                return viewModel;
            }
        }

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // initialize view model after the page is loaded
            DataContext = ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsDataLoaded)
            {
                ViewModel.LoadData();
            }
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            // use dispatcher to prevent jumping elements on the screen
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
            });
        }
    }
}