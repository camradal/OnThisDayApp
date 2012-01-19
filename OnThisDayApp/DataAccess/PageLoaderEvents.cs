using System;
using System.IO;
using AgFx;
using OnThisDayApp.Parsers;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.DataAccess
{
    public sealed class PageLoaderEvents : IDataLoader<DayLoadContext>
    {
        private const string sourceUriFormat = @"http://en.wikipedia.org/wiki/{0}";
        private readonly PageParser parser = new PageParser();

        public LoadRequest GetLoadRequest(DayLoadContext loadContext, Type objectType)
        {
            string uri = string.Format(sourceUriFormat, loadContext.Day);
            return new WebLoadRequest(loadContext, new Uri(uri, UriKind.Absolute));
        }

        /// <summary>
        /// Executes once LoadRequest has executed. Will also happen when deserializing cached data
        /// </remarks>
        public object Deserialize(DayLoadContext lc, Type objectType, Stream stream)
        {
            var entries = parser.ExtractEventEntriesFromHtml(stream);
            var vm = new EventsViewModel(lc.Day);

            foreach (var wp in entries["Events"])
            {
                vm.Events.Add(wp);
            }

            foreach (var wp in entries["Births"])
            {
                vm.Births.Add(wp);
            }

            foreach (var wp in entries["Deaths"])
            {
                vm.Deaths.Add(wp);
            }

            return vm;
        }
    }
}
