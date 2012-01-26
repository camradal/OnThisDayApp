using System.ComponentModel;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Utilities
{
    public class GlobalLoading : INotifyPropertyChanged
    {
        private ProgressIndicator indicator;
        private static GlobalLoading instance;
        private int loadingCount;
        private string text;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDataManagerLoading { get; set; }

        public bool ActualIsLoading
        {
            get
            {
                return IsLoading || IsDataManagerLoading;
            }
        }

        public bool IsLoading
        {
            get
            {
                return loadingCount > 0;
            }
            set
            {
                bool loading = IsLoading;
                if (value)
                {
                    ++loadingCount;
                }
                else
                {
                    --loadingCount;
                }

                NotifyValueChanged();
            }
        }

        public string LoadingText
        {
            set
            {
                text = value;
                NotifyValueChanged();
            }
        }

        public static GlobalLoading Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GlobalLoading();
                }

                return instance;
            }
        }

        private GlobalLoading()
        {
        }

        public void Initialize(PhoneApplicationFrame frame)
        {
            indicator = new ProgressIndicator();
            frame.Navigated += OnRootFrameNavigated;
        }

        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            // Use in Mango to share a single progress indicator instance.
            var ee = e.Content;
            var pp = ee as PhoneApplicationPage;
            if (pp != null)
            {
                pp.SetValue(SystemTray.ProgressIndicatorProperty, indicator);
            }
        }

        private void OnDataManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("IsLoading" == e.PropertyName)
            {
                NotifyValueChanged();
            }
        }

        private void NotifyValueChanged()
        {
            if (indicator != null)
            {
                indicator.IsIndeterminate = loadingCount > 0 || IsDataManagerLoading;

                // set text value
                indicator.Text = text;

                // for now, just make sure it's always visible.
                if (indicator.IsVisible == false)
                {
                    indicator.IsVisible = true;
                }
            }
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}