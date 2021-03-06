using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLayer.Helpers;

namespace BusinessLayer
{
    public class CategoryService: BaseService<Entities.Category>
    {
        public CategoryService() : base() { }

        private string className { get { return this.GetType().Name; } }

        /// <summary>
        /// Lấy thư mục theo ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Entities.Category GetById(int Id)
        {
            var e = (from c in Context.Categories
                     where c.Id == Id
                     select new Entities.Category
                     {
                         Id = c.Id,
                         Parent = c.Parent,
                         Text = c.Text,
                         Code = c.Code,
                         SortOrder = c.SortOrder,
                         Type = c.Type,
                         TypeText = c.TypeText,
                         Display = c.Display
                     }).FirstOrDefault();
            return e;
        }
        
        /// <summary>
        /// Lưu thông tin thư mục
        /// </summary>
        public int Save(Entities.Category e)
        {
            try
            {
                DataLayer.Category category = (from c in Context.Categories
                                               where c.Id == e.Id
                                               select c).FirstOrDefault();
                bool isNew = false;
                if (category == null)
                {
                    category = new DataLayer.Category();
                    isNew = true;
                }
                category.Id = e.Id;
                category.Parent = e.Parent;
                category.Text = e.Text;
                category.Code = e.Code;
                category.SortOrder = e.SortOrder;
                category.Type = e.Type;
                category.TypeText = e.TypeText;
                category.Display = e.Display;

                if (isNew)
                {
                    Context.Categories.InsertOnSubmit(category);
                }
                Context.SubmitChanges();
                return category.Id;
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
        public List<Entities.Category> List()
        {
            List<Entities.Category> list = new List<Entities.Category>();
            list = (from c in Context.Categories
                    select new Entities.Category
                    {
                        Id = c.Id,
                        Parent = c.Parent,
                        Text = c.Text,
                        Code = c.Code,
                        SortOrder = c.SortOrder,
                        Type = c.Type,
                        TypeText = c.TypeText,
                        Display = c.Display
                    }).ToList();
            return list;
        }

        /// <summary>
        /// Lấy thư mục theo tìm kiếm
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public List<Entities.Category> List(string text)
        {
            List<Entities.Category> list = new List<Entities.Category>();
            list = (from c in Context.Categories
                    join c2 in Context.Categories on c.Parent equals c2.Id into cat2
                    from res in cat2.DefaultIfEmpty()
                    where c.Text.Contains(text)
                    select new Entities.Category
                    {
                        Id = c.Id,
                        Parent = c.Parent,
                        Text = c.Text,
                        ParentText = res.Text,
                        Code = c.Code,
                        SortOrder = c.SortOrder,
                        Type = c.Type,
                        TypeText = c.TypeText,
                        Display = c.Display
                    }).ToList();
            return list;
        }

        public List<Entities.Item> ListItem()
        {
            List<Entities.Item> list = new List<Entities.Item>();
            list = (from c in Context.Categories
                    select new Entities.Item
                    {
                        Id = c.Id,
                        Text = c.Text
                    }).ToList();
            return list;
        }
    }
}
