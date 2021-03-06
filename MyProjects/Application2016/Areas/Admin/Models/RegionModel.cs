using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Application2016.Areas.Admin.Models
{
    /// <summary>
    /// Model Region
    /// </summary>
    public class RegionModel : BaseModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Yêu cầu nhập tên vùng")]
        public string Text { get; set; }

        [Required(ErrorMessage="Yêu cầu chọn tỉnh, thành phố")]
        public int CityId { get; set; }

        [Required(ErrorMessage="Yêu cầu chọn quận, huyện")]
        public int DistrictId { get; set; }

        public bool Status { get; set; }

        public System.Nullable<int> NeighborId { get; set; }

        public IEnumerable<Entities.Item> ListCity { get; set; }
        public IEnumerable<Entities.Item> ListDistrict { get; set; }

        [Required(ErrorMessage="Yêu cầu chọn xã, phường")]
        public List<int> WardOfRegionIds { get; set; }
        public IEnumerable<Entities.Item> ListWardOfRegion { get; set; }

        public List<int> WardOfDistrictIds { get; set; }
        public IEnumerable<Entities.Item> ListWardOfDistrict { get; set; }

        public IEnumerable<Entities.Item> ListRegion { get; set; }

        public void MapFrom(Entities.Region e, ref Models.RegionModel m)
        {
            m.Id = e.Id;
            m.Text = e.Text;
            m.CityId = e.CityId;
            m.DistrictId = e.DistrictId;
            m.Status = e.Status;
            m.NeighborId = e.NeighborId;
        }

        public void MapFrom(Models.RegionModel m, ref Entities.Region e)
        {
            e.Id = m.Id;
            e.Text = m.Text;
            e.CityId = m.CityId;
            e.DistrictId = m.DistrictId;
            e.Status = m.Status;
            e.NeighborId = m.NeighborId;
        }

    }

    public class RegionUserModel
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public int RoleId { get; set; }
        public List<Entities.Item> ListRole { get; set; }

        [Required(ErrorMessage = "Yêu cầu chọn user")]
        public List<int> UserOfRegionIds { get; set; }

        //public IEnumerable<Entities.UserItem> ListUserOfRegion { get; set; }

        public List<Entities.Item> ListUserOfRegion { get; set; }

        // Danh sách người dùng (loại môi giới)
        public List<int> Users { get; set; }
        //public IEnumerable<Entities.UserItem> ListUsers { get; set; }
        public IEnumerable<Entities.Item> ListUsers { get; set; }

        public int MaxAgency { get; set; }
    }

    public class RegionExcel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public int Status { get; set; }
        public int NeighborId { get; set; }
        public string ListWard { get; set; }
        public string ListUser { get; set; }
    }

    public class ListRegionModel
    {
        public List<Entities.Region> ListRegion { get; set; }
        public RegionCondition Condition { get; set; }
        public List<Entities.Item> ListCity { get; set; }
        public List<Entities.Item> ListDistrict { get; set; }
    }

    public class RegionCondition:BaseModel
    {
        public int CityId { get; set; }
        public int DistrictId { get; set; }
    }
}