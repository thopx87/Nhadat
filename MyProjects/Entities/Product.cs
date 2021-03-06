using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class Product
    {
        public int Id { get; set; }
        public int UserId { get; set; } // ID người đăng
        public short Transaction_Type { get; set; } // Kiểu trao đổi: mua, bán, cho thuê
        public short Product_Type { get; set; } // Kiểu nhà: trung cư, thổ cư, trong ngõ, mặt phố...
        public int CityId { get; set; } // ID tỉnh, thành phố
        public int DistrictId { get; set; } // ID quận huyện
        public int WardId { get; set; } // ID xã, phường.
        public string Text { get; set; } // Tiêu đề
        public string Text_Alias { get; set; } // Tiêu đề bi danh
        public string HouseNumber { get; set; } // Số nhà
        public string Address { get; set; } // Địa chỉ chi tiết
        public decimal Area { get; set; } // Diện tích ngôi nhà (met vuông)
        public string Description { get; set; } // Mô tả ngôi nhà
        public long StandardCost { get; set; } // Giá chuẩn (giá khi đăng, giá gốc)
        public System.Nullable<System.DateTime> SellStartDate { get; set; } // Bán từ ngày (mặc định lấy thời gian hiện tại)
        public System.Nullable<System.DateTime> SellEndDate { get; set; } // Bán đến ngày
        public System.Nullable<short> State { get; set; } // Trạng thái (đã bán, chưa bán)
        public System.Nullable<decimal> AgencyCost { get; set; } // Chi phí cho môi giới
        public short CostUnit { get; set; } // Đơn vị giá (%, đồng, dola) khi tính phí môi giới
        public string Tag { get; set; } // Thẻ để seo
        public System.Nullable<bool> Approval_Flag { get; set; } // Cờ thông báo đã phê duyệt
        public System.Nullable<int> ApprovalBy{ get; set; } // Người phê duyệt
        public System.Nullable<System.DateTime> ApprovalDate{ get; set; } // Ngày phê duyệt
        public System.Nullable<bool> Delete_Flag{ get; set; } // Cờ Delete
        public System.Nullable<int> DeleteBy{ get; set; } // Người delete
        public System.Nullable<System.DateTime> DeleteDate { get; set; } // Ngày delete
        public string Avatar { get; set; } // Ảnh đại diện.
        public string Avatar_Url { get; set; } // Url avatar
        public System.Nullable<long> UpdateCost { get; set; } // Giá chuẩn (giá khi đăng, giá gốc)
        public System.Nullable<System.DateTime> UpdateTime { get; set; } // Ngày đăng
        public System.Nullable<int> UpdateBy { get; set; } // Người đăng
        public System.Nullable<int> Total { get; set; }
        // Thông tin các bảng tham chiếu.
        public string PTypeText { get; set; }
        public string CityText { get; set; }
        public string DistrictText { get; set; }
        public string UserName { get; set; }
        public System.Nullable<int> RoleId { get; set; }
        public string RoleText { get; set; }

        // Trạng thái đang theo dõi
        public bool IsFollow { get; set; }

        // Danh sách vùng nhờ môi giới bán hộ.
        public string ListRegionSelling { get; set; }
    }

    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public System.Nullable<int> Size { get; set; }
        public short Order { get; set; }
        public System.Nullable<System.DateTime> DateUpload { get; set; }
        public System.Nullable<System.DateTime> DateModified { get; set; }
    }

    public class Product_Region
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int RegionId { get; set; }
        public bool Fixed { get; set; }
        public bool Status { get; set; }
    }

    public class ProductCondtions
    {
        // Lấy danh sách ngoài ID này.
        public int withOutId { get; set; }
        public short TransactionType { get; set; }
        public int UserId { get; set; }
        public short ProductType { get; set; }
        public int City { get; set; }
        public int District { get; set; }
        public int Ward { get; set; }
        public string Text { get; set; }
        public int Page { get; set; }
        public int OrderBy { get; set; }
        public int Limit { get; set; }
        public long MinCost { get; set; }
        public long MaxCost { get; set; }
        public int MinArea { get; set; }
        public int MaxArea { get; set; }
    }

    public class Product_ChangeCost
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public long Cost { get; set; }
        public short Times { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public string Email { get; set; }
    }

    public class Product_ChangeCost_History
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public short Times { get; set; }
        public long Cost { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public bool Delete_Flag { get; set; }
    }

    public class Product_Follow
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public bool IsFollow { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public class Product_WantSell
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Text { get; set; }
        public int CostSell { get; set; }
        public int BuyerId { get; set; }
        public int CostBuy { get; set; }
        public DateTime UpdateTime { get; set; }
        public short Times { get; set; }
        public System.Nullable<bool> IsChecked { get; set; }
        public System.Nullable<DateTime> CheckTime { get; set; }
    }

    public class ProductMessage
    {
        public int Id { get; set; }

        // From UserId
        public int From { get; set; }

        // To Id
        public int To { get; set; }

        // Id sản phẩm
        public int ProductId { get; set; }

        // Tên sản phẩm
        public string ProductText { get; set; }

        // Nội dung thông báo
        public string Content { get; set; }

        // Cờ báo đã đọc chưa. 0--> chưa đọc. 1--> đã đọc
        public bool Read_Flag { get; set; }

        // Cờ báo xóa 0--> chưa xóa. 1 --> đã xóa.
        public bool Delete_Flag { get; set; }

        // Ngày tạo
        public DateTime CreateDate { get; set; }

        // Ngày cập nhật (Đổi cờ read)
        public DateTime UpdateDate { get; set; }

        // More information
        public string UserName { get; set; }

        public string Avatar { get; set; }

        public string Phone { get; set; }
    }
}
