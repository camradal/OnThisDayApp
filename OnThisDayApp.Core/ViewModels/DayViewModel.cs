using System.Collections.ObjectModel;
using AgFx;
using OnThisDayApp.DataAccess;
using System;

namespace OnThisDayApp.ViewModels
{
    /// <summary>
    /// Primary view model used for binding to the view
    /// </summary>
    [CachePolicy(CachePolicy.CacheThenRefresh, 60 * 60 * 24)]
    [DataLoader(typeof (HighlightsPageLoader))]
    public sealed class DayViewModel : ModelItemBase<DayLoadContext>
    {
        private readonly BatchObservableCollection<EntryViewModel> highlights =
            new BatchObservableCollection<EntryViewModel>();

        public DayViewModel()
        {
        }

        public DayViewModel(string day) : base(new DayLoadContext(day))
        {
        }

        public ObservableCollection<EntryViewModel> Highlights
        {
            get { return highlights; }
            set
            {
                if (highlights != null)
                {
                    highlights.Clear();

                    if (value != null)
                    {
                        // TODO: optimize insert perf
                        if (LoadContext.ReverseOrder)
                        {
                            foreach (EntryViewModel item in value)
                            {
                                highlights.Insert(0, item);
                            }
                        }
                        else
                        {
                            foreach (EntryViewModel item in value)
                            {
                                highlights.Add(item);
                            }
                        }
                    }
                }
                RaisePropertyChanged("Highlights");
            }
        }

        public EventsViewModel Events
        {
            get { return DataManager.Current.Load<EventsViewModel>(LoadContext.Day); }
        }
    }
}