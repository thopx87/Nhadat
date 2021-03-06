using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Application2016.Helpers;
using Application2016.Enums;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using Application2016.Areas.Admin.Models;

namespace Application2016.Areas.Admin.Controllers
{
    public class RegionController : BaseController
    {
        PlaceService placeService = new PlaceService();
        RegionService regionService = new RegionService();
        UserService userService = new UserService();
        //
        // GET: /Admin/Region/

        public RegionController()
        {
            ViewBag.ActionMenu = "AdminSetting";
            ViewBag.ActionSubMenu = "Region";
        }

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
        public ActionResult Index(int page =1, string text="")
        {
            int errorId = CheckRole(_roles);
            if (errorId < 0)
            {
                return Redirect(errorId);
            }
            if (page < 0) page = 1;
            Page = page;
            LoadData(text);
            return View();
        }

        public ActionResult Index2(RegionCondition Condition)
        {
            ListRegionModel model = new ListRegionModel();

            int totalRecord = 0;
            model.ListRegion = regionService.List(Condition.CityId, Condition.DistrictId, Condition.SearchText, Condition.Page, Condition.PageSize, out totalRecord);
            model.ListCity = placeService.ListPlaceItemByParent(0);
            model.ListCity.Insert(0, new Entities.Item() { Id = 0, Text = "Chọn tỉnh, thành phố" });
            if (Condition.CityId > 0)
            {
                model.ListDistrict = placeService.ListPlaceItemByParent(Condition.CityId);
            }
            else
            {
                model.ListDistrict = new List<Entities.Item>();
            }
            model.ListDistrict.Insert(0, new Entities.Item() { Id = 0, Text = "Chọn quận huyện" });
            model.Condition = Condition;
            Paging(Condition.Page, totalRecord);
            return View(model);
        }

        public void LoadData(string text=null)
        {
            int totalRecord = 0;
            List<Entities.Region> lst = new List<Entities.Region>();
            lst = !string.IsNullOrEmpty(text) ? regionService.List(text, Page, pageSize, out totalRecord) : regionService.List(Page, pageSize, out totalRecord);
            
            ViewBag.ListRegion = lst;
            ViewBag.NumberItem = lst.Count;
            ViewBag.text = text;
            Paging(Page, totalRecord);
        }

        [HttpGet]
        public ActionResult UpdateRegion(int id, int cityId, int districtId)
        {
            ModelState.Clear();
            Models.RegionModel model = new Models.RegionModel();

            model.CityId = cityId;
            model.DistrictId = districtId;

            if (id > 0)
            {
                model.Id = id;
                var region = regionService.GetById(id);

                // Có thể sẽ kiểm tra hack
                if (cityId != region.CityId || districtId != region.DistrictId)
                {
                    // Trường hợp này sẽ xử lý hacker.
                }
                model.Text = region.Text;
                model.Status = region.Status;
                model.NeighborId = region.NeighborId;
            }

            // Lấy danh sách Tỉnh/ Thành phố
            model.ListCity = placeService.ListPlaceItemByParent(0);

            // Lấy danh sách Quận Huyện
            model.ListDistrict = placeService.ListPlaceItemByParent(cityId);

            // Lấy danh sách phường, xã trong vùng
            model.ListWardOfRegion = placeService.ListPlaceItemOfRegion(id);

            // Lấy danh sách phường, xã trong quận
            model.ListWardOfDistrict = placeService.ListPlaceItemByParent(districtId).Except(model.ListWardOfRegion);

            model.ListRegion = regionService.ListItem();

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateRegion(Models.RegionModel model)
        {
            if (ModelState.IsValid)
            {
                Entities.Region r = new Entities.Region();
                model.MapFrom(model, ref r);
                int result = 0;
                if (model.Id > 0)
                {
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
                }
                else
                {
                    // Cập nhật lại ID vùng của xã, phường cũ.'
                    if (model.Id > 0)
                    {
                        foreach (Entities.Place p in placeService.ListWardInRegion(model.Id))
                        {
                            p.RegionId = null;
                            placeService.Update(p);
                        }
                    }

                    // Cập nhật lại id vùng của xã phường mới
                    Entities.Place e;
                    foreach (int wardId in model.WardOfRegionIds)
                    {
                        e = placeService.GetById(wardId);
                        e.RegionId = result;
                        placeService.Update(e);
                    }

                    TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_SUCCESS;
                    TempData[AdminConfigs.TEMP_REDIRECT] = @Url.Action("Index2", "Region");

                    // Xóa đi những thông tin cơ bản của vùng
                    model.Text = "";                    
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_ERROR;
            }

            // Lấy danh sách Tỉnh/ Thành phố
            model.ListCity = placeService.ListPlaceItemByParent(0);

            // Lấy danh sách Quận Huyện
            model.ListDistrict = placeService.ListPlaceItemByParent(model.CityId);

            // Lấy danh sách xã phường đã lọc
            model.ListWardOfRegion = placeService.ListPlaceItemByIds(model.WardOfRegionIds);
            model.ListWardOfDistrict = placeService.ListPlaceItemByIds(model.WardOfDistrictIds);

            model.ListRegion = regionService.ListItem();
            return View(model);
        }

        [HttpGet]
        public ActionResult UpdateUser(int id)
        {
            ModelState.Clear();
            Models.RegionUserModel model = new Models.RegionUserModel();
            
            if (id > 0)
            {
                var region = regionService.GetById(id);
                model.Id = region.Id;
                model.Text = region.Text;
            }
            // Danh sách User của vùng
            //model.ListUserOfRegion = regionService.ListUserItemByRegionId(id);
            model.ListUserOfRegion = userService.ListAgencyByRegion(id);

            // Danh sách Môi giới
            //model.ListUsers = userService.ListUserItemByRoleId(-1);
            model.ListUsers = userService.ListAgency();

            // Get List Role
            RoleService roleService = new RoleService();
            model.ListRole = roleService.ListAgency();
            model.ListRole.Insert(0, new Entities.Item() { Id = 0, Text = "Chọn loại môi giới" });

            // Tính số lượng môi giới tối đa trong vùng
            model.MaxAgency = regionService.CountMaxAgency(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateUser(Models.RegionUserModel model)
        {
            if (ModelState.IsValid)
            {
                // Cập nhật vào bảng userInRegion
                UserInRegionService userInRegionService = new UserInRegionService();
                var lstUserIDNew = model.UserOfRegionIds;
                // Bỏ những user trong vùng cũ (nếu không còn trong danh sách mới)

                // Lấy danh sách user của vùng cũ.
                var lstUserOld = regionService.ListUserItemByRegionId(model.Id);
                if (lstUserOld != null)
                {
                    // Duyệt, xử lý dữ liệu cũ
                    foreach (var u in lstUserOld)
                    {
                        // Nếu danh sách mới mà không chứa ID user cũ thì xóa user đó đi.
                        // Cũng có thể chọn cách khác là cập nhật trạng thái, chuyển về Status = 0;
                        if (!lstUserIDNew.Contains(u.Id))
                        {
                            userInRegionService.DeleteByUser(u.Id);
                        }
                    }
                }

                // Cập nhật những user mới.
                Entities.UserInRegion userInRegion;
                foreach (var uId in lstUserIDNew)
                {
                    if (!lstUserOld.Any(x=>x.Id == uId))
                    {
                        userInRegion = new Entities.UserInRegion();
                        userInRegion.UserId = uId;
                        userInRegion.RegionId = model.Id;
                        userInRegion.Status = true;
                        //userInRegionService.Insert(userInRegion);
                        userInRegionService.Save(userInRegion);
                    }
                }
                TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_SUCCESS;
                TempData[AdminConfigs.TEMP_REDIRECT] = @Url.Action("Index2", "Region");
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_ERROR;
            }
            // Get List Role
            RoleService roleService = new RoleService();
            model.ListRole = roleService.ListAgency();
            model.ListRole.Insert(0, new Entities.Item() { Id = 0, Text = "Chọn loại môi giới" });

            // Lấy danh sách user của region
            //m.ListUserOfRegion = regionService.ListUserItemByRegionId(id);
            model.ListUserOfRegion = userService.ListAgencyByRegion(model.RoleId);
            // lấy danh sách user
            //m.ListUsers = userService.ListUserItemByRoleId(-1);
            model.ListUsers = userService.ListAgency();

            ViewBag.ActionForm = "UpdateUser";

            return View(model);
        }

        public ActionResult DeleteRegion(int id)
        {
            regionService.Delete(id);
            ModelState.Clear();
            return Redirect(Request.UrlReferrer.ToString());
        }

        public void ExportToExcel()
        {
            var lstRegion = regionService.List();
            // Convert data to table.
            List<Models.RegionExcel> lstRegionExcel = new List<Models.RegionExcel>();
            Models.RegionExcel r = new Models.RegionExcel();
            string delimiter = ",";
            foreach (var region in lstRegion)
            {
                r = new Models.RegionExcel()
                {
                    Id = region.Id,
                    CityId = region.CityId,
                    DistrictId = region.DistrictId,
                    Text = region.Text,
                    NeighborId = region.NeighborId != null ? region.NeighborId.Value : 0,
                    Status = region.Status?1:0,
                    ListWard = string.Join(delimiter,region.ListWard.Select(x=>x.Id)),
                    ListUser = string.Join(delimiter, region.ListUser.Select(x=>x.Id))
                };
                lstRegionExcel.Add(r);
            }


            var grid = new GridView();
            grid.DataSource = lstRegionExcel;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Regions.xls");
            Response.Charset = "UTF-8";
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            htw.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            grid.RenderControl(htw);

            Response.Write(sw.ToString());
            Response.End();
        }

        public void ExportToCSV()
        {
            StringWriter sw = new StringWriter();

            sw.WriteLine("\"Id\",\"Text\",\"CityId\",\"DistrictId\",\"Status\",\"NeighborId\",\"ListWard\",\"ListUser\"");

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=Regions.csv");
            Response.ContentType = "application/ms-excel";
            Response.ContentEncoding = System.Text.Encoding.Unicode;
            Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
            Response.Charset = "UTF-8";

            var lstRegion = regionService.List();
            // Convert data to table.
            List<Models.RegionExcel> lstRegionExcel = new List<Models.RegionExcel>();
            Models.RegionExcel r = new Models.RegionExcel();
            string delimiter = ",";
            foreach (var region in lstRegion)
            {
                r = new Models.RegionExcel()
                {
                    Id = region.Id,
                    CityId = region.CityId,
                    DistrictId = region.DistrictId,
                    Text = region.Text,
                    NeighborId = region.NeighborId != null ? region.NeighborId.Value : 0,
                    Status = region.Status ? 1 : 0,
                    ListWard = string.Join(delimiter, region.ListWard.Select(x => x.Id)),
                    ListUser = string.Join(delimiter, region.ListUser.Select(x => x.Id))
                };
                lstRegionExcel.Add(r);
            }

            foreach (var region in lstRegionExcel)
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",

                region.Id,
                region.Text,
                region.CityId,
                region.DistrictId,
                region.Status,
                region.NeighborId,
                region.ListWard,
                region.ListUser
                ));
            }
            Response.Write(sw.ToString());
            Response.End();
        }
    }
}
