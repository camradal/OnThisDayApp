using System;
using System.IO;
using System.Net;
using OnThisDayApp.Parsers;

namespace OnThisDayApp.DataAccess
{
    public sealed class PageLoader
    {
        private readonly Uri sourceUri = new Uri(@"http://en.wikipedia.org/wiki/Main_Page", UriKind.Absolute);

        public event EventHandler<PageLoadedEventArgs> Loaded;

        /// <summary>
        /// Loads page asynchronously, result will returned in Loaded event
        /// </summary>
        public void LoadAsync()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(sourceUri);
                request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
            }
            catch
            {
                // handle failure to submit request
            }
        }

        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)callbackResult.AsyncState;
                using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(callbackResult))
                using (StreamReader httpWebStreamReader = new StreamReader(response.GetResponseStream()))
                {
                    string html = httpWebStreamReader.ReadToEnd();

                    PageParser parser = new PageParser();
                    var events = parser.ExtractOnThisDayFromTextHtml(html);

                    // this is async operation, raise event that page has been loaded
                    if (html != null)
                    {
                        Loaded(this, new PageLoadedEventArgs(events));
                    }
                }
            }
            catch
            {
                // handle failure to read response
            }
        }
    }
}
