using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.Parsers
{
    internal static class PageParser
    {
        #region Public Methods

        public static IEnumerable<EntryViewModel> ExtractHighlightEntriesFromHtml(Stream stream)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream);

            HtmlNode listNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='mw-content-ltr']/ul");
            List<EntryViewModel> entries = listNode.Descendants("li").Select(ExtractEntryFromNode).ToList();
            AttachPictureToNode(htmlDoc, entries);

            return entries;
        }

        private static void AttachPictureToNode(HtmlDocument htmlDoc, List<EntryViewModel> entries)
        {
            HtmlNode contentNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='mw-content-ltr']");
            HtmlNode imageNode = contentNode.Descendants("img").LastOrDefault();
            if (imageNode != null)
            {
                string url = "http:" + imageNode.Attributes["src"].Value;
                var pictureEntry = entries.FirstOrDefault(entry => entry.Description.Contains("pictured"));
                if (pictureEntry != null)
                {
                    pictureEntry.ImageUrl = url;
                }
            }
        }

        public static Dictionary<string, List<EntryViewModel>> ExtractEventEntriesFromHtml(Stream stream)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream);

            List<EntryViewModel> events = ExtractById(htmlDoc, "Events");
            List<EntryViewModel> births = ExtractById(htmlDoc, "Births");
            List<EntryViewModel> deaths = ExtractById(htmlDoc, "Deaths");

            List<EntryViewModel> holidays;
            try
            {
                holidays = ExtractHolidays(htmlDoc, "Holidays_and_observances");
            }
            catch (Exception)
            {
                EntryViewModel entry = new EntryViewModel();
                entry.Year = "Can't find any holidays for this day";
                holidays = new List<EntryViewModel>() { entry };
            }

            var result = new Dictionary<string, List<EntryViewModel>>
                         {
                             { "Events", events },
                             { "Births", births },
                             { "Deaths", deaths },
                             { "Holidays", holidays }
                         };

            return result;
        }

        #endregion

        #region Private Methods

        private static EntryViewModel ExtractEntryFromNode(HtmlNode node)
        {
            try
            {
                EntryViewModel entry = new EntryViewModel();

                string innerText = node.InnerText;

                // assumption - there is always a space around the separator
                // otherwise some string truncation will occur
                int splitIndex = innerText.IndexOfAny(new[] { '–', '-', '—' });
                string secondPart = innerText.Substring(splitIndex + 1);
                string firstPart = innerText.Substring(0, splitIndex);

                entry.Year = HttpUtility.HtmlDecode(firstPart).Trim();
                entry.Description = HttpUtility.HtmlDecode(secondPart).Trim();
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
            var hyperlinks = entry.Descendants("a").Select(_ =>
                new KeyValuePair<string, string>(
                    HttpUtility.HtmlDecode(_.InnerText).ToLower(),
                    _.Attributes["href"].Value));

            var result = new Dictionary<string, string>() { { "share...", string.Empty } };
            foreach (KeyValuePair<string, string> hyperlink in hyperlinks)
            {
                result.Add(hyperlink.Key, hyperlink.Value);
            }

            return result;
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

        private static EntryViewModel ExtractHolidayFromNode(HtmlNode node)
        {
            EntryViewModel entry = new EntryViewModel();
            entry.Links = ExtractAllLinksFromHtmlNode(node);
            entry.Link = ExtractFirstLink(node, entry);

            // put sublists into a description
            if (node.HasChildNodes)
            {
                HtmlNode extraListNode = node.Descendants("ul").FirstOrDefault();
                if (extraListNode != null)
                {
                    entry.Description = HttpUtility.HtmlDecode(extraListNode.InnerText.Trim());
                    node.RemoveChild(extraListNode);
                }
            }

            entry.Year = HttpUtility.HtmlDecode(node.InnerText.Trim().TrimEnd(':'));

            return entry;
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
            var entries = list.ChildNodes.Where(node => node.Name == "li");

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

        #endregion
    }
}
