using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.Parsers
{
    public sealed class PageParser
    {
        public List<EntryViewModel> ExtractHighlightEntriesFromHtml(Stream stream)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream);
            var list = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='mw-content-ltr']/ul");

            List<EntryViewModel> events = new List<EntryViewModel>();
            foreach (var entry in list.Descendants("li"))
            {
                try
                {
                    EntryViewModel newEvent = ExtractEntryFromNode(entry);
                    events.Add(newEvent);
                }
                catch
                {
                    // TODO
                    // failed to add 1 event, skip for now
                }
            }


                //Event newEvent = new Event();
                //newEvent.Year = int.Parse(match.Groups["Year"].Value);
                //newEvent.Description = htmlTags.Replace(match.Groups["Description"].Value, string.Empty);
                //newEvent.Link = match.Groups["Link"].Value;
                //events.Add(newEvent);

            return events;
        }

        private static EntryViewModel ExtractEntryFromNode(HtmlNode entry)
        {
            EntryViewModel newEvent = new EntryViewModel();

            var yearLink = entry.Descendants("a").First();
            var description = entry.InnerText;

            int firstSpace = description.IndexOf(' ');
            int secondSpace = description.IndexOf(' ', firstSpace + 1);
            description = description.Substring(secondSpace + 1);
            description = HttpUtility.HtmlDecode(description);

            HtmlNode firstLink;

            var firstBoldItem = entry.Descendants("b").FirstOrDefault();
            if (firstBoldItem != null)
            {
                firstLink = firstBoldItem.Descendants("a").First();
            }
            else
            {
                firstLink = entry.Descendants("a").Skip(1).First();
            }

            newEvent.Year = yearLink.InnerText;
            newEvent.Description = description;
            newEvent.Link = firstLink.Attributes["href"].Value;
            return newEvent;
        }

        private static EntryViewModel ExtractHolidayFromNode(HtmlNode entry)
        {
            EntryViewModel newEvent = new EntryViewModel();

            string description = entry.InnerText;
            description = HttpUtility.HtmlDecode(description);

            HtmlNode firstLink;

            var firstBoldItem = entry.Descendants("b").FirstOrDefault();
            if (firstBoldItem != null)
            {
                firstLink = firstBoldItem.Descendants("a").First();
            }
            else
            {
                firstLink = entry.Descendants("a").First();
            }

            newEvent.Description = description;
            newEvent.Link = firstLink.Attributes["href"].Value;
            return newEvent;
        }

        public Dictionary<string, List<EntryViewModel>> ExtractEventEntriesFromHtml(Stream stream)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream);

            List<EntryViewModel> events = ExtractById(htmlDoc, "Events");
            List<EntryViewModel> births = ExtractById(htmlDoc, "Births");
            List<EntryViewModel> deaths = ExtractById(htmlDoc, "Deaths");
            List<EntryViewModel> holidays = ExtractHolidays(htmlDoc, "Holidays_and_observances");

            Dictionary<string, List<EntryViewModel>> result = new Dictionary<string, List<EntryViewModel>>();
            result.Add("Events", events);
            result.Add("Births", births);
            result.Add("Deaths", deaths);
            result.Add("Holidays", holidays);

            return result;
        }

        private static List<EntryViewModel> ExtractById(HtmlDocument htmlDoc, string id)
        {
            var otd = htmlDoc.GetElementbyId(id);
            var list = otd.ParentNode.NextSibling.NextSibling;
            var entries = list.Descendants("li");

            List<EntryViewModel> events = new List<EntryViewModel>();
            foreach (var entry in entries)
            {
                try
                {
                    EntryViewModel newEvent = ExtractEntryFromNode(entry);
                    events.Add(newEvent);
                }
                catch
                {
                    // TODO
                    // failed to add 1 event, skip for now
                }
            }
            return events;
        }

        private static List<EntryViewModel> ExtractHolidays(HtmlDocument htmlDoc, string id)
        {
            var otd = htmlDoc.GetElementbyId(id);
            var list = otd.ParentNode.NextSibling.NextSibling;
            var entries = list.Descendants("li");

            List<EntryViewModel> events = new List<EntryViewModel>();
            foreach (var entry in entries)
            {
                try
                {
                    EntryViewModel newEvent = ExtractHolidayFromNode(entry);
                    events.Add(newEvent);
                }
                catch
                {
                    // TODO
                    // failed to add 1 event, skip for now
                }
            }
            return events;
        }
    }
}
