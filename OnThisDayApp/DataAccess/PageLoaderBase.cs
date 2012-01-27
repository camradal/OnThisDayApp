using System;
using System.IO;
using AgFx;
using OnThisDayApp.Parsers;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.DataAccess
{
    public abstract class PageLoaderBase : IDataLoader<DayLoadContext>
    {
        protected readonly PageParser parser = new PageParser();
        protected abstract string SourceUriFormat { get; }

        public LoadRequest GetLoadRequest(DayLoadContext loadContext, Type objectType)
        {
            string uri = string.Format(SourceUriFormat, loadContext.Day);
            return new WebLoadRequest(loadContext, new Uri(uri, UriKind.Absolute));
        }

        /// <summary>
        /// Executes once LoadRequest has executed. Will also happen when deserializing cached data
        /// </summary>
        public abstract object Deserialize(DayLoadContext loadContext, Type objectType, Stream stream);
    }
}