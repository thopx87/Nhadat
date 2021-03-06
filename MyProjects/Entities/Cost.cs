using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class CostSetting
    {
        // Id setting
        public int Id { get; set; }

        // Tên cài đặt phí.
        public string SettingName { get; set; }

        // Kiểu chế độ (1: chung, 2: riêng)
        public short Mode { get; set; }

        // Id Tỉnh/ thành phố
        public int CityId { get; set; }

        // Id Huyện/ quận
        public int DistrictId { get; set; }

        // Id Vùng
        public int RegionId { get; set; }

        // Id Phường/ xã
        public int WardId { get; set; }

        // Kiểu giao dịch
        public short TransactionType { get; set; }

        // Kiểu sản phẩm (nhà)
        public short ProductType { get; set; }

        // Tên dự án
        public string ProjectCode { get; set; }

        // Nhóm đối tượng (người dùng, môi giới)
        public int UserRole { get; set; }

        // Danh sách người dùng
        public string UserIds { get; set; }

        // Thời gian bắt đầu áp dụng
        public Nullable<DateTime> TimeStart { get; set; }

        // Thời gian kết thúc áp dụng
        public Nullable<DateTime> TimeEnd { get; set; }

        // Trạng thái áp dụng
        // 1 -- đang áp dụng
        // 0 -- không áp dụng
        public bool IsApply { get; set; }

        // Thời gian cập nhật
        public DateTime UpdateTime { get; set; }

        // Người cập nhật
        public int UpdateBy { get; set; }

        // Extendsion
        public string CityName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
        public string RegionName { get; set; }
        public string TransactionName { get; set; }
        public string ProductTypeName { get; set; }
        public string GroupName { get; set; }
        public List<string> ListUserName { get; set; }
    }

    public class Cost
    {
        public int CostSettingId { get; set; }
        public int CostMasterId { get; set; }
        public string CostMasterText { get; set; }
        public short SortOrder { get; set; }
        public bool IsApply { get; set; }
        public Nullable<int> MoneyCode1 { get; set; }
        public Nullable<int> MoneyCode2 { get; set; }
        public Nullable<int> MoneyCode3 { get; set; }
    }

    public class CostMaster
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Code { get; set; }
    }
}
