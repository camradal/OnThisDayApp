using System;
using System.Collections.Generic;
using System.IO;
using OnThisDayApp.Parsers;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.DataAccess
{
    public sealed class HighlightsPageLoader : PageLoaderBase
    {
        private const string sourceUriFormat = @"http://en.wikipedia.org/wiki/Wikipedia:Selected_anniversaries/{0}";

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

            List<Entry> entries = PageParser.ExtractHighlightEntriesFromHtml(stream);
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
