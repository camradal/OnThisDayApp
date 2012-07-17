using System.Collections.Generic;
using System.Collections.ObjectModel;
using AgFx;
using OnThisDayApp.DataAccess;

namespace OnThisDayApp.ViewModels
{
    [CachePolicy(CachePolicy.Forever, 60 * 60 * 24)]
    [DataLoader(typeof(BirthsPageLoader))]
    public sealed class BirthsViewModel : ModelItemBase<DayLoadContext>
    {
        private readonly BatchObservableCollection<GroupedEntries> births =
            new BatchObservableCollection<GroupedEntries>(7);

        public ObservableCollection<GroupedEntries> Births
        {
            get { return births; }
            set
            {
                SetCollection(value, births);
                RaisePropertyChanged("Births");
            }
        }

        public BirthsViewModel()
        {
        }

        public BirthsViewModel(DayLoadContext loadContext)
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
            UpdateLayout(births);
            RaisePropertyChanged("Births");
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