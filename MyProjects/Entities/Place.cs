using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class Place
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Parent { get; set; }
        public short Type { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public System.Nullable<decimal> Longitude { get; set; }
        public System.Nullable<decimal> Latitude { get; set; }

        public List<Place> ListChild { get; set; }

        //Region ID is one element of place.
        public System.Nullable<int> RegionId { get; set; } // xử dụng cho cấp xã
        public System.Nullable<int> MaxAgency { get; set; }// xử dụng cho cấp xã
        public System.Nullable<int> MaxAgencyFree { get; set; } // Xử dụng cho môi giới online

        public int CountRegion { get; set; }
        public int CountAgency { get; set; }
    }
}
