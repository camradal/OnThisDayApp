using System;
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
        public IEnumerable<EntryViewModel> ExtractHighlightEntriesFromHtml(Stream stream)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream);

            HtmlNode listNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='mw-content-ltr']/ul");
            IEnumerable<EntryViewModel> entries = listNode.Descendants("li").Select(ExtractEntryFromNode);

            return entries;
        }

        private static EntryViewModel ExtractEntryFromNode(HtmlNode node)
        {
            try
            {
                EntryViewModel entry = new EntryViewModel();

                string innerText = node.InnerText;

                // assumption - there is always a space around the separator
                // otherwise some string truncation will occur
                int splitIndex = innerText.IndexOfAny(new[] { '–', '-' });
                string secondPart = innerText.Substring(splitIndex + 2);
                string firstPart = innerText.Substring(0, splitIndex - 1);

                entry.Year = firstPart;
                entry.Description = HttpUtility.HtmlDecode(secondPart);
                entry.Links = ExtractAllLinksFromHtmlNode(node);
                entry.Link = ExtractFirstLink(node, entry);

                return entry;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Dictionary<string, string> ExtractAllLinksFromHtmlNode(HtmlNode entry)
        {
            return entry.Descendants("a").Select(
                item => new { text = item.InnerText, url = item.Attributes["href"].Value }).ToDictionary(
                    pair => pair.text, pair => pair.url);
        }

        private static string ExtractFirstLink(HtmlNode node, EntryViewModel entry)
        {
            string firstLink;
            HtmlNode firstBoldItem = node.Descendants("b").FirstOrDefault();
            if (firstBoldItem != null)
            {
                firstLink = firstBoldItem.Descendants("a").First().Attributes["href"].Value;
            }
            else
            {
                // assumption - there is always one link besides a year
                int value;
                firstLink = entry.Links.FirstOrDefault(link => !int.TryParse(link.Key, out value)).Value;
            }
            return firstLink;
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

            // TODO: uncomment for next version
            //List<EntryViewModel> holidays = ExtractHolidays(htmlDoc, "Holidays_and_observances");

            Dictionary<string, List<EntryViewModel>> result = new Dictionary<string, List<EntryViewModel>>
                                                              {
                                                                  {"Events", events},
                                                                  {"Births", births},
                                                                  {"Deaths", deaths}
                                                              };
            //result.Add("Holidays", holidays);

            return result;
        }

        private static List<EntryViewModel> ExtractById(HtmlDocument htmlDoc, string id)
        {
            var otd = htmlDoc.GetElementbyId(id);
            var list = otd.ParentNode.NextSibling.NextSibling;
            var entries = list.Descendants("li");

            return entries.Select(ExtractEntryFromNode).ToList();
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
