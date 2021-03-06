using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Application2016.Enums;
using Application2016.Areas.Admin.Models;
using Application2016.Helpers;

namespace Application2016.Areas.Admin.Controllers
{
    public class RoleController : BaseController
    {
        UserInRolesService userRole = new UserInRolesService();
        RoleService role = new RoleService();

        public RoleController()
        {
            ViewBag.ActionMenu = "AdminSetting";
            ViewBag.ActionSubMenu = "Role";
        }

        Roles[] _roles
        {
            get
            {
                return new Roles[]{
                    Roles.Administrator,
                    Roles.Super_Administrator
                };
            }
        }

        [HttpGet]
        public ActionResult Index(BaseModel condition)
        {
            int errorId = CheckRole(_roles);
            if (errorId < 0)
            {
                return Redirect(errorId);
            }

            ListRoleModel model = new ListRoleModel();
            int totalRecord = 0;
            model.ListRole = role.List(condition.SearchText, condition.Page, condition.PageSize, condition.State, out totalRecord);

            condition.TotalRecord = totalRecord;
            model.Condition = condition;
            Paging(condition.Page, totalRecord);
            return View(model);
        }

        [HttpGet]
        public ActionResult RoleUpdate(int id)
        {
            RoleModel model = new RoleModel();
            RegionService regionService = new RegionService();
            var tempRegion = regionService.ListItemActive();

            if (id > 0)
            {
                model.Role = role.GetById(id);

                // Lấy những vùng đã được chọn.
                string strRegion = model.Role.BuffetRegions;
                if (!string.IsNullOrEmpty(strRegion))
                {
                    List<string> arrRegion = strRegion.Split(',').ToList();
                    tempRegion.Where(x => arrRegion.Contains(x.Id.ToString())).ToList().ForEach(y => y.Checked = true);
                }
            }
            else
            {
                model.Role = new Entities.Role();
            }
            model.ListRegion = tempRegion;
            model.ListRole = role.ListActive();
            return View(model);
        }

        [HttpPost]
        public ActionResult RoleUpdate(RoleModel model)
        {
            //if (model.Role.OnlySendFixedRegion)
            //{
            //    var temp = model.ListRegion.Where(x => x.Checked).Select(y => y.Id.ToString()).ToList();
            //    var strRegion = String.Join(",", temp);
            //    model.Role.BuffetRegions = strRegion;
            //}

            int result = role.Save(model.Role);
            if (result > 0)
            {
                TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_SUCCESS;
                TempData[AdminConfigs.TEMP_REDIRECT] = @Url.Action("Index", "Role");
                model.Role = role.GetById(result);
            }
            else
            {
                TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_ERROR;
            }
            
            model.ListRole = role.ListActive();
            RegionService regionService = new RegionService();
            var tempRegion = regionService.ListItemActive();
            model.ListRegion = tempRegion;

            return View(model);
        }
    }
}
