using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using BusinessLayer.Helpers;

namespace BusinessLayer
{
    public class Road_VillageService: BaseService<Entities.Road_Village>
    {
        private string className { get { return this.GetType().Name; } }
        private PlaceService placeService = new PlaceService();
        public Road_VillageService() : base() { }

        public Road_Village GetById(int id)
        {
            var result = (from r in Context.Road_Villages
                          where id == r.Id
                          select new Road_Village()
                          {
                              Id = r.Id,
                              Text = r.Text,
                              Type = r.Type,
                              Description = r.Description,
                              WardId = r.WardId,
                              RegionId = r.RegionId
                          }).FirstOrDefault();
            return result;
        }

        public int Insert(Road_Village e)
        {
            DataLayer.Road_Village r = new DataLayer.Road_Village();
            r.Text = e.Text;
            r.Description = e.Description;
            r.Type = e.Type;
            r.WardId = e.WardId;
            r.RegionId = e.RegionId;

            try
            {
                Context.Road_Villages.InsertOnSubmit(r);
                Context.SubmitChanges();
                return r.Id;
            }
            catch(Exception ex)
            {
                string data = className + " " + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
        }

        public int Update(Road_Village e)
        {
            DataLayer.Road_Village r = (from t in Context.Road_Villages
                                        where t.Id == e.Id
                                        select t).FirstOrDefault();
            if (r != null)
            {
                r.Id = e.Id;
                r.Text = e.Text;
                r.Description = e.Description;
                r.Type = e.Type;
                r.WardId = e.WardId;
                r.RegionId = e.RegionId;
                try
                {
                    Context.SubmitChanges();
                    return r.Id;
                }
                catch (Exception ex)
                {
                    string data = className + " " + ex.Message.ToString();
                    Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                    return (int)Enums.Errors.UPDATE_ERROR;
                }
            }
            else
            {
                string data = className + " Không có thông tin trong database";
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.UPDATE_ERROR;
            }
        }

        public int Delete(int id)
        {
            DataLayer.Road_Village r = (from t in Context.Road_Villages
                                        where t.Id == id
                                        select t).FirstOrDefault();
            if (r != null)
            {
                try
                {
                    Context.Road_Villages.DeleteOnSubmit(r);
                    Context.SubmitChanges();
                    return id;
                }
                catch (Exception ex)
                {
                    string data = className + " " + ex.Message.ToString();
                    Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                    return (int)Enums.Errors.DELETE_ERROR;
                }
            }
            else
            {
                string data = className + " Không có trong database";
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.DELETE_ERROR;
            }
        }

        public List<Road_Village> List()
        {
            var result = (from road in Context.Road_Villages
                          select new Entities.Road_Village()
                          {
                              Id = road.Id,
                              Text = road.Text,
                              Description = road.Description,
                              Type = road.Type,
                              WardId = road.WardId,
                              RegionId = road.RegionId
                          }).ToList();
            if (result != null)
            {
                RegionService regionService = new RegionService();
                foreach (var r in result)
                {
                    r.Ward = placeService.GetPlaceItem(r.WardId);
                    r.District = placeService.GetParentItem(r.WardId);
                    r.City = placeService.GetParentItem(r.District.Id);
                    r.Region = regionService.GetRegionItem(r.RegionId);
                }
            }
            return result;
        }

        public List<Road_Village> List(string text = null, bool type = true, int wardId = -1, int regionId = -1)
        {
            var result = (from road in Context.Road_Villages
                          where 1 == 1
                          && (string.IsNullOrEmpty(text)||road.Text == text)
                          && (wardId == -1 || road.WardId ==wardId)
                          && (regionId == -1 || regionId == road.RegionId)
                          select new Entities.Road_Village()
                          {
                              Id = road.Id,
                              Text = road.Text,
                              Description = road.Description,
                              Type = road.Type,
                              WardId = road.WardId,
                              RegionId = road.RegionId
                          }).ToList();
            return result;
        }

        public List<Item> ListItem(int wardId = -1, int regionId = -1)
        {
            var result = (from r in Context.Road_Villages
                          where (wardId == -1 || r.WardId == wardId)
                          && (regionId == -1 || r.RegionId == regionId)
                          select new Item()
                          {
                              Id = r.Id,
                              Text = r.Text
                          }).ToList();
            return result;
        }
    }
}
