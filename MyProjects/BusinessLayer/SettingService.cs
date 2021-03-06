using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using DataLayer;
using BusinessLayer.Helpers;
using System.Data.Common;
using System.Data.SqlClient;

namespace BusinessLayer
{
    public class SettingService: BaseService<Entities.Setting>
    {
        private string className { get { return this.GetType().Name; } }
        public SettingService() : base() { }

        public int Insert(Entities.Setting e)
        {
            DataLayer.Setting setting = new DataLayer.Setting();

            setting.SettingName = e.SettingName;
            setting.Title = e.Title;
            setting.Value = e.Value;
            setting.Display = e.Display;
            setting.DisplayState = e.DisplayState;
            try
            {
                Context.Settings.InsertOnSubmit(setting);
                Context.SubmitChanges();
                return setting.Id;
            }
            catch (Exception ex)
            {
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
            finally
            {
                
            }

        }

        public int Update(Entities.Setting e)
        {
            DataLayer.Setting setting = (from u in Context.Settings
                                  where u.Id == e.Id
                                  select u).FirstOrDefault();
            setting.Value = e.Value;
            setting.DisplayState = e.DisplayState;
            try
            {
                Context.SubmitChanges();
                return e.Id;
            }
            catch (Exception ex)
            {
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.UPDATE_ERROR;

            }
            
        }

        public int Delete(int entityId)
        {
            try
            {
                DataLayer.Setting setting = (from u in Context.Settings
                                       where u.Id == entityId
                                       select u).FirstOrDefault();
                Context.Settings.DeleteOnSubmit(setting);
                Context.SubmitChanges();
                return entityId;
            }
            catch (Exception ex)
            {
                Context.Transaction.Rollback();
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.DELETE_ERROR;
            }
        }

        public Entities.Setting GetById(int entityId)
        {
            Entities.Setting setting = null;
            setting = (from u in Context.Settings
                    where entityId == u.Id
                    select new Entities.Setting
                    {
                        Id = u.Id,
                        SettingName = u.SettingName,
                        Title = u.Title,
                        Value = u.Value,
                        Display = u.Display
                    }).FirstOrDefault();
            return setting;
        }

        public Entities.Setting Get(string settingName, string title)
        {
            Entities.Setting setting = null;
            setting = (from u in Context.Settings
                       where settingName == u.SettingName && title == u.Title
                       select new Entities.Setting
                       {
                           Id = u.Id,
                           SettingName = u.SettingName,
                           Title = u.Title,
                           Value = u.Value,
                           Display = u.Display,
                           DisplayState = u.DisplayState
                       }).FirstOrDefault();
            return setting;
        }

        /// <summary>
        /// Get All setting
        /// </summary>
        /// <returns></returns>
        public List<Entities.Setting> List()
        {
            List<Entities.Setting> lst = new List<Entities.Setting>();
            lst = (from u in Context.Settings
                   select new Entities.Setting
                   {
                       Id = u.Id,
                       SettingName = u.SettingName,
                       Title = u.Title,
                       Value = u.Value,
                       Display = u.Display,
                       DisplayState = u.DisplayState
                   }).ToList();

            return lst;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public List<Entities.Setting> List(string settingName)
        {
            List<Entities.Setting> lst = new List<Entities.Setting>();
            lst = (from u in Context.Settings
                   where settingName == u.SettingName
                   select new Entities.Setting
                   {
                       Id = u.Id,
                       SettingName = u.SettingName,
                       Title = u.Title,
                       Value = u.Value,
                       Display = u.Display,
                       DisplayState = u.DisplayState
                   }).ToList();

            return lst;
        }

        public string UpdateDB(string Query, int Type)
        {
            try
            {
                Context.ExecuteCommand(Query);
                Logs.LogWrite(Query, "DBQuery.txt");

                return null;
            }
            catch(Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public void UpdateData()
        {
            try
            {
                //using (var db = new MyDBDataContext())
                //{
                //    // Cập nhật bảng Product_ChangeCost
                //    // Set Phonenumber.
                //    db.Product_ChangeCosts.Join(db.Users,u =>
                //}
            }
            catch
            {

            }
        }

        /// <summary>
        /// Cập nhật toàn bộ Alias cho sản phẩm.
        /// </summary>
        /// <returns></returns>
        public string UpdateAlias()
        {
            string result = "";
            try
            {
                //using (MyDBDataContext db = new MyDBDataContext())
                //{
                //    db.Products
                //      .ToList()
                //      .ForEach(a => a.Text_Alias = a.Text.ToAlias());
                //    db.SubmitChanges();
                //    result = "Update Success";
                //}
            }
            catch
            {
                result = "Update Error";
            }
            return result;
        }
    }
}
