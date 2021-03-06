using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Application2016.Areas.Admin.Models
{
    /// <summary>
    /// Model City
    /// </summary>
    public class CityModel : BaseModel
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public bool isCity { get; set; } // true --> type = thành phố; false --> type= tỉnh
        public string Address { get; set; }
    }

    /// <summary>
    /// Model District
    /// </summary>
    public class DistrictModel
    {
        public int Id { get; set; }
        public int Parent { get; set; }
        public IEnumerable<Entities.Item> ListCity { get; set; }
        public string Text { get; set; }
        public short districtType { get; set; }
        public string Address { get; set; }
    }

    /// <summary>
    /// Model Ward
    /// </summary>
    public class WardModel
    {
        public int Id { get; set; }

        [Required (ErrorMessage="Yêu cầu nhập tên")]
        public string Text { get; set; }
        
        [Required(ErrorMessage="Yêu cầu chọn tỉnh, thành phố")]
        public int CityId { get; set; }

        [Required(ErrorMessage="Yêu cầu chọn quận, huyện")]
        public int Parent { get; set; }

        public IEnumerable<Entities.Item> ListCity { get; set; }
        public IEnumerable<Entities.Item> ListDistrict { get; set; }
        public IEnumerable<Entities.Item> ListWard { get; set; }
        
        public short wardType { get; set; } // true -->Type = Phường; false --> Xã
        public string Address { get; set; }
        public System.Nullable<decimal> Longitude { get; set; }
        public System.Nullable<decimal> Latitude { get; set; }

        public System.Nullable<int> RegionId { get; set; }
        public System.Nullable<int> MaxAgency { get; set; }// Số lượng môi giới tối đa. xử dụng cho cấp xã
        public System.Nullable<int> MaxAgencyFree { get; set; } // Số lượng môi giới tối đa cho người đăng ký online.

        /// <summary>
        /// Xử dụng khi cập nhật dữ liệu
        /// </summary>
        /// <param name="m"></param>
        /// <param name="e"></param>
        public void MapFrom(WardModel m, ref Entities.Place e)
        {
            e.Id = m.Id;
            e.Text = m.Text;
            e.Type = m.wardType;
            e.Address = m.Address;
            e.Parent = m.Parent;
            e.RegionId = m.RegionId;
            e.MaxAgency = m.MaxAgency;
            e.MaxAgencyFree = m.MaxAgencyFree;
        }

        public void MapFrom(Entities.Place e, ref WardModel m)
        {
            m.Id = e.Id;
            m.Text = e.Text;
            m.wardType = e.Type;
            m.Address = e.Address;
            m.Parent = e.Parent;
            m.RegionId = e.RegionId;
            m.MaxAgency = e.MaxAgency;
            m.MaxAgencyFree = e.MaxAgencyFree;
        }
    }

    public class PlaceCondition: BaseModel
    {
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public int RegionId { get; set; }
        public string strCityId { get; set; }
        public string strDistrictId { get; set; }
    }

    public class PlaceModel
    {
        public PlaceCondition Condition { get; set; }
        public List<Entities.Place> ListCity { get; set; }
        public List<Entities.Place> ListDistrict { get; set; }
        public List<Entities.Place> ListWard { get; set; }
        public List<Entities.Region> ListRegion { get; set; }
    }
}