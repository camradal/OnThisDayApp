using System;
using System.Collections.Generic;
using OnThisDayApp.ViewModels;

namespace OnThisDayApp.DataAccess
{
    public sealed class PageLoadedEventArgs : EventArgs
    {
        public List<EntryViewModel> Events { get; private set; }

        public PageLoadedEventArgs(List<EntryViewModel> entries)
        {
            this.Events = entries;
        }
    }
}
