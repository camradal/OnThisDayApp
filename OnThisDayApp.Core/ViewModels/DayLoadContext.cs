﻿using AgFx;

namespace OnThisDayApp.ViewModels
{
    /// <summary>
    /// Load context that loads with just a provided day
    /// </summary>
    public sealed class DayLoadContext : LoadContext
    {
        public string Day
        {
            get
            {
                return (string)Identity;
            }
        }

        public DayLoadContext(string day) : base(day)
        {
        }
    }
}