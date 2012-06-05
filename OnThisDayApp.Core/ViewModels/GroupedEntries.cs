using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using AgFx;
using OnThisDayApp.ViewModels;

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
