using System;
using System.Collections.Generic;

namespace OnThisDayApp.Model
{
    public sealed class Day
    {
        public DateTime Date { get; set; }
        public List<Event> Events { get; set; }
    }
}
