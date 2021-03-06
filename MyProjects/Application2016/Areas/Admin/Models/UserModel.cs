using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace Application2016.Areas.Admin.Models
{
    public class UserModel : BaseModel
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
        public bool Detele { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateUpdate { get; set; }
        public DateTime DateLogin { get; set; }
        public System.Nullable<int> PlaceId { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
        public DateTime DateUpgrade { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }
        public string CompanyLogo { get; set; }
        public string CodeActive { get; set; }
        public string CompanyName { get; set; }
        public string Avatar { get; set; }

        //extra
        public int[] NewRoles { get; set; }
        public ICollection<Entities.Item> UserRoles { get; set; }
        public ICollection<Entities.Item> ListRole { get; set; }
        public string RoleName { get; set; }

        // Phí áp dụng
        public int CostId { get; set; }

        // Danh sách địa lý
        public System.Nullable<int> CityId { get; set; }
        public System.Nullable<int> DistrictId { get; set; }
        public System.Nullable<int> WardId { get; set; }
        public Models.UserWardModel ward { get; set; }

        // Thông tin vùng hoạt động
        // Danh sách vùng gửi
        public ListUserInRegionModel ListUserInRegionSend { get; set; }
        // Danh sách vùng nhận
        public ListUserInRegionModel ListUserInRegionReceive { get; set; }

        public void MapFrom(UserModel m, ref Entities.User e)
        {
            e.Id = m.Id;
            e.DisplayName = m.DisplayName;
            e.FirstName = m.FirstName;
            e.LastName = m.LastName;
            e.Gender = m.Gender;
            e.DateOfBirth = m.DateOfBirth;
            e.UserName = m.UserName;
            e.Email = m.Email;
            e.Password = m.Password;
            e.Status = m.Status;
            e.Description = m.Description;
            e.DateCreate = m.DateCreate;
            e.DateUpdate = m.DateUpdate;
            e.DateLogin = m.DateLogin;
            e.PlaceId = m.PlaceId;
            e.Address = m.Address;
            e.Zipcode = m.Zipcode;
            e.CompanyLogo = m.CompanyLogo;
            e.CompanyName = m.CompanyName;
            e.CodeActive = m.CodeActive;
            e.Phone = m.Phone;
            e.Avatar = m.Avatar;
            e.UserRoles = m.UserRoles;
        }

        public void MapFrom(Entities.User e, ref UserModel m)
        {
            m.Id = e.Id;
            m.DisplayName = e.DisplayName;
            m.FirstName = e.FirstName;
            m.LastName = e.LastName;
            m.Gender = e.Gender;

            DateTime temp = Convert.ToDateTime(e.DateOfBirth);
            m.DateOfBirth = temp.ToString("dd/MM/yyyy");
            m.UserName = e.UserName;
            m.Email = e.Email;
            m.Password = e.Password;
            m.Status = e.Status;
            m.Description = e.Description;
            m.DateCreate = e.DateCreate;
            m.DateUpdate = e.DateUpdate;
            m.DateLogin = e.DateLogin;
            m.PlaceId = e.PlaceId;
            m.Address = e.Address;
            m.Zipcode = e.Zipcode;
            m.CompanyLogo = e.CompanyLogo;
            m.CompanyName = e.CompanyName;
            m.CodeActive = e.CodeActive;
            m.Phone = e.Phone;
            m.Avatar = e.Avatar;
            m.UserRoles = e.UserRoles;
            m.ward = new UserWardModel();
        }
    }

    public class ListUserInRegionModel
    {
        public int RegionType { get; set; }
        public int regionNum { get; set; }

        public int[] region_city { get; set; }

        public int[] region_district { get; set; }
        [RequiredArray(ErrorMessage = "Yêu cầu chọn xã/phường")]
        [Required(ErrorMessage="Yêu cầu chọn xã/phường")]
        public int[] region_ward { get; set; }
        public int[] regionIds { get; set; }

        public List<Entities.UserInRegion> ListUserInRegion { get; set; }
        public ICollection<Entities.Item> ListItem { get; set; }
    }

    public class RequiredArray : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var list = value as IList;
            if (list != null)
            {
                return list.Count > 0;
            }
            return false;
        }
    }

    public class ListUserModel
    {
        public int Id { get; set; }
        public List<UserModel> lstUser { get; set; }

        public ICollection<Entities.Item> ListAllRegion { get; set; }

        public ICollection<Entities.Item> ListRole { get; set; }

        public ICollection<Entities.Item> ListCost { get; set; }

        public UserCondition Condition { get; set; }
    }

    public class UserWardModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Yêu cầu chọn tỉnh, thành phố")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "Yêu cầu chọn quận, huyện")]
        public int Parent { get; set; }

        public IEnumerable<Entities.Item> ListCity { get; set; }
        public IEnumerable<Entities.Item> ListDistrict { get; set; }
        public IEnumerable<Entities.Item> ListWard { get; set; }
    }

    public class UserCondition: BaseModel
    {
        public int id { get; set; }
        public int RegionId { get; set; }
        public int RoleId { get; set; }
    }

    public class UserViewModel
    {
        public UserModel User { get; set; }
        public UserCondition Condition { get; set; }
        public List<Entities.Item> ListUser { get; set; }
    }
}