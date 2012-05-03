using System;
using System.Collections.Generic;
using System.IO;
using OnThisDayApp.Parsers;
using OnThisDayApp.ViewModels;
using System.Diagnostics;

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
            Dictionary<string, List<Entry>> entries = PageParser.ExtractEventEntriesFromHtml(stream);
            EventsViewModel viewModel = new EventsViewModel(loadContext);

            // TODO: get rid of this and use backwards for loop
            if (loadContext.ReverseOrder)
            {
                entries["Events"].Reverse();
                entries["Births"].Reverse();
                entries["Deaths"].Reverse();
            }

            foreach (Entry entry in entries["Holidays"])
            {
                viewModel.Holidays.Add(entry);
            }

            foreach (Entry entry in entries["Births"])
            {
                viewModel.Births.Add(entry);
            }

            foreach (Entry entry in entries["Events"])
            {
                viewModel.Events.Add(entry);
            }

            foreach (Entry entry in entries["Deaths"])
            {
                viewModel.Deaths.Add(entry);
            }

            return viewModel;
        }
    }
}
