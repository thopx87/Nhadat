using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application2016.Helpers
{
    public static class DefaultData
    {
        public static List<Entities.Item> ListTransactionType()
        {
            List<Entities.Item> listItem = new List<Entities.Item>();
            listItem.Add(new Entities.Item()
            {
                Id = 0,
                Text = "Tất cả"
            }); 
            listItem.Add(new Entities.Item()
            {
                Id = (int)Enums.TransactionType.BAN_NHA,
                Text ="Bán nhà"
            });
            listItem.Add(new Entities.Item()
            {
                Id = (int)Enums.TransactionType.CHO_THUE,
                Text = "Cho thuê nhà"
            });
            listItem.Add(new Entities.Item()
            {
                Id = (int)Enums.TransactionType.MUA_NHA,
                Text = "Mua nhà"
            });
            listItem.Add(new Entities.Item()
            {
                Id = (int)Enums.TransactionType.THUE_NHA,
                Text = "Thuê nhà"
            });
            return listItem;
        }

        public static List<Entities.Item> ListState()
        {
            List<Entities.Item> listItem = new List<Entities.Item>();
            listItem.Add(new Entities.Item()
            {
                Id = 0,
                Text = "Tất cả"
            });
            listItem.Add(new Entities.Item()
            {
                Id = 1,
                Text = "Được hoạt động"
            });
            listItem.Add(new Entities.Item()
            {
                Id = 2,
                Text = "Không được hoạt động"
            });
            return listItem;
        }

        public static List<Entities.Item> ListCategoryType()
        {
            List<Entities.Item> listItem = new List<Entities.Item>();
            listItem.Add(new Entities.Item()
            {
                Id = 0,
                Text = "Thông tin chung"
            });

            listItem.Add(new Entities.Item()
            {
                Id = 1,
                Text = "Thông báo"
            });
            listItem.Add(new Entities.Item()
            {
                Id = 2,
                Text = "Cảnh báo"
            });
            listItem.Add(new Entities.Item()
            {
                Id = 3,
                Text = "Email"
            });

            return listItem;
        }

        public static List<Entities.Item> ListArea()
        {
            List<Entities.Item> listItem = new List<Entities.Item>();
            listItem.Add(new Entities.Item()
            {
                Id = 0,
                Text = "0"
            });
            listItem.Add(new Entities.Item()
            {
                Id = 50,
                Text = "50"
            });
            listItem.Add(new Entities.Item()
            {
                Id = 100,
                Text = "100"
            });
            listItem.Add(new Entities.Item()
            {
                Id = 150,
                Text = "150"
            });
            listItem.Add(new Entities.Item()
            {
                Id = 200,
                Text = "200"
            });
            listItem.Add(new Entities.Item()
            {
                Id = 250,
                Text = "250"
            });
            listItem.Add(new Entities.Item()
            {
                Id = 300,
                Text = "300"
            });
            return listItem;
        }
    }
}