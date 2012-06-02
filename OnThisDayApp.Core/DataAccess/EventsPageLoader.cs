using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using AgFx;
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

            var groupings = entries["Births"].GroupBy(GroupByYear());
            foreach (IGrouping<string, Entry> grouping in groupings)
            {
                var displayGroup = new GroupedEntries
                {
                    Name = grouping.Key,
                    Entries = grouping.ToList()
                };
                viewModel.Births.Add(displayGroup);
            }

            groupings = entries["Events"].GroupBy(GroupByYear());
            foreach (IGrouping<string, Entry> grouping in groupings)
            {
                var displayGroup = new GroupedEntries
                {
                    Name = grouping.Key,
                    Entries = grouping.ToList()
                };
                viewModel.Events.Add(displayGroup);
            }

            groupings = entries["Deaths"].GroupBy(GroupByYear());
            foreach (IGrouping<string, Entry> grouping in groupings)
            {
                var displayGroup = new GroupedEntries
                {
                    Name = grouping.Key,
                    Entries = grouping.ToList()
                };
                viewModel.Deaths.Add(displayGroup);
            }

            return viewModel;
        }

        private static Func<Entry, string> GroupByYear()
        {
            return item =>
                   {
                       if (item.Year.Length == 4)
                       {
                           return item.Year.StartsWith("19")
                                      ? item.Year.Substring(0, 3) + "0s"
                                      : item.Year.Substring(0, 2) + "00s";
                       }
                       if (item.Year.Length == 3)
                       {
                           return item.Year.Substring(0, 1) + "00s";
                       }
                       return item.Year.Length == 2 ? "AD" : "BC";
                   };
        }
    }
}
