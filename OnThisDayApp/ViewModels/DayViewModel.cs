using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using OnThisDayApp.DataAccess;
using OnThisDayApp.Models;
using System.Collections.ObjectModel;
using Utilities;

namespace OnThisDayApp.ViewModel
{
    public class DayViewModel : ViewModelBase
    {
        private PageLoader pageLoader = new PageLoader();

        public bool IsDataLoaded { get; set; }

        public ObservableCollection<EntryViewModel> Entries { get; private set; }

        public DayViewModel()
        {
            Entries = new ObservableCollection<EntryViewModel>();
            pageLoader.Loaded += new EventHandler<PageLoadedEventArgs>(pageLoader_Loaded);
        }

        public void LoadData()
        {
            pageLoader.LoadAsync();
            GlobalLoading.Instance.IsLoading = true;
        }

        void pageLoader_Loaded(object sender, PageLoadedEventArgs e)
        {
            // update needs to happen on main thread
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                foreach (Entry newEvent in e.Events)
                {
                    Entries.Add(new EntryViewModel(newEvent));
                }

                GlobalLoading.Instance.IsLoading = false;
            });
        }
    }
}
