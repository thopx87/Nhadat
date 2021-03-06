using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLayer.Helpers;

namespace BusinessLayer
{
    public class ArticleService: BaseService<Entities.Article>
    {
        public ArticleService() : base() { }

        private string className { get { return this.GetType().Name; } }

        /// <summary>
        /// Lấy thư mục theo ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Entities.Article GetById(int Id)
        {
            var e = (from a in Context.Articles
                     where a.Id == Id
                     select new Entities.Article
                     {
                         Id = a.Id,
                         CategoryId = a.CategoryId,
                         Title = a.Title,
                         Alias = a.Alias,
                         Body = a.Body,
                         MetaTitle = a.MetaTitle,
                         MetaKeyWord = a.MetaKeyWord,
                         MetaDescription = a.MetaDescription,
                         UpdateDate = DateTime.Now,
                         UpdateBy = a.UpdateBy,
                         Display = a.Display
                     }).FirstOrDefault();
            return e;
        }

        public Entities.Article GetByAlias(string alias)
        {
            var e = (from a in Context.Articles
                     where a.Alias == alias
                     select new Entities.Article
                     {
                         Id = a.Id,
                         CategoryId = a.CategoryId,
                         Title = a.Title,
                         Alias = a.Alias,
                         Body = a.Body,
                         MetaTitle = a.MetaTitle,
                         MetaKeyWord = a.MetaKeyWord,
                         MetaDescription = a.MetaDescription,
                         UpdateDate = DateTime.Now,
                         UpdateBy = a.UpdateBy,
                         Display = a.Display
                     }).FirstOrDefault();
            return e;
        }

        /// <summary>
        /// Lưu thông tin bài viết
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public int Save(Entities.Article e)
        {
            try
            {
                DataLayer.Article article = (from a in Context.Articles
                                             where a.Id == e.Id
                                             select a).FirstOrDefault();
                bool isNew = false;
                if (article == null)
                {
                    article = new DataLayer.Article();
                    isNew = true;
                }
                article.Id = e.Id;
                article.CategoryId = e.CategoryId;
                article.Title = e.Title;
                article.Alias = e.Alias;
                article.Body = e.Body;
                article.MetaTitle = string.IsNullOrEmpty(e.MetaTitle) ? e.Title.NonUnicode() : e.MetaTitle;
                article.MetaKeyWord = string.IsNullOrEmpty(e.MetaKeyWord) ? e.Title : e.MetaKeyWord;
                article.MetaDescription = string.IsNullOrEmpty(e.MetaDescription) ? e.Title : e.MetaDescription;
                article.UpdateDate = DateTime.Now;
                article.UpdateBy = e.UpdateBy;
                article.Display = e.Display;

                if (isNew)
                {
                    Context.Articles.InsertOnSubmit(article);
                }
                Context.SubmitChanges();
                return article.Id;
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

        /// <summary>
        /// Lấy toàn bộ danh sách thư mục        
        /// </summary>
        /// <returns></returns>
        public List<Entities.Article> List()
        {
            List<Entities.Article> list = new List<Entities.Article>();
            list = (from a in Context.Articles
                    select new Entities.Article
                    {
                        Id = a.Id,
                        CategoryId = a.CategoryId,
                        Title = a.Title,
                        Alias = a.Alias,
                        Body = a.Body,
                        MetaTitle = a.MetaTitle,
                        MetaKeyWord = a.MetaKeyWord,
                        MetaDescription = a.MetaDescription,
                        UpdateDate = DateTime.Now,
                        UpdateBy = a.UpdateBy,
                        Display = a.Display
                    }).ToList();
            return list;
        }

        /// <summary>
        /// Lấy bài viết theo tìm kiếm
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public List<Entities.Article> List(int categoryId, string text, bool displayOnly, int pageIndex, int pageSize, out int total)
        {
            if (text == null) text = "";
            var query = (from a in Context.Articles
                         join c in Context.Categories on a.CategoryId equals c.Id into cat
                         from res in cat.DefaultIfEmpty()
                         where (a.Title.Contains(text) || a.Body.Contains(text))
                         && (categoryId > 0 ? a.CategoryId == categoryId : true)
                         && (displayOnly?a.Display == true: true)
                         select new Entities.Article
                         {
                             Id = a.Id,
                             CategoryId = a.CategoryId,
                             CategoryText = res.Text,
                             Title = a.Title,
                             Alias = a.Alias,
                             Body = a.Body,
                             MetaTitle = a.MetaTitle,
                             MetaKeyWord = a.MetaKeyWord,
                             MetaDescription = a.MetaDescription,
                             UpdateDate = DateTime.Now,
                             UpdateBy = a.UpdateBy,
                             Display = a.Display
                         });
            total = 0;
            if (query.Count() > 0)
            {
                total = query.Count();
            }
            var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).OrderByDescending(x=>x.UpdateDate).ToList();

            return result;
        }

        public List<Entities.Article> List(int categoryType)
        {
            List<Entities.Article> list = new List<Entities.Article>();
            list = (from a in Context.Articles
                    join c in Context.Categories on a.CategoryId equals c.Id into cat
                    from res in cat.DefaultIfEmpty()
                    where res.Type == categoryType
                    && res.Display
                    && a.Display
                    select new Entities.Article
                    {
                        Id = a.Id,
                        CategoryId = a.CategoryId,
                        CategoryText = res.Text,
                        Title = a.Title,
                        Alias = a.Alias,
                        Body = a.Body,
                        MetaTitle = a.MetaTitle,
                        MetaKeyWord = a.MetaKeyWord,
                        MetaDescription = a.MetaDescription,
                        UpdateDate = DateTime.Now,
                        UpdateBy = a.UpdateBy,
                        Display = a.Display
                    }).OrderByDescending(x => x.UpdateDate).ToList();
            return list;
        }

        /// <summary>
        /// Lấy danh sách thư mục
        /// </summary>
        /// <returns></returns>
        public List<Entities.Item> ListCategory()
        {
            List<Entities.Item> list = new List<Entities.Item>();
            list = (from c in Context.Categories
                    where c.Display == true
                    select new Entities.Item
                    {
                        Id = c.Id,
                        Text = c.Text
                    }).ToList();
            return list;
        }

    }
}
