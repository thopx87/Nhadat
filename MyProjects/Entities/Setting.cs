using System;

namespace Entities
{
    public class Setting
    {
        public int Id { get; set; }
        public string SettingName { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public string Display { get; set; }
        public System.Nullable<bool> DisplayState { get; set; }
    }
    
}
