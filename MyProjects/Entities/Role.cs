using System;
using System.Collections.Generic;

namespace Entities
{
    public class Role
    {        
        // Id quyền
        public int Id { get; set; }

        // Tên quyền
        public string Text { get; set; }

        // Mã quyền (không trùng với các mã khác
        public string Code { get; set; }

        // Trạng thái hoạt động
        public bool Active { get; set; }

        // True --> là môi giới, false --> là loại khác.
        public bool IsAgency { get; set; }

        // True --> được phép post bài. False --> không được phép post.
        public bool Post { get; set; }

        // True --> Được nhận bài từ môi giới khác. False --> không được nhận.
        public bool ResiveFromAgency { get; set; }

        // True --> Được nhận bài từ thành viên
        public bool ResiveFromMember { get; set; }

        // Số vùng được gửi
        public short SendRegionNum { get; set; }

        // Số vùng được nhận.
        public short ResiveRegionNum { get; set; }

        // Có thể xem được số nhà.
        // true --> cho phép xem; false --> không cho phép.
        public bool CanViewHouseNum { get; set; }

        // Có thể check được tin chính chủ.
        // True --> nhận biết được; false --> không nhận biết được.
        public bool CanCheckOwnerArticle { get; set; }

        // Chỉ gửi được vùng cố định
        public bool OnlySendFixedRegion { get; set; }

        // Danh sách vùng gửi tự chọn.
        public string BuffetRegions { get; set; }

        public bool IsAdmin { get; set; }
    }
}
