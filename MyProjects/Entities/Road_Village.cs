using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class Road_Village
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public bool Type { get; set; }
        public int WardId { get; set; }
        public int RegionId { get; set; }

        public Item City { get; set; }
        public Item District { get; set; }
        public Item Ward { get; set; }

        public Item Region { get; set; }
    }
}
