using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLayer.Helpers;

namespace BusinessLayer
{
    public class UserInRolesService : BaseService<Entities.UserInRole>
    {
        public UserInRolesService() : base() { }

        private string className { get { return this.GetType().Name; } }

        public int Save(Entities.UserInRole e)
        {
            DataLayer.UserInRole u = (from x in Context.UserInRoles
                                      where x.Id == e.Id
                                      select x).FirstOrDefault();
            u.RolesId = e.RolesId;
            u.UserId = e.UserId;
            u.State = e.State;
            try
            {
                Context.SubmitChanges();
                return u.Id;
            }
            catch (Exception ex)
            {
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
        }

        public int Insert(Entities.UserInRole e)
        {
            DataLayer.UserInRole u = new DataLayer.UserInRole();
            u.RolesId = e.RolesId;
            u.UserId = e.UserId;
            u.State = e.State;
            try
            {
                Context.UserInRoles.InsertOnSubmit(u);
                Context.SubmitChanges();
                return u.Id;
            }
            catch(Exception ex)
            {
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
        }

        public int Update(Entities.UserInRole e)
        {
            DataLayer.UserInRole u = (from x in Context.UserInRoles
                                      where x.Id == e.Id
                                      select x).FirstOrDefault();
            u.RolesId = e.RolesId;
            u.UserId = e.UserId;
            u.State = e.State;
            try
            {
                Context.SubmitChanges();
                return u.Id;
            }
            catch (Exception ex)
            {
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
        }

        public int Delete(int userId)
        {
            try
            {
                var result = (from u in Context.UserInRoles
                                           where u.UserId == userId
                                           select u).ToList();
                if (result != null)
                {
                    Context.UserInRoles.DeleteAllOnSubmit(result);
                    Context.SubmitChanges();
                    return userId;
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

        public Entities.UserInRole GetById(int entityId)
        {
            return (from c in Context.UserInRoles
                    where c.Id == entityId
                    select new Entities.UserInRole()
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        RolesId = c.RolesId,
                        State = c.State
                    }).FirstOrDefault();
        }

        public Entities.UserInRole GetByRole(int roleId)
        {
            return (from c in Context.UserInRoles
                    where c.RolesId == roleId
                    select new Entities.UserInRole()
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        RolesId = c.RolesId,
                        State = c.State
                    }).FirstOrDefault();
        }

        public List<Entities.UserInRole> List()
        {
            throw new NotImplementedException();
        }

        public List<Entities.UserInRole> List(int userId)
        {
            var result = (from x in Context.UserInRoles
                          where x.UserId == userId
                          select new Entities.UserInRole()
                          {
                              Id = x.Id,
                              UserId = x.UserId,
                              RolesId = x.RolesId,
                              State = x.State
                          }).ToList();
            return result;
        }

        public bool CheckUserRole(int role)
        {
            if (role == -1) return true;
            return false;
        }

        public bool CheckUserRole(int roleId, int userId)
        {
            if (roleId < 0 || userId < 0)
            {
                return false;
            }

            var userRole = (from u in Context.UserInRoles
                            where u.RolesId == roleId && u.UserId == userId
                            select u).FirstOrDefault();
            if (userRole != null)
            {
                return true;
            }              
            return false;
        }

        public bool CheckUserRole(int[] roleIds, int userId)
        {
            if (roleIds == null || userId < 0)
            {
                return false;
            }

            var userRole = (from u in Context.UserInRoles
                            where roleIds.Contains(u.RolesId) && u.UserId == userId && u.State ==true
                            select u).FirstOrDefault();
            if (userRole != null)
            {
                return true;
            }
            return false;
        }

        public bool CheckUserIsAgency(int userId)
        {
            var result = (from u in Context.UserInRoles
                          join r in Context.Roles on u.RolesId equals r.Id
                          where u.UserId == userId && u.State == true && r.IsAgency == true
                          select 1).FirstOrDefault();
            return result > 0;
        }

        public bool CheckUserIsAdmin(int userId)
        {
            var result = (from u in Context.UserInRoles
                          join r in Context.Roles on u.RolesId equals r.Id
                          where u.UserId == userId && u.State == true && r.IsAdmin == true
                          select 1).FirstOrDefault();
            return result > 0;
        }

        public Entities.Item GetRoleAllowPost(int userId)
        {
            var result = (from u in Context.UserInRoles
                          join r in Context.Roles on u.RolesId equals r.Id
                          where u.UserId == userId && u.State == true && r.Post == true
                          select new Entities.Item() { 
                            Id = r.Id,
                            Text = r.Text
                          }).FirstOrDefault();
            return result;
        }

        public List<Entities.Item> GetByUser(int userId)
        {
            var result = (from uir in Context.UserInRoles
                          join r in Context.Roles on uir.RolesId equals r.Id
                          where uir.UserId == userId
                          && uir.State == true
                          select new Entities.Item()
                          {
                              Id = r.Id,
                              Text = r.Text
                          }).ToList();
            return result;
        }

        /// <summary>
        /// Lấy Role ID đầu tiên tìm thấy từ User ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetFirstRoleIdByUser(int userId)
        {
            var result = (from r in Context.UserInRoles
                          where r.State == true
                          && r.UserId == userId
                          select r.RolesId).FirstOrDefault();
            return result;
        }
    }
}
