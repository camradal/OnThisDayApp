using System.Collections.Generic;
using System.Collections.ObjectModel;
using AgFx;
using OnThisDayApp.DataAccess;

namespace OnThisDayApp.ViewModels
{
    [CachePolicy(CachePolicy.Forever, 60 * 60 * 24)]
    [DataLoader(typeof(EventsPageLoader))]
    public sealed class EventsViewModel : ModelItemBase<DayLoadContext>
    {
        private readonly ObservableCollection<GroupedEntries> events = new ObservableCollection<GroupedEntries>();

        public ObservableCollection<GroupedEntries> Events
        {
            get { return events; }
            set
            {
                SetCollection(value, events);
                RaisePropertyChanged("Events");
            }
        }

        public EventsViewModel()
        {
        }

        public EventsViewModel(DayLoadContext loadContext)
            : base(loadContext)
        {
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

        public void UpdateLayout()
        {
            UpdateLayout(events);
            RaisePropertyChanged("Events");
        }

        private  void UpdateLayout(ObservableCollection<GroupedEntries> collection)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                var entry = collection[i];
                collection[i] = null;
                collection[i] = entry;
            }
        }
    }
}