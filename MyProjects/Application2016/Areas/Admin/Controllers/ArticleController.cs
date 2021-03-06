using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application2016.Areas.Admin.Models;
using BusinessLayer;
using Application2016.Helpers;

namespace Application2016.Areas.Admin.Controllers
{
    public class ArticleController : BaseController
    {
        public ArticleController()
        {
            ViewBag.ActionMenu = "AdminSetting";
            ViewBag.ActionSubMenu = "Article";
        }

        ArticleService _service = new ArticleService();
        //
        // GET: /Admin/Article/
        [HttpGet]
        public ActionResult Index(ArticleCondition Condition)
        {
            ListArticleModel model = new ListArticleModel();
            CategoryService service = new CategoryService();

            model.Condition = Condition;
            model.ListCategory = service.ListItem();
            model.ListCategory.Insert(0, new Entities.Item() { Text = "Chọn thư mục", Id = 0 });
            int total = 0;
            try
            {
                model.ListArticle = _service.List(Condition.CatId, Condition.SearchText, Condition.DisplayOnly, Condition.Page, Condition.PageSize, out total);
            }
            catch (Exception ex)
            {
                Logs.LogWrite(ex.ToString());
            }
            Paging(Condition.Page, total);

            return View(model);
        }

        [HttpGet]
        public ActionResult Update(int Id)
        {
            ArticleModel model = new ArticleModel();
            if (Id <= 0)
            {
                model.Article = new Entities.Article();
                model.Article.Display = true;
            }
            else
            {
                model.Article = _service.GetById(Id);
            }
            model.ListCategory = _service.ListCategory();

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(ArticleModel model)
        {
            if (ModelState.IsValid)
            {
                model.Article.Alias = string.IsNullOrEmpty(model.Article.Alias)? model.Article.Title.ToAlias(): model.Article.Alias.ToAlias();
                int result = _service.Save(model.Article);
                if (result > 0)
                {
                    TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_SUCCESS;
                    TempData[AdminConfigs.TEMP_REDIRECT] = @Url.Action("Index", "Article");
                }
                else
                {
                    TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_ERROR;
                }
            }
            model.ListCategory = _service.ListCategory();
            return View(model);
        }
    }
}
