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
using OnThisDayApp.Model;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Linq;

namespace OnThisDayApp.Parsers
{
    public sealed class PageParser
    {
        public List<Event> ExtractOnThisDayFromTextHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var otd = htmlDoc.GetElementbyId("mp-otd");
            var entries = otd.Descendants("li");

            List<Event> events = new List<Event>();
            foreach (var entry in entries)
            {
                try
                {
                    var yearLink = entry.Descendants("a").First();
                    var description = entry.InnerText;
                    int firstSpace = description.IndexOf(' ');
                    int secondSpace = description.IndexOf(' ', firstSpace + 1);
                    description = description.Substring(secondSpace + 1);
                    description = HttpUtility.HtmlDecode(description);
                    var firstLink = entry.Descendants("b").First().Descendants("a").First();
                    Event newEvent = new Event();
                    newEvent.Year = int.Parse(yearLink.InnerText);
                    newEvent.Description = description;
                    newEvent.Link = firstLink.Attributes["href"].Value;
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
    }
}
