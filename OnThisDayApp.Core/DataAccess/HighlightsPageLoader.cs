using System;
using System.Collections.Generic;
using System.IO;
using OnThisDayApp.Parsers;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.DataAccess
{
    public sealed class HighlightsPageLoader : PageLoaderBase
    {
        private const string sourceUriFormat = @"https://en.wikipedia.org/w/api.php?action=parse&prop=text&page=Wikipedia:Selected_anniversaries/{0}&format=xml";

        protected override string SourceUriFormat
        {
            get { return sourceUriFormat; }
        }

        /// <summary>
        /// Executes once LoadRequest has executed. Will also happen when deserializing cached data
        /// </summary>
        public override object Deserialize(DayLoadContext loadContext, Type objectType, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            string html = PageParser.GetHtml(stream);

            List<Entry> entries = PageParser.ExtractEntriesFromHtml(html, true);
            var viewModel = new DayViewModel(loadContext);

            if (loadContext.ReverseOrder)
            {
                for (int i = entries.Count - 1; i >= 0; i--)
                {
                    viewModel.Highlights.Add(entries[i]);
                }
            }
            else
            {
                foreach (Entry entry in entries)
                {
                    viewModel.Highlights.Add(entry);
                }
            }

            return viewModel;
        }
    }
}
