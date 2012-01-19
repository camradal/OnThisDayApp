using System;
using System.Net;
using Microsoft.Phone.Controls;

namespace OnThisDayApp
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        private const string sourceUriFormat = @"http://en.wikipedia.org{0}";

        public DetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
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
    }
}