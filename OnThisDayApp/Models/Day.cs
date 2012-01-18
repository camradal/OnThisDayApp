using System;
using System.Collections.Generic;

namespace OnThisDayApp.Models
{
    public sealed class Day
    {
        public DateTime Date { get; set; }
        public List<Entry> Events { get; set; }
    }
}
