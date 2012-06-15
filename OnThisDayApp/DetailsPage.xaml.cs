﻿using System;
using System.Net;
using System.Windows.Navigation;
using OnThisDayApp.Resources;
using Utilities;

namespace OnThisDayApp
{
    public partial class DetailsPage
    {
        private volatile bool navigating;
        private const string sourceUriFormat = @"http://en.wikipedia.org{0}";
        private Uri sourceUrl;

        public DetailsPage()
        {
            InitializeComponent();

            webBrowser1.Navigated += webBrowser1_Navigated;
            webBrowser1.LoadCompleted += webBrowser1_LoadCompleted;
        }

        #region Navigation

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string uri;
            if (NavigationContext.QueryString.TryGetValue("uri", out uri))
            {
                string decodedUri = HttpUtility.HtmlDecode(uri);
                string sourceString = string.Format(sourceUriFormat, decodedUri);
                sourceUrl = new Uri(sourceString, UriKind.Absolute);
                webBrowser1.Source = sourceUrl;
            }
            //else if (isNewPage && State.ContainsKey("SourceUrl"))
            //{
            //    sourceUrl = new Uri(State["SourceUrl"].ToString());
            //    webBrowser1.Source = sourceUrl;
            //}
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

            if (e.NavigationMode != NavigationMode.Back)
            {
                //State["SourceUrl"] = sourceUrl;
            }
        }

        #endregion

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