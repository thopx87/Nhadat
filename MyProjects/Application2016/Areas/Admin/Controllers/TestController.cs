using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application2016.Areas.Admin.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Admin/Test/

        public ActionResult Index()
        {
            Models.TestModel m = new Models.TestModel();
            Models.Elem e = new Models.Elem();
            List<Models.Elem> lst = new List<Models.Elem>();
            e.id = 1;
            e.value = "one";
            lst.Add(e);

            e = new Models.Elem();
            e.id = 2;
            e.value = "two";
            lst.Add(e);

            m.id = 1;
            m.Elems = lst;
            return View(m);
        }

        [HttpPost]
        public ActionResult Update(int id, Models.TestModel m)
        {

            return View();
        }

        public ActionResult CheckBoxList()
        {
            Models.TestModel m = new Models.TestModel();
            Models.Elem e = new Models.Elem();
            List<Models.Elem> lst = new List<Models.Elem>();
            e.id = 1;
            e.value = "one";
            lst.Add(e);

            e = new Models.Elem();
            e.id = 2;
            e.value = "two";
            lst.Add(e);

            m.id = 1;
            m.Elems = lst;
            return View(m);
        }

        [HttpPost]
        public ActionResult CheckBoxList(Models.TestModel m)
        {

            return View(m);
        }

        public ActionResult Avatar()
        {
            return View();
        }

        public ActionResult Gallery()
        {
            return View();
        }
    }
}
