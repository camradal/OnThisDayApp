using System.Collections.Generic;

namespace OnThisDayApp.ViewModels
{
    public sealed class Entry
    {
        public string Year { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
        public Dictionary<string, string> Links { get; set; }
    }
}
