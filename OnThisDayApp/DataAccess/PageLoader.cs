using System;
using System.IO;
using AgFx;
using OnThisDayApp.Parsers;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.DataAccess
{
    public sealed class PageLoader : IDataLoader<DayLoadContext>
    {
        private const string sourceUriFormat = @"http://en.wikipedia.org/wiki/Wikipedia:Selected_anniversaries/{0}";
        private readonly PageParser parser = new PageParser();

        public LoadRequest GetLoadRequest(DayLoadContext loadContext, Type objectType)
        {
            string uri = string.Format(sourceUriFormat, loadContext.Day);
            return new WebLoadRequest(loadContext, new Uri(uri, UriKind.Absolute));
        }

        /// <summary>
        /// Executes once LoadRequest has executed. Will also happen when deserializing cached data
        /// </summary>
        public object Deserialize(DayLoadContext loadContext, Type objectType, Stream stream)
        {
            var entries = parser.ExtractHighlightEntriesFromHtml(stream);
            var vm = new DayViewModel(loadContext.Day);

            // push in the values
            foreach (var entry in entries)
            {
                vm.Highlights.Add(entry);
            }

            return vm;
        }
    }
}
