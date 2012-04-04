using System.Collections.Generic;
using AgFx;

namespace OnThisDayApp.ViewModels
{
    public struct Link
    {
        string Text;
        string Url;
    }

    [CachePolicy(CachePolicy.CacheThenRefresh, 60 * 60 * 24)]
    public sealed class EntryViewModel : ModelItemBase<DayLoadContext>
    {
        private string description;
        private string link;
        private Dictionary<string, string> links;
        private string year;
        private string imageUrl;

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

        public string ImageUrl
        {
            get { return imageUrl; }
            set
            {
                imageUrl = value;
                RaisePropertyChanged("ImageUrl");
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

        public Dictionary<string, string> Links
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