using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class Category
    {
        public int Id { get; set; }
        public int Parent { get; set; }
        public string Text { get; set; }
        public string Code { get; set; }
        public int SortOrder { get; set; }
        public int Type { get; set; }
        public string TypeText { get; set; }
        public bool Display { get; set; }

        // More
        public string ParentText { get; set; }
    }
}
