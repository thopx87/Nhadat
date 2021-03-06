using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Configuration;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using BusinessLayer;
using Application2016.Enums;
using Application2016.Areas.Admin.Models;
using Application2016.Helpers;

namespace Application2016.Areas.Admin.Controllers
{
    public class SettingController : BaseController
    {
        //
        // GET: /Admin/Setting/
        private static Models.SettingModel settingModel = new Models.SettingModel();
        private SettingService _settingService = new SettingService();
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

        public SettingController()
        {
            ViewBag.ActionMenu = "";
            ViewBag.ActionSubMenu = "";
        }

        public ActionResult Index()
        {
            // Kiểm tra đăng nhập
            int errorId = CheckRole(_roles);
            if (errorId < 0)
            {
                return Redirect(errorId);
            }

            settingModel.RoleSetting = GetRoleSetting(0);
            settingModel.MailSetting = GetMailSetting();
            settingModel.webSetting = GetDataSetting(AdminConfigs.SETTING_WEBSITE);
            settingModel.socialNetworkSetting = GetDataSetting(AdminConfigs.SETTING_SOCIAL_NETWORK);
            return View(settingModel);
        }

        private Models.RoleSettingModel GetRoleSetting(int id)
        {
            Models.RoleSettingModel m = new Models.RoleSettingModel();
            RoleService roleService = new RoleService();
            Entities.Role role = new Entities.Role();
            if (id > 0)
            {
                role = roleService.GetById(id);
                if (role != null)
                {
                    m.Id = role.Id;
                    m.Post = role.Post;
                    m.ResiveFromAgency = role.ResiveFromAgency;
                    m.ResiveFromMember = role.ResiveFromMember;
                    m.SendRegionNum = role.SendRegionNum;
                    m.ResiveRegionNum = role.ResiveRegionNum;
                }
            }
            // Lấy danh sách role
            m.ListRole = roleService.ListItem();
            return m;
        }

        private Models.MailModel GetMailSetting()
        {
            Models.MailModel m = new Models.MailModel();
            m.Port = 0;
            m.Email = "";
            m.Host = "";
            return m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult UpdatePlace()
        {
            return PartialView("index");
        }

        [HttpPost]
        public ActionResult UpdatePlace(HttpPostedFileBase file)
        {
            // Tạm thời coi như xử lý xong.
            return PartialView("index");
            if(ModelState.IsValid)
            {
                if (file.ContentLength > 0 && file!=null)
                {
                    Excel.Application app = new Excel.Application();
                    Excel.Workbook workBook;
                    Excel.Worksheet workSheet;
                    // step 1: save file.
                    string fileLocation = Server.MapPath("~/Content/" + file.FileName);
                    if (!System.IO.File.Exists(fileLocation))
                    {
                        file.SaveAs(fileLocation);
                    }                   

                    // step 2: Read file.

                    workBook = app.Workbooks.Open(fileLocation);
                    workSheet = workBook.ActiveSheet;
                    Excel.Range range = workSheet.UsedRange;

                    // Lấy toàn bộ danh sách tỉnh, huyện, xã đưa vào dictionary
                    Dictionary<string, Dictionary<string, List<string>>> dictCity = new Dictionary<string, Dictionary<string, List<string>>>();
                    Dictionary<string, List<string>> dictDistrict = new Dictionary<string, List<string>>();
                    List<string> lstWard = new List<string>();
                    string strCity = "";
                    string strDistrict = "";
                    string strWard = "";
                    for (int row = 2; row < range.Rows.Count; row++)
                    {
                        strCity = range.Cells[row, 1].Value; // Lấy tên tỉnh
                        strDistrict = range.Cells[row, 3].Value; // Lấy tên huyện
                        strWard = range.Cells[row,5].Value; // Lấy tên xã
                        
                        // Nếu chưa có tỉnh thì lấy tỉnh
                        if (!dictCity.ContainsKey(strCity))
                        {
                            dictCity.Add(strCity, new Dictionary<string, List<string>>());

                            // Xóa đi danh sách huyện của tỉnh cũ.
                            dictDistrict = new Dictionary<string, List<string>>();
                        }
                        
                        // Nếu chưa có huyện thì lấy huyện
                        if (!dictDistrict.ContainsKey(strDistrict))
                        {
                            dictDistrict.Add(strDistrict, new List<string>());
                            
                            // Xóa đi danh sách xã cũ.
                            lstWard = new List<string>();
                        }

                        // Xử lý thêm xã
                        lstWard.Add(strWard);

                        // Xử lý thêm huyện.
                        dictDistrict[strDistrict] = lstWard;

                        // Xử lý thêm tỉnh
                        dictCity[strCity] = dictDistrict;

                    }

                    // Duyệt danh sách tỉnh, cập nhật vào database
                    PlaceService placeService = new PlaceService();
                    int cityId = 0;
                    int districtId = 0;
                    foreach (string city in dictCity.Keys)
                    {
                        // Thêm tỉnh
                        cityId = InsertCity(city, placeService);
                        dictDistrict = dictCity[city];

                        foreach (string district in dictDistrict.Keys)
                        {
                            // Thêm huyện
                            districtId = InsertDistrict(district, cityId, placeService);
                            lstWard = dictDistrict[district];

                            foreach (string ward in lstWard)
                            {
                                // Thêm xã
                                InsertWard(ward, districtId, placeService);
                            }
                        }
                    }


                    workBook.Close(false, null, null);
                    app.Quit();

                    releaseObject(workSheet);
                    releaseObject(workBook);
                    releaseObject(app);
                }
            }
            return PartialView("index");
        }

        [HttpGet]
        public PartialViewResult UpdateRegion()
        {
            return PartialView("index");
        }

        [HttpPost]
        public ActionResult UpdateRegion(HttpPostedFileBase file, int chkFrom)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    if (chkFrom == 1)
                    {
                        WriteCSVRegion(file);
                    }
                    else
                    {
                        WriteExcelRegion(file);
                    }
                }
            }
            return RedirectToAction("Index", "Setting");
        }

        public void WriteExcelRegion(HttpPostedFileBase file)
        {
            Excel.Application app = new Excel.Application();
            Excel.Workbook workBook;
            Excel.Worksheet workSheet;
            // step 1: save file.
            string fileLocation = Server.MapPath("~/Content/" + file.FileName);
            if (System.IO.File.Exists(fileLocation))
            {
                System.IO.File.Delete(fileLocation);
            }
            file.SaveAs(fileLocation);

            // step 2: Read file.

            workBook = app.Workbooks.Open(fileLocation);
            workSheet = workBook.ActiveSheet;
            Excel.Range range = workSheet.UsedRange;

            // Lấy toàn bộ danh sách vùng đưa vào list                    
            List<Models.RegionExcel> lstRegion = new List<Models.RegionExcel>();
            Models.RegionExcel region = new RegionExcel();
            for (int row = 2; row <= range.Rows.Count; row++)
            {
                region = new RegionExcel()
                {
                    Id = int.Parse(range.Cells[row, 1].Text.ToString()),
                    Text = range.Cells[row, 2].Text,
                    CityId = int.Parse(range.Cells[row, 3].Text.ToString()),
                    DistrictId = int.Parse(range.Cells[row, 4].Text.ToString()),
                    Status = int.Parse(range.Cells[row, 5].Text.ToString()),
                    NeighborId = int.Parse(range.Cells[row, 6].Text.ToString()),
                    ListWard = range.Cells[row, 7].Text,
                    ListUser = range.Cells[row, 8].Text
                };
                lstRegion.Add(region);

            }

            // Update To DB
            UpdateRegionToDB(lstRegion);

            workBook.Close(false, null, null);
            app.Quit();

            releaseObject(workSheet);
            releaseObject(workBook);
            releaseObject(app);
        }

        public void WriteCSVRegion(HttpPostedFileBase file)
        {
            string fileLocation = Server.MapPath("~/Content/" + file.FileName);

            if(System.IO.File.Exists(fileLocation))
            {
                System.IO.File.Delete(fileLocation);
            }
            file.SaveAs(fileLocation);

            List<Models.RegionExcel> lstRegion = new List<Models.RegionExcel>();
            RegionExcel region = new RegionExcel();
            // Read sample data from CSV file
            using (CsvFileReader reader = new CsvFileReader(fileLocation))
            {
                
                CsvRow row = new CsvRow();
                reader.ReadRow(row);
                while (reader.ReadRow(row))
                {
                    region = new RegionExcel()
                    {
                        Id = int.Parse(row[0]),
                        Text = row[1],
                        CityId = int.Parse(row[2]),
                        DistrictId = int.Parse(row[3]),
                        Status = int.Parse(row[4]),
                        NeighborId = int.Parse(row[5]),
                        ListWard = row[6],
                        ListUser = row[7]
                    };

                    lstRegion.Add(region);
                }
            }

            // Update To DB
            UpdateRegionToDB(lstRegion);
        }

        public void UpdateRegionToDB(List<RegionExcel> listRegion)
        {
            // Duyệt danh sách vùng, cập nhật vào database
            RegionService regionService = new RegionService();
            PlaceService placeService = new PlaceService();
            UserInRegionService userInRegionService = new UserInRegionService();
            List<RegionExcel> listRegionLast = listRegion;
            bool isUpdateLast = false;
            int count = listRegion.Count;
            for(int i=0; i<count;i++)
            {
                RegionExcel item = listRegion[i];
                // Xử lý thêm, cập nhật danh sách vùng.
                Entities.Region entity = regionService.GetById(item.Id);
                entity.Text = item.Text;
                entity.CityId = item.CityId;
                entity.DistrictId = item.DistrictId;
                entity.Status = item.Status == 1 ? true : false;
                entity.NeighborId = item.NeighborId;
                if (entity.Id > 0)
                {
                    // Update.
                    regionService.Update(entity);
                }
                else
                {
                    // Insert. lấy lại ID region.
                    entity.Id = regionService.Insert(entity);

                    // Cập nhật lại id vùng.
                    listRegionLast[i].Id = entity.Id;

                    // Cập nhật lại vùng lân cận.

                    listRegionLast.Where(r => r.NeighborId == item.Id).ToList().ForEach(r => r.NeighborId = entity.Id);

                    isUpdateLast = true;
                }

                // Xử lý cập nhật vào bảng Place.
                if (item.ListWard != "")
                {
                    string[] lstWard = item.ListWard.Split(',');
                    foreach (string ward in lstWard)
                    {
                        placeService.UpdateRegion(int.Parse(ward), entity.Id);
                    }
                }

                // Xử lý cập nhật vào bảng UserInRegion
                if (item.ListUser != "")
                {
                    // Lấy danh sách user của vùng cũ.                        
                    var lstUserOld = regionService.ListUserItemByRegionId(item.Id);
                    var lstUserIDNew = item.ListUser.Split(',').ToList();
                    if (lstUserOld != null)
                    {
                        // Duyệt, xử lý dữ liệu cũ
                        foreach (var u in lstUserOld)
                        {
                            // Nếu danh sách mới mà không chứa ID user cũ thì xóa user đó đi.
                            // Cũng có thể chọn cách khác là cập nhật trạng thái, chuyển về Status = 0;
                            if (!lstUserIDNew.Contains(u.Id.ToString()))
                            {
                                userInRegionService.DeleteByUser(u.Id);
                            }
                        }
                    }

                    // Cập nhật những user mới.
                    Entities.UserInRegion userInRegion;
                    foreach (var uId in lstUserIDNew)
                    {
                        if (!lstUserOld.Any(x => x.Id.ToString() == uId))
                        {
                            userInRegion = new Entities.UserInRegion();
                            userInRegion.UserId = int.Parse(uId);
                            userInRegion.RegionId = entity.Id;
                            userInRegion.Status = true;
                            userInRegionService.Save(userInRegion);
                            //userInRegionService.Insert(userInRegion);
                        }
                    }
                }
            }

            if (isUpdateLast)
            {
                UpdateRegionToDB(listRegionLast);
            }
        }

        private int InsertCity(string city, PlaceService placeService)
        {

            Entities.Place e = new Entities.Place();
            int type = city.Contains("Thành phố") ? 1 : 2; // Nếu có chữ thành phố thì là thành phố (1), không thì sẽ là tỉnh (2)
            string text = city.Replace("Thành phố", "");
            text = text.Replace("Tỉnh", "");
            text = text.Trim();
            e.Type = (short)type;
            e.Text = text;
            e.Parent = 0;
            return placeService.Insert(e);
        }

        private int InsertDistrict(string district, int cityId, PlaceService placeService)
        {
            Entities.Place e = new Entities.Place();
            int type = district.Contains("Quận") ? 3 : district.Contains("Huyện") ? 4 : 5; // Nếu có chữ Quận --> 3, Huyện --> 4, Thị Xã -->5
            string text = district.Replace("Quận", "");
            text = text.Replace("Huyện", "");
            text = text.Replace("Thị xã", "");
            text = text.Trim();
            e.Type = (short)type;
            e.Text = text;
            e.Parent = cityId;
            return placeService.Insert(e);
        }

        private int InsertWard(string ward, int districtId, PlaceService placeService)
        {
            Entities.Place e = new Entities.Place();
            int type = ward.Contains("Phường") ? 6 : ward.Contains("Xã") ? 7 : 8; // Nếu có chữ Phường --> 6, Xã --> 7, Thị trấn -->8
            string text = ward.Replace("Phường", "");
            text = text.Replace("Xã", "");
            text = text.Replace("Thị trấn", "");
            text = text.Trim();
            e.Type = (short)type;
            e.Text = text;
            e.Parent = districtId;
            e.MaxAgency = 2;
            e.MaxAgencyFree = 2;
            return placeService.Insert(e);
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }

        [HttpGet]
        public PartialViewResult UpdateRoleConfig(int id = 0)
        {
            settingModel.RoleSetting = GetRoleSetting(id);
            settingModel.MailSetting = new Models.MailModel();
            return PartialView("_RolePartial", settingModel.RoleSetting);
        }
        
        [HttpPost]
        public ActionResult UpdateRoleConfig(int id, Models.RoleSettingModel m)
        {
            // Xử lý cập nhật thông tin cho Role
            if (id > 0)
            {
                RoleService roleService = new RoleService();
                Entities.Role e = roleService.GetById(id);
                if (e != null)
                {
                    e.Post = m.Post;
                    e.ResiveFromAgency = m.ResiveFromAgency;
                    e.ResiveFromMember = m.ResiveFromMember;
                    e.ResiveRegionNum = m.ResiveRegionNum;
                    e.SendRegionNum = m.SendRegionNum;

                    roleService.Save(e);
                }
                else
                {
                    return PartialView("_RolePartial", settingModel.RoleSetting);
                }
            }

            settingModel.RoleSetting = GetRoleSetting(id);
            return RedirectToAction("Index","Setting");
        }

        [HttpGet]
        public PartialViewResult UpdateMailConfig()
        {

            return PartialView("index", settingModel);
        }

        [HttpPost]
        public ActionResult UpdateMailConfig(Models.MailModel m)
        {
            return PartialView("index", settingModel);
        }

        public SettingCommonModel GetDataSetting(string settingName)
        {
            List<Entities.Setting> lst = _settingService.List(settingName);
            SettingCommonModel model = new SettingCommonModel();
            model.list = (from e in lst
                          select new SettingCommonModel()
                          {
                              Id = e.Id,
                              SettingName = e.SettingName,
                              Title = e.Title,
                              Value = e.Value,
                              Display = e.Display
                          }).ToList();
            return model;
        }

        [HttpPost]
        public ActionResult UpdateSettingTable(Models.SettingCommonModel m)
        {
            var strTitle = Request.Form["setting.Title"];
            var strValue = Request.Form["setting.Value"];
            string settingName = Request.Form["settingName"];
            var arrTitle = strTitle.Split(',');
            var arrValue = strValue.Split(',');
            for (int i = 0; i < arrTitle.Length; i++)
            {
                Entities.Setting setting = _settingService.Get(settingName, arrTitle[i]);
                setting.Value = arrValue[i].ToString();

                _settingService.Update(setting);
            }

            return RedirectToAction("Index", "Setting");
        }

        public ActionResult DBQuery()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DBQuery(string Query)
        {
            _settingService.UpdateDB(Query, 1);
            return View();
        }

        public PartialViewResult UpdateData()
        {
            return PartialView("_UpdateData");
        }

        [HttpPost]
        public JsonResult UpdateAlias()
        {
            string result = _settingService.UpdateAlias();
            return Json(result);
        }

        //[HttpPost]
        //public ActionResult UpdateData()
        //{
        //    _settingService.UpdateData();

        //    return RedirectToAction("Index", "Setting");
        //}
    }
}
