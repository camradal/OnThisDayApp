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

namespace OnThisDayApp.Parsers
{
    public class PageParser
    {
        private static Regex extractor = new Regex(@"(<li.+?(?<Year>[\d]{1,4}).+? [–-] (?<Description>.*?(<b>.+href=""(?<Link>.*?)"".+</b>).+)</li>)+");
        private static Regex htmlTags = new Regex(@"<.*?>");

        public List<Event> ExtractOnThisDayFromTextHtml(string html)
        {
            List<Event> events = new List<Event>();

            MatchCollection matches = extractor.Matches(html);
            foreach (Match match in matches)
            {
                Event newEvent = new Event();
                newEvent.Year = int.Parse(match.Groups["Year"].Value);
                newEvent.Description = htmlTags.Replace(match.Groups["Description"].Value, string.Empty);
                newEvent.Link = match.Groups["Link"].Value;
                events.Add(newEvent);
            }

            return events;
        }
    }
}
