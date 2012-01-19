using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Linq;
using System.IO;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.Parsers
{
    public sealed class PageParser
    {
        public List<EntryViewModel> ExtractHighlightEntriesFromHtml(Stream stream)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream);
            var otd = htmlDoc.GetElementbyId("mp-otd");
            var entries = otd.Descendants("li");

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
                firstLink = entry.Descendants("a").First();
            }

            newEvent.Year = int.Parse(yearLink.InnerText);
            newEvent.Description = description;
            newEvent.Link = firstLink.Attributes["href"].Value;
            return newEvent;
        }

        public List<EntryViewModel> ExtractEventEntriesFromHtml(Stream stream)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream);

            var otd = htmlDoc.GetElementbyId("Events");
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
    }
}
