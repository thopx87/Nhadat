using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class Topic
    {
        public int ID { get; set; }
        public string SystemName { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
    }
    
}
