using System.Collections.ObjectModel;
using AgFx;

namespace OnThisDayApp.ViewModels
{
    [CachePolicy(CachePolicy.Forever)]
    public sealed class DayViewModel : ModelItemBase<DayLoadContext>
    {
        private readonly BatchObservableCollection<EntryViewModel> highlights = new BatchObservableCollection<EntryViewModel>();

        public ObservableCollection<EntryViewModel> Highlights
        {
            get
            {
                return highlights;
            }
            set
            {
                if (highlights != null)
                {
                    highlights.Clear();

                    if (value != null)
                    {
                        foreach (var item in value)
                        {
                            highlights.Add(item);
                        }
                    }
                }
                RaisePropertyChanged("Highlights");
            }
        }

        public DayViewModel()
        {
        }

        public DayViewModel(string day) : base(new DayLoadContext(day))
        {
        }
    }
}
