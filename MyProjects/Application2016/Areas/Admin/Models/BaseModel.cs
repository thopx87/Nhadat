using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Application2016.Helpers;

namespace Application2016.Areas.Admin.Models
{
    public class BaseModel
    {
        // Trang cần xem.
        public int Page { get; set; }

        // Từ khóa tìm kiếm.
        public string SearchText { get; set; }

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

        public int Result { get; set; }

        public string Message { get; set; }

        public BaseModel()
        {
            Page = 1;
            SearchText = "";
            PageSize = AdminConfigs.PAGE_SIZE;
            TotalRecord = 0;
            ListState = Helpers.DefaultData.ListState();
        }
    }
}