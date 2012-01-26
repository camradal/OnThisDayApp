﻿using System;
using System.Globalization;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AgFx;
using Microsoft.Phone.Controls.Primitives;
using OnThisDayApp.Resources;
using OnThisDayApp.ViewModels;
using Microsoft.Phone.Controls;

namespace OnThisDayApp
{
    public partial class MainPage
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
        public string CurrentDate
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

        public MainPage()
        {
            InitializeComponent();
            LoadData();
        }

        /// <summary>
        /// Load either from the cache on internet
        /// </summary>
        private void LoadData()
        {
            this.DataContext = DataManager.Current.Load<DayViewModel>(
                CurrentDateForWiki,
                vm => { },
                ex =>
                {
                    MessageBox.Show("Failed to get data for " + CurrentDate);
                });

            MainPivot.Title = string.Format(
                CultureInfo.CurrentCulture, "{0} {1}", Strings.AppTitleCapitalized, CurrentDate).ToUpper();
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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (page != null && page.Value.HasValue)
            {
                currentDate = page.Value.Value;
                page = null;
                LoadData();
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            page = e.Content as IDateTimePickerPage;
            if (page != null)
            {
                page.Value = DateTime.Now;
            }

            base.OnNavigatedTo(e);
        }

        #endregion

        #region Context Menu

        private void mainMenu_Loaded(object sender, RoutedEventArgs e)
        {
            // extremely hacky - hopefully fix at some point
            var menu = (ContextMenu)sender;
            var model = (DayViewModel)this.DataContext;
            var entry = model.Highlights.Single(viewModel => viewModel.Year == (string)menu.Tag);

            foreach (MenuItem item in entry.Links.Select(link => new MenuItem {Header = link.Key, Tag = link.Value}))
            {
                item.Click += (obj, args) =>
                {
                    MenuItem menuItem = (MenuItem)obj;
                    string encodedUri = HttpUtility.HtmlEncode((string)menuItem.Tag);
                    Uri uri = new Uri("/DetailsPage.xaml?uri=" + encodedUri, UriKind.Relative);
                    NavigationService.Navigate(uri);
                };
                menu.Items.Add(item);
            }
        }

        #endregion
    }
}