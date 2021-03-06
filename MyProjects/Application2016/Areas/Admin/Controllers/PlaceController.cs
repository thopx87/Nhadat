using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Entities;
using Application2016.Helpers;
using Application2016.Enums;
using Application2016.Areas.Admin.Models;

namespace Application2016.Areas.Admin.Controllers
{
    public class PlaceController : BaseController
    {
        PlaceService placeService = new PlaceService();
        RegionService regionService = new RegionService();

        public PlaceController()
        {
            ViewBag.ActionMenu = "AdminSetting";
            ViewBag.ActionSubMenu = "Place";
        }
        
        #region Place
        Roles[] _roles
        {
            get
            {
                return new Roles[]{
                    Roles.Administrator,
                    Roles.Super_Administrator
                };
            }
        }
        public ActionResult Index(PlaceCondition Condition)
        {
            int errorId = CheckRole(_roles);
            if (errorId < 0)
            {
                return Redirect(errorId);
            }

            LoadData();
            return View();
        }

        [HttpGet]
        public PartialViewResult UpdateCity(int id)
        {
            ModelState.Clear();
            Models.CityModel m = new Models.CityModel();
            if (id > 0)
            {
                Entities.Place e = placeService.GetById(id);
                if (e != null)
                {
                    m.Id = e.Id;
                    m.Text = e.Text;
                    m.isCity = e.Type == 1 ? true : false;
                    m.Address = e.Address;
                }
            }

            LoadData();
            ViewBag.ActionForm = "UpdateCity";
            ViewBag.SubmitValue = id > 0 ? AdminConfigs.BUTTON_UPDATE : AdminConfigs.BUTTON_ADD;
            return PartialView("index",m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCity(int id, Models.CityModel m)
        {
            if (ModelState.IsValid)
            {
                Entities.Place e = new Entities.Place();
                e.Text = m.Text;
                e.Type = m.isCity == true ? (short)BusinessLayer.Enums.Place.Thanh_Pho : (short)BusinessLayer.Enums.Place.Tinh;
                e.Address = m.Address;
                e.Parent = 0;
                int result = 0;
                if (id > 0)
                {
                    e.Id = id;
                    result = placeService.Update(e);
                }
                else
                {
                    result = placeService.Insert(e);
                }
                if (result <= 0)
                {
                    ViewBag.Message = AdminConfigs.MESSAGE_UPDATE_ERROR;
                    ViewBag.AlertClass = AdminConfigs.CLASS_ALERT_DANGER;
                    return PartialView(m);
                }
                else
                {
                    ViewBag.AlertClass = AdminConfigs.CLASS_ALERT_SUCCESS;
                    ViewBag.Message = AdminConfigs.MESSAGE_UPDATE_SUCCESS;
                    
                    // Xóa đi text (thông tin bắt buộc nhập) để tránh click nhiều lần.
                    m.Text = "";
                }
            }
            LoadData();
            ViewBag.ActionForm = "UpdateCity";
            return PartialView("index",m);
        }

        [HttpGet]
        public PartialViewResult UpdateDistrict(int id, int cityId)
        {
            ModelState.Clear();
            Models.DistrictModel m = new Models.DistrictModel();
            if (id > 0)
            {
                Entities.Place e = placeService.GetById(id);
                if (e != null)
                {
                    m.Id = e.Id;
                    m.Parent = e.Parent;
                    m.Text = e.Text;
                    m.districtType = e.Type;
                    m.Address = e.Address;
                }
                
            }

            // Get List city (Level = 0)
            List<Entities.Place> lstCity = placeService.List(0);
            m.ListCity = placeService.ListPlaceItemByParent(0);
            if (cityId > 0)
            {
                m.Parent = cityId;
            }
            LoadData();
            ViewBag.ActionForm = "UpdateDistrict";
            ViewBag.SubmitValue = id > 0 ? AdminConfigs.BUTTON_UPDATE : AdminConfigs.BUTTON_ADD;

            ViewBag.DistrictId = id;
            ViewBag.CityId = cityId;

            return PartialView("index",m);
        }

        [HttpPost]
        public ActionResult UpdateDistrict(int id, Models.DistrictModel m)
        {
            if (ModelState.IsValid)
            {
                Entities.Place e = new Entities.Place();
                e.Text = m.Text;
                e.Type = m.districtType;
                e.Address = m.Address;
                e.Parent = m.Parent;
                int result = 0;
                if (id > 0)
                {
                    e.Id = id;
                    result = placeService.Update(e);
                }
                else
                {
                    result = placeService.Insert(e);
                }
                
                if (result <= 0)
                {
                    ViewBag.Message = AdminConfigs.MESSAGE_UPDATE_ERROR;
                    ViewBag.AlertClass = AdminConfigs.CLASS_ALERT_DANGER;
                    return PartialView(m);
                }
                else
                {
                    ViewBag.AlertClass = AdminConfigs.CLASS_ALERT_SUCCESS;
                    ViewBag.Message = AdminConfigs.MESSAGE_UPDATE_SUCCESS;
                    ModelState.Clear();

                    // Xóa đi text, tránh update nhiều
                    m.Text = "";
                }
            }
            LoadData();
            ViewBag.ActionForm = "UpdateDistrict";
            m.ListCity = placeService.ListPlaceItemByParent(0);

            ViewBag.DistrictId = id;
            ViewBag.CityId = m.Parent;

            return PartialView("index", m);
        }

        [HttpGet]
        public PartialViewResult UpdateWard(int id, int cityId, int districtId)
        {
            ModelState.Clear();
            Models.WardModel m = new Models.WardModel();
            if (id > 0)
            {
                Entities.Place e = placeService.GetById(id);
                if (e != null)
                {
                    m.MapFrom(e, ref m);
                }
            }

            // Lấy danh sách Tỉnh/ Thành phố
            m.ListCity = placeService.ListPlaceItemByParent(0);

            // Lấy danh sách Quận Huyện
            m.ListDistrict = placeService.ListPlaceItemByParent(cityId);

            m.CityId = cityId;
            m.Parent = districtId;
            LoadData();
            ViewBag.ActionForm = "UpdateWard";
            ViewBag.SubmitValue = id > 0 ? AdminConfigs.BUTTON_UPDATE : AdminConfigs.BUTTON_ADD;

            ViewBag.DistrictId = districtId;
            ViewBag.CityId = cityId;

            return PartialView("index",m);
        }

        [HttpPost]
        public ActionResult UpdateWard(int id, Models.WardModel m)
        {
            if (ModelState.IsValid)
            {
                Entities.Place e = new Entities.Place();
                m.MapFrom(m, ref e);

                int result = 0;
                if (id > 0)
                {
                    result = placeService.Update(e);
                }
                else
                {
                    result = placeService.Insert(e);
                }
                if (result <= 0)
                {
                    ViewBag.Message = AdminConfigs.MESSAGE_UPDATE_ERROR;
                    ViewBag.AlertClass = AdminConfigs.CLASS_ALERT_DANGER;
                    return PartialView(m);
                }
                else
                {
                    ViewBag.AlertClass = AdminConfigs.CLASS_ALERT_SUCCESS;
                    ViewBag.Message = AdminConfigs.MESSAGE_UPDATE_SUCCESS;
                    ModelState.Clear();

                    // xóa text
                    m.Text = "";
                }
            }

            // Lấy danh sách Tỉnh/ Thành phố
            m.ListCity = placeService.ListPlaceItemByParent(0);

            // Lấy danh sách Quận Huyện
            m.ListDistrict = placeService.ListPlaceItemByParent(m.CityId);

            LoadData();
            ViewBag.ActionForm = "UpdateWard";

            ViewBag.DistrictId = id;
            ViewBag.CityId = m.CityId;

            return PartialView("index", m);
        }

        /// <summary>
        /// Danh sách phường/ xã
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public PartialViewResult ListWard(int districtId, int cityId)
        {
            // Get List ward (Parent = districtId)
            List<Entities.Place> lstWard = placeService.List(districtId);
            ViewBag.ListWard = lstWard;
            ViewBag.NumberItem = lstWard.Count;
            ViewBag.CityId = cityId;
            return PartialView();
        }

        public ActionResult DeletePlace(int id)
        {
            // Get Tree list place
            List<int> lst = new List<int>();
            placeService.TreeListId(id, ref lst);
            if (lst != null)
            {
                placeService.Delete(lst);
            }
            ModelState.Clear();
            return Redirect(Request.UrlReferrer.ToString());
        }

        #endregion

        /// <summary>
        /// Lấy danh sách vùng trong tỉnh
        /// </summary>
        /// <param name="districtId">id tỉnh</param>
        /// <param name="cityId">id thành phố</param>
        /// <returns>danh sách vùng</returns>
        public PartialViewResult ListRegion(int districtId, int cityId)
        {
            Models.RegionModel m = new Models.RegionModel();

            // Get List ward (Parent = districtId)
            List<Entities.Region> lstRegion = regionService.List(cityId, districtId);
            ViewBag.ListRegion = lstRegion;
            ViewBag.NumberItem = lstRegion.Count;
            ViewBag.CityId = cityId;
            return PartialView(m);
        }

        [HttpGet]
        public PartialViewResult UpdateRegion(int id, int cityId, int districtId)
        {
            ModelState.Clear();
            Models.RegionModel m = new Models.RegionModel();
           
            if (id > 0)
            {
                m.Id = id;
                var region = regionService.GetById(id);

                // Có thể sẽ kiểm tra hack
                if (cityId != region.CityId || districtId != region.DistrictId)
                {
                    // Trường hợp này sẽ xử lý hacker.
                }
                m.Text = region.Text;
                m.Status = region.Status;
                m.NeighborId = region.NeighborId;
            }
            
            m.CityId = cityId;
            m.DistrictId = districtId;

            // Lấy danh sách Tỉnh/ Thành phố
            m.ListCity = placeService.ListPlaceItemByParent(0);

            // Lấy danh sách Quận Huyện
            m.ListDistrict = placeService.ListPlaceItemByParent(cityId);

            // Lấy danh sách phường, xã trong vùng
            m.ListWardOfRegion = placeService.ListPlaceItemOfRegion(id);

            // Lấy danh sách phường, xã trong quận
            m.ListWardOfDistrict = placeService.ListPlaceItemByParent(districtId).Except(m.ListWardOfRegion);
            
            LoadData();
            ViewBag.ActionForm = "UpdateRegion";
            ViewBag.SubmitValue = id > 0 ? AdminConfigs.BUTTON_UPDATE : AdminConfigs.BUTTON_ADD;

            ViewBag.DistrictId = districtId;
            ViewBag.CityId = cityId;

            return PartialView("index", m);
        }

        /// <summary>
        /// Xử lý cập nhật Vùng.
        /// </summary>
        /// <param name="id">id Vùng</param>
        /// <param name="m">Model vùng (Models/place.cs)</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateRegion(int id, Models.RegionModel m)
        {
            if (ModelState.IsValid)
            {
                Entities.Region r = new Entities.Region();
                r.Text = m.Text;
                r.CityId = m.CityId;
                r.DistrictId = m.DistrictId;
                r.Status = m.Status;
                r.NeighborId = m.NeighborId;
                int result = 0;
                if (id > 0)
                {
                    r.Id = id;
                    result = regionService.Update(r);
                }
                else
                {
                    result = regionService.Insert(r);
                }
                if (result <= 0)
                {
                    ViewBag.Message = AdminConfigs.MESSAGE_UPDATE_ERROR;
                    ViewBag.AlertClass = AdminConfigs.CLASS_ALERT_DANGER;
                    return PartialView(m);
                }
                else
                {
                    // Cập nhật lại ID vùng của xã, phường cũ.'
                    if (id > 0)
                    {
                        foreach (Entities.Place p in placeService.ListWardInRegion(id))
                        {
                            p.RegionId = null;
                            placeService.Update(p);
                        }
                    }

                    // Cập nhật lại id vùng của xã phường mới
                    Entities.Place e;
                    foreach (int wardId in m.WardOfRegionIds)
                    {
                        e = placeService.GetById(wardId);
                        e.RegionId = result;
                        placeService.Update(e);
                    }

                    ViewBag.AlertClass = AdminConfigs.CLASS_ALERT_SUCCESS;
                    ViewBag.Message = AdminConfigs.MESSAGE_UPDATE_SUCCESS;

                    // xóa text
                    m.Text = "";
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage);
            }

            // Lấy danh sách Tỉnh/ Thành phố
            m.ListCity = placeService.ListPlaceItemByParent(0);

            // Lấy danh sách Quận Huyện
            m.ListDistrict = placeService.ListPlaceItemByParent(m.CityId);

            // Lấy danh sách xã phường đã lọc
            m.ListWardOfRegion = placeService.ListPlaceItemByIds(m.WardOfRegionIds);
            m.ListWardOfDistrict = placeService.ListPlaceItemByIds(m.WardOfDistrictIds);

            LoadData();
            ViewBag.ActionForm = "UpdateRegion";

            ViewBag.DistrictId = id;
            ViewBag.CityId = m.CityId;

            return PartialView("index", m);
        }


        /// <summary>
        /// Hàm xử lý khi khởi tạo, xử lý dữ liệu sau khi cập nhật.
        /// </summary>
        public void LoadData()
        {
            int totalRecord = 0;
            // Lấy danh sách City.
            List<Entities.Place> lstCity = placeService.ListCity("", Page, pageSize, out totalRecord);

            // Add paramater
            ViewBag.ListCity = lstCity;
            ViewBag.NumberItem = lstCity.Count;

            // Paging
            ViewBag.query = Query;

            Paging(Page, totalRecord);
        }

    }
}
