using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class Region
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public bool Status { get; set; }
        public System.Nullable<int> NeighborId { get; set; }

        public Item Neighbor { get; set; }
        public List<Item> ListWard { get; set; }
        public List<UserItem> ListUser { get; set; }

        public int MaxAgency { get; set; }
    }
}
