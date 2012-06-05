using System;

namespace Utilities
{
    internal static class RandomLoader
    {
        private static readonly Random random = new Random();

        private static readonly string[] strings = new[]
                                                   {
                                                       "Tap any entry to jump to wiki",
                                                       "Hold any entry for context menu",
                                                       "Hold any entry to share it",
                                                       "Check the about page to see what's new",
                                                       "Pin the live tile for updates",
                                                       "Comments? Send us your thoughts",
                                                       "Like this app? Please rate it",
                                                       "Checking the archives",
                                                       "Sorting dates and figures",
                                                       "Uncovering the history",
                                                       "Digitizing the collections",
                                                       "Converting the calendar",
                                                       "Double-checking the facts"
                                                   };

        public static string GetRandomString()
        {
            return strings[random.Next(strings.Length)];
        }
    }
}