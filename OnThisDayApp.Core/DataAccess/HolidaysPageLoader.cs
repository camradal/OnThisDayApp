using System;
using System.IO;
using OnThisDayApp.Parsers;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.DataAccess
{
    public sealed class HolidaysPageLoader : PageLoaderBase
    {
        private const string sourceUriFormat = @"http://en.wikipedia.org/w/api.php?action=parse&prop=text&page={0}&section=4&format=xml";

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
            var holidays = PageParser.ExtractHolidaysFromHtml(html);
            var viewModel = new HolidaysViewModel(loadContext);

            foreach (Entry entry in holidays)
            {
                viewModel.Holidays.Add(entry);
            }

            return viewModel;
        }
    }
}
