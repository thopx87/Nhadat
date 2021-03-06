using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using BusinessLayer.Helpers;

namespace BusinessLayer
{
    public class RegionService: BaseService<Region>
    {
        public RegionService() : base() { }
        private string className { get { return this.GetType().Name; } }
        
        public Entities.Region GetById(int id)
        {
            var e = (from p in Context.Regions
                     where p.Id == id
                     select new Entities.Region
                     {
                         Id = p.Id,
                         Text = p.Text,
                         CityId = p.CityId,
                         DistrictId = p.DistrictId,
                         Status = p.Status,
                         NeighborId = p.NeighborId,
                         Neighbor = p.NeighborId != null ? GetRegionItem((int)p.NeighborId) : new Item()
                     }).FirstOrDefault();
            return e != null ? e : new Entities.Region();
        }

        /// <summary>
        /// Lấy vùng theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.Item GetRegionItem(int? id)
        {
            if (id == null) return new Item();

            var e = (from p in Context.Regions
                     where p.Id == id && p.Status == true
                     select new Entities.Item
                     {
                         Id = p.Id,
                         Text = p.Text,
                     }).FirstOrDefault();
            return e != null ? e : new Entities.Item();
        }

        /// <summary>
        /// Lấy vùng theo id xã, phường
        /// </summary>
        /// <param name="wardId"></param>
        /// <returns></returns>
        public Entities.Item GetRegionByWard(int wardId)
        {
            var result = (from r in Context.Regions
                          join p in Context.Places on r.Id equals p.RegionId
                          where p.Id == wardId && r.Status == true
                          select new Entities.Item()
                          {
                              Id = r.Id,
                              Text = r.Text
                          }).FirstOrDefault();
            return result;
        }

        public int Save(Entities.Region e)
        {
            DataLayer.Region region = (from r in Context.Regions
                                       where r.Id == e.Id
                                       select r).FirstOrDefault();
            bool isNew = false;
            if (region == null)
            {
                region = new DataLayer.Region();
                isNew = true;
            }
            region.Text = e.Text;
            region.CityId = e.CityId;
            region.DistrictId = e.DistrictId;
            region.Status = e.Status;
            region.NeighborId = e.NeighborId;
            try
            {
                if (isNew)
                {
                    Context.Regions.InsertOnSubmit(region);
                }
                Context.SubmitChanges();
                return region.Id;
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.EXIST;
            }
        }

        public int Insert(Entities.Region e)
        {
            DataLayer.Region p = new DataLayer.Region();
            p.Text = e.Text;
            p.CityId = e.CityId;
            p.DistrictId = e.DistrictId;
            p.Status = e.Status;
            p.NeighborId = e.NeighborId;
            try
            {
                Context.Regions.InsertOnSubmit(p);
                Context.SubmitChanges();
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
            return p.Id;
        }

        public int Update(Entities.Region e)
        {
            DataLayer.Region region = (from p in Context.Regions
                                       where p.Id == e.Id
                                       select p).FirstOrDefault();
            if (region == null)
            {
                return (int)Enums.Errors.NOT_EXIST;
            }
            region.Text = e.Text;
            region.CityId = e.CityId;
            region.DistrictId = e.DistrictId;
            region.Status = e.Status;
            region.NeighborId = e.NeighborId;
            try
            {
                Context.SubmitChanges();
                return region.Id;
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.EXIST;
            }
        }

        public int Delete(int id)
        {
            try
            {
                DataLayer.Region region = (from u in Context.Regions
                                           where u.Id == id
                                           select u).FirstOrDefault();
                if (region != null)
                {
                    Context.Regions.DeleteOnSubmit(region);
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

        public List<Entities.Region> List(int pageIndex, int pageSize, out int total)
        {
            List<Entities.Region> lst = new List<Entities.Region>();
            total = (from r in Context.Regions
                     select r.Id).Count();
            lst = (from r in Context.Regions
                   select new Entities.Region
                   {
                       Id = r.Id,
                       Text = r.Text,
                       CityId = r.CityId,
                       DistrictId = r.DistrictId,
                       Status = r.Status,
                       NeighborId = r.Status==true? r.NeighborId:null,
                       Neighbor = GetRegionItem(r.NeighborId),
                       ListWard = ListWardInRegion(r.Id),
                       ListUser = ListUserItemByRegionId(r.Id),
                       MaxAgency = CountAgency(r.Id)
                   }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return lst;
        }
        
        public List<Entities.Region> List(string text, int pageIndex, int pageSize, out int total)
        {
            List<Entities.Region> lst = new List<Entities.Region>();
            total = (from u in Context.Regions
                     where u.Text.Contains(text)
                     select u.Id).Count();
            lst = (from r in Context.Regions
                   where r.Text.Contains(text)
                   select new Entities.Region
                   {
                       Id = r.Id,
                       Text = r.Text,
                       CityId = r.CityId,
                       DistrictId = r.DistrictId,
                       Status = r.Status,
                       NeighborId = r.Status == true ? r.NeighborId : null,
                       Neighbor = GetRegionItem(r.NeighborId),
                       ListWard = ListWardInRegion(r.Id),
                       ListUser = ListUserItemByRegionId(r.Id),
                       MaxAgency = CountAgency(r.Id)
                   }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return lst;
        }

        public List<Entities.Region> List(int cityId, int districtId, string textSearch, int pageIndex, int pageSize, out int total)
        {
            if (string.IsNullOrEmpty(textSearch))
            {
                textSearch = "";
            }
            List<Entities.Region> lst = new List<Entities.Region>();
            total = (from r in Context.Regions
                     where r.Text.Contains(textSearch)
                     && (cityId>0? r.CityId == cityId: true)
                     && (districtId>0?r.DistrictId == districtId: true)
                     select r.Id).Count();
            lst = (from r in Context.Regions
                   where r.Text.Contains(textSearch)
                     && (cityId > 0 ? r.CityId == cityId : true)
                     && (districtId > 0 ? r.DistrictId == districtId : true)
                   select new Entities.Region
                   {
                       Id = r.Id,
                       Text = r.Text,
                       CityId = r.CityId,
                       DistrictId = r.DistrictId,
                       Status = r.Status,
                       NeighborId = r.Status == true ? r.NeighborId : null,
                       Neighbor = GetRegionItem(r.NeighborId),
                       ListWard = ListWardInRegion(r.Id),
                       ListUser = ListUserItemByRegionId(r.Id),
                       MaxAgency = CountAgency(r.Id)
                   }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return lst;
        }

        public List<Entities.Region> List()
        {
            List<Entities.Region> lst = new List<Entities.Region>();
            lst = (from r in Context.Regions
                   select new Entities.Region
                   {
                       Id = r.Id,
                       Text = r.Text,
                       CityId = r.CityId,
                       DistrictId = r.DistrictId,
                       Status = r.Status,
                       NeighborId = r.Status == true ? r.NeighborId : null,
                       Neighbor = GetRegionItem(r.NeighborId),
                       ListWard = ListWardInRegion(r.Id),
                       ListUser = ListUserItemByRegionId(r.Id),
                       MaxAgency = CountAgency(r.Id)
                   }).ToList();
            return lst;
        }

        private int CountAgency(int regionId)
        {
            var result = (from p in Context.Places
                          where p.RegionId == regionId
                          select p.MaxAgency).Sum();
            return result != null ? (int)result : 0;
        }

        public List<Entities.Item> ListItem()
        {
            var lst = (from r in Context.Regions
                       select new Entities.Item
                       {
                           Id = r.Id,
                           Text = r.Text,
                       }).ToList();
            return lst;
        }

        public List<Entities.Item> ListItemActive()
        {
            var lst = (from r in Context.Regions
                       where r.Status == true
                       select new Entities.Item
                       {
                           Id = r.Id,
                           Text = r.Text,
                       }).ToList();
            return lst;
        }

        public List<Entities.Item> ListItemActive(int cityId)
        {
            var lst = (from r in Context.Regions
                       where r.Status == true
                       && r.CityId == cityId
                       select new Entities.Item
                       {
                           Id = r.Id,
                           Text = r.Text,
                       }).ToList();
            return lst;
        }

        public List<Entities.Item> ListItem(int cityId)
        {
            var lst = (from r in Context.Regions
                       where r.CityId == cityId
                       select new Entities.Item
                       {
                           Id = r.Id,
                           Text = r.Text,
                       }).ToList();
            return lst;
        }

        public List<Entities.Item> ListItem(int cityId, int districtId)
        {
            var lst = (from r in Context.Regions
                       where r.CityId == cityId && r.DistrictId == districtId
                       select new Entities.Item
                       {
                           Id = r.Id,
                           Text = r.Text,
                       }).ToList();
            return lst;
        }

        public List<Entities.Region> List(int cityId)
        {
            List<Entities.Region> lst = new List<Entities.Region>();

            lst = (from p in Context.Regions
                   where p.CityId == cityId
                   select new Entities.Region
                   {
                       Id = p.Id,
                       Text = p.Text,
                       CityId = p.CityId,
                       DistrictId = p.DistrictId,
                       Status = p.Status,
                       NeighborId = p.NeighborId
                   }).ToList();
            return lst;
        }

        public List<Entities.Region> List(int cityId, int districtId)
        {
            List<Entities.Region> lst = new List<Entities.Region>();

            lst = (from p in Context.Regions
                   where p.CityId == cityId && p.DistrictId == districtId
                   select new Entities.Region
                   {
                       Id = p.Id,
                       Text = p.Text,
                       CityId = p.CityId,
                       NeighborId = p.NeighborId,
                       DistrictId = p.DistrictId,
                       ListWard = ListWardInRegion(p.Id)
                   }).ToList();
            return lst;
        }

        public bool CheckExistsCode(string code)
        {
            //return List().Any(x => x.Code == code);
            return true;
        }

        public List<Entities.Item> ListWardInRegion(int regionId)
        {
            var result = (from p in Context.Places
                   where p.RegionId == regionId
                          select new Entities.Item
                   {
                       Id = p.Id,
                       Text = p.Text
                   }).ToList();
            return result;
        }
        
        /// <summary>
        /// Lấy danh sách user item theo region ID
        /// Nếu cần lấy thêm trường dữ liệu thì trước tiên bổ sung vào class UserItem.
        /// </summary>
        /// <param name="id">role ID</param>
        /// <returns>danh sách UserItem</returns>
        public List<Entities.UserItem> ListUserItemByRegionId(int id)
        {
            var result = (from r in Context.Regions
                            join uir in Context.UserInRegions on r.Id equals uir.RegionId
                            join u in Context.Users on uir.UserId equals u.Id
                            where r.Id == id
                            select new Entities.UserItem()
                            {
                                Id = u.Id,
                                UserName = u.UserName,
                                Email = u.Email,
                                Avatar = u.Avatar
                            }).ToList();
            return result;
        }

        /// <summary>
        /// Lấy danh sách xã, phường theo region ID
        /// </summary>
        /// <param name="id"> region ID </param>
        /// <returns>danh sách xã, phường</returns>
        public List<Entities.Item> ListWardItemByRegionId(int id)
        {
            var result = (from p in Context.Places
                          where p.RegionId == id
                          select new Entities.Item()
                          {
                              Id = p.Id,
                              Text = p.Text
                          }).ToList();
            return result;
        }

        public int CountMaxAgency(int regionId)
        {
            var result = (from p in Context.Places
                          where p.RegionId == regionId
                          select p.MaxAgency).Sum();
            return result != null ? (int)result : 0;
        }

        /// <summary>
        /// Lấy vùng lân cận.
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public Entities.Item GetNeighBorRegion(int regionId)
        {
            var result = (from r1 in Context.Regions
                          join r2 in Context.Regions on r1.NeighborId equals r2.Id
                          where r1.Id == regionId
                          select new Entities.Item()
                          {
                              Id = r2.Id,
                              Text = r2.Text
                          }).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Lấy Vùng cố định
        /// </summary>
        /// <param name="wardId"></param>
        /// <returns></returns>
        public Entities.Item GetFixedRegion(int wardId)
        {
            var result = (from r in Context.Regions
                          join p in Context.Places on r.Id equals p.RegionId
                          where p.Id == wardId
                          select new Entities.Item()
                          {
                              Id = r.Id,
                              Text = r.Text
                          }).FirstOrDefault();
            return result;
        }

        public Entities.Item GetItem(int id)
        {
            var result = (from r in Context.Regions
                          where r.Id == id
                          select new Entities.Item()
                          {
                              Id = r.Id,
                              Text = r.Text
                          }).FirstOrDefault();
            return result;
        }
    }

}
