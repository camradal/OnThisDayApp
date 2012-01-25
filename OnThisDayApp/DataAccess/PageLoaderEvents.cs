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
        /// </summary>
        public object Deserialize(DayLoadContext lc, Type objectType, Stream stream)
        {
            var entries = parser.ExtractEventEntriesFromHtml(stream);
            var vm = new EventsViewModel(lc.Day);

            foreach (var entry in entries["Events"])
            {
                vm.Events.Add(entry);
            }

            foreach (var entry in entries["Births"])
            {
                vm.Births.Add(entry);
            }

            foreach (var entry in entries["Deaths"])
            {
                vm.Deaths.Add(entry);
            }

            foreach (var entry in entries["Holidays"])
            {
                vm.Holidays.Add(entry);
            }

            return vm;
        }
    }
}
