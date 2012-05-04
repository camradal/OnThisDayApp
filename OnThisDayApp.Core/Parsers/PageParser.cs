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

        public static List<Entry> ExtractHighlightEntriesFromHtml(Stream stream)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream);

            HtmlNode listNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='mw-content-ltr']/ul");
            List<Entry> entries = listNode.Descendants("li").Select(ExtractEntryFromNode).Where(e => e != null).ToList();
            AttachPictureToNode(htmlDoc, entries);

            return entries;
        }

        private static void AttachPictureToNode(HtmlDocument htmlDoc, List<Entry> entries)
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

        public static Dictionary<string, List<Entry>> ExtractEventEntriesFromHtml(Stream stream)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream);

            List<Entry> events = ExtractById(htmlDoc, "Events");
            List<Entry> births = ExtractById(htmlDoc, "Births");
            List<Entry> deaths = ExtractById(htmlDoc, "Deaths");

            List<Entry> holidays;
            try
            {
                holidays = ExtractHolidays(htmlDoc, "Holidays_and_observances");
            }
            catch (Exception)
            {
                Entry entry = new Entry();
                entry.Year = "Can't find any holidays for this day";
                holidays = new List<Entry>() { entry };
            }

            var result = new Dictionary<string, List<Entry>>
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

        private static Entry ExtractEntryFromNode(HtmlNode node)
        {
            try
            {
                Entry entry = new Entry();

                string innerText = HttpUtility.HtmlDecode(node.InnerText);

                // assumption - there is always a space around the separator
                // otherwise some string truncation will occur
                int splitIndex = innerText.IndexOfAny(new[] { '–', '-', '—' });
                string secondPart = innerText.Substring(splitIndex + 1);
                string firstPart = innerText.Substring(0, splitIndex);

                entry.Year = firstPart.Trim();
                entry.Description = secondPart.Trim();
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
                if (!result.ContainsKey(hyperlink.Key))
                {
                    result.Add(hyperlink.Key, hyperlink.Value);
                }
            }

            return result;
        }

        private static string ExtractFirstLink(HtmlNode node, Entry entry)
        {
            // assumption - there is always one link besides a year
            int value;
            string firstLink = entry.Links.FirstOrDefault(e => !int.TryParse(e.Key, out value) && e.Key != "share...").Value;

            HtmlNode firstBoldItem = node.Descendants("b").FirstOrDefault();
            if (firstBoldItem != null)
            {
                var first = firstBoldItem.Descendants("a").FirstOrDefault();
                if (first != null)
                {
                    firstLink = first.Attributes["href"].Value;
                }
            }

            return firstLink;
        }

        private static Entry ExtractHolidayFromNode(HtmlNode node)
        {
            Entry entry = new Entry();
            entry.Links = ExtractAllLinksFromHtmlNode(node);
            entry.Link = ExtractFirstLink(node, entry);

            // put sublists into a description
            if (node.HasChildNodes)
            {
                // TODO: redo this as node parsing
                HtmlNode extraListNode = node.Descendants("ul").FirstOrDefault();
                if (extraListNode != null)
                {
                    entry.Description = HttpUtility.HtmlDecode(extraListNode.InnerText).Trim();
                    node.RemoveChild(extraListNode);
                }
            }

            entry.Year = HttpUtility.HtmlDecode(node.InnerText.Trim().TrimEnd(':'));

            return entry;
        }

        private static List<Entry> ExtractById(HtmlDocument htmlDoc, string id)
        {
            var otd = htmlDoc.GetElementbyId(id);
            var parentNode = otd.ParentNode;
            var currentNode = parentNode.NextSibling;
            while (currentNode != null && currentNode.Name != "ul")
            {
                currentNode = currentNode.NextSibling;
            }

            var entries = currentNode.Descendants("li");
            return entries.Select(ExtractEntryFromNode).Where(e => e != null).ToList();
        }

        private static List<Entry> ExtractHolidays(HtmlDocument htmlDoc, string id)
        {
            var otd = htmlDoc.GetElementbyId(id);
            var parentNode = otd.ParentNode;
            var currentNode = parentNode.NextSibling;
            while (currentNode != null && currentNode.Name != "ul")
            {
                currentNode = currentNode.NextSibling;
            }
            var entries = currentNode.ChildNodes.Where(node => node.Name == "li");

            List<Entry> events = new List<Entry>();
            foreach (var entry in entries)
            {
                try
                {
                    Entry newEvent = ExtractHolidayFromNode(entry);
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
