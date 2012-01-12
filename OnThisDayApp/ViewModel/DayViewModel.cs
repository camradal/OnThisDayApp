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
using OnThisDayApp.Model;
using System.Collections.ObjectModel;

namespace OnThisDayApp.ViewModel
{
    public class DayViewModel : ViewModelBase
    {
        private PageLoader pageLoader = new PageLoader();

        public bool IsDataLoaded { get; set; }

        public ObservableCollection<EventViewModel> Events { get; private set; }

        public DayViewModel()
        {
            Events = new ObservableCollection<EventViewModel>();
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
                foreach (Event newEvent in e.Events)
                {
                    Events.Add(new EventViewModel(newEvent));
                }

                GlobalLoading.Instance.IsLoading = false;
            });
        }
    }
}
