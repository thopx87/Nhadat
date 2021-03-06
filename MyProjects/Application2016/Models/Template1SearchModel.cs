using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Application2016.Helpers;

namespace Application2016.Models
{
    public class Template1SearchModel
    {
        // Tab (menu)
        public int TabId { get; set; }

        // Tab name.
        public string TabName { get; set; }

        // Trang cần xem.
        public int Page { get; set; }

        // Từ khóa tìm kiếm.
        public string SearchText { get; set; }

        // Kiểu đăng
        public short TransactionType { get; set; }

        public string Username { get; set; }

        // UserId
        public int UserId { get; set; }

        // Kiểu nhà
        public short ProductType { get; set; }

        // Tỉnh
        public int CityId { get; set; }

        // Quận huyện
        public int DistrictId { get; set; }

        // Xã phường
        public int WardId { get; set; }

        // Vùng
        public int RegionId { get; set; }

        // Diện tích nhỏ nhất.
        public int MinArea { get; set; }

        // Diện tích lớn nhất.
        public int MaxArea { get; set; }

        // Giá thấp nhất
        public long MinCost { get; set; }

        // Giá cao nhất
        public long MaxCost { get; set; }

        // Kích thước bảng.
        public int PageSize { get; set; }

        // Tổng số bản ghi.
        public int TotalRecord { get; set; }

        // Trạng thái hoạt động.
        // 0 --> Tất cả
        // 1 --> Cái được hoạt động
        // 2 --> Cái không được hoạt động.
        public int State { get; set; }

        public List<Entities.Item> ListState { get; set; }

        // Danh sách người dùng
        public List<Entities.Item> ListUser { get; set; }

        public Template1SearchModel()
        {
            Page = 1;
            SearchText = "";
            PageSize = AdminConfigs.PAGE_SIZE;
            TransactionType = 0;
            TotalRecord = 0;
            MinCost = 0;
            MaxCost = 20;
            MinArea = 0;
            MaxArea = 200;
            ListState = Helpers.DefaultData.ListState();
            ListUser = new List<Entities.Item>();
        }

        public void Mapping(Template1SearchModel m, ref Entities.ProductCondtions e)
        {
            e.TransactionType = m.TransactionType;
            e.UserId = m.UserId;
            e.City = m.CityId;
            e.District = m.DistrictId;
            e.Ward = m.WardId;
            e.Text = m.SearchText !=null? m.SearchText: "";
            e.Page = m.Page;
            e.Limit = m.PageSize;
            e.MinCost = m.MinCost*1000000000; // Nhân với 1 tỷ đồng
            e.MaxCost = m.MaxCost * 1000000000; // Nhân với 1 tỷ đồng
            e.MinArea = m.MinArea;
            e.MaxArea = m.MaxArea;
            e.ProductType = m.ProductType;
        }
    }
}