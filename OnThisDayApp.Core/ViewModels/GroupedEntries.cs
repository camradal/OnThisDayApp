using System.Collections.Generic;

namespace OnThisDayApp.ViewModels
{
    public class GroupedEntries : System.Collections.IEnumerable
    {
        private readonly List<Entry> entries =
            new List<Entry>();

        public string Name { get; set; }

        public List<Entry> Entries
        {
            get { return entries; }
            set
            {
                SetCollection(value, entries);
            }
        }

        private void SetCollection(
            IEnumerable<Entry> source,
            List<Entry> destination)
        {
            if (destination != null)
            {
                destination.Clear();

                if (source != null)
                {
                    foreach (Entry item in source)
                    {
                        destination.Add(item);
                    }
                }
            }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return this.entries.GetEnumerator();
        }
    }
}
