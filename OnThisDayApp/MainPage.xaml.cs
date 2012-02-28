using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using AgFx;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Primitives;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using OnThisDayApp.Resources;
using OnThisDayApp.ViewModels;
using Utilities;

namespace OnThisDayApp
{
    public partial class MainPage
    {
        /// <summary>
        /// Reference to datetime picker page, so we can get the value from it
        /// </summary>
        private IDateTimePickerPage page;
        private DateTime currentDate = DateTime.Now;

        private BackgroundAgent backgroundAgent = new BackgroundAgent();

        #region Properties

        /// <summary>
        /// Date displayed in UI
        /// </summary>
        private string CurrentDate
        {
            get
            {
                return currentDate.ToString("M", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Date passed as a primary key for wiki
        /// </summary>
        private string CurrentDateForWiki
        {
            get
            {
                return currentDate.ToString("MMMM_d", CultureInfo.InvariantCulture);
            }
        }

        #endregion

        #region Constructors and loaders

        public MainPage()
        {
            InitializeComponent();

            LoadData();

            // specify the text explicitly on the app bar using our resource string
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = Strings.ButtonToday;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = Strings.ButtonChooseDate;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).Text = Strings.ButtonPrevDay;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[3]).Text = Strings.ButtonNextDay;

            if (backgroundAgent.LiveTileDisabled)
            {
                ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = Strings.MenuItemEnableLiveTile;
            }
            {
                ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = Strings.MenuItemDisableLiveTile;
            }
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).Text = Strings.MenuItemRateThisApp;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[2]).Text = Strings.MenuItemAbout;

            Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // show review pane
            ReviewThisAppTask rate = new ReviewThisAppTask();
            rate.NumberOfStarts++;
            rate.ShowAfterThreshold();

            // make sure background agent is running
            backgroundAgent.StartIfEnabled();
        }

        /// <summary>
        /// Load either from the cache on internet
        /// </summary>
        private void LoadData()
        {
            GlobalLoading.Instance.IsLoading = true;

            if (App.FirstLoad)
            {
                GlobalLoading.Instance.LoadingText = RandomLoader.GetRandomString();
            }

            this.DataContext = DataManager.Current.Load<DayViewModel>(
                CurrentDateForWiki,
                vm =>
                {
                    GlobalLoading.Instance.IsLoading = false;

                    if (App.FirstLoad)
                    {
                        GlobalLoading.Instance.LoadingText = null;
                        App.FirstLoad = false;
                    }

                    EventsListBox.ItemsSource = ((DayViewModel)this.DataContext).Events.Events;
                    BirthsListBox.ItemsSource = ((DayViewModel)this.DataContext).Events.Births;
                    DeathsListBox.ItemsSource = ((DayViewModel)this.DataContext).Events.Deaths;
                    HolidaysListBox.ItemsSource = ((DayViewModel)this.DataContext).Events.Holidays;
                },
                ex =>
                {
                    MessageBox.Show("Failed to get data for " + CurrentDate);
                    GlobalLoading.Instance.IsLoading = false;
                    GlobalLoading.Instance.LoadingText = null;
                });

            SetPivotTitle();
        }

        private void SetPivotTitle()
        {
            MainPivot.Title = string.Format(
                CultureInfo.CurrentCulture, "{0} {1}", Strings.AppTitleCapitalized, CurrentDate).ToUpper();
        }

        #endregion
      
        #region ListBox Handlers

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;

            // if selected index is -1 (no selection) do nothing
            if (listBox.SelectedIndex == -1)
            {
                return;
            }

            // navigate to the new page
            var selectedItem = (EntryViewModel)listBox.SelectedItem;
            OpenDetailsPage(selectedItem.Link);

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

        private void LiveTileMenuItem_Click(object sender, EventArgs e)
        {
            backgroundAgent.Toggle();
        }

        private void RateThisAppMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MarketplaceReviewTask task = new MarketplaceReviewTask();
                task.Show();
            }
            catch
            {
                // prevent exceptions from double-click
            }
        }

        private void AppBarButtonChooseDate_Click(object sender, EventArgs e)
        {
            // use dispatcher to prevent jumping elements on the screen
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/Microsoft.Phone.Controls.Toolkit;component/DateTimePickers/DatePickerPage.xaml", UriKind.Relative));
            });
        }

        private void AppBarButtonToday_Click(object sender, EventArgs e)
        {
            currentDate = DateTime.Now;
            LoadData();
        }

        private void AppBarButtonPrevDay_Click(object sender, EventArgs e)
        {
            currentDate = currentDate.AddDays(-1);
            LoadData();
        }

        private void AppBarButtonNextDay_Click(object sender, EventArgs e)
        {
            currentDate = currentDate.AddDays(1);
            LoadData();
        }

        #endregion

        #region Navigation handlers

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // restore from datetime picker page
            if (page != null && page.Value.HasValue)
            {
                currentDate = page.Value.Value;
                page = null;
                LoadData();
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // set current date in datetime picker page
            page = e.Content as IDateTimePickerPage;
            if (page != null)
            {
                page.Value = DateTime.Now;
            }
        }

        private void OpenDetailsPage(string url)
        {
            string encodedUri = HttpUtility.HtmlEncode(url);
            Uri uri = new Uri("/DetailsPage.xaml?uri=" + encodedUri, UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        #endregion

        #region Context Menu

        private void mainMenu_Loaded(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = (ContextMenu)sender;

            if (menu.ItemContainerGenerator == null)
                return;

            EntryViewModel model = (EntryViewModel)menu.DataContext;
            Dictionary<string, string> links = model.Links;

            foreach (KeyValuePair<string, string> link in links)
            {
                MenuItem container = (MenuItem)menu.ItemContainerGenerator.ContainerFromItem(link.Key);
                string url = link.Value;
                container.Click += (obj, args) =>
                {
                    OpenDetailsPage(url);
                };
            }
        }

        

        #endregion
    }
}