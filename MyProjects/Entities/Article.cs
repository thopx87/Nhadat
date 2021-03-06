using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class Article
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Body { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyWord { get; set; }
        public string MetaDescription { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateBy { get; set; }
        public bool Display { get; set; }

        // More
        public string CategoryText { get; set; }
    }
}
