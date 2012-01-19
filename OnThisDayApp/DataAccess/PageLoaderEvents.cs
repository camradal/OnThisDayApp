using System;
using System.IO;
using AgFx;
using OnThisDayApp.Parsers;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.DataAccess
{
    public sealed class PageLoaderEvents : IDataLoader<DayLoadContext>
    {
        private const string sourceUriFormat = @"http://en.wikipedia.org/wiki/Main_Page/";
        private readonly PageParser parser = new PageParser();

        public LoadRequest GetLoadRequest(DayLoadContext loadContext, Type objectType)
        {
            string uri = string.Format(sourceUriFormat, loadContext.Day);
            return new WebLoadRequest(loadContext, new Uri(sourceUriFormat));
        }

        /// <summary>
        /// Executes once LoadRequest has executed. Will also happen when deserializing cached data
        /// </remarks>
        public object Deserialize(DayLoadContext lc, Type objectType, Stream stream)
        {
            var entries = parser.ExtractHighlightEntriesFromHtml(stream);
            var vm = new DayViewModel(lc.Day);

            // push in the weather periods
            foreach (var wp in entries)
            {
                vm.Highlights.Add(wp);
            }

            return vm;
        }
    }
}
