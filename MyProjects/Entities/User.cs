using System;
using System.Collections.Generic;

namespace Entities
{
    public class User
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public System.Nullable<short> Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
        public bool Delete { get; set; }
        public System.DateTime DateCreate { get; set; }
        public System.DateTime DateUpdate { get; set; }
        public System.DateTime DateLogin { get; set; }
        public System.Nullable<int> PlaceId { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string CompanyLogo { get; set; }
        public string CodeActive { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public System.Nullable<bool> IsUpdated { get; set; }
    
        public virtual ICollection<FileUpload> FileUploads { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Entities.Item> UserRoles { get; set; }
    }

    public class UserItem
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
    }

    public class UserInRegion
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        /// <summary>
        /// Loại vùng: 1 --> vùng gửi, 2 --> vùng nhận.
        /// </summary>
        public int RegionType { get; set; }

        /// <summary>
        /// ID vùng.
        /// </summary>
        public int RegionId { get; set; }

        /// <summary>
        /// Trạng thái hoạt động. True --> được phép hoạt động; False --> không được phép.
        /// Dự kiến mỗi lần cập nhật sẽ xóa đi, thêm mới nên có thể không sử dụng.
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Id tỉnh, thành phố của vùng.
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// Id huyện, quận của vùng
        /// </summary>
        public int DistrictId { get; set; }

        /// <summary>
        /// Id xã, phường của vùng.
        /// </summary>
        public int WardId { get; set; }

        /// <summary>
        /// Tên vùng.
        /// </summary>
        public string RegionText { get; set; }

        public IEnumerable<Entities.Item> ListCity { get; set; }
        public IEnumerable<Entities.Item> ListDistrict { get; set; }
        public IEnumerable<Entities.Item> ListWard { get; set; }
        public IEnumerable<Entities.Item> ListRegion { get; set; }
    }

    public class UserInRole
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RolesId { get; set; }
        public System.Nullable<bool> State { get; set; }
    }
}
