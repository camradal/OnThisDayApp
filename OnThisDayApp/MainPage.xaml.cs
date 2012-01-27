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
using OnThisDayApp.Resources;
using OnThisDayApp.ViewModels;
using Utilities;

namespace OnThisDayApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// Reference to datetime picker page, so we can get the value from it
        /// </summary>
        private IDateTimePickerPage page;
        private DateTime currentDate = DateTime.Now;

        #region Properties

        /// <summary>
        /// Date displayed in UI
        /// </summary>
        private string CurrentDate
        {
            get
            {
                return currentDate.ToString("MMMM d", CultureInfo.CurrentCulture);
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
            var today = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            today.Text = Strings.ButtonToday;

            var chooseDate = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            chooseDate.Text = Strings.ButtonChooseDate;

            var menuItemAbout = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
            menuItemAbout.Text = Strings.MenuItemAbout;

            Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ReviewThisAppTask rate = new ReviewThisAppTask();
            rate.NumberOfStarts++;
            rate.ShowAfterThreshold();
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
                        SetPivotTitle();
                        GlobalLoading.Instance.LoadingText = null;
                        App.FirstLoad = false;
                    }
                },
                ex =>
                {
                    MessageBox.Show("Failed to get data for " + CurrentDate);
                    GlobalLoading.Instance.IsLoading = false;
                    GlobalLoading.Instance.LoadingText = null;
                });

            if (!App.FirstLoad)
            {
                SetPivotTitle();
            }
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