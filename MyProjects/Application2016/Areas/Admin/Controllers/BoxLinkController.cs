using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application2016.Areas.Admin.Models;
using BusinessLayer;

namespace Application2016.Areas.Admin.Controllers
{
    public class BoxLinkController : BaseController
    {
        BoxLinkService _service = new BoxLinkService();

        public BoxLinkController()
        {
            ViewBag.ActionMenu = "BoxLink";
            ViewBag.ActionSubMenu = "BoxLink";
        }

        public ActionResult Index()
        {
            ListBoxLinkModel model = new ListBoxLinkModel();
            model.ListBoxLink = _service.List();

            return View(model);
        }

        [HttpGet]
        public ActionResult Update(int Id)
        {
            BoxLinkModel model = new BoxLinkModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Update(BoxLinkModel model)
        {
            return View(model);
        }
    }
}
