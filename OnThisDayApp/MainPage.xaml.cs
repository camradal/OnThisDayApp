﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using AgFx;
using BugSense;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Primitives;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using OnThisDayApp.Resources;
using OnThisDayApp.ViewModels;
using Utilities;

namespace OnThisDayApp
{
    public partial class MainPage
    {
        #region Variables

        /// <summary>
        /// Reference to datetime picker page, so we can get the value from it
        /// </summary>
        private IDateTimePickerPage page;
        private DateTime currentDate = DateTime.Now;
        private BackgroundAgent backgroundAgent = new BackgroundAgent();
        private ManualResetEvent loadEvent = new ManualResetEvent(false);

        #endregion

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

        #region Constructors and Loaders

        public MainPage()
        {
            InitializeComponent();
            LoadData();
            SetApplicationBarLocalizedStrings();
            ShowReviewPane();
        }

        /// <summary>
        /// Load either from the cache on internet
        /// </summary>
        private void LoadData()
        {
            int numberOfStarts =  AppSettings.NumberOfStarts;
            IndicateStartedLoading(numberOfStarts);

            this.DataContext = DataManager.Current.Load<DayViewModel>(
                CurrentDateForWiki,
                vm =>
                {
                    if (!App.IsMemoryLimited && App.FirstLoad)
                    {
                        SetUpLiveTile(numberOfStarts);
                    }

                    IndicateStoppedLoading();
                },
                ex =>
                {
                    GlobalLoading.Instance.IsLoading = false;
                    GlobalLoading.Instance.LoadingText = null;

                    if (NetworkInterface.GetIsNetworkAvailable())
                    {
                        BugSenseHandler.Instance.LogError(ex, "Failed to get data for " + CurrentDateForWiki);
                    }
                    else
                    {
                        MessageBox.Show(Strings.ErrorInternetConnection);
                    }

                });

            SetPivotTitle();
        }

        private void SetApplicationBarLocalizedStrings()
        {
            // specify the text explicitly on the app bar using our resource string
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = Strings.ButtonToday;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = Strings.ButtonChooseDate;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).Text = Strings.ButtonPrevDay;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[3]).Text = Strings.ButtonNextDay;

            // menu bar
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = Strings.MenuItemRateThisApp;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).Text = Strings.MenuItemSettings;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[2]).Text = Strings.MenuItemAbout;
        }

        private void IndicateStartedLoading(int numberOfStarts)
        {
            GlobalLoading.Instance.IsLoading = true;
            if (App.FirstLoad)
            {
                if (numberOfStarts == 0)
                {
                    GlobalLoading.Instance.LoadingText = App.IsMemoryLimited ?
                        Strings.InitialLoadLowMemoryDevice :
                        Strings.InitialLoadRegularDevice;
                }
                else
                {
                    GlobalLoading.Instance.LoadingText = RandomLoader.GetRandomString();
                }
            }
        }

        private void IndicateStoppedLoading()
        {
            GlobalLoading.Instance.IsLoading = false;
            if (App.FirstLoad)
            {
                GlobalLoading.Instance.LoadingText = null;
                App.FirstLoad = false;
            }
        }

        private void SetUpLiveTile(int numberOfStarts)
        {
            bool agentStarted = backgroundAgent.StartIfEnabled();
            if (agentStarted && (numberOfStarts == 0))
            {
                InitialTileSetup();
            }
            if (!agentStarted)
            {
                AppSettings.LiveTileDisabled = true;
                backgroundAgent.ResetTileToDefault();
            }
        }

        private void InitialTileSetup()
        {
            DayViewModel data = ((DayViewModel)this.DataContext);
            if (data != null && data.Highlights != null && data.Highlights.Count > 0)
            {
                LiveTile.UpdateLiveTile(data.Highlights[0].Year, data.Highlights[0].Description);
            }
        }

        private void ShowReviewPane()
        {
            ReviewThisAppTask rate = new ReviewThisAppTask();
            rate.ShowAfterThreshold();
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
            FrameworkElement root = Application.Current.RootVisual as FrameworkElement;
            var selectedItem = (EntryViewModel)listBox.SelectedItem;
            root.DataContext = selectedItem;

            OpenDetailsPage(selectedItem.Link);

            // reset selected index to -1 (no selection)
            listBox.SelectedIndex = -1;
        }

        #endregion

        #region Application Bar Handlers

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

        private void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            // use dispatcher to prevent jumping elements on the screen
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
            });
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            // use dispatcher to prevent jumping elements on the screen
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
            });
        }

        private void AppBarButtonToday_Click(object sender, EventArgs e)
        {
            currentDate = DateTime.Now;
            LoadData();
        }

        private void AppBarButtonChooseDate_Click(object sender, EventArgs e)
        {
            // use dispatcher to prevent jumping elements on the screen
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/Microsoft.Phone.Controls.Toolkit;component/DateTimePickers/DatePickerPage.xaml", UriKind.Relative));
            });
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
            else if (App.ReloadRequired)
            {
                LoadData();
                App.ReloadRequired = false;
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

            string suffix = string.IsNullOrEmpty(menu.Tag as string) ? string.Empty : " " + menu.Tag;

            EntryViewModel model = (EntryViewModel)menu.DataContext;
            Dictionary<string, string> links = model.Links;

            foreach (KeyValuePair<string, string> link in links)
            {
                MenuItem container = (MenuItem)menu.ItemContainerGenerator.ContainerFromItem(link.Key);

                if (string.Equals(link.Key, "share...", StringComparison.InvariantCultureIgnoreCase))
                {
                    container.Click += (obj, args) =>
                    {
                        string title = "On this day in " + model.Year;
                        try
                        {
                            ShareLinkTask task = new ShareLinkTask()
                            {
                                Title = title,
                                Message = title + ": " + model.Description + suffix,
                                LinkUri = new Uri(@"http://en.wikipedia.org" + model.Link, UriKind.Absolute)
                            };
                            task.Show();
                        }
                        catch (Exception)
                        {
                            // fast-clicking can result in exception, so we just handle it
                        }
                    };
                }
                else
                {
                    string url = link.Value;
                    container.Click += (obj, args) =>
                    {
                        OpenDetailsPage(url);
                    };
                }
            }
        }

        private void mainMenuHolidays_Loaded(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = (ContextMenu)sender;

            if (menu.ItemContainerGenerator == null)
                return;

            EntryViewModel model = (EntryViewModel)menu.DataContext;
            Dictionary<string, string> links = model.Links;

            foreach (KeyValuePair<string, string> link in links)
            {
                MenuItem container = (MenuItem)menu.ItemContainerGenerator.ContainerFromItem(link.Key);

                if (string.Equals(link.Key, "share...", StringComparison.InvariantCultureIgnoreCase))
                {
                    container.Click += (obj, args) =>
                    {
                        string title = "Today is " + model.Year;
                        try
                        {
                            string buffer = ReformatDescriptionForHolidays(model.Description);
                            ShareLinkTask task = new ShareLinkTask()
                            {
                                Title = title,
                                Message = title + buffer,
                                LinkUri = new Uri(@"http://en.wikipedia.org" + model.Link, UriKind.Absolute)
                            };
                            task.Show();
                        }
                        catch (Exception)
                        {
                            // fast-clicking can result in exception, so we just handle it
                        }
                    };
                }
                else
                {
                    string url = link.Value;
                    container.Click += (obj, args) =>
                    {
                        OpenDetailsPage(url);
                    };
                }
            }
        }

        private static string ReformatDescriptionForHolidays(string description)
        {
            if (string.IsNullOrEmpty(description))
                return string.Empty;

            var lines = description.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (lines.Count <= 1)
                return description;

            StringBuilder buffer = new StringBuilder(": ");
            for (int i = 0; i < lines.Count - 1; i++)
            {
                buffer.AppendFormat("{0}, ", lines[i]);
            }
            var lastLine = lines.LastOrDefault();
            if (!string.IsNullOrEmpty(lastLine))
            {
                buffer.Append(lastLine);
            }
            return buffer.ToString();
        }

        #endregion
    }
}