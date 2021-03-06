using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Application2016.Helpers;

namespace Application2016.Areas.Admin.Models
{
    public class ProductModel : BaseModel
    {
        public int Id { get; set; }
        public int UserId { get; set; } // ID người đăng
        public bool IsAgency { get; set; } // Người đăng là môi giới?

        public short Transaction_Type { get; set; } // Kiểu trao đổi: mua, bán, cho thuê

        [Required(ErrorMessage = "Xin vui lòng chọn kiểu nhà")]
        public short Product_Type { get; set; } // Kiểu nhà: trung cư, thổ cư, trong ngõ, mặt phố...

        [Required(ErrorMessage = "Xin vui lòng chọn tỉnh, thành phố")]
        public int CityId { get; set; } // ID tỉnh, thành phố

        [Required(ErrorMessage = "Xin vui lòng chọn quận, huyện")]
        public int DistrictId { get; set; } // ID quận huyện

        [Required(ErrorMessage = "Xin vui lòng chọn xã, phường")]
        public int WardId { get; set; } // ID xã, phường.

        [Required(ErrorMessage = "Xin vui lòng nhập tiêu đề")]
        public string Text { get; set; } // Tiêu đề

        public string Text_Alias { get; set; }

        public string HouseNumber { get; set; } // Số nhà

        [Required(ErrorMessage = "Xin vui lòng nhập địa chỉ nhà")]
        [StringLength(800, ErrorMessage = "Giới hạn từ 10 - 800 ký tự", MinimumLength = 10)]
        [AllowHtml]
        public string Address { get; set; } // Địa chỉ chi tiết

        [Required(ErrorMessage = "Xin vui lòng nhập diện tích nhà")]
        [Range(1, int.MaxValue, ErrorMessage = "Xin vui lòng nhập diện tích nhà")]
        [RegularExpression("^[0-9]{1,9}([,.][0-9]{1,2})?$", ErrorMessage = "Diện tích nhà không hợp lệ")]
        public decimal Area { get; set; } // Diện tích ngôi nhà (met vuông)

        [AllowHtml]
        [StringLength(800, ErrorMessage = "Tối đa - 800 ký tự")]
        public string Description { get; set; } // Mô tả ngôi nhà

        [Required(ErrorMessage = "Xin vui lòng nhập giá bán")]
        [Range(1, long.MaxValue, ErrorMessage = "Xin vui lòng nhập giá bán")]
        public long StandardCost { get; set; } // Giá chuẩn (giá khi đăng, giá gốc)

        public System.Nullable<System.DateTime> SellStartDate { get; set; } // Bán từ ngày (mặc định lấy thời gian hiện tại)
        public System.Nullable<System.DateTime> SellEndDate { get; set; } // Bán đến ngày

        public System.Nullable<short> State { get; set; } // Trạng thái (đã bán, chưa bán)
        [RegularExpression("^[0-9]{1,2}([,.][0-9]{1,2})?$", ErrorMessage = "Chi phí cho môi giới không hợp lệ")]
        public System.Nullable<decimal> AgencyCost { get; set; } // Chi phí cho môi giới
        public short CostUnit { get; set; } // Đơn vị giá (%, đồng, dola) khi tính phí môi giới
        public string Tag { get; set; } // Thẻ để seo
        public System.Nullable<bool> Approval_Flag { get; set; } // Cờ thông báo đã phê duyệt
        public System.Nullable<int> ApprovalBy { get; set; } // Người phê duyệt
        public System.Nullable<System.DateTime> ApprovalDate { get; set; } // Ngày phê duyệt
        public System.Nullable<bool> Delete_Flag { get; set; } // Cờ Delete
        public System.Nullable<int> DeleteBy { get; set; } // Người delete
        public System.Nullable<System.DateTime> DeleteDate { get; set; } // Ngày delete
        public string Avatar { get; set; } // Avatar
        public string Avatar_Url { get; set; } // Url avatar.

        // More
        public int UpdateBy { get; set; }
        public string UserName { get; set; } // Tên người đăng
        public int regionFixedId { get; set; }
        public string regionFixedText { get; set; }
        public string StrWards1 { get; set; } // Danh sách xã, phường theo vùng với vùng cố định theo quyền môi giới.
        public string StrWards2 { get; set; } // Danh sách xã, phường theo vùng cố định khi chọn địa chỉ.
        public List<Entities.Item> ListProductType { get; set; } // Danh sách kiểu nhà
        public List<Entities.Item> ListCity { get; set; } // danh sách city
        public List<Entities.Item> ListDistrict { get; set; } // Danh sách quận huyện
        public List<Entities.Item> ListWard { get; set; } // Danh sách xã phường
        public int[] ListRegionId { get; set; } // Danh sách vùng sẽ chọn
        public List<Entities.Item> ListRegion { get; set; } // Danh sách vùng
        public List<Entities.Item> ListCostUnit { get; set; } // Danh sách đơn vị tính tiền
        public string StrTutrialCostUnit { get; set; } // Hướng dẫn về đơn vị tính tiền
        public string[] ListImage { get; set; } // Danh sách ảnh sẽ chọn
        public ImageConfig[] LstImageConfig { get; set; } // Danh sách ảnh.
        public List<Entities.Item> ListFixedSendRegion { get; set; } // Danh sách vùng gửi cố định được setting trong list user.

        public int RoleId { get; set; }
        public string RoleText { get; set; }

        public void MapFrom(Models.ProductModel m, ref Entities.Product e)
        {
            e.Id = m.Id;
            e.UserId = m.UserId;
            e.Transaction_Type = m.Transaction_Type;
            e.Product_Type = m.Product_Type;
            e.CityId = m.CityId;
            e.DistrictId = m.DistrictId;
            e.WardId = m.WardId;
            e.Text = m.Text;
            e.Text_Alias = m.Text.ToAlias();
            e.HouseNumber = m.HouseNumber;
            e.Address = m.Address;
            e.Area = m.Area;
            e.Description = m.Description;
            e.StandardCost = m.StandardCost;
            e.SellStartDate = m.SellEndDate;
            e.SellEndDate = m.SellEndDate;
            e.State = m.State;
            e.AgencyCost = m.AgencyCost;
            e.CostUnit = m.CostUnit;
            e.Tag = m.Tag;
            e.Approval_Flag = m.Approval_Flag;
            e.ApprovalBy = m.ApprovalBy;
            e.ApprovalDate = m.ApprovalDate;
            e.Delete_Flag = m.Delete_Flag;
            e.DeleteBy = m.DeleteBy;
            e.DeleteDate = m.DeleteDate;
            e.Avatar = m.Avatar;
            e.Avatar_Url = m.Avatar_Url;
            e.UpdateCost = m.StandardCost;
            e.UpdateTime = DateTime.Now;
            e.UpdateBy = m.UpdateBy;
            e.UserName = m.UserName;
            e.RoleId = m.RoleId;
            e.RoleText = m.RoleText;
        }

        public void MapFrom(Entities.Product e, ref Models.ProductModel m)
        {
            m.Id = e.Id;
            m.UserId = e.UserId;
            m.Transaction_Type = e.Transaction_Type;
            m.Product_Type = e.Product_Type;
            m.CityId = e.CityId;
            m.DistrictId = e.DistrictId;
            m.WardId = e.WardId;
            m.Text = e.Text;
            m.Text_Alias = e.Text_Alias;
            m.HouseNumber = e.HouseNumber;
            m.Address = e.Address;
            m.Area = e.Area;
            m.Description = e.Description;
            m.StandardCost = StringHelperExtension.MoneyExchange(e.StandardCost,'/');
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
            m.UpdateBy = e.UpdateBy.GetValueOrDefault();
            m.UserName = e.UserName;
            m.RoleId = e.RoleId.GetValueOrDefault();
            m.RoleText = e.RoleText;
        }
    }

    public class ImageConfig
    {
        public string caption { get; set; }
        public System.Nullable<int> size { get; set; }
        public string url { get; set; }
        public string key { get; set; }
    }
}