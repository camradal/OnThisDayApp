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
using Microsoft.Phone.UserData;

namespace OnThisDayApp
{
    public partial class MyEventsPage : PhoneApplicationPage
    {
        private readonly Appointments appointments = new Appointments();

        public MyEventsPage()
        {
            InitializeComponent();
            appointments.SearchCompleted += appointments_SearchCompleted;
            appointments.SearchAsync(DateTime.MinValue, DateTime.MaxValue, null);
        }

        void appointments_SearchCompleted(object sender, AppointmentsSearchEventArgs e)
        {
            if (!e.Results.Any())
            {

            }
            else
            {
                DatesListBox.ItemsSource = e.Results;
            }
        }
    }
}