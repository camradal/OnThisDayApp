using AgFx;

namespace OnThisDayApp.ViewModels
{
    /// <summary>
    /// Load context that loads with just a provided day
    /// </summary>
    public sealed class DayLoadContext : LoadContext
    {
        public bool ReverseOrder { get; private set; }

        public string Day
        {
            get
            {
                return (string)Identity;
            }
        }

        public DayLoadContext(string day)
            : this(day, false)
        {
        }

        public DayLoadContext(string day, bool reverseOrder)
            : base(day)
        {
            ReverseOrder = reverseOrder;
        }
    }
}
