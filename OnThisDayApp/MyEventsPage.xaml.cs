﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.UserData;
using OnThisDayApp.ViewModels;
using Utilities;

namespace OnThisDayApp
{
    public partial class MyEventsPage : PhoneApplicationPage
    {
        private bool isLoading;
        private readonly Appointments appointments = new Appointments();
        private readonly Contacts contacts = new Contacts();

        public MyEventsPage()
        {
            InitializeComponent();
            isLoading = true;
            GlobalLoading.Instance.IsLoading = true;
            GlobalLoading.Instance.LoadingText = "Checking the calendar...";
            appointments.SearchCompleted += appointments_SearchCompleted;
            appointments.SearchAsync(DateTime.Now.AddMonths(-3), DateTime.Now.AddMonths(9), null);
            contacts.SearchCompleted += contacts_SearchCompleted;
            contacts.SearchAsync(string.Empty, FilterKind.None, null);
        }

        void appointments_SearchCompleted(object sender, AppointmentsSearchEventArgs e)
        {
            if (!e.Results.Any())
            {
                var items = new { StartTime = "Add some events to your calendar to see them here...", Subject = string.Empty };
                CalendarListBox.ItemsSource = new[] { items };
            }
            else
            {
                var items = e.Results.Select(i => new DateDisplay { StartTime = i.StartTime, Subject = i.Subject });
                CalendarListBox.ItemsSource = items;
                GlobalLoading.Instance.IsLoading = false;
                GlobalLoading.Instance.LoadingText = null;
                isLoading = false;
            }
        }

        void contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            if (!e.Results.Any())
            {
                var items = new { StartTime = "Add birthdays to your contacts to see them here...", Subject = string.Empty };
                BirthdaysListBox.ItemsSource = new[] { items };
            }
            else
            {
                var birthdays =
                    from contact in e.Results
                    let birthday = contact.Birthdays.FirstOrDefault()
                    where birthday != DateTime.MinValue
                    select new DateDisplay { StartTime = birthday, Subject = contact.DisplayName };
                BirthdaysListBox.ItemsSource = birthdays;
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (isLoading)
            {
                GlobalLoading.Instance.IsLoading = false;
                GlobalLoading.Instance.LoadingText = null;
            }

            base.OnNavigatedFrom(e);
        }

        private void CalendarListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;

            // if selected index is null (no selection) do nothing
            var selectedItem = listBox.SelectedItem as DateDisplay;
            if (selectedItem == null)
            {
                return;
            }

            // navigate to the new page
            FrameworkElement root = Application.Current.RootVisual as FrameworkElement;
            root.DataContext = selectedItem;
            App.MyDateTimeSet = true;
            App.MyDateTime = selectedItem.StartTime;

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            });

            // reset selected index to null (no selection)
            listBox.SelectedItem = null;
        }
    }

    public class DateDisplay
    {
        public DateTime StartTime { get; set; }
        public string Subject { get; set; }
    }
}
