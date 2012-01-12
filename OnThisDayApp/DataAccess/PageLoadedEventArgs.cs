using System;
using System.Collections.Generic;
using OnThisDayApp.Model;

namespace OnThisDayApp.DataAccess
{
    public sealed class PageLoadedEventArgs : EventArgs
    {
        public List<Event> Events { get; private set; }

        public PageLoadedEventArgs(List<Event> events)
        {
            this.Events = events;
        }
    }
}
