using System.Collections.ObjectModel;
using AgFx;
using OnThisDayApp.DataAccess;

namespace OnThisDayApp.ViewModels
{
    /// <summary>
    /// Primary view model used for binding to the view
    /// </summary>
    [CachePolicy(CachePolicy.Forever)]
    [DataLoader(typeof (PageLoader))]
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
                        foreach (EntryViewModel item in value)
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
            get { return DataManager.Current.Load<EventsViewModel>(LoadContext.Day); }
        }
    }
}