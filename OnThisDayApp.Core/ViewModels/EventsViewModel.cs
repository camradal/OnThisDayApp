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
        private readonly BatchObservableCollection<GroupedEntries> births =
            new BatchObservableCollection<GroupedEntries>();

        private readonly BatchObservableCollection<GroupedEntries> deaths =
            new BatchObservableCollection<GroupedEntries>();

        private readonly BatchObservableCollection<GroupedEntries> events =
            new BatchObservableCollection<GroupedEntries>();

        private readonly BatchObservableCollection<Entry> holidays =
            new BatchObservableCollection<Entry>();

        #region Properties

        public ObservableCollection<GroupedEntries> Events
        {
            get { return events; }
            set
            {
                SetCollection(value, events);
                RaisePropertyChanged("Events");
            }
        }

        public ObservableCollection<GroupedEntries> Births
        {
            get { return births; }
            set
            {
                SetCollection(value, births);
                RaisePropertyChanged("Births");
            }
        }

        public ObservableCollection<GroupedEntries> Deaths
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

        private void SetCollection(IEnumerable<Entry> source, ICollection<Entry> destination)
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

        private void SetCollection(IEnumerable<GroupedEntries> source, ICollection<GroupedEntries> destination)
        {
            if (destination != null)
            {
                destination.Clear();

                if (source != null)
                {
                    foreach (GroupedEntries item in source)
                    {
                        destination.Add(item);
                    }
                }
            }
        }
    }
}