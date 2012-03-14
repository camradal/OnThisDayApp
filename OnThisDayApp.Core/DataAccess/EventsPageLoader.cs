using System;
using System.Collections.Generic;
using System.IO;
using OnThisDayApp.Parsers;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.DataAccess
{
    public sealed class EventsPageLoader : PageLoaderBase
    {
        private const string sourceUriFormat = @"http://en.wikipedia.org/wiki/{0}";

        protected override string SourceUriFormat
        {
            get { return sourceUriFormat; }
        }

        /// <summary>
        /// Executes once LoadRequest has executed. Will also happen when deserializing cached data
        /// </summary>
        public override object Deserialize(DayLoadContext loadContext, Type objectType, Stream stream)
        {
            Dictionary<string, List<EntryViewModel>> entries = PageParser.ExtractEventEntriesFromHtml(stream);
            EventsViewModel viewModel = new EventsViewModel(loadContext.Day);

            foreach (EntryViewModel entry in entries["Events"])
            {
                viewModel.Events.Add(entry);
            }

            foreach (EntryViewModel entry in entries["Births"])
            {
                viewModel.Births.Add(entry);
            }

            foreach (EntryViewModel entry in entries["Deaths"])
            {
                viewModel.Deaths.Add(entry);
            }

            foreach (EntryViewModel entry in entries["Holidays"])
            {
                viewModel.Holidays.Add(entry);
            }

            return viewModel;
        }
    }
}
