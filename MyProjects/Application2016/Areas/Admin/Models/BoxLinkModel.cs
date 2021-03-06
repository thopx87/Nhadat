using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application2016.Areas.Admin.Models
{
    public class ListBoxLinkModel
    {
        public BaseModel Condition { get; set; }
        public List<Entities.BoxLink> ListBoxLink { get; set; }
    }

    public class BoxLinkModel
    {
        public Entities.BoxLink BoxLink { get; set; }
        public List<Entities.Item> ListPosition { get; set; }
        public List<Entities.Item> ListColor { get; set; }
        public List<Entities.Item> ListTable { get; set; }
        public List<Entities.Item> ListCondition { get; set; }
    }
}