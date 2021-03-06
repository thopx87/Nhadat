using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLayer.Helpers;

namespace BusinessLayer
{
    public class PageService: BaseService<Entities.Page>
    {
        private string className { get { return this.GetType().Name; } }
        public PageService() : base() { }

        public List<Entities.Item> ListPageMaster()
        {
            var result = (from p in Context.PageMasters
                          select new Entities.Item()
                          {
                              Id = p.Id,
                              Text = p.Text
                          }).ToList();
            return result;
        }

        public int SavePageMaster(Entities.PageMaster e)
        {
            try
            {
                DataLayer.PageMaster page = (from p in Context.PageMasters
                                               where p.Id == e.Id
                                               select p).FirstOrDefault();
                bool isNew = false;
                if (page == null)
                {
                    page = new DataLayer.PageMaster();
                    isNew = true;
                }
                page.Id = e.Id;
                page.Text = e.Text;

                if (isNew)
                {
                    Context.PageMasters.InsertOnSubmit(page);
                }
                Context.SubmitChanges();
                return page.Id;
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.EXIST;
            }
            finally
            {

            }
        }

        public int DeletePageMaster(int id)
        {
            try
            {
                DataLayer.PageMaster page = (from p in Context.PageMasters
                                           where p.Id == id
                                           select p).FirstOrDefault();
                if (page != null)
                {
                    Context.PageMasters.DeleteOnSubmit(page);
                    Context.SubmitChanges();
                    return id;
                }
                else
                {
                    return (int)Enums.Errors.NOT_EXIST;
                }
            }
            catch
            {
                return (int)Enums.Errors.DELETE_ERROR;
            }
        }

        public int SavePageCategory(Entities.PageCategory e)
        {
            try
            {
                DataLayer.Page_Category page = (from p in Context.Page_Categories
                                             where p.Id == e.Id
                                             select p).FirstOrDefault();
                bool isNew = false;
                if (page == null)
                {
                    page = new DataLayer.Page_Category();
                    isNew = true;
                }
                page.Id = e.Id;
                page.PageId = e.PageId;
                page.CategoryId = e.CategoryId;

                if (isNew)
                {
                    Context.Page_Categories.InsertOnSubmit(page);
                }
                Context.SubmitChanges();
                return page.Id;
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.EXIST;
            }
            finally
            {

            }
        }

        public int DeletePageCategory(int id)
        {
            try
            {
                DataLayer.Page_Category page = (from p in Context.Page_Categories
                                             where p.Id == id
                                             select p).FirstOrDefault();
                if (page != null)
                {
                    Context.Page_Categories.DeleteOnSubmit(page);
                    Context.SubmitChanges();
                    return id;
                }
                else
                {
                    return (int)Enums.Errors.NOT_EXIST;
                }
            }
            catch
            {
                return (int)Enums.Errors.DELETE_ERROR;
            }
        }

        public Entities.PageCategory GetPageCategoryById(int id)
        {
            var result = (from p in Context.Page_Categories
                          where p.Id == id
                          select new Entities.PageCategory()
                          {
                              Id = p.Id,
                              PageId = p.PageId,
                              CategoryId = p.CategoryId
                          }).FirstOrDefault();
            return result;
        }

        public List<Entities.PageCategory> ListPageCategory()
        {
            var result = (from p in Context.Page_Categories
                          select new Entities.PageCategory()
                          {
                              Id = p.Id,
                              PageId = p.PageId,
                              CategoryId = p.CategoryId
                          }).ToList();
            return result;
        }

        public List<Entities.PageCategory> ListPageCategory(int categoryId)
        {
            var result = (from p in Context.Page_Categories
                          join page in Context.PageMasters on p.PageId equals page.Id
                          where p.CategoryId == categoryId
                          select new Entities.PageCategory()
                          {
                              Id = p.Id,
                              PageId = p.PageId,
                              CategoryId = p.CategoryId,
                              PageMasterText = page.Text
                          }).ToList();
            return result;
        }
        
        public int SavePageArticle(Entities.PageArticle e)
        {
            try
            {
                DataLayer.Page_Article page = (from p in Context.Page_Articles
                                                where p.Id == e.Id
                                                select p).FirstOrDefault();
                bool isNew = false;
                if (page == null)
                {
                    page = new DataLayer.Page_Article();
                    isNew = true;
                }
                page.Id = e.Id;
                page.PageId = e.PageId;
                page.ArticleId = e.ArticleId;

                if (isNew)
                {
                    Context.Page_Articles.InsertOnSubmit(page);
                }
                Context.SubmitChanges();
                return page.Id;
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.EXIST;
            }
            finally
            {

            }
        }

        public int DeletePageArticle(int id)
        {
            try
            {
                DataLayer.Page_Article page = (from p in Context.Page_Articles
                                                where p.Id == id
                                                select p).FirstOrDefault();
                if (page != null)
                {
                    Context.Page_Articles.DeleteOnSubmit(page);
                    Context.SubmitChanges();
                    return id;
                }
                else
                {
                    return (int)Enums.Errors.NOT_EXIST;
                }
            }
            catch
            {
                return (int)Enums.Errors.DELETE_ERROR;
            }
        }

        public List<Entities.PageArticle> ListPageArticle(int articleId)
        {
            var result = (from p in Context.Page_Articles
                          join page in Context.PageMasters on p.PageId equals page.Id
                          where p.ArticleId == articleId
                          select new Entities.PageArticle()
                          {
                              Id = p.Id,
                              PageId = p.PageId,
                              ArticleId = p.ArticleId,
                              PageMasterText = page.Text
                          }).ToList();
            return result;
        }
    }
}
