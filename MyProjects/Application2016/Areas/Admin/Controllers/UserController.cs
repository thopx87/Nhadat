using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using BusinessLayer.Enums;
using BusinessLayer.Helpers;
using Application2016.Helpers;
using CaptchaMvc.HtmlHelpers;
using Entities;
using System.Text.RegularExpressions;
using Application2016.Enums;
using Application2016.Areas.Admin.Models;

namespace Application2016.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        private UserService userService = new UserService();
        private UserInRolesService userInRoleService = new UserInRolesService();
        private RoleService roleService = new RoleService();

        public UserController()
        {
            ViewBag.ActionMenu = "AdminSetting";
            ViewBag.ActionSubMenu = "User";
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
        
        public ActionResult Index(int page, string query)
        {
            return View();
        }

        public ActionResult List(UserCondition Condition)
        {
            if (Condition.Page <= 0) Condition.Page = 1;
            int errorId = CheckRole(_roles);
            if (errorId < 0)
            {
                return Redirect(errorId);
            }
            if (!userInRoleService.CheckUserRole(-1))
            {
                ViewBag.Message = AdminConfigs.ERROR_ROLE_WRONG;
                return View("Message");
            }

            // Gán lại page từ base controller
            Page = Condition.Page;
            ListUserModel listUser = new ListUserModel();
            listUser.Condition = Condition;
            LoadData(ref listUser);

            return View(listUser);
        }

        /// <summary>
        /// Xử lý load dữ liệu vào trang index và các trang sử dụng index.
        /// </summary>
        /// <param name="userModels"></param>
        private void LoadData(ref ListUserModel userModels)
        {
            List<Models.UserModel> lstUser = new List<Models.UserModel>();
            Models.UserModel m;

            int totalRecord = 0;
            List<Entities.User> lstEntity = new List<User>();
            UserCondition conditions = userModels.Condition != null ? userModels.Condition : new UserCondition();
            lstEntity = userService.List(conditions.RegionId, conditions.RoleId, conditions.SearchText, conditions.Page, pageSize, out totalRecord);
            
            if (lstEntity.Count > 0)
            {
                userModels.Id = 1;
                foreach (Entities.User e in lstEntity)
                {
                    m = new Models.UserModel();
                    m.MapFrom(e, ref m);
                    lstUser.Add(m);
                }
            }
            // Add paramater
            ViewBag.NumberItem = lstEntity.Count;

            // Paging
            ViewBag.query = Query;
            Paging(conditions.Page, totalRecord);

            userModels.lstUser = lstUser;                        

            // Get List Role
            List<Entities.Item> lstItem;

            // Get List Region
            UserInRegionService userInRegionService = new UserInRegionService();
            foreach (var user in userModels.lstUser)
            {
                lstItem = roleService.ListItem();
                user.ListRole = lstItem;
                user.ListUserInRegionSend = new ListUserInRegionModel();
                user.ListUserInRegionSend.ListItem = userInRegionService.GetListItemByUser(user.Id);
                user.ListUserInRegionReceive = new ListUserInRegionModel();
                user.ListUserInRegionReceive.ListItem = userInRegionService.GetListItemByUser(user.Id, 2);
            }


            // Get list regions.
            RegionService regionService = new RegionService();
            lstItem = null;
            lstItem = regionService.ListItemActive();
            lstItem.Insert(0, new Item { Text = "-- Chọn vùng --", Id = -1 });
            userModels.ListAllRegion = lstItem;
            
            // Get list role.
            lstItem = null;
            lstItem = roleService.ListItem();
            lstItem.Insert(0, new Item { Text = "-- Chọn quyền --", Id = -1 });
            userModels.ListRole = lstItem;
            userModels.Condition = conditions;
        }

        /// <summary>
        /// Cập nhật thông tin cho các user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userModels"></param>
        /// <param name="selectedUser"></param>
        /// <returns></returns>
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult List(ListUserModel model, int[] selectedUser)
        {
            try
            {
                if (selectedUser != null)
                {
                    Entities.User user;
                    Entities.UserInRole userInRole;
                    List<Entities.UserInRole> lstUserInRoleOld;
                    foreach (int userId in selectedUser)
                    {
                        // Lấy ra thông tin cũ của User.
                        user = userService.GetById(userId);
                        Models.UserModel m = model.lstUser.Where(x => x.Id == userId).FirstOrDefault();

                        user.Status = m.Status;

                        // Cập nhật trạng thái
                        userService.Update(user);

                        // Cập nhật phân quyền người dùng.

                        // Danh sách quyền người dùng mới.
                        int[] arrNewRole = m.NewRoles;
                        // Danh sách quyền người dùng cũ.
                        lstUserInRoleOld = userInRoleService.List(m.Id);

                        // Nếu quyền mới rỗng thì update lại những quyền cũ, đưa về trạng thái false.
                        if (arrNewRole == null)
                        {
                            if (lstUserInRoleOld != null)
                            {
                                foreach (var uir in lstUserInRoleOld)
                                {
                                    uir.State = false;
                                    userInRoleService.Update(uir);
                                }
                            }
                        }
                        else
                        {
                            // Nếu có quyền người dùng mới thì kiểm tra xem quyền đó đã tồn tại chưa.
                            // Nếu tồn tại thì update, chưa tồn tại thì thêm mới.
                            foreach (int newRoleId in arrNewRole)
                            {
                                if (lstUserInRoleOld.Select(x => x.RolesId).Contains(newRoleId))
                                {
                                    userInRole = lstUserInRoleOld.Where(u => u.RolesId == newRoleId).FirstOrDefault();
                                    userInRole.State = true;
                                    userInRoleService.Update(userInRole);
                                }
                                else
                                {
                                    userInRole = new UserInRole();
                                    userInRole.RolesId = newRoleId;
                                    userInRole.UserId = userId;
                                    userInRole.State = true;
                                    userInRoleService.Insert(userInRole);
                                }
                            }

                            // Cập nhật quyền cho những quyền là quyền cũ nhưng không có trong quyền mới.
                            foreach (var uri in lstUserInRoleOld)
                            {
                                if (!arrNewRole.Contains(uri.RolesId))
                                {
                                    uri.State = false;
                                    userInRoleService.Update(uri);
                                }
                            }

                            // Region Service
                            RegionService regionService = new RegionService();

                            // Xóa đi những vùng gửi cũ.
                            UserInRegionService userInRegionService = new UserInRegionService();
                            userInRegionService.DeleteByUser(userId, 1);
                            // Cập nhật lại danh sách vùng gửi
                            int[] listRegion;
                            if (m.ListUserInRegionSend != null)
                            {
                                listRegion = m.ListUserInRegionSend.regionIds;
                                if (listRegion.Count() > 0)
                                {

                                    UserInRegion userInRegion;
                                    foreach (int regionId in listRegion)
                                    {
                                        userInRegion = new UserInRegion();
                                        userInRegion.RegionId = regionId;
                                        userInRegion.UserId = userId;
                                        userInRegion.Status = true;
                                        userInRegion.RegionType = 1; // vùng gửi.

                                        // Lấy thông tin tỉnh, huyện theo vùng mới.
                                        var region = regionService.GetById(regionId);
                                        userInRegion.CityId = region.CityId;
                                        userInRegion.DistrictId = region.DistrictId;

                                        userInRegionService.Save(userInRegion);
                                    }
                                }
                            }
                            // Xóa đi những vùng gửi cũ.
                            userInRegionService.DeleteByUser(userId, 2);
                            // Cập nhật lại danh sách vùng nhận                        
                            if (m.ListUserInRegionReceive != null)
                            {
                                listRegion = m.ListUserInRegionReceive.regionIds;
                                if (listRegion.Count() > 0)
                                {

                                    UserInRegion userInRegion;
                                    foreach (int regionId in listRegion)
                                    {
                                        userInRegion = new UserInRegion();
                                        userInRegion.RegionId = regionId;
                                        userInRegion.UserId = userId;
                                        userInRegion.Status = true;
                                        userInRegion.RegionType = 2; // vùng nhận

                                        // Lấy thông tin tỉnh, huyện theo vùng mới.
                                        var region = regionService.GetById(regionId);
                                        userInRegion.CityId = region.CityId;
                                        userInRegion.DistrictId = region.DistrictId;

                                        userInRegionService.Save(userInRegion);
                                    }
                                }
                            }
                        }
                    }
                }
                TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_SUCCESS;
            }
            catch
            {
                TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_ERROR;
            }

            LoadData(ref model);
            return View(model);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteMultiple(int[] usersDelete)
        {
            int result = userService.DeleteMultiple(usersDelete);
            return Json(result);
        }

        /// <summary>
        /// Hiển thị trang đăng nhập
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            // Kiểm tra trạng thái đăng nhập từ cookie.
            // Nếu đã đăng nhập trước đó rồi thì sẽ chuyển đến trang cá nhân.
            if (CheckLogon())
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("PersonalInfo");
                }
                
            }
            ViewBag.ReturnURL = returnUrl;
            return View();
        }

        /// <summary>
        /// Xử lý đăng nhập
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(Models.LoginModel acc, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                UserService userService = new UserService();
                string username = "";
                int result = userService.CheckLogin(acc.Username, acc.Password, out username);
                if (result < 0)
                {
                    string strErr = "";
                    switch (result)
                    {
                        case (int)Errors.NOT_EXIST:
                            strErr = "Tài khoản bạn nhập không đúng. Xin vui lòng kiểm tra lại!";
                            break;
                        case (int)Errors.BLOCK:
                            strErr = "Tài khoản của bạn đang bị khóa. Xin vui lòng liên lạc với {0}";
                            break;
                    }
                    ModelState.AddModelError("", strErr);
                    return View("Login", acc);
                }

                // Tạo cookie cho tài khoản.
                var user = userService.GetById(result);
                InitAccountInfo(user);

                // Kiểm tra danh sách nhà muốn bán
                ProductService productService = new ProductService();
                var lstWantSell = productService.GetListProductWantSell(user.Id, false);
                //if (lstWantSell.Count > 0)
                //{
                //    TempData[AdminConfigs.TEMP_MESSAGE] = "Có nhà muốn bán cho bạn!";
                //}

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("PersonalInfo", "User");
            }
            return View("Login", acc);
        }

        /// <summary>
        /// Xử lý hiển thị trang đăng ký
        /// </summary>
        /// <param name="Type"> 0 --> Thành viên thường. 1 --> môi giới.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Register(int Type = 0)
        {
            Models.RegisterModel model = new Models.RegisterModel();
            model.Type = Type;
            return View(model);
        }

        /// <summary>
        /// Xử lý đăng ký
        /// </summary>
        /// <param name="model"></param>
        /// <param name="CaptchaText"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(Models.RegisterModel model, string CaptchaText)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!this.IsCaptchaValid(""))
            {
                ViewBag.ErrMessage = "Captcha không đúng. Xin hãy nhập lại";
                return View();
            }

            // Kiểm tra sự tồn tại của username
            bool result = userService.CheckExistsUsername(model.UserName);
            if (result)
            {
                ModelState.AddModelError(string.Empty, AdminConfigs.ERROR_USERNAME_EXISTS);
                return View();
            }

            // Kiểm tra sự tồn tại của email.
            result = userService.CheckExistsEmail(model.Email);
            if (result)
            {
                ModelState.AddModelError(string.Empty, AdminConfigs.ERROR_EMAIL_EXISTS);
                return View();
            }

            // Mã hóa Password
            model.Password = EncryptionHelper.Encrypt(model.Password);

            // Mã hóa email để lấy code cho người dùng xác nhận bằng email.
            string EnUser = EncryptionHelper.GetMD5HashData(model.Email);

            // Cập nhật thông tin cho người dùng.
            Entities.User u = new Entities.User();
            u.DateCreate = DateTime.Now;
            u.DateLogin = DateTime.Now;
            u.DateUpdate = DateTime.Now;
            u.UserName = model.UserName;
            u.Email = model.Email;
            u.Password = model.Password;
            u.Phone = model.PhoneNumber;
            u.Status = false; // Trạng thái này sẽ thành true sau khi người dùng confirm.
            u.CodeActive = EnUser;
            u.Delete = false;
            var us = userService.Insert(u);
            if (us > 0)
            {
                // Lấy ID vừa sinh của user
                u.Id = us;

                // Tạo quyền cho người dùng.
                UserInRole userInRole = new UserInRole();
                userInRole.UserId = u.Id;
                userInRole.State = false;
                userInRole.RolesId = (int)Roles.Member_Normal;
                userInRoleService.Insert(userInRole);

                // Kiểm tra kiểu đăng ký
                if (model.Type == 1)
                {
                    // Chuyển tới trang đăng ký môi giới.
                   return RedirectToAction("RegistryAgency", "User", new { area = "Admin", Id = u.Id });
                }
                else
                {
                    // Send email
                    Uri uri = Request.Url;
                    string urlHost = uri.GetLeftPart(UriPartial.Authority);
                    if (MailHelper.MailRegister(u, urlHost, u.CodeActive))
                    {
                        return View("RegistrySuccess");
                    }
                }
            }
            return View(model);
        }

        /// <summary>
        /// Xử lý hiển thị trang quên mật khẩu
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Xử lý quên mật khẩu
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            // Kiểm tra định dạng email
            if (!Regex.IsMatch(Email, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
            {
                ModelState.AddModelError(string.Empty, "Email không đúng định dạng.");
                return View();
            }

            // Kiểm tra sự tồn tại của email.
            if (!userService.CheckExistsEmail(Email))
            {
                ModelState.AddModelError(string.Empty, "Email không tồn tại trong hệ thống.");
                return View();
            }

            // Gửi email để lấy lại mật khẩu.
            var user = userService.GetByEmail(Email);

            Uri uri = Request.Url;
            string urlHost = uri.GetLeftPart(UriPartial.Authority);
            if (MailHelper.MailPasswordRetrieval(user, urlHost))
            {
                ViewBag.Message = "Đăng ký lấy lại mật khẩu thành công. Xin hãy kiểm tra mail để lấy lại mật khẩu!";
                return View("Message");
            }
            return View();
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(Models.ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (!CheckLogon())
            {
                return Redirect((int)Errors.NOT_LOGIN);
            }

            // Lấy thông tin tài khoản.
            User user = userService.GetById(int.Parse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID)));

            // Kiểm tra thông tin mật khẩu cũ
            if (user.Password != EncryptionHelper.Encrypt(model.OldPassword))
            {
                // Mật khẩu không đúng.
                ModelState.AddModelError(string.Empty, "Mật khẩu cũ không chính xác");
                return View();
            }

            user.Password = EncryptionHelper.Encrypt(model.NewPassword);
            userService.Update(user);
            TempData[AdminConfigs.TEMP_MESSAGE] = "Bạn đã đổi mật khẩu thành công!";

            return RedirectToAction("PersonalInfo", "User");
        }

        /// <summary>
        /// Trang đổi mật khẩu.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PasswordRetrieval(int id, string code)
        {
            ViewBag.Code = code;
            return View();
        }

        /// <summary>
        /// Trang xử lý đổi mật khẩu
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Password"></param>
        /// <param name="Repassword"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PasswordRetrieval(string Code, string Password, string Repassword, int Key)
        {
            // Kiểm tra mật khẩu
            if (Password.Length < 6 || Password.Length > 100)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu phải từ 6 đến 100 ký tự");
                return View();
            }

            // Kiểm tra 2 chỗ nhập mật khẩu
            if (Password != Repassword)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu không khớp.");
                return View();
            }

            // Kiểm tra key
            if (Key <= 0)
            {
                ModelState.AddModelError(string.Empty, "Bạn chưa nhập mã.");
                return View();
            }

            // Kiểm tra key, code
            var user = userService.GetById(Key);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "mã không đúng");
                return View();
            }

            // Kiểm tra code có khớp với code lưu trong cơ sở dữ liệu không.
            if (Code != user.CodeActive)
            {
                ModelState.AddModelError(string.Empty, "Đường link không đúng hoặc tài khoản có vấn đề. Vui lòng liên hệ với admin để biết thêm chi tiết");
                return View();
            }
            user.Password = EncryptionHelper.Encrypt(Password);

            if (userService.Update(user) > 0)
            {
                // Đăng nhập vào hệ thống và chuyển đến trang cá nhân.
                Helpers.CookieHelper.Set(AdminConfigs.COOKIES_USERNAME, user.UserName, 1);
                Helpers.CookieHelper.Set(AdminConfigs.COOKIES_USER_ID, Key.ToString(), 1);

                TempData[AdminConfigs.TEMP_USERNAME] = user.UserName;
                TempData[AdminConfigs.TEMP_USER_ID] = Key.ToString();

                TempData[AdminConfigs.TEMP_MESSAGE] = "Cập nhật mật khẩu mới thành công!";
                return RedirectToAction("PersonalInfo", "User");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Cập nhật thất bại. Vui lòng liên hệ với admin để biết thêm chi tiết.");
                return View();
            }

        }

        /// <summary>
        /// Trang xác nhận đăng ký
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ConfirmRegister(int id, string code)
        {
            var user = userService.GetById(id);
            // Nếu không tồn tại hoặc lỗi Code active
            if(user == null|| user.CodeActive == ""){
                ViewBag.Message = "Kích hoạt tài khoản thất bại. Xin hãy đăng ký lại!";
                ViewBag.State = "ERROR";
                return View();
            }

            // Trường hợp đã được kích hoạt.
            if (user.Status)
            {
                ViewBag.Message = "Tài khoản đã được kích hoạt thành công!";
                ViewBag.State = "DONE";
                return View();
            }

            if (user.CodeActive == code)
            {
                // Cập nhật trạng thái cho user
                user.Status = true;
                var res= userService.DoConfirmRegisMember(user);
                if (res > 0)
                {
                    ViewBag.Message = "Tài khoản đã được kích hoạt. Xin hãy đăng nhập để sử dụng website!";
                    ViewBag.State = "DONE";
                }
                else
                {
                    ViewBag.Message = "Kích hoạt tài khoản thất bại. Xin hãy đăng ký lại!";
                    ViewBag.State = "ERROR";
                }
            }
            else
            {
                ViewBag.Message = "Kích hoạt tài khoản thất bại. Xin hãy đăng ký lại!";
                ViewBag.State = "ERROR";
            }

            return View();
        }

        /// <summary>
        /// Trang hiển thị thông tin cá nhân.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PersonalInfo()
        {
            if (!CheckLogon())
            {
                return Redirect((int)Errors.NOT_LOGIN);
            }

            ModelState.Clear();
            Models.UserModel m = new Models.UserModel();

            LoadDataPersonalInfo(int.Parse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID)), ref m);
            ViewBag.ActionSubMenu = "";
            return View(m);
        }

        [HttpGet]
        public ActionResult PersonalView(UserCondition condition)
        {
            if (condition == null)
            {
                return Redirect((int)Errors.ROLE_WRONG);
            }
            if (condition.id == 0)
            {
                return null;
            }

            // Check Login user is Admin
            if (!CheckIsAdmin())
            {
                return Redirect((int)Errors.ROLE_WRONG);
            }

            Models.UserViewModel m = new UserViewModel();
            m.Condition = condition;

            Models.UserModel temp = new UserModel();
            
            LoadDataPersonalInfo(condition.id, ref temp, false);

            m.User = temp;

            int total = 0;
            m.ListUser = userService.ListItem(condition.RegionId, condition.RoleId, condition.SearchText, 1, 100, out total);

            List<Entities.Item> lstItem = new List<Item>();
            // Get list regions.
            RegionService regionService = new RegionService();
            lstItem = null;
            lstItem = regionService.ListItemActive();
            lstItem.Insert(0, new Item { Text = "-- Chọn vùng --", Id = -1 });
            ViewBag.ListRegion = lstItem;

            // Get list role.
            lstItem = null;
            lstItem = roleService.ListItem();
            lstItem.Insert(0, new Item { Text = "-- Chọn quyền --", Id = -1 });
            ViewBag.ListRole = lstItem;

            ViewBag.ActionSubMenu = "";
            return View(m);
        }
        
        private void LoadDataPersonalInfo(int Id, ref Models.UserModel m, bool init=true)
        {            
            PlaceService placeService = new PlaceService();

            // Lấy thông tin người dùng.
            Entities.User user = userService.GetById(Id);
            if (user != null)
            {
                m.MapFrom(user, ref m);
            }

            // Lấy danh sách tỉnh, huyện, xã ,..
            // Lấy danh sách tỉnh.
            m.ward.ListCity = placeService.ListPlaceItemByParent(0);
            Entities.Place place = new Entities.Place();
            if (user.PlaceId != null)
            {
                // Lấy thông tin xã, phường.
                place = placeService.GetById((int)user.PlaceId);
                if (place != null)
                {
                    // Lấy ID quận, huyện.
                    m.ward.Parent = place.Parent;

                    // Lấy thông tin quận, huyện
                    place = placeService.GetById(m.ward.Parent);

                    // Lấy ID tỉnh, thành phố.
                    m.ward.CityId = place.Parent;

                    // Lấy ID xã phường
                    m.ward.Id = (int)user.PlaceId;
                }
            }
            else
            {
                m.ward.CityId = -1;
                m.ward.Parent = -1;
                m.ward.Id = -1;
                m.PlaceId = -1;
            }

            // Lấy danh sách quận huyện.
            if (m.ward.CityId > 0)
            {
                m.ward.ListDistrict = placeService.ListPlaceItemByParent(m.ward.CityId);
            }
            else
            {
                m.ward.ListDistrict = new List<Entities.Item>();
            }

            // Lấy danh sách xã phường.
            if (m.ward.Parent > 0)
            {
                m.ward.ListWard = placeService.ListPlaceItemByParent(m.ward.Parent);
            }
            else
            {
                m.ward.ListWard = new List<Entities.Item>();
            }

            // Lấy thông tin phân vùng gửi (type = 1 - default)
            UserInRegionService userInRegionService = new UserInRegionService();
            m.ListUserInRegionSend = new ListUserInRegionModel();
            m.ListUserInRegionSend.ListItem = userInRegionService.GetListItemByUser(user.Id);

            // Lấy thông tin vùng nhận (type = 2)
            m.ListUserInRegionReceive = new ListUserInRegionModel();
            m.ListUserInRegionReceive.ListItem = userInRegionService.GetListItemByUser(user.Id, 2);
            
            // Lấy thông tin quyền hạn
            m.UserRoles = userInRoleService.GetByUser(user.Id);

            // Lấy thông tin khởi tạo
            if (init)
            {
                InitAccountInfo(m.Id.ToString(), m.UserName, m.Avatar);
            }
        }

        /// <summary>
        /// Trang xử lý thông tin cá nhân.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdatePersonalInfo(int id, Models.UserModel m)
        {
            if (ModelState.IsValid)
            {
                UpdatePersional(m);

                LoadDataPersonalInfo(int.Parse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID)), ref m);
                return RedirectToAction("PersonalInfo", "User");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage);
                 ModelState.AddModelError(string.Empty, string.Join(", ", errors));
                 ViewBag.Message = string.Join(", ", errors);
            }
            LoadDataPersonalInfo(int.Parse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID)), ref m);
            return View("PersonalInfo", m);
        }

        /// <summary>
        /// Đăng ký môi giới
        /// </summary>
        /// <param name="Id">id user</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RegistryAgency(int Id)
        {
            Models.UserModel model = new Models.UserModel();

            LoadDataPersonalInfo(Id, ref model);

            // Lấy số vùng hoạt động.
            var role = roleService.GetByCode("AgencyOnline");

            ListUserInRegionModel listUir = new ListUserInRegionModel();
            // Lấy thông tin vùng gửi
            listUir.regionNum = role.SendRegionNum;
            listUir.ListUserInRegion = GetListUserInRegion(model.Id, role.SendRegionNum, 1);
            listUir.region_city = listUir.ListUserInRegion.Select(x => x.CityId).ToArray();
            listUir.region_district = listUir.ListUserInRegion.Select(x => x.DistrictId).ToArray();
            listUir.region_ward = listUir.ListUserInRegion.Select(x => x.WardId).ToArray();
            model.ListUserInRegionSend = listUir;

            listUir = new ListUserInRegionModel();
            // Lấy thông tin vùng nhận.
            listUir.regionNum = role.ResiveRegionNum;
            listUir.ListUserInRegion = GetListUserInRegion(model.Id, role.ResiveRegionNum, 2);
            listUir.region_city = listUir.ListUserInRegion.Select(x => x.CityId).ToArray();
            listUir.region_district = listUir.ListUserInRegion.Select(x => x.DistrictId).ToArray();
            listUir.region_ward = listUir.ListUserInRegion.Select(x => x.WardId).ToArray();
            model.ListUserInRegionReceive = listUir;

            return View(model);
        }

        /// <summary>
        /// Đăng ký làm môi giới.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RegistryAgency(Models.UserModel model)
        {
            // Cập nhật trạng thái môi giới
            var role = roleService.GetByCode("AgencyOnline");
            if (ModelState.IsValid)
            {
                // Kiểm tra vùng đăng ký môi giới.
                if (!CheckRegionAgency(model))
                {
                    LoadDataPersonalInfo(model.Id, ref model);
                    return View("RegistryAgency", model);
                }

                // Cập nhật thông tin cá nhân.
                UpdatePersional(model);

                // Cập nhật vùng.
                UpdateRegionAgency(model);

                // Xóa trạng thái hiện tại
                userInRoleService.Delete(model.Id);
                
                UserInRole uir = new UserInRole();
                uir.UserId = model.Id;
                uir.RolesId = role.Id;
                uir.State = true;
                userInRoleService.Insert(uir);
                // Send email
                Uri uri = Request.Url;
                string urlHost = uri.GetLeftPart(UriPartial.Authority);

                Entities.User u = userService.GetById(model.Id);
                if (MailHelper.MailRegisterAgency(u, urlHost, u.CodeActive))
                {
                    UserInRole userInRole = new UserInRole();
                    userInRole.UserId = u.Id;
                    userInRole.State = false;
                    userInRole.RolesId = (int)Roles.Agency_Online;
                    userInRoleService.Insert(userInRole);

                    return View("RegistrySuccess");
                }
                LoadDataPersonalInfo(model.Id, ref model);

                ListUserInRegionModel listUir = new ListUserInRegionModel();
                // Lấy thông tin vùng gửi
                listUir.regionNum = role.SendRegionNum;
                listUir.ListUserInRegion = GetListUserInRegion(model.Id, role.SendRegionNum, 1);
                listUir.region_city = listUir.ListUserInRegion.Select(x => x.CityId).ToArray();
                listUir.region_district = listUir.ListUserInRegion.Select(x => x.DistrictId).ToArray();
                listUir.region_ward = listUir.ListUserInRegion.Select(x => x.WardId).ToArray();
                model.ListUserInRegionSend = listUir;

                listUir = new ListUserInRegionModel();
                // Lấy thông tin vùng nhận.
                listUir.regionNum = role.ResiveRegionNum;
                listUir.ListUserInRegion = GetListUserInRegion(model.Id, role.ResiveRegionNum, 2);
                listUir.region_city = listUir.ListUserInRegion.Select(x => x.CityId).ToArray();
                listUir.region_district = listUir.ListUserInRegion.Select(x => x.DistrictId).ToArray();
                listUir.region_ward = listUir.ListUserInRegion.Select(x => x.WardId).ToArray();
                model.ListUserInRegionReceive = listUir;
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage);
                ModelState.AddModelError(string.Empty, string.Join(", ", errors));
                TempData[AdminConfigs.TEMP_MESSAGE] = "Có lỗi trong quá trình cập nhật! Vui lòng xem lại các thông tin bắt buộc nhập!";
                LoadDataPersonalInfo(model.Id, ref model);
            }

            ListUserInRegionModel listUirTemp = new ListUserInRegionModel();
            // Lấy thông tin vùng gửi
            listUirTemp.regionNum = role.SendRegionNum;
            listUirTemp.ListUserInRegion = GetListUserInRegion(model.Id, role.SendRegionNum, 1);
            listUirTemp.region_city = listUirTemp.ListUserInRegion.Select(x => x.CityId).ToArray();
            listUirTemp.region_district = listUirTemp.ListUserInRegion.Select(x => x.DistrictId).ToArray();
            listUirTemp.region_ward = listUirTemp.ListUserInRegion.Select(x => x.WardId).ToArray();
            model.ListUserInRegionSend = listUirTemp;

            listUirTemp = new ListUserInRegionModel();
            // Lấy thông tin vùng nhận.
            listUirTemp.regionNum = role.ResiveRegionNum;
            listUirTemp.ListUserInRegion = GetListUserInRegion(model.Id, role.ResiveRegionNum, 2);
            listUirTemp.region_city = listUirTemp.ListUserInRegion.Select(x => x.CityId).ToArray();
            listUirTemp.region_district = listUirTemp.ListUserInRegion.Select(x => x.DistrictId).ToArray();
            listUirTemp.region_ward = listUirTemp.ListUserInRegion.Select(x => x.WardId).ToArray();
            model.ListUserInRegionReceive = listUirTemp;

            return View("RegistryAgency", model);
        }

        private bool CheckRegionAgency(Models.UserModel model)
        {
            bool result = false;
            bool result2 = false;
            
            // Duyệt qua tất cả xã của vùng gửi môi giới chọn.
            foreach (int id in model.ListUserInRegionSend.region_ward)
            {
                if (id > 0)
                {
                    // Nếu có xã được chọn thì duyệt tiếp.
                    result = true;
                    break;
                }
            }

            // Duyệt qua tất cả xã của vùng nhận môi giới chọn.
            foreach (int id in model.ListUserInRegionReceive.region_ward)
            {
                if (id > 0)
                {
                    // Nếu có xã được chọn thì duyệt tiếp.
                    result2 = true;
                    break;
                }
            }

            return result && result2;
        }

        private void UpdateRegionAgency(Models.UserModel model)
        {
            UserInRegionService _service = new UserInRegionService();
            // Cập nhật danh sách vùng gửi.
            _service.DeleteByUser(model.Id, 1);
            UserInRegion entity = new UserInRegion();
            for (int i = 0; i < model.ListUserInRegionSend.region_ward.Length; i++)
            {
                if (model.ListUserInRegionSend.region_ward[i] > 0)
                {
                    entity = new UserInRegion();
                    entity.UserId = model.Id;
                    entity.RegionId = model.ListUserInRegionSend.regionIds[i];
                    entity.Status = true;
                    entity.CityId = model.ListUserInRegionSend.region_city[i];
                    entity.DistrictId = model.ListUserInRegionSend.region_district[i];
                    entity.WardId = model.ListUserInRegionSend.region_ward[i];
                    entity.RegionType = 1;
                    _service.Save(entity);
                }
            }

            // Cập nhật danh sách vùng nhân.
            _service.DeleteByUser(model.Id, 2);
            entity = new UserInRegion();
            for (int i = 0; i < model.ListUserInRegionReceive.region_ward.Length; i++)
            {
                if (model.ListUserInRegionReceive.region_ward[i] > 0)
                {
                    entity = new UserInRegion();
                    entity.UserId = model.Id;
                    entity.RegionId = model.ListUserInRegionReceive.regionIds[i];
                    entity.Status = true;
                    entity.CityId = model.ListUserInRegionReceive.region_city[i];
                    entity.DistrictId = model.ListUserInRegionReceive.region_district[i];
                    entity.WardId = model.ListUserInRegionReceive.region_ward[i];
                    entity.RegionType = 2;
                    _service.Save(entity);
                }
            }
        }
        
        private void UpdatePersional(Models.UserModel m)
        {
            Entities.User user;
            user = userService.GetById(m.Id);
            if (user != null)
            {

                // Upload Avatar
                if (m.Avatar != "" && m.Avatar != null)
                {
                    // Set Avatar url.
                    string avatarUrl = AdminConfigs.PHYSICAL_PATH + user.UserName + AdminConfigs.DIRSEPARATOR;
                    string fileName = "Avatar_" + DateTime.Now.ToString("yyyy_MM_dd.HH_mm_ss") + ".png";
                    ImageHelper.SaveBase64ToImage(avatarUrl, fileName, m.Avatar);
                    m.Avatar = "";

                    user.Avatar = fileName;
                }

                // Cập nhật họ tên
                user.FirstName = m.FirstName;
                user.LastName = m.LastName;

                // Cập nhật place
                user.PlaceId = m.PlaceId > 0 ? m.PlaceId : 0;

                // Cập nhật địa chỉ nhà
                user.Address = m.Address;

                // Cập nhật giới tính
                user.Gender = (short)m.Gender;

                // Cập nhật ngày sinh
                user.DateOfBirth = m.DateOfBirth;

                // Update description
                user.Description = m.Description;

                // Update Phone
                user.Phone = m.Phone;

                // Cập nhật tên hiển thị
                user.DisplayName = m.DisplayName;

                userService.Update(user);
            }
        }

        /// <summary>
        /// Lấy danh sách vùng của người dùng.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="regionNum"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<Entities.UserInRegion> GetListUserInRegion(int userId, int regionNum, int type)
        {
            List<Entities.UserInRegion> result = new List<Entities.UserInRegion>();
            Entities.UserInRegion model = new Entities.UserInRegion();
            UserInRegionService _service = new UserInRegionService();
            PlaceService placeService = new PlaceService();
            RegionService regionService = new RegionService();

            // Lấy danh sách vùng theo người dùng và kiểu vùng.
            var lst = _service.GetListByUser(userId, type);

            if (lst != null && lst.Count >0)
            {
                foreach (Entities.UserInRegion e in lst)
                {
                    model = new Entities.UserInRegion();

                    model.CityId = e.CityId;
                    model.DistrictId = e.DistrictId;
                    model.WardId = e.WardId;
                    model.UserId = e.UserId;
                    model.RegionId = e.RegionId;

                    model.ListCity = placeService.ListPlaceItemByParent(0);

                    if (model.CityId > 0)
                    {
                        model.ListDistrict = placeService.ListPlaceItemByParent(model.CityId);
                    }
                    else
                    {
                        model.ListDistrict = new List<Entities.Item>();
                    }

                    if (model.DistrictId > 0)
                    {
                        model.ListWard = placeService.ListPlaceItemByParent(model.DistrictId);
                    }
                    else
                    {
                        model.ListWard = new List<Entities.Item>();
                    }

                    if (model.RegionId > 0)
                    {
                        model.RegionText = regionService.GetById(model.RegionId).Text;
                    }

                    result.Add(model);
                }
            }

            if (lst.Count < regionNum)
            {
                int addMore = regionNum - lst.Count;
                for (int i = 0; i < addMore; i++)
                {
                    model = new Entities.UserInRegion();
                    model.CityId = -1;
                    model.DistrictId = -1;
                    model.WardId = -1;
                    model.RegionId = -1;
                    model.RegionText = "";

                    model.ListCity = placeService.ListPlaceItemByParent(0);
                    model.ListDistrict = new List<Entities.Item>();
                    model.ListWard = new List<Entities.Item>();

                    result.Add(model);
                }
            }

            return result;
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SendEmail(int[] usersId)
        {
            ViewBag.ListEmail = userService.ListEmail(usersId);

            return PartialView("EmailForm");
        }

        /// <summary>
        /// Thực hiện gửi email.
        /// </summary>
        /// <returns></returns>
        public ActionResult DoSendMailMulti()
        {
            // Get parameter value 
            int objSend = int.Parse(Request.Form.Get("objSend"));
            string listEmail = Request.Form.Get("SendTo");

            if (objSend == 2)
            {
                listEmail = userService.GetAllEmail();
            }

            string subject = Request.Form.Get("Subject");
            string content = Request.Form.Get("Content");

            //// Lấy thông tin người gửi.
            //int senderId = int.Parse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID));
            //var senderEmail = userService.GetEmailById(senderId);

            Article model = new Article();
            model.Body = content;
            model.Title = subject;

            MailHelper.MailArticle(model, listEmail);

            ViewBag.Message = "Email đang được gửi đi ...";
            ViewBag.Result = 1;

            return PartialView("EmailForm");
        }
    }
}
