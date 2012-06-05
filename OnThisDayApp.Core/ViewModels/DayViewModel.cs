using System.Collections.ObjectModel;
using AgFx;
using OnThisDayApp.DataAccess;

namespace OnThisDayApp.ViewModels
{
    /// <summary>
    /// Primary view model used for binding to the view
    /// </summary>
    [CachePolicy(CachePolicy.CacheThenRefresh, 60 * 60 * 24)]
    [DataLoader(typeof (HighlightsPageLoader))]
    public sealed class DayViewModel : ModelItemBase<DayLoadContext>
    {
        private readonly BatchObservableCollection<Entry> highlights =
            new BatchObservableCollection<Entry>(7);

        public DayViewModel()
        {
        }

        public DayViewModel(DayLoadContext loadContext)
            : base(loadContext)
        {
        }

        public ObservableCollection<Entry> Highlights
        {
            get { return highlights; }
            set
            {
                if (highlights != null)
                {
                    highlights.Clear();

                    if (value != null)
                    {
                        foreach (Entry item in value)
                        {
                            highlights.Add(item);
                        }
                    }
                }
                RaisePropertyChanged("Highlights");
            }
        }

        public EventsViewModel Events
        {
            get { return DataManager.Current.Load<EventsViewModel>(LoadContext); }
        }
    }
}