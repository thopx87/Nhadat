using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Application2016.Areas.Admin.Models
{
    public class LoginModel : BaseModel
    {
        [Required(ErrorMessage="Xin hãy nhập tên")]
        [Display(Name= "User name")]
        public string Username { get; set; }

        [Required(ErrorMessage="Xin hãy nhập mật khẩu")]
        [Display(Name="Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name="Remember?")]
        public bool Remember { get; set; }
    }

    public class RegisterModel : BaseModel
    {
        /// <summary>
        /// Kiểu đăng ký. Mặc định là 0 (Đăng ký thường)
        /// 1 --> Đăng ký môi giới.
        /// </summary>
        public int Type { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Xin hãy nhập tên đăng nhập!")]
        // Tên đăng nhập dài từ 6 đến 20 ký tự, bắt đầu bằng chữ cái hoặc số, không được có dấu cách.
        [RegularExpression("^(?=.{6,20}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$", ErrorMessage = "Tên đăng nhập dài từ 6 đến 20 ký tự, bắt đầu bằng chữ cái hoặc số, không được có dấu cách hoặc ký tự đặc biệt")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Xin hãy nhập Email!")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự, trong đó có ít nhất 01 số.")]
        [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{6,}$", ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự, trong đó có ít nhất 01 số.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Không khớp với mật khẩu.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage="Xin hãy nhập số điện thoại")]
        [RegularExpression("([0-9]{10,11})|(\\([0-9]{3}\\)\\s+[0-9]{3}\\-[0-9]{4})", ErrorMessage = "Số điện thoại không đúng")]
        public string PhoneNumber { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string DateOfBirth { get; set; }

        public short Gender { get; set; }

        public System.Nullable<int> Regis_CityId { get; set; }

        public System.Nullable<int> Regis_DistrictId { get; set; }

        public System.Nullable<int> Regis_WardId { get; set; }

        public bool IsAgency { get; set; }

        public bool IsAcept { get; set; }

        public List<Entities.Item> ListCity { get; set; }
        public List<Entities.Item> ListDistrict { get; set; }
        public List<Entities.Item> ListWard { get; set; }

        public RegisterModel()
        {
            ListCity = new List<Entities.Item>();
            ListDistrict = new List<Entities.Item>();
            ListWard = new List<Entities.Item>();
        }
    }

    public class RegistryAgencyModel1 : RegisterModel
    {
        // Thông tin vùng hoạt động
        // Danh sách vùng gửi
        public ListUserInRegionModel ListUserInRegionSend { get; set; }
        // Danh sách vùng nhận
        public ListUserInRegionModel ListUserInRegionReceive { get; set; }

        
    }

    public class RegistryAgencyModel2 : BaseModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string DateOfBirth { get; set; }

        public string Gender { get; set; }

        public System.Nullable<int> Regis_CityId { get; set; }

        public System.Nullable<int> Regis_DistrictId { get; set; }

        public System.Nullable<int> Regis_WardId { get; set; }

        public bool IsAgency { get; set; }

        public bool IsAcept { get; set; }

        public List<Entities.Item> ListCity { get; set; }
        public List<Entities.Item> ListDistrict { get; set; }
        public List<Entities.Item> ListWard { get; set; }

        // Thông tin vùng hoạt động
        // Danh sách vùng gửi
        public ListUserInRegionModel ListUserInRegionSend { get; set; }
        // Danh sách vùng nhận
        public ListUserInRegionModel ListUserInRegionReceive { get; set; }

        public void MapFrom(Entities.User e, ref RegistryAgencyModel2 m)
        {
            m.Id = e.Id;
            m.FirstName = e.FirstName;
            m.LastName = e.LastName;
            m.Gender = e.Gender.ToString();
            m.DateOfBirth = e.DateOfBirth.ToString();
            m.UserName = e.UserName;
            m.Email = e.Email;
            m.PhoneNumber = e.Phone;
            m.Regis_WardId = e.PlaceId;
        }
    }

    public class ChangePasswordModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Display(Name = "Mật khẩu cũ")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("NewPassword", ErrorMessage = "Không khớp với mật khẩu.")]
        public string RePassword { get; set; }
    }

    public class AccountInfo
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Avatar { get; set; }

        public int RoleId { get; set; }
    }
}