using System;
using System.Net;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using OnThisDayApp.Resources;
using Utilities;

namespace OnThisDayApp
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        private bool navigating;
        private const string sourceUriFormat = @"http://en.wikipedia.org{0}";

        public DetailsPage()
        {
            InitializeComponent();

            webBrowser1.Navigated += webBrowser1_Navigated;
            webBrowser1.LoadCompleted += webBrowser1_LoadCompleted;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string uri;
            if (NavigationContext.QueryString.TryGetValue("uri", out uri))
            {
                string decodedUri = HttpUtility.HtmlDecode(uri);
                string sourceString = string.Format(sourceUriFormat, decodedUri);
                Uri source = new Uri(sourceString, UriKind.Absolute);
                webBrowser1.Source = source;
            }
        }

        #region Web browser

        void webBrowser1_Navigated(object sender, NavigationEventArgs e)
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