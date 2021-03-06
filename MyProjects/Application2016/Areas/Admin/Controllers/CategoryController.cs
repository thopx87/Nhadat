using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application2016.Areas.Admin.Models;
using BusinessLayer;
using Application2016.Enums;
using Application2016.Helpers;

namespace Application2016.Areas.Admin.Controllers
{
    public class CategoryController : BaseController
    {
        public CategoryController()
        {
            ViewBag.ActionMenu = "AdminSetting";
            ViewBag.ActionSubMenu = "Category";
        }
        CategoryService _service = new CategoryService();
        //
        // GET: /Admin/Category/
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
        public ActionResult Index(CategoryCondition Condition)
        {
            int errorId = CheckRole(_roles);
            if (errorId < 0)
            {
                return Redirect(errorId);
            }
            ListCategoryModel model = new ListCategoryModel();

            model.ListCategory =  _service.List(Condition.SearchText);
            if (model.ListCategory.Count == 0)
            {
                model.ListCategory = new List<Entities.Category>();
            }
            model.Condition = Condition;

            return View(model);
        }

        [HttpGet]
        public ActionResult Update(int Id)
        {
            CategoryModel model = new CategoryModel();
            
            if (Id <= 0)
            {
                model.Category = new Entities.Category();
                model.Category.Display = true;
            }
            else
            {
                model.Category = _service.GetById(Id);
            }
            model.ListCategory = _service.ListItem();
            model.ListCategory.Insert(0, new Entities.Item() { Text = "", Id = 0 });
            model.ListCategoryType = DefaultData.ListCategoryType();

            return View(model);
        }

        [HttpPost]
        public ActionResult Update(CategoryModel model)
        {
            var listCategoryType = DefaultData.ListCategoryType();
            if (ModelState.IsValid)
            {


                model.Category.TypeText = listCategoryType.Where(x => x.Id == model.Category.Type).Select(x => x.Text).First();
                int result = _service.Save(model.Category);
                if (result > 0)
                {
                    TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_SUCCESS;
                    TempData[AdminConfigs.TEMP_REDIRECT] = @Url.Action("Index", "Category");
                }
                else
                {
                    TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_ERROR;
                }
            }

            model.ListCategory = _service.ListItem();
            model.ListCategoryType = listCategoryType;
            return View(model);
        }
    }
}
