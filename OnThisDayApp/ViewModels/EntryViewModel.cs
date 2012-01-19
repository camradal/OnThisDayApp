using AgFx;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.ViewModels
{
    public sealed class EntryViewModel : ModelItemBase<DayLoadContext>
    {
        private int year;
        private string description;
        private string link;

        public EntryViewModel()
        {
        }

        public EntryViewModel(string day) : base(new DayLoadContext(day))
        {
        }

        public int Year
        {
            get
            {
                return year;
            }
            set
            {
                year = value;
                RaisePropertyChanged("Year");
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                RaisePropertyChanged("Description");
            }
        }

        public string Link
        {
            get
            {
                return link;
            }
            set
            {
                link = value;
                RaisePropertyChanged("Link");
            }
        }
    }
}
