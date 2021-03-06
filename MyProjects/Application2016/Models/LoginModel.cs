using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Application2016.Models
{
    public class LoginModel:BaseModel
    {
        [Required(ErrorMessage = "Xin hãy nhập tên / Email")]
        [Display(Name = "User name")]
        public string Username_Model { get; set; }

        [Required(ErrorMessage = "Xin hãy nhập mật khẩu")]
        [Display(Name = "Password")]
        public string Password_Model { get; set; }

        [Required]
        [Display(Name = "Remember?")]
        public bool Remember_Model { get; set; }
    }
}