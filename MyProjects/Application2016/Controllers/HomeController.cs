using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application2016.Models;
using BusinessLayer;

namespace Application2016.Controllers
{
    public class HomeController : BaseController
    {
        ProductService productService = new ProductService();

        public ActionResult Index(ProductConditions condition)
        {
            return RedirectToAction("Index", "Template1");
            // Check Cost

            // Product type
            condition.ListProductType = productService.ListProductType();
            
            // List Limited
            List<SelectListItem> listLimit = new List<SelectListItem>();

            listLimit.Add(new SelectListItem() { Text = "20", Value = "20" });
            listLimit.Add(new SelectListItem() { Text = "30", Value = "30", Selected = true });
            listLimit.Add(new SelectListItem() { Text = "40", Value = "40" });
            listLimit.Add(new SelectListItem() { Text = "50", Value = "50" });
            condition.ListLimit = listLimit;

            return View(condition);
        }

    }
}
