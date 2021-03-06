using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Application2016.Areas.Admin.Models
{
    public class ListRoleModel
    {
        public BaseModel Condition { get; set; }
        public List<Entities.Role> ListRole { get; set; }
    }
    public class RoleModel
    {
        public Entities.Role Role { get; set; }
        public List<Entities.Role> ListRole { get; set; }
        public List<Entities.Item> ListRegion { get; set; }
        public RoleModel()
        {
            ListRegion = new List<Entities.Item>();
        }
    }
}