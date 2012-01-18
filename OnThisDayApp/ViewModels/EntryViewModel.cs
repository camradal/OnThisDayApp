using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using OnThisDayApp.Models;

namespace OnThisDayApp.ViewModel
{
    public class EntryViewModel : ViewModelBase
    {
        private Entry newEvent;

        public EntryViewModel(Entry newEvent)
        {
            this.newEvent = newEvent;
        }

        public int Year
        {
            get
            {
                return newEvent.Year;
            }
            set
            {
                newEvent.Year = value;
                base.NotifyPropertyChanged("Year");
            }
        }

        public string Description
        {
            get
            {
                return newEvent.Description;
            }
            set
            {
                newEvent.Description = value;
                base.NotifyPropertyChanged("Description");
            }
        }

        public string Link
        {
            get
            {
                return newEvent.Link;
            }
            set
            {
                newEvent.Link = value;
                base.NotifyPropertyChanged("Link");
            }
        }
    }
}
