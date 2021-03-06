using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application2016.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public int UserId { get; set; } // ID người đăng
        public string Username { get; set; } // Username người đăng

        public short Transaction_Type { get; set; } // Kiểu trao đổi: mua, bán, cho thuê
        public short Product_Type { get; set; } // Kiểu nhà: trung cư, thổ cư, trong ngõ, mặt phố...
        public string ProductTypeText { get; set; }// Kiểu nhà bằng chữ.
        public int CityId { get; set; } // ID tỉnh, thành phố
        public string CityText { get; set; }
        public int DistrictId { get; set; } // ID quận huyện
        public string DistrictText { get; set; }
        public int WardId { get; set; } // ID xã, phường.
        public string Text { get; set; } // Tiêu đề
        public string HouseNumber { get; set; } // Số nhà
        public string Address { get; set; } // Địa chỉ chi tiết
        public decimal Area { get; set; } // Diện tích ngôi nhà (met vuông)
        public string Description { get; set; } // Mô tả ngôi nhà
        public decimal StandardCost { get; set; } // Giá chuẩn (giá khi đăng, giá gốc)
        public decimal NewCost { get; set; } // Giá bán mới nhất.
        public System.Nullable<System.DateTime> SellStartDate { get; set; } // Bán từ ngày (mặc định lấy thời gian hiện tại)
        public System.Nullable<System.DateTime> SellEndDate { get; set; } // Bán đến ngày

        public System.Nullable<short> State { get; set; } // Trạng thái (đã bán, chưa bán)
        public System.Nullable<decimal> AgencyCost { get; set; } // Chi phí cho môi giới
        public System.Nullable<short> CostUnit { get; set; } // Đơn vị giá (%, đồng, dola) khi tính phí môi giới
        public string Tag { get; set; } // Thẻ để seo
        public System.Nullable<bool> Approval_Flag { get; set; } // Cờ thông báo đã phê duyệt
        public System.Nullable<int> ApprovalBy { get; set; } // Người phê duyệt
        public System.Nullable<System.DateTime> ApprovalDate { get; set; } // Ngày phê duyệt
        public System.Nullable<bool> Delete_Flag { get; set; } // Cờ Delete
        public System.Nullable<int> DeleteBy { get; set; } // Người delete
        public System.Nullable<System.DateTime> DeleteDate { get; set; } // Ngày delete
        public string Avatar { get; set; } // Avatar
        public string Avatar_Url { get; set; } // Url avatar.
        public System.Nullable<decimal> UpdateCost { get; set; } // Cập nhật giá
        public System.Nullable<System.DateTime> UpdateTime { get; set; } // Ngày đăng
        public System.Nullable<int> UpdateBy { get; set; } // Người đăng

        public string strProduct_type { get; set; }// Kiểu nhà bằng chữ
        public List<Entities.ProductImage> ListImage { get; set; }
        public bool IsPoster { get; set; } // Kiểm tra có phải là người post hay không.
        public List<Entities.Product_ChangeCost> ListChangeCost { get; set; }

        public string PhoneNumber { get; set; } // Điện thoại người đăng

        public void MapFrom(Entities.Product e, ref Models.ProductModel m)
        {
            m.Id = e.Id;
            m.UserId = e.UserId;
            m.Transaction_Type = e.Transaction_Type;
            m.Product_Type = e.Product_Type;
            m.CityId = e.CityId;
            m.CityText = e.CityText;
            m.DistrictId = e.DistrictId;
            m.DistrictText = e.DistrictText;
            m.WardId = e.WardId;
            m.Text = e.Text;
            m.HouseNumber = e.HouseNumber;
            m.Address = e.Address;
            m.Area = e.Area;
            m.Description = e.Description;
            m.StandardCost = e.StandardCost;
            m.SellStartDate = e.SellEndDate;
            m.SellEndDate = e.SellEndDate;
            m.State = e.State;
            m.AgencyCost = e.AgencyCost;
            m.CostUnit = e.CostUnit;
            m.Tag = e.Tag;
            m.Approval_Flag = e.Approval_Flag;
            m.ApprovalBy = e.ApprovalBy;
            m.ApprovalDate = e.ApprovalDate;
            m.Delete_Flag = e.Delete_Flag;
            m.DeleteBy = e.DeleteBy;
            m.DeleteDate = e.DeleteDate;
            m.Avatar = e.Avatar;
            m.Avatar_Url = e.Avatar_Url;
            m.UpdateCost = e.UpdateCost;
            m.UpdateBy = e.UpdateBy;
            m.UpdateTime = e.UpdateTime;
        }
    }

    public class ProductConditions
    {
        public string displayType { get; set; }
        public short ProductType { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public string Text { get; set; }
        public int Page { get; set; }
        public int OrderBy { get; set; }
        public int Limit { get; set; }

        // List
        public List<Entities.Item> ListProductType { get; set; }
        public List<Entities.Place> ListCity { get; set; }
        public List<Entities.Place> ListWard { get; set; }
        public List<Entities.Place> ListDistrict { get; set; }
        public List<SelectListItem> ListLimit { get; set; }

        public ProductConditions()
        {
            displayType = "Grid";
            Page = 1;
            Limit = 20;
            ListProductType = new List<Entities.Item>();
            ListCity = new List<Entities.Place>();
            ListDistrict = new List<Entities.Place>();
            ListWard = new List<Entities.Place>();
            ListLimit = new List<SelectListItem>();
        }

        public void MapFrom(ProductConditions m, ref Entities.ProductCondtions e)
        {
            e.ProductType = m.ProductType;
            e.City = m.CityId;
            e.District = m.DistrictId;
            e.Ward = m.WardId;
            e.Text = m.Text;
            e.Page = m.Page;
            e.OrderBy = m.OrderBy;
            e.Limit = m.Limit;
        }
    }

    public class Product_ChangeCost
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int Cost { get; set; }
        public short Times { get; set; }
        public System.DateTime UpdateTime { get; set; }

        public void MapFrom(Product_ChangeCost m, ref Entities.Product_ChangeCost e)
        {
            e.Id = m.Id;
            e.ProductId = m.ProductId;
            e.UserId = m.UserId;
            e.UserName = m.UserName;
            e.Cost = m.Cost;
        }
    }

    public class ProductListMessage
    {
        public List<Entities.ProductMessage> ListMessage { get; set; }
        public Models.BaseModel Condition { get; set; }
    }

}