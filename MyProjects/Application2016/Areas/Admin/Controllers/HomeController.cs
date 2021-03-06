using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;

namespace Application2016.Areas.Admin.Controllers
{
    [Authorize(Roles = "SA")]
    public class HomeController : BaseController
    {
        UserInRolesService userRole = new UserInRolesService();
        RoleService role = new RoleService();
        
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }
    }
}
