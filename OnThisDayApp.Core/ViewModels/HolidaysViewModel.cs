using System.Collections.Generic;
using System.Collections.ObjectModel;
using AgFx;
using OnThisDayApp.DataAccess;

namespace OnThisDayApp.ViewModels
{
    [CachePolicy(CachePolicy.Forever, 60 * 60 * 24)]
    [DataLoader(typeof(HolidaysPageLoader))]
    public sealed class HolidaysViewModel : ModelItemBase<DayLoadContext>
    {
        private readonly ObservableCollection<Entry> holidays = new ObservableCollection<Entry>();

        public ObservableCollection<Entry> Holidays
        {
            get { return holidays; }
            set
            {
                SetCollection(value, holidays);
                RaisePropertyChanged("Holidays");
            }
        }

        public HolidaysViewModel()
        {
        }

        public HolidaysViewModel(DayLoadContext loadContext)
            : base(loadContext)
        {
        }

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

        public void UpdateLayout()
        {
            UpdateLayout(holidays);
            RaisePropertyChanged("Holidays");
        }

        private void UpdateLayout(ObservableCollection<Entry> collection)
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