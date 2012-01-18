using System;
using System.Collections.Generic;
using OnThisDayApp.Models;

namespace OnThisDayApp.DataAccess
{
    public sealed class PageLoadedEventArgs : EventArgs
    {
        public List<Entry> Events { get; private set; }

        public PageLoadedEventArgs(List<Entry> entries)
        {
            this.Events = entries;
        }
    }
}
