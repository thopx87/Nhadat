using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application2016.Areas.Admin.Controllers
{
    public sealed class ErrorsController : Controller
    {
        public ActionResult NotFound()
        {
            ActionResult result;

            object model = Request.Url.PathAndQuery;

            if (!Request.IsAjaxRequest())
                result = View("404", model);
            else
                result = View("404", model);

            return result;
        }

        public ActionResult ServerError()
        {
            ActionResult result;

            object model = Request.Url.PathAndQuery;

            if (!Request.IsAjaxRequest())
                result = View("500", model);
            else
                result = View("500", model);

            return result;
        }
    }
}
