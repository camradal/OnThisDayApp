using System;
using System.IO;
using System.Linq;
using OnThisDayApp.Parsers;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.DataAccess
{
    public sealed class DeathsPageLoader : PageLoaderBase
    {
        private const string sourceUriFormat = @"http://en.wikipedia.org/w/api.php?action=parse&prop=text&page={0}&section=3&format=xml";

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
            var entries = PageParser.ExtractEntriesFromHtml(html, false);
            var viewModel = new DeathsViewModel(loadContext);

            if (loadContext.ReverseOrder)
            {
                entries.Reverse();
            }

            var groupings = entries.GroupBy(PageParser.GroupByYear());
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
    }
}
