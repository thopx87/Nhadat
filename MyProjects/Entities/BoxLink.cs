using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class BoxLink
    {
        public int Id { get; set; }
        public bool Display { get; set; }
        public int Order { get; set; }
        public int Position { get; set; }
        public string BoxName { get; set; }
        public bool DisplayBoxName { get; set; }
        public string Color { get; set; }
        public string FromTable { get; set; }
        public string ColumnName { get; set; }
        public string Condition { get; set; }
        public int Count { get; set; }
        public string OrderBy { get; set; }

        public List<LinkItem> ListLink { get; set; }

        public BoxLink()
        {
            ListLink = new List<LinkItem>();
        }
    }

    public class LinkItem
    {
        public string Icon { get; set; }
        public string LinkUrl { get; set; }
        public string Text { get; set; }
    }
}
