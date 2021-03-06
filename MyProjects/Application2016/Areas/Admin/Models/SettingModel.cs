using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Application2016.Areas.Admin.Models
{
    public class SettingModel : BaseModel
    {
        public RoleSettingModel RoleSetting { get; set; }
        public MailModel MailSetting { get; set; }

        public SettingCommonModel webSetting { get; set; }

        public SettingCommonModel socialNetworkSetting { get; set; }

    }

    public class RoleSettingModel
    {
        public int Id { get; set; } // Role ID
        public bool Post { get; set; }
        public bool ResiveFromAgency { get; set; }
        public bool ResiveFromMember { get; set; }
        public short SendRegionNum { get; set; }
        public short ResiveRegionNum { get; set; }
        public decimal Cost { get; set; }

        public List<Entities.Item> ListRole { get; set; }
    }

    public class MailModel
    {
        public int Port { get; set; }
        public string Email { get; set; }
        public string Host { get; set; }
    }

    public class SettingCommonModel
    {
        public int Id { get; set; }

        public string SettingName { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public string Display { get; set; }
        public Nullable<bool> DisplayState { get; set; }

        public void MapFrom(Entities.Setting e, ref SettingCommonModel m)
        {
            m.Id = e.Id;
            m.SettingName = e.SettingName;
            m.Title = e.Title;
            m.Value = e.Value;
            m.Display = e.Display;
            m.DisplayState = e.DisplayState;
        }
        public List<SettingCommonModel> list { get; set; }
    }
}
