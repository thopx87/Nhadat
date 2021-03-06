using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLayer.Helpers;

namespace BusinessLayer
{
    public class RoleService: BaseService<Entities.Role>
    {
        public RoleService() : base() { }

        private string className { get { return this.GetType().Name; } }

        public int Save(Entities.Role e)
        {
            // Check Rule data
            if (!CheckData(e))
            {
                string data = className + " Name: " + e.Text + "& Code: " + e.Code;
                Logs.LogWrite(string.Format(Configs.ERROR_DATA_WRONG, data));
                return (int)Enums.Errors.INPUT;
            }

            // Check Exists
            if (CheckExistCode(e.Id , e.Code))
            {
                string data = className + " Code: " + e.Code;
                Logs.LogWrite(string.Format(Configs.ERROR_ENTITY_EXISTS, data));
                return (int)Enums.Errors.EXIST;
            }

            DataLayer.Role role;

            bool isNew = false;
            if (e.Id > 0)
            {
                role = (from r in Context.Roles
                        where r.Id == e.Id
                        select r).FirstOrDefault();
            }
            else
            {
                role = new DataLayer.Role();
                isNew = true;
            }

            role.Text = e.Text;
            role.Code = e.Code;
            role.Active = e.Active;
            role.IsAgency = e.IsAgency;
            role.Post = e.Post;
            role.ResiveFromAgency = e.ResiveFromAgency;
            role.ResiveFromMember = e.ResiveFromMember;
            role.SendRegionNum = e.SendRegionNum;
            role.ResiveRegionNum = e.ResiveRegionNum;
            role.CanViewHouseNum = e.CanViewHouseNum;
            role.CanCheckOwnerArticle = e.CanCheckOwnerArticle;
            role.OnlySendFixedRegion = e.OnlySendFixedRegion;
            //role.BuffetRegions = e.BuffetRegions;
            role.IsAdmin = e.IsAdmin;
            try
            {
                if (isNew)
                {
                    Context.Roles.InsertOnSubmit(role);
                }
                Context.SubmitChanges();
                return role.Id;
            }
            catch(Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.EXIST;
            }
        }

        public int Delete(int entityId)
        {
            try
            {
                DataLayer.Role role = (from r in Context.Roles
                                       where r.Id == entityId
                                       select r).FirstOrDefault();
                Context.Roles.DeleteOnSubmit(role);
                Context.SubmitChanges();
                return role.Id;
            }
            catch(Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.DELETE_ERROR;
            }
        }

        public Entities.Role GetById(int entityId)
        {
            var role= (from r in Context.Roles
                 where r.Id == entityId
                   select new Entities.Role
                   {
                       Id = r.Id,
                       Text = r.Text,
                       Code = r.Code,
                       Active = r.Active,
                       Post = r.Post,
                       IsAgency = r.IsAgency,
                       ResiveFromAgency = r.ResiveFromAgency,
                       ResiveFromMember = r.ResiveFromMember,
                       SendRegionNum = r.SendRegionNum,
                       ResiveRegionNum = r.ResiveRegionNum,
                       CanViewHouseNum = r.CanViewHouseNum,
                       CanCheckOwnerArticle = r.CanCheckOwnerArticle,
                       OnlySendFixedRegion = r.OnlySendFixedRegion,
                       BuffetRegions = r.BuffetRegions,
                       IsAdmin = r.IsAdmin
                   }).FirstOrDefault();
            return role;
        }

        public Entities.Role GetByCode(string code)
        {
            var role = (from r in Context.Roles
                 where r.Code == code
                 select new Entities.Role
                 {
                     Id = r.Id,
                     Text = r.Text,
                     Code = r.Code,
                     Active = r.Active,
                     Post = r.Post,
                     IsAgency = r.IsAgency,
                     ResiveFromAgency = r.ResiveFromAgency,
                     ResiveFromMember = r.ResiveFromMember,
                     SendRegionNum = r.SendRegionNum,
                     ResiveRegionNum = r.ResiveRegionNum,
                     CanViewHouseNum = r.CanViewHouseNum,
                     CanCheckOwnerArticle = r.CanCheckOwnerArticle,
                     OnlySendFixedRegion = r.OnlySendFixedRegion,
                     BuffetRegions = r.BuffetRegions,
                     IsAdmin = r.IsAdmin
                 }).FirstOrDefault();
            return role;
        }

        public List<Entities.Role> List()
        {
            var lst = (from r in Context.Roles
                   select new Entities.Role
                   {
                       Id = r.Id,
                       Text = r.Text,
                       Code = r.Code,
                       Active = r.Active,
                       Post = r.Post,
                       IsAgency = r.IsAgency,
                       ResiveFromAgency = r.ResiveFromAgency,
                       ResiveFromMember = r.ResiveFromMember,
                       SendRegionNum = r.SendRegionNum,
                       ResiveRegionNum = r.ResiveRegionNum,
                       CanViewHouseNum = r.CanViewHouseNum,
                       CanCheckOwnerArticle = r.CanCheckOwnerArticle,
                       OnlySendFixedRegion = r.OnlySendFixedRegion,
                       BuffetRegions = r.BuffetRegions,
                       IsAdmin = r.IsAdmin
                   }).ToList();
            return lst;
        }

        public List<Entities.Item> ListItem()
        {
            var result = (from r in Context.Roles
                   select new Entities.Item
                   {
                       Id = r.Id,
                       Text = r.Text
                   }).ToList();
            return result;
        }

        public List<Entities.Item> ListAgency()
        {
            var result = (from r in Context.Roles
                          where r.IsAgency == true
                          && r.Active == true
                          select new Entities.Item
                          {
                              Id = r.Id,
                              Text = r.Text
                          }).ToList();
            return result;
        }

        public Entities.Item GetItem(int id)
        {
            var result = (from r in Context.Roles
                          where r.Id == id
                          select new Entities.Item
                          {
                              Id = r.Id,
                              Text = r.Text
                          }).FirstOrDefault();
            return result;
        }

        public List<Entities.Role> List(string strName, int pageIndex, int pageSize, int state, out int total)
        {
            if (string.IsNullOrEmpty(strName))
            {
                strName = "";
            }
            total = 0;
            total = (from r in Context.Roles
                     where (r.Text.ToString().Contains(strName) || r.Code.Contains(strName))
                     && (state == 1 ? r.Active == true : state == 2 ? r.Active == false : true)
                     select r.Id).Count();
            var result = (from r in Context.Roles
                          where (r.Text.ToString().Contains(strName) || r.Code.Contains(strName))
                            && (state==1?r.Active==true: state==2?r.Active==false:true)
                              select new Entities.Role
                                {
                                    Id = r.Id,
                                    Text = r.Text,
                                    Code = r.Code,
                                    Active = r.Active,
                                    IsAgency = r.IsAgency,
                                    Post = r.Post,
                                    ResiveFromAgency = r.ResiveFromAgency,
                                    ResiveFromMember = r.ResiveFromMember,
                                    SendRegionNum =r.SendRegionNum,
                                    ResiveRegionNum = r.ResiveRegionNum,
                                    CanViewHouseNum = r.CanViewHouseNum,
                                    CanCheckOwnerArticle = r.CanCheckOwnerArticle,
                                    OnlySendFixedRegion = r.OnlySendFixedRegion,
                                    BuffetRegions = r.BuffetRegions,
                                    IsAdmin = r.IsAdmin
                                }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }

        /// <summary>
        /// Kiểm tra role có hợp lệ hay không.
        /// </summary>
        /// <param name="e"></param>
        /// <returns>true nếu hợp lệ, false nếu lỗi</returns>
        public bool CheckData(Entities.Role e)
        {
            return StringHelper.CheckInputStandar(e.Text) && StringHelper.CheckInputStandar(e.Code);
        }

        /// <summary>
        /// Kiểm tra role đã tồn tại hay chưa
        /// </summary>
        /// <param name="e"></param>
        /// <returns>true nếu tồn tại, false nếu không tồn tại</returns>
        public bool CheckExistCode(int id, string code)
        {
            var old = (from r in Context.Roles
                       where r.Code.Trim().ToLower() == code.Trim().ToLower() && r.Id!=id
                       select r).FirstOrDefault();
            if (old != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public bool CheckRoleAgency(int roleId)
        {
            var result = (from r in Context.Roles
                          where r.Id == roleId
                          select r.IsAgency).FirstOrDefault();
            return result;
        }

        public List<Entities.Role> ListActive()
        {
            var lst = (from r in Context.Roles
                       where r.Active ==true
                       select new Entities.Role
                       {
                           Id = r.Id,
                           Text = r.Text,
                           Code = r.Code
                       }).ToList();
            if (lst == null) return new List<Entities.Role>();
            return lst;
        }

        public Entities.Role GetMaxRolesOfUser(int userId)
        {
            Entities.Role maxRole = new Entities.Role();
            // Get List role by User Id.
            var result = (from uir in Context.UserInRoles
                          join r in Context.Roles on uir.RolesId equals r.Id
                          where uir.UserId == userId
                          && uir.State == true
                          select new Entities.Item()
                          {
                              Id = r.Id
                          }).ToList();
            if (result != null && result.Count > 0)
            {
                foreach (var uir in result)
                {
                    var tempRole = GetById(uir.Id);
                    if (tempRole.ResiveFromAgency || tempRole.IsAdmin)
                    {
                        maxRole.ResiveFromAgency = true;
                    }
                    if (tempRole.ResiveFromMember || tempRole.IsAdmin)
                    {
                        maxRole.ResiveFromMember = true;
                    }
                    if (tempRole.CanCheckOwnerArticle || tempRole.IsAdmin)
                    {
                        maxRole.CanCheckOwnerArticle = true;
                    }
                    if (tempRole.CanViewHouseNum || tempRole.IsAdmin)
                    {
                        maxRole.CanViewHouseNum = true;
                    }
                }
            }
            return maxRole;
        }

        public List<int> GetListRoleAllowPost()
        {
            var result = (from r in Context.Roles
                          where r.Post
                          select r.Id).ToList();
            return result;
        }
    }
}
