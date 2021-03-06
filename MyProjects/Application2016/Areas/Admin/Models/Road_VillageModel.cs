using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Application2016.Areas.Admin.Models
{
    public class Road_VillageModel : BaseModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage="Yêu cầu nhập tên")]
        public string Text { get; set; }

        public string Description { get; set; }

        public bool Type { get; set; }

        [Required(ErrorMessage="Yêu cầu chọn xã, phường")]
        public int WardId { get; set; }

        [Required(ErrorMessage="Yêu cầu chọn vùng")]
        public int RegionId { get; set; }

        public int CityId { get; set; }
        public int DistrictId { get; set; }

        public List<Entities.Item> ListCity { get; set; }
        public List<Entities.Item> ListDistrict { get; set; }
        public List<Entities.Item> ListWard { get; set; }

        public List<Entities.Region> ListRegion { get; set; }

        public void MapFrom(Models.Road_VillageModel m, ref Entities.Road_Village e)
        {
            e.Id = m.Id;
            e.Text = m.Text;
            e.Description = m.Description;
            e.Type = m.Type;
            e.WardId = m.WardId;
            e.RegionId = m.RegionId;
        }

        public void MapFrom(Entities.Road_Village e, ref Models.Road_VillageModel m)
        {
            m.Id = e.Id;
            m.Text = e.Text;
            m.Description = e.Description;
            m.Type = e.Type;
            m.WardId = e.WardId;
            m.RegionId = e.RegionId;
        }
    }
}