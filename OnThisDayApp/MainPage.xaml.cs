using AgFx;
using BugSense;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Primitives;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using OnThisDayApp.Resources;
using OnThisDayApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
        private readonly BackgroundAgent backgroundAgent = new BackgroundAgent();
        private NavigationOutTransition oldOut;
        private NavigationInTransition oldIn;
        private volatile bool inTransition = false;

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
            AppSettings.NumberOfStarts++;
            ShowReviewPane();
            ShowBuyThisAppPane();

            // ads
            AdBox.ErrorOccurred += AdBox_ErrorOccurred;
            AdBox.AdRefreshed += AdBox_AdRefreshed;
        }

        /// <summary>
        /// Load either from the cache on internet
        /// </summary>
        private void LoadData(ITransition transition = null)
        {
            int numberOfStarts =  AppSettings.NumberOfStarts;
            IndicateStartedLoading(numberOfStarts);

            var loadContext = new DayLoadContext(CurrentDateForWiki, AppSettings.ShowNewestItemsFirst);
            this.DataContext = DataManager.Current.Load<DayViewModel>(
                loadContext,
                vm =>
                {
                    if (App.ReloadRequired)
                    {
                        App.ReloadRequired = false;
                    }
                    else if (App.FontSizeChanged)
                    {
                        vm.UpdateLayout();
                        App.FontSizeChanged = false;
                    }

                    if (App.ReverseRequired)
                    {
                        vm.Highlights = new ObservableCollection<Entry>(vm.Highlights.Reverse());
                        vm.Events.Events = new ObservableCollection<GroupedEntries>(vm.Events.Events.Reverse());
                        vm.Births.Births = new ObservableCollection<GroupedEntries>(vm.Births.Births.Reverse());
                        vm.Deaths.Deaths = new ObservableCollection<GroupedEntries>(vm.Deaths.Deaths.Reverse());
                        App.ReverseRequired = false;
                    }

                    if (!App.IsMemoryLimited && App.FirstLoad)
                    {
                        SetUpLiveTile(numberOfStarts);
                    }

                    if (App.IsMemoryLimited)
                    {
                        ((ApplicationBarMenuItem)ApplicationBar.MenuItems[2]).IsEnabled = false;
                    }
                     
                    IndicateStoppedLoading();

                    if (!inTransition && transition != null)
                    {
                        inTransition = true;
                        transition.Begin();
                    }

                    AdPanel.Opacity = 100;
                },
                ex =>
                {
                    GlobalLoading.Instance.IsLoading = false;
                    GlobalLoading.Instance.LoadingText = null;

                    if (NetworkInterface.GetIsNetworkAvailable())
                    {
                        BugSenseHandler.Instance.LogError(ex, "Failed to get data for " + CurrentDateForWiki + ex.Message);
                    }
                    else
                    {
                        MessageBox.Show(Strings.ErrorInternetConnection);
                    }

                    if (!inTransition && transition != null)
                    {
                        inTransition = true;
                        transition.Begin();
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
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = Strings.MenuItemMyEvents;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).Text = Strings.MenuItemRateThisApp;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[2]).Text = Strings.MenuItemPinLiveTile;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[3]).Text = Strings.MenuItemSettings;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[4]).Text = Strings.MenuItemAbout;
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
            var rate = new ReviewThisAppTask();
            rate.ShowAfterThreshold();
        }

        private void ShowBuyThisAppPane()
        {
            var buy = new BuyThisAppTask();
            buy.ShowAfterThreshold();
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
            var listBox = (LongListSelector)sender;

            // if selected index is null (no selection) do nothing
            var selectedItem = listBox.SelectedItem as Entry;
            if (selectedItem == null)
            {
                return;
            }

            // navigate to the new page
            FrameworkElement root = Application.Current.RootVisual as FrameworkElement;
            root.DataContext = selectedItem;

            OpenDetailsPage(selectedItem.Link);

            // reset selected index to null (no selection)
            listBox.SelectedItem = null;
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

        private void PinLiveTileMenuItem_Click(object sender, EventArgs e)
        {
            bool agentStarted = backgroundAgent.StartIfEnabled();
            if (agentStarted)
            {
                AppSettings.LiveTileEnabled = true;

                ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DefaultTitle=Highlights"));
                if (tile != null)
                {
                    tile.Delete();                    
                }
                
                var data = (DayViewModel)this.DataContext;
                if (data != null && data.Highlights != null && data.Highlights.Count > 0)
                {
                    string title = data.Highlights[0].Year;
                    string description = data.Highlights[0].Description;
                    LiveTileCreator.CreateLiveTile(title, description);
                }
            }
        }

        private void MyEventsMenuItem_Click(object sender, EventArgs e)
        {
            // use dispatcher to prevent jumping elements on the screen
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/MyEventsPage.xaml", UriKind.Relative));
            });
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
            inTransition = false;
            SlideTransition transitionOut;
            SlideTransition transitionIn;

            if (currentDate.DayOfYear == DateTime.Now.DayOfYear)
            {
                LoadData();
                return;
            }
            
            if (currentDate.DayOfYear > DateTime.Now.DayOfYear)
            {
                transitionOut = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeOut };
                transitionIn = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeIn };
            }
            else
            {
                transitionOut = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeOut };
                transitionIn = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeIn };
            }

            var pivotItem = (PivotItem)MainPivot.SelectedItem;
            var tran = transitionOut.GetTransition(pivotItem);
            tran.Completed += (o, args) =>
            {
                currentDate = DateTime.Now;
                LoadData(transitionIn.GetTransition(pivotItem));
            };
            tran.Begin();
        }

        private void AppBarButtonChooseDate_Click(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                var frame = (PhoneApplicationFrame)Application.Current.RootVisual;
                var currentPage = (PhoneApplicationPage)frame.Content;

                // Save the transitions
                oldIn = TransitionService.GetNavigationInTransition(currentPage);
                oldOut = TransitionService.GetNavigationOutTransition(currentPage);

                // Clear the transitions
                TransitionService.SetNavigationInTransition(currentPage, null);
                TransitionService.SetNavigationOutTransition(currentPage, null);

                NavigationService.Navigate(new Uri("/Microsoft.Phone.Controls.Toolkit;component/DateTimePickers/DatePickerPage.xaml", UriKind.Relative));
            });
        }

        private void AppBarButtonPrevDay_Click(object sender, EventArgs e)
        {
            inTransition = false;
            var transitionOut = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeOut };
            var transitionIn = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeIn };
            var pivotItem = (PivotItem)MainPivot.SelectedItem;
            var tran = transitionOut.GetTransition(pivotItem);
            tran.Completed += (o, args) =>
            {
                currentDate = currentDate.AddDays(-1);
                LoadData(transitionIn.GetTransition(pivotItem));
            };
            tran.Begin();
        }

        private void AppBarButtonNextDay_Click(object sender, EventArgs e)
        {
            inTransition = false;
            var transitionOut = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeOut };
            var transitionIn = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeIn };
            var pivotItem = (PivotItem)MainPivot.SelectedItem;
            var tran = transitionOut.GetTransition(pivotItem);
            tran.Completed += (o, args) =>
            {
                currentDate = currentDate.AddDays(1);
                LoadData(transitionIn.GetTransition(pivotItem));
            };
            tran.Begin();
        }

        #endregion

        #region Navigation handlers

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (page != null)
            {
                var frame = (PhoneApplicationFrame)Application.Current.RootVisual;
                var currentPage = (PhoneApplicationPage)frame.Content;

                // Restore the transitions
                TransitionService.SetNavigationInTransition(currentPage, oldIn);
                TransitionService.SetNavigationOutTransition(currentPage, oldOut);
            }

            // restore from datetime picker page
            if (page != null && page.Value.HasValue)
            {
                currentDate = page.Value.Value;
                page = null;
                LoadData();
            }
            else if (App.MyDateTimeSet)
            {
                currentDate = App.MyDateTime;
                App.MyDateTimeSet = false;
                LoadData();
            }
            else if (App.FontSizeChanged)
            {
                LoadData();
            }
            else if (App.ReverseRequired || App.ReloadRequired)
            {
                LoadData();
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // set current date in datetime picker page
            page = e.Content as IDateTimePickerPage;
            if (page != null)
            {
                page.Value = DateTime.Now;
            }
        }

        private void OpenDetailsPage(string url)
        {
            if (AppSettings.BrowserSelection)
            {
                try
                {
                    var uri = new Uri(@"http://en.wikipedia.org" + url, UriKind.Absolute);
                    var webBrowserTask = new WebBrowserTask { Uri = uri };
                    webBrowserTask.Show();
                }
                catch
                {
                }
            }
            else
            {
                string encodedUri = HttpUtility.HtmlEncode(url);
                var uri = new Uri("/DetailsPage.xaml?uri=" + encodedUri, UriKind.Relative);
                Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        NavigationService.Navigate(uri);
                    }
                    catch (Exception)
                    {
                        // prevent double-click errors
                    }
                });
            }
        }

        #endregion

        #region Context Menu

        private void mainMenu_Loaded(object sender, RoutedEventArgs e)
        {
            var menu = sender as ContextMenu;
            if (menu == null || menu.ItemContainerGenerator == null)
                return;

            string suffix = string.IsNullOrEmpty(menu.Tag as string) ? string.Empty : " " + menu.Tag;

            Entry model = (Entry)menu.DataContext;
            Dictionary<string, string> links = model.Links;

            foreach (KeyValuePair<string, string> link in links)
            {
                MenuItem container = (MenuItem)menu.ItemContainerGenerator.ContainerFromItem(link.Key);

                if (string.Equals(link.Key, "share...", StringComparison.InvariantCultureIgnoreCase))
                {
                    container.Click += (obj, args) =>
                    {
                        string title = "On this day in " + model.Year;
                        string description = title + ": " + model.Description + suffix;
                        App.ShareViewModel = new ShareModel
                        {
                            Title = title,
                            Description = description,
                            Link = model.Link
                        };
                        Dispatcher.BeginInvoke(() =>
                        {
                            try
                            {
                                NavigationService.Navigate(new Uri("/SharePage.xaml", UriKind.Relative));
                            }
                            catch (Exception)
                            {
                                // prevent double-click errors
                            }
                        });
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
            var menu = sender as ContextMenu;
            if (menu == null || menu.ItemContainerGenerator == null)
                return;

            Entry model = (Entry)menu.DataContext;
            Dictionary<string, string> links = model.Links;

            foreach (KeyValuePair<string, string> link in links)
            {
                MenuItem container = (MenuItem)menu.ItemContainerGenerator.ContainerFromItem(link.Key);

                if (string.Equals(link.Key, "share...", StringComparison.InvariantCultureIgnoreCase))
                {
                    container.Click += (obj, args) =>
                    {
                        string title = "Today is " + model.Year;
                        string description = title + ReformatDescriptionForHolidays(model.Description);
                        App.ShareViewModel = new ShareModel
                        {
                            Title = title,
                            Description = description,
                            Link = model.Link
                        };
                        Dispatcher.BeginInvoke(() =>
                        {
                            try
                            {
                                NavigationService.Navigate(new Uri("/SharePage.xaml", UriKind.Relative));
                            }
                            catch (Exception)
                            {
                                // prevent double-click errors
                            }
                        });
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

        #region Ads

        void AdBox_AdRefreshed(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                AdDuplexAdControl.Visibility = Visibility.Collapsed;
                AdBox.Visibility = Visibility.Visible;
            });
        }

        void AdBox_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                AdBox.Visibility = Visibility.Collapsed;
                AdDuplexAdControl.Visibility = Visibility.Visible;
            });
        }

        #endregion
    }
}