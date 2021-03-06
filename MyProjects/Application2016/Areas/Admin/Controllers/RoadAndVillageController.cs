using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using BusinessLayer.Helpers;
using Application2016.Helpers;

namespace Application2016.Areas.Admin.Controllers
{
    public class RoadAndVillageController : BaseController
    {
        Road_VillageService roadService = new Road_VillageService();
        //
        // GET: /Admin/RoadAndVillage/

        public ActionResult Index()
        {
            LoadData();
            return View();
        }
        public void LoadData()
        {
            // Get list road
            var lstRoad = roadService.List();
            ViewBag.ListRoadAndVillage = lstRoad;
        }

        [HttpGet]
        public PartialViewResult Update(int id = 0, int cityId = -1, int districtId = -1, int wardId =-1, int regionId = -1)
        {
            PlaceService placeService = new PlaceService();
            RegionService regionService = new RegionService();
            Models.Road_VillageModel m = new Models.Road_VillageModel();
            if (id > 0)
            {
                var road = roadService.GetById(id);
                m.MapFrom(road, ref m);
                m.DistrictId = placeService.GetParentItem(m.WardId).Id;
                m.CityId = placeService.GetParentItem(m.DistrictId).Id;
                cityId = m.CityId;
                districtId = m.DistrictId;
                wardId = m.WardId;

            }            

            m.ListCity = placeService.ListPlaceItemByParent(0);
            m.ListDistrict = placeService.ListPlaceItemByParent(cityId);
            m.ListWard = placeService.ListPlaceItemByParent(districtId);
            m.ListRegion = regionService.List();

            LoadData();

            ViewBag.ListRoad = roadService.ListItem(wardId, regionId);
            ViewBag.ActionForm = "Update";
            return PartialView("index", m);
        }

        [HttpPost]
        public ActionResult Update(int id, Models.Road_VillageModel m)
        {
            if (ModelState.IsValid)
            {
                Entities.Road_Village e = new Entities.Road_Village();
                m.MapFrom(m, ref e);
                int result = 0;
                if (id > 0)
                {
                    result = roadService.Update(e);
                }
                else
                {
                    result = roadService.Insert(e);
                }
                if (result <= 0)
                {
                    ViewBag.Message = AdminConfigs.MESSAGE_UPDATE_ERROR;
                    ViewBag.AlertClass = AdminConfigs.CLASS_ALERT_DANGER;
                    return PartialView("index", m);
                }
                else
                {
                    ViewBag.AlertClass = AdminConfigs.CLASS_ALERT_SUCCESS;
                    ViewBag.Message = AdminConfigs.MESSAGE_UPDATE_SUCCESS;
                    ModelState.Clear();

                    //
                    // Xóa đi những thông tin vừa được cập nhật tránh trường hợp click 2 lần.                    
                    m.Text = "";
                    m.Description = "";
                }
            }
            else
            {

            }

            PlaceService placeService = new PlaceService();
            RegionService regionService = new RegionService();

            m.ListCity = placeService.ListPlaceItemByParent(0);
            m.ListDistrict = placeService.ListPlaceItemByParent(m.CityId);
            m.ListWard = placeService.ListPlaceItemByParent(m.DistrictId);
            m.ListRegion = regionService.List();
            LoadData();
            ViewBag.ListRoad = roadService.ListItem(m.WardId, m.RegionId);
            ViewBag.ActionForm = "Update";
            return PartialView("index", m);
        }

        /// <summary>
        /// Xử lý xóa đường
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            roadService.Delete(id);
            ModelState.Clear();
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}
