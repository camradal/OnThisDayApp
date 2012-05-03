using System.Collections.Generic;
using System.Collections.ObjectModel;
using AgFx;
using OnThisDayApp.DataAccess;

namespace OnThisDayApp.ViewModels
{
    [CachePolicy(CachePolicy.CacheThenRefresh, 60 * 60 * 24)]
    [DataLoader(typeof(EventsPageLoader))]
    public sealed class EventsViewModel : ModelItemBase<DayLoadContext>
    {
        private readonly BatchObservableCollection<Entry> births =
            new BatchObservableCollection<Entry>();

        private readonly BatchObservableCollection<Entry> deaths =
            new BatchObservableCollection<Entry>();

        private readonly BatchObservableCollection<Entry> events =
            new BatchObservableCollection<Entry>();

        private readonly BatchObservableCollection<Entry> holidays =
            new BatchObservableCollection<Entry>();

        #region Properties

        public ObservableCollection<Entry> Events
        {
            get { return events; }
            set
            {
                SetCollection(value, events);
                RaisePropertyChanged("Events");
            }
        }

        public ObservableCollection<Entry> Births
        {
            get { return births; }
            set
            {
                SetCollection(value, births);
                RaisePropertyChanged("Births");
            }
        }

        public ObservableCollection<Entry> Deaths
        {
            get { return deaths; }
            set
            {
                SetCollection(value, deaths);
                RaisePropertyChanged("Deaths");
            }
        }

        public ObservableCollection<Entry> Holidays
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

        public EventsViewModel(DayLoadContext loadContext)
            : base(loadContext)
        {
        }

        #endregion

        private void SetCollection(
            IEnumerable<Entry> source,
            ObservableCollection<Entry> destination)
        {
            if (destination != null)
            {
                destination.Clear();

                if (source != null)
                {
                    foreach (Entry item in source)
                    {
                        destination.Add(item);
                    }
                }
            }
        }
    }
}