using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLayer.Helpers;

namespace BusinessLayer
{
    public class BoxLinkService : BaseService<Entities.BoxLink>
    {
        public BoxLinkService() : base() { }

        private string className { get { return this.GetType().Name; } }

        /// <summary>
        /// Lấy thư mục theo ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Entities.BoxLink GetById(int Id)
        {
            var e = (from c in Context.BoxLinks
                     where c.Id == Id
                     select new Entities.BoxLink
                     {
                         Id = c.Id,
                         Display = c.Display,
                         Order = c.Order,
                         Position = c.Position,
                         BoxName = c.BoxName,
                         DisplayBoxName = c.DisplayBoxName,
                         Color = c.Color,
                         FromTable = c.FromTable,
                         ColumnName = c.ColumnName,
                         Condition = c.Condition,
                         Count = c.Count,
                         OrderBy = c.OrderBy
                     }).FirstOrDefault();
            return e;
        }
        
        /// <summary>
        /// Lưu thông tin thư mục
        /// </summary>
        public int Save(Entities.BoxLink e)
        {
            try
            {
                DataLayer.BoxLink boxLink = (from c in Context.BoxLinks
                                               where c.Id == e.Id
                                               select c).FirstOrDefault();
                bool isNew = false;
                if (boxLink == null)
                {
                    boxLink = new DataLayer.BoxLink();
                    isNew = true;
                }
                boxLink.Id = e.Id;
                boxLink.Display = e.Display;
                boxLink.Order = e.Order;
                boxLink.Position = e.Position;
                boxLink.BoxName = e.BoxName;
                boxLink.DisplayBoxName = e.DisplayBoxName;
                boxLink.ColumnName = e.ColumnName;
                boxLink.Color = e.Color;
                boxLink.FromTable = e.FromTable;
                boxLink.Condition = e.Condition;
                boxLink.Count = e.Count;
                boxLink.OrderBy = e.OrderBy;

                if (isNew)
                {
                    Context.BoxLinks.InsertOnSubmit(boxLink);
                }
                Context.SubmitChanges();
                return boxLink.Id;
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
        public List<Entities.BoxLink> List()
        {
            List<Entities.BoxLink> list = new List<Entities.BoxLink>();
            list = (from c in Context.BoxLinks
                    select new Entities.BoxLink
                    {
                        Id = c.Id,
                        Display = c.Display,
                        Order = c.Order,
                        Position = c.Position,
                        BoxName = c.BoxName,
                        DisplayBoxName = c.DisplayBoxName,
                        Color = c.Color,
                        FromTable = c.FromTable,
                        ColumnName = c.ColumnName,
                        Condition = c.Condition,
                        Count = c.Count,
                        OrderBy = c.OrderBy
                    }).ToList();
            return list;
        }

        /// <summary>
        /// Lấy thư mục theo tìm kiếm
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public List<Entities.BoxLink> List(string text)
        {
            List<Entities.BoxLink> list = new List<Entities.BoxLink>();
            list = (from c in Context.BoxLinks
                    where c.BoxName.Contains(text)
                    select new Entities.BoxLink
                    {
                        Id = c.Id,
                        Display = c.Display,
                        Order = c.Order,
                        Position = c.Position,
                        BoxName = c.BoxName,
                        DisplayBoxName = c.DisplayBoxName,
                        Color = c.Color,
                        FromTable = c.FromTable,
                        ColumnName = c.ColumnName,
                        Condition = c.Condition,
                        Count = c.Count,
                        OrderBy = c.OrderBy
                    }).ToList();
            return list;
        }

        public List<Entities.BoxLink> List(int position)
        {
            List<Entities.BoxLink> list = new List<Entities.BoxLink>();
            list = (from c in Context.BoxLinks
                    where c.Position == position
                    && c.Display
                    select new Entities.BoxLink
                    {
                        Id = c.Id,
                        Order = c.Order,
                        Position = c.Position,
                        BoxName = c.BoxName,
                        DisplayBoxName = c.DisplayBoxName,
                        ColumnName = c.ColumnName,
                        Color = c.Color,
                        FromTable = c.FromTable,
                        Condition = c.Condition,
                        Count = c.Count,
                        OrderBy = c.OrderBy
                    }).ToList();

            if (list.Count > 0)
            {
                StringBuilder sql = new StringBuilder();
                foreach (Entities.BoxLink box in list)
                {
                    sql.Length = 0;
                    sql.AppendLine("SELECT * FROM " + box.FromTable + " ");
                    if (box.Condition != null)
                    {
                        sql.AppendLine("WHERE " + box.Condition);
                    }
                    
                    var result = Context.ExecuteQuery<Entities.LinkItem>(sql.ToString()).ToList();
                    box.ListLink = result;
                }
            }
            return list;
        }

        public List<Entities.Item> ListItem()
        {
            List<Entities.Item> list = new List<Entities.Item>();
            list = (from c in Context.BoxLinks
                    select new Entities.Item
                    {
                        Id = c.Id,
                        Text = c.BoxName
                    }).ToList();
            return list;
        }
    }
}
