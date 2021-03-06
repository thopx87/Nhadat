using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application2016.Enums;
using Application2016.Areas.Admin.Models;
using BusinessLayer;
using Application2016.Helpers;

namespace Application2016.Areas.Admin.Controllers
{
    public class CostController : BaseController
    {
        
        public CostController()
        {
            ViewBag.ActionMenu = "AdminSetting";
            ViewBag.ActionSubMenu = "Cost";
        }

        CostService costService = new CostService();
        PlaceService placeService = new PlaceService();
        RegionService regionService = new RegionService();
        ProductService productService = new ProductService();

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

        [HttpGet]
        public ActionResult Index(CostCondition condition)
        {
            int errorId = CheckRole(_roles);
            if (errorId < 0)
            {
                return Redirect(errorId);
            }
            ListCostModel model = new ListCostModel();
            int totalRecord = 0;
            List<Entities.CostSetting> listCostSetting = costService.ListCostSetting(condition.SearchText, condition.TransactionType, condition.ProductType, condition.Page, condition.PageSize, condition.State, out totalRecord);
            if (listCostSetting.Count > 0)
            {
                GetFullCostSetting(ref listCostSetting);
            }
            else
            {
                listCostSetting = new List<Entities.CostSetting>();
            }
            model.ListCostSetting = listCostSetting;

            condition.ListProductType = productService.ListProductType();
            condition.ListTransactionType = Helpers.DefaultData.ListTransactionType();
            model.Condition = condition;
            Paging(condition.Page, totalRecord);
            return View(model);
        }

        [HttpGet]
        public ActionResult CostSetting(int Id)
        {
            CostModel model = GetModel(Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult CostSetting(CostModel model)
        {
            int userId = 0;
            int.TryParse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID), out userId);

            // Lấy danh sách user được chọn.
            model.CostSetting.UserIds = GetUserIds(model.ListUser);
            
            int id = costService.Save(model.CostSetting, model.ListCost, userId);
            if (id > 0)
            {
                TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_SUCCESS;
                TempData[AdminConfigs.TEMP_REDIRECT] = @Url.Action("Index", "Cost");

                model = GetModel(id);
            }
            else
            {
                TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_ERROR;
            }
            return View(model);
        }

        public CostModel GetModel(int Id)
        {
            CostModel model = new CostModel();
            // Get list city (level0)
            List<Entities.Place> lstCity = placeService.List(0);
            model.ListCity = placeService.ListPlaceItemByParent(0);
            model.ListCity.Insert(0, new Entities.Item() { Text = "--- Chọn thành phố ---", Id = -1 });
            model.ListRegion = new List<Entities.Item>();

            // Get CostSetting
            if (Id > 0)
            {
                model.CostSetting = costService.GetById(Id);
            }

            // Get List district by city.
            if (model.CostSetting.CityId > 0)
            {
                model.ListDistrict = placeService.ListPlaceItemByParent(model.CostSetting.CityId);
                model.ListRegion = regionService.ListItem(model.CostSetting.CityId);
            }
            else
            {
                model.ListDistrict = new List<Entities.Item>();
            }
            model.ListDistrict.Insert(0, new Entities.Item() { Text = "--- Chọn quận huyện ---", Id = -1 });

            //
            if (model.CostSetting.DistrictId > 0)
            {
                model.ListWard = placeService.ListPlaceItemByParent(model.CostSetting.DistrictId);
                model.ListRegion = regionService.ListItem(model.CostSetting.CityId, model.CostSetting.DistrictId);
            }
            else
            {
                model.ListWard = new List<Entities.Item>();
            }
            model.ListRegion.Insert(0, new Entities.Item() { Text = "--- Chọn vùng ---", Id = -1 });
            model.ListWard.Insert(0, new Entities.Item() { Text = "--- Chọn xã phường ---", Id = -1 });

            // Lấy thông tin loại nhà
            model.ListProductType = productService.ListProductType();
            model.ListProductType.Insert(0, new Entities.Item() { Text = AdminConfigs.TEXT_CHOOSE_HOUSE_TYPE, Id = -1 });

            // Lấy thông tin các nhóm đối tượng
            RoleService roleService = new RoleService();
            model.ListRole = roleService.ListItem();
            model.ListRole.Insert(0, new Entities.Item() { Text = "--- Chọn quyền ---", Id = -1 });

            // Lấy thông tin người dùng theo quyền.
            UserService userService = new UserService();
            bool isAgency = false;
            List<Entities.Item> lstUserTemp = new List<Entities.Item>();
            if (model.CostSetting.UserRole > 0)
            {
                lstUserTemp = userService.ListItemByRoleId(model.CostSetting.UserRole);
                isAgency = roleService.CheckRoleAgency(model.CostSetting.UserRole);
            }
            else
            {
                lstUserTemp = new List<Entities.Item>();
            }

            // Nếu là dạng môi giới thì kiểm tra theo vùng.
            if (isAgency)
            {
                if (model.CostSetting.RegionId > 0)
                {
                    // Lấy môi giới theo vùng.
                    List<Entities.Item> lstTemp = userService.ListAgencyByRegion(model.CostSetting.RegionId, model.CostSetting.UserRole);
                    if (lstTemp.Count > 0)
                    {
                        lstUserTemp = lstUserTemp.Where(x => lstTemp.Any(y => y.Id == x.Id)).ToList();
                    }
                }
            }

            model.ListUser = lstUserTemp;

            // Lấy thông tin những người được chọn.
            if (!string.IsNullOrEmpty(model.CostSetting.UserIds))
            {
                string[] arrUser = model.CostSetting.UserIds.Split(',');
                foreach (var item in model.ListUser)
                {
                    if(arrUser.Contains(item.Id.ToString()))
                    {
                        item.Checked = true;
                    }
                }
            }

            // Lấy danh sách cost master.
            var listCostMaster = costService.ListCostMaster();
            model.ListCostMaster = listCostMaster;
            List<Entities.Cost> listCost = new List<Entities.Cost>();
            if (model.CostSetting.Id > 0)
            {
                listCost = costService.ListCost(model.CostSetting.Id);
            }
            else
            {
                foreach (Entities.Item costMaster in listCostMaster)
                {
                    listCost.Add(new Entities.Cost()
                    {
                        CostSettingId = -1,
                        CostMasterId = costMaster.Id,
                        CostMasterText = costMaster.Text,
                        IsApply = true,
                        SortOrder = (short)costMaster.Id,
                        MoneyCode1 = 0,
                        MoneyCode2 = 0,
                        MoneyCode3 = 0
                    });
                }
            }
            model.ListCost = listCost;

            // Lấy danh sách kiểu giao dịch.
            model.ListTransactionType = Helpers.DefaultData.ListTransactionType();
            model.ListTransactionType.Insert(0, new Entities.Item() { Id = 0, Text = "Chọn loại giao dịch" });
            
            return model;
        }

        public void GetFullCostSetting(ref List<Entities.CostSetting> listCostSetting)
        {
            RoleService roleService = new RoleService();
            UserService userService = new UserService();
            List<Entities.Item> AllPlace = placeService.AllPlace();
            foreach (Entities.CostSetting cost in listCostSetting)
            {
                // Lấy thông tin tên địa lý.
                if(cost.CityId>0)
                {
                    cost.CityName = AllPlace.Where(x => x.Id == cost.CityId).FirstOrDefault().Text;
                }

                if (cost.DistrictId > 0)
                {
                    cost.DistrictName = AllPlace.Where(x => x.Id == cost.DistrictId).FirstOrDefault().Text;
                }
                if (cost.WardId > 0)
                {
                    cost.WardName = AllPlace.Where(x => x.Id == cost.WardId).FirstOrDefault().Text;
                }

                // Lấy thông tin tên vùng.
                if (cost.RegionId > 0)
                {
                    cost.RegionName = regionService.GetItem(cost.RegionId).Text;
                }

                // Lấy kiểu nhà 
                if (cost.ProductType > 0)
                {
                    cost.ProductTypeName = productService.ProductType(cost.ProductType).Text;
                }

                // Lấy tên nhóm.
                if (cost.UserRole > 0)
                {
                    cost.GroupName = roleService.GetItem(cost.UserRole).Text;
                }

                // Lấy tên người dùng.
                if (!string.IsNullOrEmpty(cost.UserIds))
                {
                    string[] arr = cost.UserIds.Split(',');
                    List<int> lstUserIds = arr.Select(Int32.Parse).ToList();
                    var listUser = userService.ListItem(lstUserIds);
                    cost.ListUserName = listUser.Select(x => x.Text).ToList();
                }
                else
                {
                    cost.ListUserName = new List<string>();
                }
            }
        }

        /// <summary>
        /// Lấy userid từ danh sách user được chọn.
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        public string GetUserIds(List<Entities.Item> listUser)
        {
            string result = string.Empty;
            if (listUser!=null && listUser.Count > 0)
            {
                foreach (Entities.Item user in listUser)
                {
                    if (user.Checked)
                    {
                        result += user.Id + ",";
                    }
                }
                if (result.Length > 1)
                {
                    result = result.Remove(result.Length - 1);
                }
            }
            return result;
        }
    }
}
