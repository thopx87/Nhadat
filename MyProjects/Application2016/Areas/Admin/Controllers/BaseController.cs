using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using BusinessLayer.Enums;
using Application2016.Helpers;

namespace Application2016.Areas.Admin.Controllers
{

    public class BaseController : Controller
    {
        protected static int Page;
        protected static List<string> Query;
        protected int pageSize = AdminConfigs.PAGE_SIZE;
        UserService userService = new UserService();
        
        public BaseController()
        {
            
        }

        /// <summary>
        /// Xử lý redirect khi có lỗi.
        /// -6 : Chưa đăng nhập.
        /// -7 : Không có quyền.
        /// </summary>
        /// <param name="errorId"></param>
        /// <returns></returns>
        public ActionResult Redirect(int errorId)
        {
            string oldUrl = string.Empty;
            if (Request.UrlReferrer != null)
            {
                oldUrl = Request.UrlReferrer.ToString();
            }
                    
            switch (errorId)
            {
                case (int)Errors.NOT_LOGIN:

                    TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.ERROR_NOT_LOGIN;
                    return RedirectToAction("Login", "User", new { returnUrl = oldUrl });
                case (int)Errors.ROLE_WRONG:
                    TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.ERROR_ROLE_WRONG;
                    return RedirectToAction("Login", "User", new { returnUrl = oldUrl });
                default:
                    return null;
            }
        }

        public void InitAccountInfo(Entities.User user)
        {
            if (user != null)
            {
                UserInRolesService uir = new UserInRolesService();
                int roleId = uir.GetFirstRoleIdByUser(user.Id);
                
                CookieHelper.Set(AdminConfigs.COOKIES_USERNAME, user.UserName);
                CookieHelper.Set(AdminConfigs.COOKIES_AVATAR, user.Avatar);
                CookieHelper.Set(AdminConfigs.COOKIES_ROLE_ID, roleId.ToString());
                CookieHelper.Set(AdminConfigs.COOKIES_USER_ID, user.Id.ToString());

                string isAdmin = CheckIsAdmin() ? "1" : "0";
                CookieHelper.Set(AdminConfigs.COOKIES_ADMIN, isAdmin);

                string isAgency = CheckIsAgency() ? "1" : "0";
                CookieHelper.Set(AdminConfigs.COOKIES_AGENCY, isAgency);

                TempData[AdminConfigs.TEMP_USERNAME] = user.UserName;
                TempData[AdminConfigs.TEMP_USER_ID] = user.Id.ToString();
                TempData[AdminConfigs.TEMP_AVATAR] = user.Avatar;                
            }
        }

        public void InitAccountInfo(string userId, string userName, string userAvatar)
        {

            CookieHelper.Set(AdminConfigs.COOKIES_USER_ID, userId);
            CookieHelper.Set(AdminConfigs.COOKIES_USERNAME, userName);
            CookieHelper.Set(AdminConfigs.COOKIES_AVATAR, userAvatar);
            //CookieHelper.Set(AdminConfigs.COOKIES_ROLE_ID , user.UserRoles);
            TempData[AdminConfigs.TEMP_USERNAME] = userName;
            TempData[AdminConfigs.TEMP_USER_ID] = userId;
            TempData[AdminConfigs.TEMP_AVATAR] = userAvatar;
        }

        /// <summary>
        /// Kiểm tra điều kiện đầu vào.
        /// </summary>
        /// <param name="checkLogin"></param>
        /// <param name="checkRole"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public ActionResult CheckInputCondition(bool checkLogin = true, bool checkRole = false, int[] roles = null)
        {
            // Kiểm tra đăng nhập.
            if (checkLogin)
            {
                if (!CheckLogon())
                {
                    return Redirect((int)Errors.NOT_LOGIN);
                }
            }

            // Kiểm tra quyền
            if (checkRole)
            {
                if (roles != null)
                {
                    int res = CheckRole(roles);
                    return Redirect(res);
                }
            }
            return null;
        }

        /// <summary>
        /// Kiểm tra quyền người dùng
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public int CheckRole(int role = -1)
        {
            if (role <0) return 1;
            UserInRolesService uirService = new UserInRolesService();
            int userId = 0;
            int.TryParse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID), out userId);
            if (userId == 0)
            {
                return (int)Errors.NOT_LOGIN;
            }

            if (!uirService.CheckUserRole(role, userId))
            {
                return (int)Errors.ROLE_WRONG;
            }

            // Nếu không có lỗi thì gán cookie để hiển thị lên website.
            TempData[AdminConfigs.TEMP_USERNAME] = CookieHelper.Get(AdminConfigs.COOKIES_USERNAME);
            TempData[AdminConfigs.TEMP_USER_ID] = CookieHelper.Get(AdminConfigs.COOKIES_USER_ID);
            TempData[AdminConfigs.TEMP_AVATAR] = CookieHelper.Get(AdminConfigs.COOKIES_AVATAR);
            return 1;
        }

        /// <summary>
        /// Kiểm tra quyền người dùng
        /// </summary>
        /// <param name="arrRole"></param>
        /// <returns></returns>
        public int CheckRole(int[] arrRole)
        {
            if (arrRole == null) return 1;
            UserInRolesService uirService = new UserInRolesService();
            int userId = 0;
            int.TryParse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID), out userId);
            if (userId == 0)
            {
                return (int)Errors.NOT_LOGIN;
            }

            if (!uirService.CheckUserRole(arrRole, userId))
            {
                return (int)Errors.ROLE_WRONG;
            }
            return 1;
        }

        public int CheckRole(Enums.Roles[] arrRole)
        {
            int[] arrRoleInt = Array.ConvertAll(arrRole, value => (int)value);

            if (arrRoleInt == null) return 1;
            UserInRolesService uirService = new UserInRolesService();
            int userId = 0;
            int.TryParse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID), out userId);
            if (userId == 0)
            {
                return (int)Errors.NOT_LOGIN;
            }

            if (!uirService.CheckUserRole(arrRoleInt, userId))
            {
                return (int)Errors.ROLE_WRONG;
            }
            return 1;
        }

        public int CheckRolePost(ref Entities.Item e)
        {
            int userId = 0;
            int.TryParse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID), out userId);
            if (userId == 0)
            {
                return (int)Errors.NOT_LOGIN;
            }
            UserInRolesService uirService = new UserInRolesService();
            var result = uirService.GetRoleAllowPost(userId);
            if (result == null)
            {
                return (int)Errors.ROLE_WRONG;
            }
            e = result;
            return 1;
        }

        public bool CheckRoleUpdate(int loginUser, int createByUser)
        {
            UserInRolesService uir = new UserInRolesService();

            bool isAdmin = uir.CheckUserIsAdmin(loginUser);
            if (!isAdmin)
            {
                return false;
            }
            return true;
        }

        public JsonResult CheckAgency()
        {
            return Json(CheckIsAgency());
        }

        /// <summary>
        /// Kiểm tra đăng nhập.
        /// </summary>
        /// <returns></returns>
        public bool CheckLogon()
        {
            string username = CookieHelper.Get(AdminConfigs.COOKIES_USERNAME);
            int id = 0;
            int.TryParse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID), out id);

            if (username != "" && id > 0)
            {
                bool chk = userService.CheckIdAndUserName(id, username);
                if (chk)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckIsAgency()
        {
            int userId = 0;
            int.TryParse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID), out userId);
            UserInRolesService uirService = new UserInRolesService();
            bool result = uirService.CheckUserIsAgency(userId);
            return result;
        }

        public bool CheckIsAdmin()
        {
            string username = CookieHelper.Get(AdminConfigs.COOKIES_USERNAME);
            int id = 0;
            int.TryParse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID), out id);

            if (username != "" && id > 0)
            {
                UserInRolesService uir = new UserInRolesService();

                bool chk = uir.CheckUserIsAdmin(id);

                return chk;
            }
            return false;
        }

        //
        // Phân trang.
        public void Paging(int page, int totalRecord)
        {
            ViewBag.page = page;
            ViewBag.pageSize = pageSize;

            ViewBag.MaxPage = (int)Math.Ceiling((double)totalRecord / pageSize);
            ViewBag.PageShow = AdminConfigs.PAGE_SHOW;
            ViewBag.PagePreview = AdminConfigs.PAGE_PREVIEW;
            ViewBag.TotalRecord = totalRecord;
        }

        public ActionResult Message()
        {
            return View();
        }

        /// <summary>
        /// Xử lý đăng xuất
        /// </summary>
        /// <returns></returns>
        public ActionResult DoLogout()
        {
            Helpers.CookieHelper.Remove(AdminConfigs.COOKIES_USERNAME);
            Helpers.CookieHelper.Remove(AdminConfigs.COOKIES_USER_ID);
            Helpers.CookieHelper.Remove(AdminConfigs.COOKIES_ADMIN);
            Helpers.CookieHelper.Remove(AdminConfigs.COOKIES_AVATAR);
            Helpers.CookieHelper.Remove(AdminConfigs.COOKIES_ROLE_ID);
            Helpers.CookieHelper.Remove(AdminConfigs.COOKIES_AGENCY);

            return Redirect("~");
        }

        /// <summary>
        /// Hàm lấy danh sách place con theo id cha, trả về 1 chuỗi JSon.
        /// </summary>
        /// <param name="parentId">ID City</param>
        /// <returns>Json</returns>
        [HttpPost]
        public JsonResult ListPlaceJson(int parentId)
        {
            PlaceService placeService = new PlaceService();
            List<Entities.Item> lstEntity = placeService.ListPlaceItemByParent(parentId);
            lstEntity.Insert(0, new Entities.Item() { Id=0, Text="" });
            return Json(lstEntity);
        }

        /// <summary>
        /// Lấy danh sách user theo role
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <returns>List user</returns>
        public JsonResult ListUserByRole(int roleId,int regionId, bool isAgency = false, bool getItem = false)
        {
            if (getItem)
            {
                List<Entities.Item> lstUserByRole = !isAgency ? userService.ListItemByRoleId(roleId) : userService.ListAgency();
                RoleService roleService = new RoleService();
                if (roleService.CheckRoleAgency(roleId))
                {
                    if (regionId > 0)
                    {
                        UserInRegionService uirService = new UserInRegionService();
                        List<int> lstUserByRegion = new List<int>();
                        lstUserByRegion = uirService.ListUserIdByRegion(regionId);
                        lstUserByRole = lstUserByRole.Where(x => lstUserByRegion.Contains(x.Id)).ToList();
                    }
                }
                return Json(lstUserByRole);
            }
            else
            {
                var result = userService.ListUserItemByRoleId(roleId);
                return Json(result);
            }
        }

        /// <summary>
        /// Lấy danh sách user theo vùng
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public JsonResult ListUserByRegion(int regionId)
        {
            var result = userService.ListUserItemByRegion(regionId);
            return Json(result);
        }

        /// <summary>
        /// Lấy danh sách đường làng
        /// </summary>
        /// <param name="wardId"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public JsonResult ListRoadAndVillageJson(int wardId, int regionId)
        {
            Road_VillageService roadService = new Road_VillageService();
            var result = roadService.ListItem(wardId, regionId);
            return Json(result);
        }

        public JsonResult ListPlaceByRegion(int regionId)
        {
            PlaceService placeService = new PlaceService();
            List<Entities.Item> lstEntity = placeService.ListPlaceItemOfRegion(regionId);
            return Json(lstEntity);
        }

        public JsonResult ListPlaceByRegions(int[] regionIds)
        {
            PlaceService placeService = new PlaceService();
            List<Entities.Item> allEntity = new List<Entities.Item>();
            if (regionIds != null)
            {
                List<Entities.Item> lstEntity;
                foreach (int regionId in regionIds)
                {
                    lstEntity = placeService.ListPlaceItemOfRegion(regionId);
                    if (lstEntity.Count > 0)
                    {
                        allEntity.AddRange(lstEntity);
                    }
                }
                allEntity.Insert(0, new Entities.Item() { Id = 0, Text = "" });
                return Json(allEntity);
            }

            return Json(allEntity);
        }

        [HttpPost]
        public JsonResult ListRegionJson(int cityId, int districtId)
        {
            RegionService regionService = new RegionService();
            List<Entities.Item> listRegion = new List<Entities.Item>();
            listRegion = regionService.ListItem(cityId, districtId);
            listRegion.Insert(0, new Entities.Item() { Id = 0, Text = "" });
            return Json(listRegion);
        }

        public string StringPlaceByRegions(int regionId)
        {
            PlaceService placeService = new PlaceService();
            List<Entities.Item> lstEntity;
            string strPlace = "";
            lstEntity = placeService.ListPlaceItemOfRegion(regionId);
            if (lstEntity.Count > 0)
            {
                foreach (Entities.Item e in lstEntity)
                {
                    strPlace += e.Text + ", ";
                }
            }
            return strPlace;
        }

        public string StringPlaceByRegions(List<int> regionIds)
        {
            PlaceService placeService = new PlaceService();
            string strPlace = "";
            if (regionIds != null)
            {
                List<Entities.Item> lstEntity;
                foreach (int regionId in regionIds)
                {
                    lstEntity = placeService.ListPlaceItemOfRegion(regionId);
                    if (lstEntity.Count > 0)
                    {
                        foreach (Entities.Item e in lstEntity)
                        {
                            strPlace += e.Text + ", ";
                        }
                    }
                }
            }
            return strPlace;
        }

        /// <summary>
        /// Lấy thông tin vùng theo xã phường.
        /// </summary>
        /// <param name="wardId"></param>
        /// <returns></returns>
        public JsonResult GetRegionByWard(int wardId)
        {
            RegionService regionService = new RegionService();
            var result = regionService.GetRegionByWard(wardId);
            if (result == null) result = new Entities.Item();
            return Json(result);
        }

        public JsonResult GetNeighBorRegion(int regionId)
        {
            RegionService regionService = new RegionService();
            var result = regionService.GetNeighBorRegion(regionId);
            return Json(result);
        }

        public ActionResult Notification()
        {
            ProductService productService = new ProductService();
            int userId = 0;
            int.TryParse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID), out userId);
            ViewBag.ListProductWantSell = productService.GetListProductWantSell(userId, false);

            return PartialView("_HeaderPartial_Notifications");
        }

        public JsonResult TestConnectEmail()
        {            
            return Json(BusinessLayer.Helpers.MailHelper.TestConnect());
        }
    }
}
