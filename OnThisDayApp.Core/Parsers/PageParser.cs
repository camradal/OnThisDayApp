using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.Parsers
{
    public static class PageParser
    {
        private const string startToken = "<text xml:space=\"preserve\">";
        private const string endToken = "</text>";

        #region Public Methods

        public static string GetHtml(Stream stream)
        {
            string html;
            using (var reader = new StreamReader(stream))
            {
                var text = reader.ReadToEnd();
                int startIndex = text.IndexOf(startToken, StringComparison.OrdinalIgnoreCase) + startToken.Length;
                int endIndex = text.IndexOf(endToken, startIndex, StringComparison.OrdinalIgnoreCase);
                if (startIndex == -1 || endIndex == -1)
                {
                    throw new Exception(text);
                }

                html = text.Substring(startIndex, endIndex - startIndex);
                html = HttpUtility.HtmlDecode(html);
            }

            return html;
        }

        public static List<Entry> ExtractEntriesFromHtml(string html, bool selectLastListOnly)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            IEnumerable<HtmlNode> nodes = selectLastListOnly ?
                htmlDoc.DocumentNode.Descendants("ul").Last().Descendants("li") :
                htmlDoc.DocumentNode.Descendants("ul").SelectMany(u => u.Descendants("li"));
            List<Entry> entries = nodes.Select(ExtractEntryFromNode).Where(e => e != null).ToList();
            AttachPictureToNode(htmlDoc, entries);

            return entries;
        }

        public static List<Entry> ExtractHolidaysFromHtml(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var nodes = htmlDoc.DocumentNode.Descendants("ul").SelectMany(u => u.Descendants("li"));
            var events = new List<Entry>();
            foreach (var node  in nodes)
            {
                try
                {
                    Entry newEvent = ExtractHolidayFromNode(node);
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

        private static void AttachPictureToNode(HtmlDocument htmlDoc, List<Entry> entries)
        {
            HtmlNode imageNode = htmlDoc.DocumentNode.Descendants("img").LastOrDefault();
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

            var result = new Dictionary<string, string>
            {
                { "share...", string.Empty }
            };
            foreach (KeyValuePair<string, string> hyperlink in hyperlinks)
            {
                if (!result.ContainsKey(hyperlink.Key))
                {
                    int year;
                    if (!int.TryParse(hyperlink.Key, out year))
                    {
                        result.Add(hyperlink.Key, hyperlink.Value);
                    }
                }
            }

            return result;
        }

        private static string ExtractFirstLink(HtmlNode node, Entry entry)
        {
            // assumption - there is always one link besides a year
            int value;
            string firstLink = entry.Links.FirstOrDefault(e => !Int32.TryParse(e.Key, out value) && e.Key != "share...").Value;

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
            var entry = new Entry();
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

        #endregion

        public static Func<Entry, string> GroupByYear()
        {
            return item =>
                   {
                       int length = item.Year.Length;
                       if (length == 4)
                       {
                           return item.Year.StartsWith("19") || item.Year.StartsWith("20")
                                      ? item.Year.Substring(0, 3) + "0s"
                                      : item.Year.Substring(0, 2) + "00s";
                       }
                       if (length == 3)
                       {
                           return item.Year.Substring(0, 1) + "00s";
                       }
                       if (item.Year.EndsWith("BC") || item.Year.EndsWith("BCE"))
                       {
                           return "BC";
                       }
                       return "AD";
                   };
        }
    }
}
