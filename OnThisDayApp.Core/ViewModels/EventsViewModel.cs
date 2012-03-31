﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using AgFx;
using OnThisDayApp.DataAccess;

namespace OnThisDayApp.ViewModels
{
    [CachePolicy(CachePolicy.CacheThenRefresh, 60 * 60 * 24)]
    [DataLoader(typeof(EventsPageLoader))]
    public sealed class EventsViewModel : ModelItemBase<DayLoadContext>
    {
        private readonly BatchObservableCollection<EntryViewModel> births =
            new BatchObservableCollection<EntryViewModel>();

        private readonly BatchObservableCollection<EntryViewModel> deaths =
            new BatchObservableCollection<EntryViewModel>();

        private readonly BatchObservableCollection<EntryViewModel> events =
            new BatchObservableCollection<EntryViewModel>();

        private readonly BatchObservableCollection<EntryViewModel> holidays =
            new BatchObservableCollection<EntryViewModel>();

        #region Properties

        public ObservableCollection<EntryViewModel> Events
        {
            get { return events; }
            set
            {
                SetCollection(value, events);
                RaisePropertyChanged("Events");
            }
        }

        public ObservableCollection<EntryViewModel> Births
        {
            get { return births; }
            set
            {
                SetCollection(value, births);
                RaisePropertyChanged("Births");
            }
        }

        public ObservableCollection<EntryViewModel> Deaths
        {
            get { return deaths; }
            set
            {
                SetCollection(value, deaths);
                RaisePropertyChanged("Deaths");
            }
        }

        public ObservableCollection<EntryViewModel> Holidays
        {
            get { return holidays; }
            set
            {
                SetCollection(value, holidays);
                RaisePropertyChanged("Holidays");
            }
        }

        #endregion

        #region Constructors

        public EventsViewModel()
        {
        }

        public EventsViewModel(string day)
            : base(new DayLoadContext(day))
        {
        }

        #endregion

        private void SetCollection(
            IEnumerable<EntryViewModel> source,
            ObservableCollection<EntryViewModel> destination)
        {
            if (destination != null)
            {
                destination.Clear();

                if (source != null)
                {
                    foreach (EntryViewModel item in source)
                    {
                        destination.Add(item);
                    }
                }
            }
        }
    }
}