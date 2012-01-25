using System.Collections.Generic;
using AgFx;

namespace OnThisDayApp.ViewModels
{
    [CachePolicy(CachePolicy.Forever)]
    public sealed class EntryViewModel : ModelItemBase<DayLoadContext>
    {
        private string description;
        private string link;
        private List<string> links;
        private string year;

        #region Properties

        public string Year
        {
            get { return year; }
            set
            {
                year = value;
                RaisePropertyChanged("Year");
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                RaisePropertyChanged("Description");
            }
        }

        public string Link
        {
            get { return link; }
            set
            {
                link = value;
                RaisePropertyChanged("Link");
            }
        }

        public List<string> Links
        {
            get { return links; }
            set
            {
                links = value;
                RaisePropertyChanged("Links");
            }
        }

        #endregion

        #region Constructors

        public EntryViewModel()
        {
        }

        public EntryViewModel(string day) : base(new DayLoadContext(day))
        {
        }

        #endregion
    }
}