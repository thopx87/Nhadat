using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application2016.Areas.Admin.Models
{
    public class TestModel
    {
        public int id { get; set; }
        public List<Elem> Elems { get; set; }
    }
    public class Elem
    {
        public int id { get; set; }
        public string value { get; set; }
        public bool Checked { get; set; }
    }
}