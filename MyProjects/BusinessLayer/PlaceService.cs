using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using BusinessLayer.Helpers;

namespace BusinessLayer
{
    public class PlaceService: BaseService<Entities.Place>
    {
        public PlaceService() : base() { }

        private string className { get { return this.GetType().Name; } }
        
        public Entities.Place GetById(int id)
        {
            var e = (from p in Context.Places
                     where p.Id == id
                     select new Entities.Place
                     {
                         Id = p.Id,
                         Text = p.Text,
                         Type = p.Type,
                         Parent =p.Parent,
                         Address = p.Address,
                         Description = p.Description,
                         Longitude = p.Longitude,
                         Latitude = p.Latitude,
                         RegionId = p.RegionId,
                         MaxAgency = p.MaxAgency,
                         MaxAgencyFree = p.MaxAgencyFree
                     }).FirstOrDefault();
            return e;
        }

        public int Insert(Entities.Place e)
        {
            DataLayer.Place place = new DataLayer.Place();
            place.Text = e.Text;
            place.Parent = e.Parent;
            place.Type = e.Type;
            place.Address =e.Address;
            place.Description = e.Description;
            place.Longitude = e.Longitude;
            place.Latitude = e.Latitude;
            place.MaxAgency = e.MaxAgency;
            place.MaxAgencyFree = e.MaxAgencyFree;
            try
            {
                Context.Places.InsertOnSubmit(place);
                Context.SubmitChanges();
            }
            catch
            {
                return -1;
            }
            return place.Id;
        }

        public int Update(Entities.Place e)
        {
            DataLayer.Place place = (from p in Context.Places
                                 where p.Id == e.Id
                                     select p).FirstOrDefault();
            if (place == null)
            {
                return (int)Enums.Errors.NOT_EXIST;
            }
            place.Text = e.Text;
            place.Parent = e.Parent;
            place.Type = e.Type;
            place.Address = e.Address;
            place.Description = e.Description;
            place.Longitude = e.Longitude;
            place.Latitude = e.Latitude;
            place.RegionId = e.RegionId;
            place.MaxAgency = e.MaxAgency;
            place.MaxAgencyFree = e.MaxAgencyFree;
            try
            {
                Context.SubmitChanges();
                return place.Id;
            }
            catch(Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.EXIST;
            }
        }

        public int UpdateRegion(int id, int regionId)
        {
            DataLayer.Place place = (from p in Context.Places
                                     where p.Id == id
                                     select p).FirstOrDefault();
            if (place == null)
            {
                return (int)Enums.Errors.NOT_EXIST;
            }
            place.RegionId = regionId;
            try
            {
                Context.SubmitChanges();
                return place.Id;
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
                DataLayer.Place place = (from p in Context.Places
                                       where p.Id == id
                                       select p).FirstOrDefault();
                if (place != null)
                {
                    List<DataLayer.Place> lstChild = (from u in Context.Places
                                                      where u.Parent == place.Id
                                                      select u).ToList();
                    if (lstChild != null)
                    {
                        Delete(lstChild.First().Id);
                    }

                    Context.Places.DeleteOnSubmit(place);
                    Context.SubmitChanges();
                }

                return id;
            }
            catch
            {
                return (int)Enums.Errors.DELETE_ERROR;
            }
        }

        public bool Delete(List<int> lstId)
        {
            try
            {
                var tempLst = (from p in Context.Places
                               where lstId.Contains(p.Id)
                               select p).ToList();
                Context.Places.DeleteAllOnSubmit(tempLst);
                Context.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Entities.Place> List()
        {
            List<Entities.Place> lst = new List<Entities.Place>();
            lst = (from p in Context.Places
                   select new Entities.Place
                   {
                       Id = p.Id,
                       Text = p.Text,
                       Parent = p.Parent,
                       Type = p.Type,
                       Address = p.Address,
                       Description = p.Description,
                       Longitude = p.Longitude,
                       Latitude = p.Latitude,
                       RegionId = p.RegionId
                   }).ToList();
            return lst;
        }

        /// <summary>
        /// Lấy danh sách Place theo id cha. ví dụ parentId = id tỉnh --> danh sách huyện
        /// </summary>
        /// <param name="parentId">id cha (cấp trên)</param>
        /// <returns>danh sách place con</returns>
        public List<Entities.Place> List(int parentId)
        {
            List<Entities.Place> lst = new List<Entities.Place>();
            lst = (from p in Context.Places
                   where p.Parent == parentId
                   select new Entities.Place
                   {
                       Id = p.Id,
                       Text = p.Text,
                       Parent = p.Parent,
                       Type = p.Type,
                       Address = p.Address,
                       Description = p.Description,
                       Longitude = p.Longitude,
                       Latitude = p.Latitude,
                       RegionId = p.RegionId
                   }).ToList();
            return lst;
        }

        public List<Entities.Place> List(List<int> lstId)
        {
            List<Entities.Place> lst = new List<Place>();
            foreach (int id in lstId)
            {
                lst.Add(GetById(id));
            }
            return lst;
        }

        public void TreeListId(int parentId, ref List<int> lst)
        {
            lst.Add(parentId);
            var tempLstId = (from p in Context.Places
                           where p.Parent == parentId
                           select p.Id).ToList();
            if (tempLstId != null)
            {
                foreach (int pId in tempLstId as List<int>)
                {
                    if (pId != parentId)
                        TreeListId(pId, ref lst);
                }
            }            
        }

        /// <summary>
        /// Lấy danh sách tỉnh
        /// </summary>
        /// <param name="key">từ khóa tìm kiếm</param>
        /// <param name="pageIndex">trang</param>
        /// <param name="pageSize">số bản ghi trên 1 trang</param>
        /// <param name="total">tổng số bản ghi</param>
        /// <returns></returns>
        public List<Entities.Place> ListCity(string key, int pageIndex, int pageSize, out int total)
        {
            List<Entities.Place> lst = new List<Entities.Place>();
            total = 0;
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;

            if (key == null || key == "")
            {
                total = (from p in Context.Places
                         where p.Parent == 0
                         select p.Id).Count();
                lst = (from p in Context.Places
                       where p.Parent == 0
                       select new Entities.Place
                       {
                           Id = p.Id,
                           Text = p.Text,
                           Parent = p.Parent,
                           Type = p.Type,
                           Address = p.Address,
                           Description = p.Description,
                           ListChild = List(p.Id),
                           RegionId = p.RegionId,
                           CountRegion = CountRegionInCity(p.Id),
                           CountAgency = CountAgency(p.Id, 1)
                       }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                
            }

            return lst;
        }

        /// <summary>
        /// Đếm số lượng vùng trong tỉnh.
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public int CountRegionInCity(int cityId)
        {
            var result = (from r in Context.Regions
                          where r.CityId == cityId
                          select r).Count();
            return result;
        }

        /// <summary>
        /// Đếm số môi giới
        /// </summary>
        /// <param name="placeId">id địa lý</param>
        /// <param name="type">
        /// kiểu đếm.
        /// 1 --> đếm theo tỉnh
        /// 2 --> đếm theo huyện
        /// </param>
        /// <returns></returns>
        public int CountAgency(int placeId, int type)
        {
            int? result = 0;
            if (type == 1)
            {
                result = (from c in Context.Places
                          join d in Context.Places on c.Id equals d.Parent
                          join w in Context.Places on d.Id equals w.Parent
                          where c.Id == placeId
                          select w.MaxAgency).Sum();
            }
            else if (type == 2)
            {
                result = (from d in Context.Places
                          join w in Context.Places on d.Id equals w.Parent
                          where d.Id == placeId
                          select w.MaxAgency).Sum();
            }
            
            return result != null ? (int)result : 0;

        }

        /// <summary>
        /// Đếm số lượng vùng trong huyện
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        public int CountRegionInDistrict(int districtId)
        {
            var result = (from r in Context.Regions
                          where r.DistrictId == districtId
                          select r).Count();
            return result;
        }

        /// <summary>
        /// Lấy danh sách xã/ phường của vùng.
        /// </summary>
        /// <param name="regionId">Id vùng</param>
        /// <returns>danh sách xã, phường</returns>
        public List<Entities.Place> ListWardOfRegion(int regionId)
        {
            List<Entities.Place> lst = new List<Entities.Place>();
            lst = (from p in Context.Places
                   where p.RegionId == regionId
                   select new Entities.Place
                   {
                       Id = p.Id,
                       Text = p.Text,
                       Parent = p.Parent,
                       Type = p.Type
                   }).ToList();
            return lst;
        }

        /// <summary>
        /// Danh sách xã phường trong 1 vùng
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public List<Entities.Place> ListWardInRegion(int regionId)
        {
            List<Entities.Place> lst = new List<Entities.Place>();
            lst = (from p in Context.Places
                   where p.RegionId == regionId
                   select new Entities.Place
                   {
                       Id = p.Id,
                       Text = p.Text,
                       Parent = p.Parent,
                       Type = p.Type,
                       Address = p.Address,
                       Description = p.Description,
                       Longitude = p.Longitude,
                       Latitude = p.Latitude,
                       RegionId = p.RegionId,
                       MaxAgency = p.MaxAgency,
                       MaxAgencyFree = p.MaxAgencyFree
                   }).ToList();
            return lst;
        }

        /// <summary>
        /// Get List place Item by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<Entities.Item> ListPlaceItemByIds(List<int> ids)
        {
            if (ids != null)
            {
                List<Entities.Item> listItem = new List<Item>();
                Entities.Place place;
                foreach (int id in ids)
                {
                    place = GetById(id);
                    listItem.Add(new Entities.Item(){Id = place.Id, Text = place.Text});
                }
                return listItem;
            }
            else
            {
                return new List<Entities.Item>();
            }
        }

        /// <summary>
        /// Lấy danh sách placeitem theo parent id
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<Entities.Item> ListPlaceItemByParent(int parentId)
        {
            if (parentId < 0)
            {
                return new List<Entities.Item>();
            }
            List<Entities.Place> lstPlace = List(parentId);
            var result = lstPlace.Select(x => new Entities.Item()
            {
                Id = x.Id,
                Text = x.Text
            }).ToList();
            return result;
        }

        /// <summary>
        /// Lấy danh sách place item (xã) của vùng
        /// </summary>
        /// <param name="id">id vùng</param>
        /// <returns>danh sách xã, phường</returns>
        public List<Entities.Item> ListPlaceItemOfRegion(int id)
        {
            if (id <= 0)
            {
                return new List<Entities.Item>();
            }

            List<Entities.Place> lstPlace = ListWardOfRegion(id);
            return lstPlace.Select(x => new Entities.Item()
            {
                Id = x.Id,
                Text = x.Text
            }).ToList();
        }

        /// <summary>
        /// Get thông tin cơ bản của cha dựa vào id của con.
        /// </summary>
        /// <param name="childId">id con</param>
        /// <returns>thông tin cơ bản của cha (id, text)</returns>
        public Entities.Item GetParentItem(int childId)
        {
            var result = (from p1 in Context.Places
                          join p2 in Context.Places on p1.Id equals p2.Parent
                          where p2.Id == childId
                          select new Entities.Item()
                          {
                              Id = p1.Id,
                              Text = p1.Text
                          }).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Lấy thông tin place Item (chỉ có ID và text)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.Item GetPlaceItem(int id)
        {
            var result = (from p in Context.Places
                          where p.Id == id
                          select new Entities.Item()
                          {
                              Id = p.Id,
                              Text = p.Text
                          }).FirstOrDefault();
            return result;
        }

        public List<Entities.Item> AllPlace()
        {
            var result = (from p in Context.Places
                          select new Entities.Item()
                          {
                              Id = p.Id,
                              Text = p.Text
                          }).ToList();
            return result;
        }
    }
}
