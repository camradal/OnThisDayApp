using System;
using System.Collections.Generic;
using System.IO;
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
            IEnumerable<EntryViewModel> entries = parser.ExtractHighlightEntriesFromHtml(stream);
            DayViewModel viewModel = new DayViewModel(loadContext.Day);

            // push in the values
            foreach (EntryViewModel entry in entries)
            {
                viewModel.Highlights.Add(entry);
            }

            return viewModel;
        }
    }
}
