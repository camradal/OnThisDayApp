using System.Collections.Generic;
using System.Collections.ObjectModel;
using AgFx;
using OnThisDayApp.DataAccess;

namespace OnThisDayApp.ViewModels
{
    [CachePolicy(CachePolicy.Forever, 60 * 60 * 24)]
    [DataLoader(typeof(DeathsPageLoader))]
    public sealed class DeathsViewModel : ModelItemBase<DayLoadContext>
    {
        private readonly BatchObservableCollection<GroupedEntries> deaths =
            new BatchObservableCollection<GroupedEntries>(7);

        public ObservableCollection<GroupedEntries> Deaths
        {
            get { return deaths; }
            set
            {
                SetCollection(value, deaths);
                RaisePropertyChanged("Deaths");
            }
        }

        public DeathsViewModel()
        {
        }

        public DeathsViewModel(DayLoadContext loadContext)
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
            UpdateLayout(deaths);
            RaisePropertyChanged("Deaths");
        }

        private  void UpdateLayout(BatchObservableCollection<GroupedEntries> collection)
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