using System;
using System.Net;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using OnThisDayApp.Resources;
using Utilities;

namespace OnThisDayApp
{
    public partial class DetailsPage
    {
        private volatile bool navigating;
        private const string sourceUriFormat = @"http://en.m.wikipedia.org{0}";
        private Uri sourceUrl;

        public DetailsPage()
        {
            InitializeComponent();

            webBrowser1.Navigating += webBrowser1_Navigating;
            webBrowser1.LoadCompleted += webBrowser1_LoadCompleted;
        }

        #region Navigation

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            bool locked = AppSettings.OrientationLock;
            SetOrientation(locked);

            if (webBrowser1.Source != null)
                return;

            string uri;
            if (NavigationContext.QueryString.TryGetValue("uri", out uri))
            {
                string decodedUri = HttpUtility.HtmlDecode(uri);
                string sourceString = string.Format(sourceUriFormat, decodedUri);
                sourceUrl = new Uri(sourceString, UriKind.Absolute);
                webBrowser1.Source = sourceUrl;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (navigating)
            {
                GlobalLoading.Instance.IsLoading = false;
                GlobalLoading.Instance.LoadingText = null;
                navigating = false;
            }
        }

        #endregion

        #region Menu

        private void ApplicationBarOrientationMenuItem_OnClick(object sender, EventArgs e)
        {
            bool locked = !AppSettings.OrientationLock;
            AppSettings.OrientationLock = locked;
            SetOrientation(locked);
        }

        private void ApplicationBarOrientationShareMenuItem_OnClick(object sender, EventArgs e)
        {
            if (App.ShareViewModel == null) return;

            var uri = new Uri("/SharePage.xaml", UriKind.Relative);
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    NavigationService.Navigate(uri);
                }
                catch (Exception)
                {
                    // prevent double-click errors
                }
            });
        }

        private void SetOrientation(bool locked)
        {
            this.SupportedOrientations = locked ? SupportedPageOrientation.Portrait : SupportedPageOrientation.PortraitOrLandscape;
            string text = locked ? "unlock orientation" : "lock orientation";
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = text;
        }

        #endregion

        #region Web browser

        private void webBrowser1_Navigating(object sender, NavigatingEventArgs e)
        {
            if (!navigating)
            {
                GlobalLoading.Instance.IsLoading = true;
                GlobalLoading.Instance.LoadingText = Strings.Loading;
            }
            navigating = true;
        }

        private void webBrowser1_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (navigating)
            {
                GlobalLoading.Instance.IsLoading = false;
                GlobalLoading.Instance.LoadingText = null;
            }
            navigating = false;
        }

        #endregion
    }
}