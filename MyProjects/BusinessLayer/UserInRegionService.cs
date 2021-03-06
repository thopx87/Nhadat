using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using BusinessLayer.Helpers;

namespace BusinessLayer
{
    public class UserInRegionService: BaseService<Entities.UserInRegion>
    {
        public UserInRegionService() : base() { }
        private string className { get { return this.GetType().Name; } }

        public UserInRegion GetById(int id)
        {
            var result = (from u in Context.UserInRegions
                          where u.Id == id
                          select new UserInRegion()
                          {
                              Id = u.Id,
                              UserId = u.UserId,
                              RegionId = u.RegionId!=null? u.RegionId.Value:0,
                              Status = u.Status,
                              CityId = u.CityId!=null? u.CityId.Value :0,
                              DistrictId = u.DistrictId!=null? u.DistrictId.Value:0,
                              WardId = u.WardId!=null? u.WardId.Value:0
                          }).FirstOrDefault();
            return result;
        }

        // Lấy danh sách vùng của người dùng.
        public List<UserInRegion> GetListByUser(int userId)
        {
            var result = (from u in Context.UserInRegions
                          where u.UserId == userId
                          select new UserInRegion()
                          {
                              Id = u.Id,
                              UserId = u.UserId,
                              RegionId = u.RegionId != null ? u.RegionId.Value : 0,
                              Status = u.Status,
                              CityId = u.CityId != null ? u.CityId.Value : 0,
                              DistrictId = u.DistrictId != null ? u.DistrictId.Value : 0,
                              WardId = u.WardId != null ? u.WardId.Value : 0
                          }).ToList();
            return result;
        }

        // Lấy danh sách vùng của người dùng theo kiểu vùng
        public List<UserInRegion> GetListByUser(int userId, int regionType)
        {
            var result = (from u in Context.UserInRegions
                          where u.UserId == userId
                          && u.RegionType == regionType
                          select new UserInRegion()
                          {
                              Id = u.Id,
                              UserId = u.UserId,
                              RegionId = u.RegionId != null ? u.RegionId.Value : 0,
                              Status = u.Status,
                              CityId = u.CityId != null ? u.CityId.Value : 0,
                              DistrictId = u.DistrictId != null ? u.DistrictId.Value : 0,
                              WardId = u.WardId != null ? u.WardId.Value : 0
                          }).ToList();
            return result;
        }

        public int Save(Entities.UserInRegion e)
        {
            DataLayer.UserInRegion u = (from x in Context.UserInRegions
                                        where x.Id == e.Id
                                        select x).FirstOrDefault();
            bool isNew = false;
            if (u == null)
            {
                u = new DataLayer.UserInRegion();
                isNew = true;
            }

            u.UserId = e.UserId;
            u.RegionId = e.RegionId;
            u.Status = e.Status;
            u.CityId = e.CityId;
            u.DistrictId = e.DistrictId;
            u.WardId = e.WardId;
            u.RegionType = e.RegionType;
            try
            {
                if (isNew)
                {
                    Context.UserInRegions.InsertOnSubmit(u);
                }
                Context.SubmitChanges();
                return u.Id;
            }
            catch (Exception ex)
            {
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.UPDATE_ERROR;
            }
        }

        /// <summary>
        /// Cập nhật lại thông tin Region (thay đổi vùng)
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public int UpdateRegion(Entities.UserInRegion e)
        {
            DataLayer.UserInRegion u = (from x in Context.UserInRegions
                                        where x.Id == e.Id
                                        select x).FirstOrDefault();
            bool isNew = false;
            if (u == null)
            {
                u = new DataLayer.UserInRegion();
                isNew = true;
            }

            u.UserId = e.UserId;
            u.RegionId = e.RegionId;
            u.Status = e.Status;
            u.RegionType = e.RegionType;
            try
            {
                if (isNew)
                {
                    Context.UserInRegions.InsertOnSubmit(u);
                }
                Context.SubmitChanges();
                return u.Id;
            }
            catch (Exception ex)
            {
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.UPDATE_ERROR;
            }
        }

        public int Delete(int id)
        {
            return id;
        }

        /// <summary>
        /// Delete by user and regiontype
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int DeleteByUser(int userId, int type = 1)
        {
            var listUIR = (from x in Context.UserInRegions
                           where x.UserId == userId
                           && x.RegionType == type
                           select x).ToList();
            try
            {
                Context.UserInRegions.DeleteAllOnSubmit(listUIR);
                Context.SubmitChanges();
                return userId;
            }
            catch(Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.DELETE_ERROR;
            }
        }

        /// <summary>
        /// Lấy danh sách vùng theo user
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="type">kiểu vùng
        /// 1 --> vùng gửi
        /// 2 --> vùng nhận
        /// </param>
        /// <returns></returns>
        public List<Item> GetListItemByUser(int userId, int type = 1)
        {
            var result = (from uIR in Context.UserInRegions
                          join r in Context.Regions on uIR.RegionId equals r.Id
                          where uIR.UserId == userId
                          && uIR.RegionType == type
                          select new Entities.Item()
                          {
                              Id = r.Id,
                              Text = r.Text                              
                          }).ToList();
            return result;
        }

        public List<int> ListUserIdByRegion(int regionId)
        {
            var result = (from uIR in Context.UserInRegions
                          where uIR.RegionId == regionId
                          select uIR.UserId).ToList();
            return result;
        }

        public int GetRegionIdByWard(int wardId)
        {
            var result = (from uir in Context.Places
                          where uir.Id == wardId
                          select uir.RegionId).FirstOrDefault();
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Lấy danh sách các môi giới dựa theo xã phường sản phẩm.
        /// 1. Từ Id xã / phường & vùng nhận (regiontype=2) lấy ra được id vùng (RegionId).
        /// 2. Từ vùng lấy ra ở trên tìm các userid. chính là các môi giới.
        /// 3. Từ các user này lấy ra được các email của họ.
        /// </summary>
        /// <param name="wardId"></param>
        /// <returns></returns>
        //public List<string> ListEmailAgencyByProductWard(int wardId)
        //{
        //    var region = (from uir in Context.UserInRegions
        //                  where uir.WardId == wardId
        //                  && uir.RegionType == 2 // Vùng nhận
        //                  select uir.RegionId).FirstOrDefault();
        //    if (region > 0)
        //    {
        //        var result = (from uir in Context.UserInRegions
        //                      join u in Context.Users on uir.UserId equals u.Id
        //                      where uir.RegionId == region
        //                      select u.Email).ToList();
        //        return result;
        //    }
        //    else
        //    {
        //        return new List<string>();
        //    }
        //}
    }
}
