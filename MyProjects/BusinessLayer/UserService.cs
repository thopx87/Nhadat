using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using DataLayer;
using BusinessLayer.Helpers;

namespace BusinessLayer
{
    public class UserService: BaseService<Entities.User>
    {
        private string className { get { return this.GetType().Name; } }
        public UserService() : base() { }

        public int Insert(Entities.User e)
        {
            DataLayer.User user = new  DataLayer.User();
            user.DisplayName = e.DisplayName;
            user.FirstName = e.FirstName;
            user.LastName = e.LastName;
            user.Gender = e.Gender;
            user.DateOfBirth = DateTime.ParseExact(e.DateOfBirth, "dd/MM/yyyy", null);
            user.UserName = e.UserName;
            user.Email = e.Email;
            user.Password = e.Password;
            user.Status = e.Status;
            user.Description = e.Description;
            user.Delete = e.Delete;
            user.DateCreate = DateTime.Now;
            user.DateUpdate = DateTime.Now;
            user.DateLogin = DateTime.Now;
            user.PlaceId = e.PlaceId;
            user.Address = e.Address;
            user.Zipcode = e.Zipcode;
            user.CompanyLogo = e.CompanyLogo;
            user.CodeActive = e.CodeActive;
            user.CompanyName = e.CompanyName;
            user.Phone = e.Phone;
            user.Avatar = e.Avatar;
            user.IsUpdated = false;
            try
            {
                Context.Users.InsertOnSubmit(user);
                Context.SubmitChanges();
                return user.Id;
            }
            catch(Exception ex)
            {
                string data = className +" " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
        }

        public int Update(Entities.User e)
        {
            DataLayer.User user = (from u in Context.Users
                                  where u.Id == e.Id
                                  select u).FirstOrDefault();
            user.DisplayName = e.DisplayName;
            user.FirstName = e.FirstName;
            user.LastName = e.LastName;
            user.Gender = e.Gender;
            user.DateOfBirth = DateTime.ParseExact(e.DateOfBirth, "dd/MM/yyyy", null);
            user.UserName = e.UserName;
            user.Email = e.Email;
            user.Password = e.Password;
            user.Status = e.Status;
            user.Description = e.Description;
            user.Delete = false;
            user.DateCreate = DateTime.Now;
            user.DateUpdate = DateTime.Now;
            user.DateLogin = DateTime.Now;
            user.PlaceId = e.PlaceId;
            user.Address = e.Address;
            user.Zipcode = e.Zipcode;
            user.CompanyLogo = e.CompanyLogo;
            user.CodeActive = e.CodeActive;
            user.CompanyName = e.CompanyName;
            user.Phone = e.Phone;
            user.Avatar = e.Avatar;
            user.IsUpdated = true;
            try
            {
                Context.SubmitChanges();
                return e.Id;
            }
            catch(Exception ex)
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
                DataLayer.User user = (from u in Context.Users
                                       where u.Id == entityId
                                       select u).FirstOrDefault();
                Context.Users.DeleteOnSubmit(user);
                Context.SubmitChanges();
                return entityId;
            }
            catch (Exception ex)
            {
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.DELETE_ERROR;
            }
        }

        public Entities.User GetById(int entityId)
        {
            Entities.User user = null;
            user = (from u in Context.Users
                    where entityId == u.Id
                    && u.Delete == false
                    select new Entities.User
                    {
                        Id = u.Id,
                        DisplayName = u.DisplayName,                        
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Gender = u.Gender,
                        DateOfBirth = u.DateOfBirth.ToString(),
                        UserName = u.UserName,
                        Email = u.Email,
                        Password = u.Password,
                        Status = u.Status,
                        Description = u.Description,
                        Delete = u.Delete,
                        DateCreate = u.DateCreate,
                        DateUpdate = u.DateUpdate,
                        DateLogin = u.DateLogin,
                        PlaceId = u.PlaceId,
                        Address = u.Address,
                        Zipcode = u.Zipcode,
                        CompanyLogo = u.CompanyLogo,
                        CodeActive = u.CodeActive,
                        CompanyName = u.CompanyName,
                        Phone    = u.Phone,
                        Avatar = u.Avatar
                    }).FirstOrDefault();
            return user;
        }

        public string GetEmailById(int entityId)
        {
            string email = (from u in Context.Users
                    where entityId == u.Id
                    && u.Delete == false
                    select u.Email
                    ).FirstOrDefault();
            return email;
        }

        public string GetAllEmail()
        {
            var listEmail = (from u in Context.Users
                            where u.Status == true
                            && u.Delete == false
                            select u.Email
                    ).ToList();
            return string.Join(",", listEmail);
        }

        public bool CheckIdAndUserName(int id, string username)
        {
            var res = (from u in Context.Users
                    where id == u.Id && username == u.UserName
                    && u.Delete == false
                    select u.Id).FirstOrDefault();
            if (res > 0) return true;
            return false;
        }

        public List<Entities.User> List()
        {
            List<Entities.User> lst = null;
            lst = (from u in Context.Users
                   where u.Delete == false
                   select new Entities.User
                   {
                       Id = u.Id,
                       UserName = u.UserName,
                       Password = u.Password,
                       Email =u.Email
                   }).ToList();
            return lst;
        }

        public List<Entities.User> List(string key, int pageIndex, int pageSize, out int total)
        {
            List<Entities.User> lst = new List<Entities.User>();
            total = 0;

            if (key == null || key == "")
            {
                total = (from u in Context.Users
                         where u.Delete == false
                         select u.Id).Count();
                lst = (from u in Context.Users
                       where u.Delete == false
                       select new Entities.User
                       {
                            Id = u.Id,
                            DisplayName = u.DisplayName,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Gender = u.Gender,
                            DateOfBirth = u.DateOfBirth.ToString(),
                            UserName = u.UserName,
                            Email = u.Email,
                            Password = u.Password,
                            Status = u.Status,
                            Delete = u.Delete,
                            DateCreate = u.DateCreate,
                            DateUpdate = u.DateUpdate,
                            DateLogin = u.DateLogin,
                            PlaceId = u.PlaceId,
                            Address = u.Address,
                            Zipcode = u.Zipcode,
                            CompanyLogo = u.CompanyLogo,
                            CompanyName = u.CompanyName,
                            Phone = u.Phone,
                            Avatar = u.Avatar,
                            UserRoles = ListRoleByUserId(u.Id)
                       }).ToList();
            }
            else
            {
                total = (from u in Context.Users
                         where u.UserName.Contains(key)
                         select u.Id).Count();
                lst = (from u in Context.Users
                        where u.UserName.Contains(key)
                        select new Entities.User
                        {
                            Id = u.Id,
                            DisplayName = u.DisplayName,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Gender = u.Gender,
                            DateOfBirth = u.DateOfBirth.ToString(),
                            UserName = u.UserName,
                            Email = u.Email,
                            Password = u.Password,
                            Status = u.Status,
                            Delete = u.Delete,
                            DateCreate = u.DateCreate,
                            DateUpdate = u.DateUpdate,
                            DateLogin = u.DateLogin,
                            PlaceId = u.PlaceId,
                            Address = u.Address,
                            Zipcode = u.Zipcode,
                            CompanyLogo = u.CompanyLogo,
                            CompanyName = u.CompanyName,
                            Phone = u.Phone,
                            Avatar = u.Avatar,
                            UserRoles = ListRoleByUserId(u.Id)
                        }).ToList();
            }

            return lst;
        }

        public List<Entities.User> List(int regionId, int roleId, string key, int page, int pageSize, out int total)
        {
            List<Entities.User> lst = new List<Entities.User>();
            if (key == null) key = "";
            total = 0;
            var query = Context.Users.Select(x=>x.Id).ToList();
            if (regionId > 0)
            {
                query = query.Join(Context.UserInRegions, x => x, uir => uir.UserId, (x, uir) => new { Id = x, rId = uir.RegionId })
                    .Where(q => q.rId == regionId).Select(x=>x.Id).ToList();
            }

            if (roleId > 0)
            {
                query = query.Join(Context.UserInRoles, x => x, uir => uir.UserId, (x, uir) => new { Id = x, rId = uir.RolesId })
                    .Where(q => q.rId == roleId).Select(x => x.Id).ToList();
            }

            if(query.Count>0){
                total = (from u in Context.Users
                         where query.Contains(u.Id)
                         && u.UserName.Contains(key)
                         && u.Delete == false
                         select u.Id).Count();
                lst = (from u in Context.Users
                       where query.Contains(u.Id)
                       && u.UserName.Contains(key)
                       && u.Delete == false
                       select new Entities.User()
                       {
                           Id = u.Id,
                           DisplayName = u.DisplayName,
                           FirstName = u.FirstName,
                           LastName = u.LastName,
                           Gender = u.Gender,
                           DateOfBirth = u.DateOfBirth.ToString(),
                           UserName = u.UserName,
                           Email = u.Email,
                           Password = u.Password,
                           Status = u.Status,
                           Delete = u.Delete,
                           DateCreate = u.DateCreate,
                           DateUpdate = u.DateUpdate,
                           DateLogin = u.DateLogin,
                           PlaceId = u.PlaceId,
                           Address = u.Address,
                           Zipcode = u.Zipcode,
                           CompanyLogo = u.CompanyLogo,
                           CompanyName = u.CompanyName,
                           Phone = u.Phone,
                           Avatar = u.Avatar,
                           UserRoles = ListRoleByUserId(u.Id)
                       }).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }            

            return lst;
        }

        /// <summary>
        /// Kiểm tra sự tồn tại của username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool CheckExistsUsername(string username)
        {
            try
            {
                var user = Context.Users.Where(u=>u.Delete == false).FirstOrDefault(u => u.UserName == username);
                if (user != null) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra sự tồn tại của email
        /// </summary>
        /// <param name="email">email cần kiểm tra</param>
        /// <returns>true if exists, false if other</returns>
        public bool CheckExistsEmail(string email)
        {
            try
            {
                var user = Context.Users.Where(u=>u.Delete == false).FirstOrDefault(u => u.Email == email);
                if(user!=null) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckIsUpdatedInfo(string username)
        {
            try
            {
                var user = Context.Users.Where(u=>u.Delete == false).FirstOrDefault(u => u.UserName == username && u.IsUpdated ==true);
                if (user != null) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        
        public List<Entities.Item> ListRoleByUserId(int uId)
        {
            var result = (from u in Context.Users
                          join uir in Context.UserInRoles on u.Id equals uir.UserId
                          join r in Context.Roles on uir.RolesId equals r.Id
                          where u.Id == uId && uir.State == true                          
                          && u.Delete == false
                          select new Entities.Item()
                          {
                              Id = r.Id,
                              Text = r.Text
                          }).ToList();
            return result;
        }

        /// <summary>
        /// Get role by user id
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        public int GetRole(int uId)
        {
            int roleId = (from r in Context.UserInRoles
                          where r.UserId == uId
                          select r.Id).FirstOrDefault();
            return roleId;
        }

        /// <summary>
        /// Get List User Item by Role ID
        /// </summary>
        /// <param name="roleId">ID of role</param>
        /// <returns> List user item</returns>
        public List<Entities.UserItem> ListUserItemByRoleId(int roleId)
        {
            if (roleId < 0)
            {
                return List().Select(x => new Entities.UserItem()
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Email = x.Email,
                    Avatar = x.Avatar
                }).ToList();
            }

            var result = (from r in Context.Roles
                          join uir in Context.UserInRoles on r.Id equals uir.RolesId
                          join u in Context.Users on uir.UserId equals u.Id
                          where r.Id == roleId
                          && u.Delete == false
                          select new Entities.UserItem()
                          {
                              Id = u.Id,
                              UserName = u.UserName,
                              Email = u.Email,
                              Avatar =u.Avatar
                          }).ToList();
            return result;
        }

        public List<Entities.UserItem> ListUserItemByRegion(int regionId)
        {
            if (regionId < 0)
            {
                return new List<UserItem>();
            }
            var result = (from r in Context.Regions
                          join uir in Context.UserInRegions on r.Id equals uir.RegionId
                          join u in Context.Users on uir.UserId equals u.Id
                          where r.Id == regionId
                          && u.Delete == false
                          select new Entities.UserItem()
                          {
                              Id = u.Id,
                              UserName = u.UserName,
                              Email = u.Email,
                              Avatar = u.Avatar
                          }).ToList();
            return result;
        }

        public List<Entities.Item> ListItemByRoleId(int roleId)
        {
            var result = (from r in Context.Roles
                          join uir in Context.UserInRoles on r.Id equals uir.RolesId
                          join u in Context.Users on uir.UserId equals u.Id
                          where r.Id == roleId
                          && u.Status == true
                          && uir.State == true
                          && u.Delete == false
                          select new Entities.Item()
                          {
                              Id = u.Id,
                              Text = u.UserName
                          }).ToList();
            return result;
        }

        /// <summary>
        /// Lấy toàn bộ user đang hoạt động
        /// </summary>
        /// <returns></returns>
        public List<Entities.Item> ListItem()
        {
            var result = (from u in Context.Users
                          where !u.Delete && u.Status
                          select new Entities.Item()
                          {
                              Id = u.Id,
                              Text = u.UserName
                          }).ToList();
            return result;
        }

        public List<Entities.Item> ListItem(List<int> userIds)
        {
            var result = (from u in Context.Users
                          where userIds.Contains(u.Id)
                          && u.Delete == false
                          select new Entities.Item()
                          {
                              Id = u.Id,
                              Text = u.UserName
                          }).ToList();
            return result;
        }

        public List<Entities.Item> ListItem(int regionId, int roleId, string key, int page, int pageSize, out int total)
        {
            List<Entities.Item> lst = new List<Entities.Item>();
            if (key == null) key = "";
            total = 0;
            var query = Context.Users.Select(x => x.Id).ToList();
            if (regionId > 0)
            {
                query = query.Join(Context.UserInRegions, x => x, uir => uir.UserId, (x, uir) => new { Id = x, rId = uir.RegionId })
                    .Where(q => q.rId == regionId).Select(x => x.Id).ToList();
            }

            if (roleId > 0)
            {
                query = query.Join(Context.UserInRoles, x => x, uir => uir.UserId, (x, uir) => new { Id = x, rId = uir.RolesId })
                    .Where(q => q.rId == roleId).Select(x => x.Id).ToList();
            }

            if (query.Count > 0)
            {
                total = (from u in Context.Users
                         where query.Contains(u.Id)
                         && u.UserName.Contains(key)
                         && u.Delete == false
                         select u.Id).Count();
                lst = (from u in Context.Users
                       where query.Contains(u.Id)
                       && u.UserName.Contains(key)
                       && u.Delete == false
                       select new Entities.Item()
                       {
                           Text = u.UserName,
                           Id = u.Id
                       }).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }

            return lst;
        }

        public List<Entities.Item> ListAgency()
        {
            var result = (from u in Context.Users
                          join uRole in Context.UserInRoles on u.Id equals uRole.UserId
                          join r in Context.Roles on uRole.RolesId equals r.Id
                          where 1==1
                          && u.Status == true
                          && uRole.State == true
                          && r.IsAgency == true
                          && u.Delete == false
                          select new Entities.Item()
                          {
                              Id = u.Id,
                              Text = u.UserName
                          }).ToList();
            return result;
        }

        public List<Entities.Item> ListAgencyByRegion(int regionId, int roleId)
        {
            var result = (from u in Context.Users
                          join uRole in Context.UserInRoles on u.Id equals uRole.UserId
                          join uRegion in Context.UserInRegions on uRole.UserId equals uRegion.UserId
                          where uRegion.RegionId == regionId 
                          && uRole.RolesId == roleId
                          && u.Status == true
                          && uRegion.Status ==true
                          && uRole.State == true
                          && u.Delete == false
                          select new Entities.Item()
                          {
                              Id = u.Id,
                              Text = u.UserName
                          }).ToList();
            return result;
        }

        public List<Entities.Item> ListAgencyByRegion(int regionId)
        {
            var result = (from u in Context.Users
                          join uRegion in Context.UserInRegions on u.Id equals uRegion.UserId
                          where uRegion.RegionId == regionId
                          && u.Status == true
                          && uRegion.Status == true
                          && u.Delete == false
                          select new Entities.Item()
                          {
                              Id = u.Id,
                              Text = u.UserName
                          }).ToList();
            return result;
        }

        #region Account
        public int CheckLogin(string username_email, string password, out string username)
        {
            DataLayer.User user = null;
            password = EncryptionHelper.Encrypt(password);
            username = "";
            if (MailHelper.IsEmail(username_email))
            {
                user = (from u in Context.Users
                        where u.Email == username_email && u.Password == password
                            && u.Delete == false
                            select u).FirstOrDefault();
            }
            else
            {
                user = (from u in Context.Users
                        where u.UserName == username_email && u.Password == password
                        && u.Delete == false
                        select u).FirstOrDefault();
            }
            if (user != null)
            {
                if (user.Status)
                {
                    username = user.UserName;
                    return user.Id;
                }
                else
                {
                    return (int)Enums.Errors.BLOCK;
                }
            }
            else
            {
                return (int)Enums.Errors.NOT_EXIST;
            }
        }

        public Entities.User GetByEmail(string email)
        {
            var user = (from u in Context.Users
                        where u.Email == email
                        && u.Delete == false
                        select new Entities.User()
                        {
                            Id = u.Id,
                            Email = u.Email,
                            UserName = u.UserName,
                            CodeActive = u.CodeActive
                        }).FirstOrDefault();
            return user;
        }
        #endregion

        #region Registry
        public int RegistryMember(Entities.User e)
        {
            try
            {
                using(MyDBDataContext db = new MyDBDataContext())
                {
                    int newUserId = InsertPersonalInformation(db, e);

                    Entities.UserInRole uir = new Entities.UserInRole();
                    uir.RolesId = 8;
                    uir.UserId = newUserId;
                    uir.State = false;
                    int memberRoleId = InsertMemberRole(db, uir);
                    if (memberRoleId <= 0)
                    {
                        Delete(newUserId);
                        return memberRoleId;
                    }

                    return newUserId;
                }
            }
            catch (Exception ex)
            {
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
        }
        public int InsertPersonalInformation(MyDBDataContext db, Entities.User e)
        {
            try
            {
                DataLayer.User user = new DataLayer.User();
                user.DisplayName = e.DisplayName;
                user.FirstName = e.FirstName;
                user.LastName = e.LastName;
                user.Gender = e.Gender;
                user.DateOfBirth = DateTime.ParseExact(e.DateOfBirth, "dd/MM/yyyy", null);
                user.UserName = e.UserName;
                user.Email = e.Email;
                user.Password = e.Password;
                user.Status = e.Status;
                user.Description = e.Description;
                user.Delete = e.Delete;
                user.DateCreate = DateTime.Now;
                user.DateUpdate = DateTime.Now;
                user.DateLogin = DateTime.Now;
                user.PlaceId = e.PlaceId;
                user.Address = e.Address;
                user.Zipcode = e.Zipcode;
                user.CompanyLogo = e.CompanyLogo;
                user.CodeActive = e.CodeActive;
                user.CompanyName = e.CompanyName;
                user.Phone = e.Phone;
                user.Avatar = e.Avatar;
                user.IsUpdated = false;
                db.Users.InsertOnSubmit(user);
                db.SubmitChanges();
                return user.Id;
            }
            catch (Exception ex)
            {
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
        }

        public int InsertMemberRole(MyDBDataContext db, Entities.UserInRole e)
        {
            try
            {
                DataLayer.UserInRole u = new DataLayer.UserInRole();
                u.RolesId = e.RolesId;
                u.UserId = e.UserId;
                u.State = e.State;
                db.UserInRoles.InsertOnSubmit(u);
                db.SubmitChanges();
                return u.Id;
            }
            catch (Exception ex)
            {
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
        }

        public int DoConfirmRegisMember(Entities.User e)
        {   
            try
            {
                using (MyDBDataContext db = new MyDBDataContext())
                {
                    DataLayer.User user = (from u in db.Users
                                           where u.Id == e.Id
                                           select u).FirstOrDefault();

                    if (user == null)
                    {
                        return (int)Enums.Errors.NOT_EXIST;
                    }
                    user.Status = e.Status;
                    
                    db.UserInRoles
                    .Where(u=>u.UserId == e.Id)
                    .ToList()
                    .ForEach(u => u.State = true);
                    db.SubmitChanges();
                }
                return e.Id;
            }
            catch (Exception ex)
            {
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.UPDATE_ERROR;
            }

        }

        #endregion

        public int DeleteMultiple(int[] usersDelete)
        {
            try
            {
                using (MyDBDataContext db = new MyDBDataContext())
                {
                    db.Users
                    .Where(u => usersDelete.Contains(u.Id))
                    .ToList()
                    .ForEach(u => { u.Delete = true; u.DateUpdate = DateTime.Now; });
                    db.SubmitChanges();
                }
                return 1;
            }
            catch (Exception ex)
            {
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.DELETE_ERROR;
            }
        }

        /// <summary>
        /// Lấy danh sách email theo user id.
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public string ListEmail(int[] userIds)
        {
            if (userIds == null || userIds.Length == 0) return "";
            var result = (from u in Context.Users
                          where userIds.Contains(u.Id)
                          select u.Email).ToList();
            if (result.Count > 0)
            {
                var strEmails = string.Join(",", result);
                return strEmails;
            }
            return "";
        }
    }
}
