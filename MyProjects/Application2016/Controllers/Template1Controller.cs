using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application2016.Models;
using BusinessLayer;
using Application2016.Helpers;
using CaptchaMvc.HtmlHelpers;
using BusinessLayer.Helpers;
using Entities;
using Application2016.Enums;

namespace Application2016.Controllers
{
    public class Template1Controller : BaseController
    {
        ProductService productService = new ProductService();
        UserService userService = new UserService();

        public ActionResult Index(Template1SearchModel model)
        {
            GetDataModel(model);

            // Get Tab ID
            int tabId = 0;
            switch (model.TransactionType)
            {
                case 0:
                    tabId = 0;
                    break;
                case 1:
                    tabId = 1;
                    break;
                case 2:
                    tabId = 2;
                    break;
                case 3:
                    break;
                default:
                    tabId = 0;
                    break;
            }
            Entities.ProductCondtions e = new Entities.ProductCondtions();
            model.Mapping(model, ref e);
            var lstTemp = productService.List(e);

            // Lấy danh sách Product Follow
            int loginUser = 0;
            int.TryParse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID), out loginUser);
            if(loginUser>0){
                var productFollows = productService.ListProductFollowId(loginUser);
                if (productFollows!=null && productFollows.Count > 0)
                {
                    foreach (var item in productFollows)
                    {
                        lstTemp.Where(x => x.Id == item.ProductId).Select(y => { y.IsFollow = item.IsFollow; return y; }).ToList();
                    }
                }
            }

            int totalRecord = 0;
            if (lstTemp.Count > 0)
            {
                totalRecord = lstTemp[0].Total.Value;
                Paging(model.Page, totalRecord, model.PageSize);
            }

            ViewBag.BoxTitle = "Danh sách nhà";
            ViewBag.ListProduct = lstTemp;
            ViewBag.TabID = tabId;

            return View(model);
        }

        public ActionResult Template1ProductFeature()
        {
            var lstTemp = productService.List();

            // Lấy danh sách Product Follow
            int loginUser = 0;
            int.TryParse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID), out loginUser);
            if (loginUser > 0)
            {
                var productFollows = productService.ListProductFollowId(loginUser);
                if (productFollows != null && productFollows.Count > 0)
                {
                    foreach (var item in productFollows)
                    {
                        lstTemp.Where(x => x.Id == item.ProductId).Select(y => { y.IsFollow = item.IsFollow; return y; }).ToList();
                    }
                }
            }

            ViewBag.ListProduct = lstTemp;
            ViewBag.BoxTitle = "Danh sách nhà gợi ý";
            return PartialView("_ListItem");
        }

        /// <summary>
        /// Danh sách sản phẩm gợi ý.
        /// </summary>
        /// <param name="withOutId"></param>
        /// <param name="cityId"></param>
        /// <param name="districtId"></param>
        /// <returns></returns>
        public ActionResult Template1ProductSuggest(int withOutId, int cityId, int districtId)
        {
            Entities.ProductCondtions condition = new Entities.ProductCondtions();
            condition.withOutId = withOutId;
            condition.City = cityId;
            condition.District = districtId;
            condition.Page = 1;
            condition.Limit = 20;
            var listProduct = productService.List(condition);

            ViewBag.ListProduct = listProduct;
            ViewBag.BoxTitle = "Danh sách nhà gợi ý";
            return PartialView("_ListItem");
        }

        public ActionResult ProductNew()
        {
            var lstTemp = productService.List();
            ViewBag.ListProduct = lstTemp;
            return PartialView("_BoxProductVertical");
        }

        public ActionResult Detail(int id)
        {
            ProductService _service = new ProductService();
            Entities.Product product = _service.GetById(id);
            ProductModel model = new ProductModel();
            model.ListImage = new List<Entities.ProductImage>();
            if (product != null)
            {
                // Lấy thông tin sản phẩm.
                model.MapFrom(product, ref model);

                // Lấy thông tin kiểu nhà.
                var listProductType = _service.ListProductType();
                model.ProductTypeText = (from p in listProductType
                                         where p.Id == model.Product_Type
                                         select p.Text).FirstOrDefault();

                // Lấy thông tin người đăng.
                UserService _userService = new UserService();
                var user = _userService.GetById(model.UserId);
                model.Username = user.UserName;
                model.PhoneNumber = user.Phone;

                // Lấy danh sách ảnh sản phẩm.
                model.ListImage = _service.GetListImage(id);

                // Kiểm tra có phải là người post hay không.
                if (CookieHelper.Get(AdminConfigs.COOKIES_USER_ID) != null)
                {
                    if (CookieHelper.Get(AdminConfigs.COOKIES_USER_ID) == model.UserId.ToString())
                    {
                        model.IsPoster = true;
                    }
                    else
                    {
                        model.IsPoster = false;
                    }
                }

                // Lấy thông tin điều chỉnh giá.
                model.NewCost = _service.GetCostChangeByOwn(id, product.UserId);

                // Lấy danh sách giá đặt mua
                List<Entities.Product_ChangeCost> lstChangeCost = _service.GetListProductChangeCost(id, product.UserId);
                model.ListChangeCost = lstChangeCost;

            }
            else
            {
                model.ListChangeCost = new List<Entities.Product_ChangeCost>();
            }
            return View(model);
        }

        public void GetDataModel(Template1SearchModel model)
        {
            // Lấy danh sách các kiểu nhà.
            ProductService _productService = new ProductService();
            ViewBag.ListProductType = _productService.ListProductType();

            // Lấy danh sách các tỉnh.
            PlaceService _placeService = new PlaceService();
            ViewBag.ListCity = _placeService.ListPlaceItemByParent(0);
            ViewBag.ListCity.Insert(0, new Entities.Item() { Text = "--- Chọn tỉnh ---", Id = -1 });
            // Lấy danh sách các huyện quận theo tỉnh.
            if (model.CityId > 0)
            {
                ViewBag.ListDistrict = _placeService.ListPlaceItemByParent(model.CityId);
            }
            else
            {
                ViewBag.ListDistrict = new List<Entities.Item>();
            }
            ViewBag.ListDistrict.Insert(0, new Entities.Item() { Text = "--- Chọn quận huyện ---", Id = -1 });

            // Lấy danh sách xã phường theo quận huyện.
            if (model.DistrictId > 0)
            {
                ViewBag.ListWard = _placeService.ListPlaceItemByParent(model.DistrictId);
            }
            else
            {
                ViewBag.ListWard = new List<Entities.Item>();
            }
            ViewBag.ListWard.Insert(0, new Entities.Item() { Text = "--- Chọn xã phường ---", Id = -1 });

            // Lấy danh sách diện tích nhỏ nhất
            var listAreaMin = Helpers.DefaultData.ListArea();

            // Lấy danh sách diện tích nhỏ nhất
            var listAreaMax = Helpers.DefaultData.ListArea();
            listAreaMax.Add(new Entities.Item()
            {
                Id = 100000,
                Text = ">"+listAreaMin[listAreaMin.Count -1].Id.ToString()
            });


            if (model.MaxArea > 0)
            {
                listAreaMin = listAreaMin.Where(x => x.Id <= model.MaxArea).ToList();

            }

            if (model.MinArea > 0)
            {
                listAreaMax = listAreaMax.Where(x => x.Id >= model.MinArea).ToList();
            }


            ViewBag.ListAreaMin = listAreaMin;

            ViewBag.ListAreaMax = listAreaMax;

            model.ListUser = userService.ListItem();
        }

        public ActionResult BoxLink(int Position)
        {
            ArticleService articleService = new ArticleService();

            var ListArticle = articleService.List(0);

            ViewBag.ListArticle = ListArticle;

            ViewBag.WebsiteInfo = HttpContext.Application[AdminConfigs.SETTING_WEBSITE];

            ViewBag.Social = HttpContext.Application[AdminConfigs.SETTING_SOCIAL_NETWORK];

            return PartialView("BoxLink");
        }

        public ActionResult Article(int Id)
        {
            ArticleService articleService = new ArticleService();
            ArticleModel model = new ArticleModel();
            model.Article = articleService.GetById(Id);

            return View(model);
        }

        public ActionResult GetMessage()
        {
            string strUserId = CookieHelper.Get(AdminConfigs.COOKIES_USER_ID);
            if (strUserId != null)
            {
                // Lấy thông tin host.
                Uri uri = Request.Url;
                string urlHost = uri.GetLeftPart(UriPartial.Authority);
                var listMessage = productService.ListMessage(int.Parse(strUserId));
                if (listMessage.Count > 0)
                {
                    foreach (var product in listMessage)
                    {
                        product.Content = product.Content.Replace("[user]", product.UserName);
                        product.Content = product.Content.Replace("[phone]", product.Phone);
                        string link = "<a href='" + urlHost + "/" + AdminConfigs.FRIENDLY_LINK_PRODUCT_DETAIL.ToFriendlyUrl(product.ProductId, product.ProductText.ToAlias()) + "'>" + product.ProductText + "</a>";
                        product.Content = product.Content.Replace("[product]", link);
                    }
                }

                ViewBag.ListMessage = listMessage;
            }
            else
            {
                ViewBag.ListMessage = new List<Entities.ProductMessage>();
            }
            return PartialView("_HeaderMessage");
        }

        [HttpPost]
        public JsonResult ChangeToReadedMessage(int Id)
        {
            productService.UpdateReadFlag(Id);

            return Json(true);
        }

        public ActionResult ProductMessage(Models.BaseModel condition)
        {
            int userId = 0; // UserId

            // Kiểm tra đăng nhập
            if (CookieHelper.Get(AdminConfigs.COOKIES_USER_ID) == null)
            {
                string oldUrl = string.Empty;
                if (Request.UrlReferrer != null)
                {
                    oldUrl = Request.UrlReferrer.ToString();
                }

                TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.ERROR_NOT_LOGIN;
                return RedirectToAction("Login", "User", new {area="Admin", returnUrl = oldUrl });
            }
            else
            {
                userId = int.Parse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID));
            }

            Models.ProductListMessage model = new ProductListMessage();
            var listMessage = productService.ListMessage(userId);
            // Lấy thông tin host.
            Uri uri = Request.Url;
            string urlHost = uri.GetLeftPart(UriPartial.Authority);
            if (listMessage.Count > 0)
            {
                foreach (var product in listMessage)
                {
                    product.Content = product.Content.Replace("[user]", product.UserName);
                    product.Content = product.Content.Replace("[phone]", product.Phone);
                    string link = "<a href='" + urlHost + "/" + AdminConfigs.FRIENDLY_LINK_PRODUCT_DETAIL.ToFriendlyUrl(product.ProductId, product.ProductText.ToAlias()) + "'>" + product.ProductText + "</a>";
                    product.Content = product.Content.Replace("[product]", link);
                }
            }
            model.ListMessage = listMessage;
            model.Condition = condition;

            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteProduct(int Id, int userId)
        {
            ProductService _service = new ProductService();
            int result = _service.Delete(Id, userId);
            return Json(result);
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }

        #region Đăng ký
        /// <summary> Đăng ký làm thành viên.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RegisterMember()
        {
            Application2016.Areas.Admin.Models.RegisterModel model = new Areas.Admin.Models.RegisterModel();
            PlaceService placeService = new PlaceService();

            // Lấy danh sách tỉnh.
            model.ListCity = placeService.ListPlaceItemByParent(0);

            return PartialView("RegistryMember_Model", model);
        }

        [HttpPost]
        public ActionResult RegisterMember(Application2016.Areas.Admin.Models.RegisterModel model, string CaptchaText)
        {
            #region Lấy danh sách thông tin cần thiết
            PlaceService placeService = new PlaceService();
            model.ListCity = placeService.ListPlaceItemByParent(0);

            if (model.Regis_CityId > 0)
            {
                model.ListDistrict = placeService.ListPlaceItemByParent((int)model.Regis_CityId);
            }

            if (model.Regis_DistrictId > 0)
            {
                model.ListWard = placeService.ListPlaceItemByParent((int)model.Regis_DistrictId);
            }

            #endregion

            #region Kiểm tra các điều kiện
            // Kiểm tra các thông tin yêu cầu nhập
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage);
                ModelState.AddModelError(string.Empty, string.Join(", ", errors));
                Application2016.Helpers.Logs.LogWrite("Lỗi đăng ký: " + string.Join(", ", errors));
                return PartialView("RegistryMember_Model", model);
            }
            
            // Kiểm tra Captcha.
            ViewBag.ErrMessage = "";
            if (!this.IsCaptchaValid(""))
            {
                ViewBag.ErrMessage = "Captcha không đúng. Xin hãy nhập lại";
                model.Result = -1;
                return PartialView("RegistryMember_Model", model);
            }

            // Kiểm tra sự tồn tại của Username
            bool result = userService.CheckExistsUsername(model.UserName);
            if (result)
            {
                //ModelState.AddModelError(string.Empty, AdminConfigs.ERROR_USERNAME_EXISTS);
                model.Result = 0;
                model.Message = AdminConfigs.ERROR_USERNAME_EXISTS;
                return PartialView("RegistryMember_Model", model);
            }

            // Kiểm tra sự tồn tại của email.
            result = userService.CheckExistsEmail(model.Email);
            if (result)
            {
                //ModelState.AddModelError(string.Empty, AdminConfigs.ERROR_EMAIL_EXISTS);
                model.Result = 0;
                model.Message = AdminConfigs.ERROR_EMAIL_EXISTS;
                return PartialView("RegistryMember_Model", model);
            }
            #endregion

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
            u.FirstName = model.FirstName;
            u.LastName = model.LastName;
            u.DateOfBirth = model.DateOfBirth;
            u.Gender = model.Gender;
            u.PlaceId = model.Regis_WardId;

            var newUserId = userService.RegistryMember(u);
            if (newUserId > 0)
            {
                u.Id = newUserId;
                Uri uri = Request.Url;
                string urlHost = uri.GetLeftPart(UriPartial.Authority);
                MailHelper.MailRegister(u, urlHost, u.CodeActive);
                model.Result = 1;
                model.Message = AdminConfigs.MESSAGE_REGISTER_MEMBER_OK;
            }
            else
            {
                // Error.
                model.Result = 0;
                model.Message = AdminConfigs.MESSAGE_REGISTER_MEMBER_ERROR;
            }

            return PartialView("RegistryMember_Model", model);
        }

        /// <summary> Đăng ký làm môi giới
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RegisterAgency()
        {
            PlaceService placeService = new PlaceService();

            RoleService roleService = new RoleService();
            // Lấy số vùng hoạt động.
            var role = roleService.GetByCode("AgencyOnline");

            // Lấy thông tin cá nhân (Theo tài khoản đăng nhập nếu có)
            int userId = 0;
            int.TryParse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID), out userId);
            if (userId > 0)
            {
                Application2016.Areas.Admin.Models.RegistryAgencyModel2 model = new Areas.Admin.Models.RegistryAgencyModel2();
                Entities.User user = userService.GetById(userId);
                model.MapFrom(user, ref model);

                if (user.PlaceId != null)
                {
                    // Lấy thông tin xã, phường.
                    Entities.Place place = new Entities.Place();
                    place = placeService.GetById((int)user.PlaceId);
                    if (place != null)
                    {
                        // Lấy ID quận, huyện.
                        model.Regis_DistrictId = place.Parent;

                        // Lấy thông tin quận, huyện
                        place = placeService.GetById((int)model.Regis_DistrictId);

                        // Lấy ID tỉnh, thành phố.
                        model.Regis_CityId = place.Parent;
                    }
                }

                // Lấy danh sách tỉnh.
                model.ListCity = placeService.ListPlaceItemByParent(0);

                // Lấy danh sách quận huyện.
                if (model.Regis_CityId > 0)
                {
                    model.ListDistrict = placeService.ListPlaceItemByParent((int)model.Regis_CityId);
                }
                else
                {
                    model.ListDistrict = new List<Item>();
                }

                // Lấy danh sách xã phường.
                if (model.Regis_DistrictId > 0)
                {
                    model.ListWard = placeService.ListPlaceItemByParent((int)model.Regis_DistrictId);
                }
                else
                {
                    model.ListWard = new List<Item>();
                }

                model.ListUserInRegionSend = new Areas.Admin.Models.ListUserInRegionModel();
                model.ListUserInRegionSend.regionNum = role.SendRegionNum;
                model.ListUserInRegionSend.ListUserInRegion = GetListUserInRegion(role.SendRegionNum, model.ListUserInRegionSend);

                model.ListUserInRegionReceive = new Areas.Admin.Models.ListUserInRegionModel();
                model.ListUserInRegionReceive.regionNum = role.ResiveRegionNum;
                model.ListUserInRegionReceive.ListUserInRegion = GetListUserInRegion(role.ResiveRegionNum, model.ListUserInRegionReceive);

                return PartialView("RegistryAgency2", model);
            }
            else
            {
                Application2016.Areas.Admin.Models.RegistryAgencyModel1 model = new Areas.Admin.Models.RegistryAgencyModel1();

                // Lấy danh sách tỉnh.
                model.ListCity = placeService.ListPlaceItemByParent(0);

                model.ListUserInRegionSend = new Areas.Admin.Models.ListUserInRegionModel();
                model.ListUserInRegionSend.regionNum = role.SendRegionNum;
                model.ListUserInRegionSend.ListUserInRegion = GetListUserInRegion(role.SendRegionNum, model.ListUserInRegionSend);

                model.ListUserInRegionReceive = new Areas.Admin.Models.ListUserInRegionModel();
                model.ListUserInRegionReceive.regionNum = role.ResiveRegionNum;
                model.ListUserInRegionReceive.ListUserInRegion = GetListUserInRegion(role.ResiveRegionNum, model.ListUserInRegionReceive);

                return PartialView("RegistryAgency1", model);
            }
        }

        [HttpPost]
        public ActionResult RegisterAgency1(Application2016.Areas.Admin.Models.RegistryAgencyModel1 model)
        {
            #region Lấy danh sách thông tin cần thiết
            PlaceService placeService = new PlaceService();
            model.ListCity = placeService.ListPlaceItemByParent(0);

            if (model.Regis_CityId > 0)
            {
                model.ListDistrict = placeService.ListPlaceItemByParent((int)model.Regis_CityId);
            }
            else
            {
                model.ListDistrict = new List<Item>();
            }

            if (model.Regis_DistrictId > 0)
            {
                model.ListWard = placeService.ListPlaceItemByParent((int)model.Regis_DistrictId);
            }
            else
            {
                model.ListWard = new List<Item>();
            }

            RoleService roleService = new RoleService();
            // Lấy số vùng hoạt động.
            var role = roleService.GetByCode("AgencyOnline");

            model.ListUserInRegionSend.regionNum = role.SendRegionNum;
            model.ListUserInRegionSend.ListUserInRegion = GetListUserInRegion(role.SendRegionNum, model.ListUserInRegionSend);

            model.ListUserInRegionReceive.regionNum = role.ResiveRegionNum;
            model.ListUserInRegionReceive.ListUserInRegion = GetListUserInRegion(role.ResiveRegionNum, model.ListUserInRegionReceive);
            #endregion

            #region Kiểm tra các điều kiện
            // Kiểm tra các thông tin yêu cầu nhập
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage);
                ModelState.AddModelError(string.Empty, string.Join(", ", errors));
                Application2016.Helpers.Logs.LogWrite("Lỗi đăng ký: " + string.Join(", ", errors));
                return PartialView("RegistryAgency1", model);
            }

            // Kiểm tra Captcha.
            if (!this.IsCaptchaValid(""))
            {
                model.Result = -1;
                model.Message = AdminConfigs.ERROR_CAPTCHA;
                return PartialView("RegistryAgency1", model);
            }

            // Kiểm tra sự tồn tại của Username
            bool result = userService.CheckExistsUsername(model.UserName);
            if (result)
            {
                model.Result = -1;
                model.Message = AdminConfigs.ERROR_USERNAME_EXISTS;
                return PartialView("RegistryAgency1", model);
            }

            // Kiểm tra sự tồn tại của email.
            result = userService.CheckExistsEmail(model.Email);
            if (result)
            {
                model.Result = -1;
                model.Message = AdminConfigs.ERROR_EMAIL_EXISTS;
                return PartialView("RegistryAgency1", model);
            }
            #endregion

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
            u.FirstName = model.FirstName;
            u.LastName = model.LastName;
            u.DateOfBirth = model.DateOfBirth;
            u.Gender = model.Gender;
            u.PlaceId = model.Regis_WardId;

            var res = userService.RegistryMember(u);
            if (res > 0)
            {
                // Xét lại ID người dùng.
                u.Id = res;

                // Xử lý đăng ký vùng môi giới.
                UpdateRegionAgency(res, model.ListUserInRegionSend, model.ListUserInRegionReceive);

                UserInRolesService userInRoleService = new UserInRolesService();

                // Xóa trạng thái hiện tại
                userInRoleService.Delete(u.Id);

                // Cập nhật trạng thái mới là môi giới.
                UserInRole uir = new UserInRole();
                uir.UserId = u.Id;
                uir.RolesId = role.Id;
                uir.State = true;
                userInRoleService.Insert(uir);

                // Gửi mail thông báo
                Uri uri = Request.Url;
                string urlHost = uri.GetLeftPart(UriPartial.Authority);

                MailHelper.MailRegisterAgency(u, urlHost, u.CodeActive);

                // Hiển thị message thông báo.
                model.Result = 1;
                model.Message = AdminConfigs.MESSAGE_REGISTER_AGENCY_OK;
            }
            else
            {
                // Error.
                model.Result = 0;
                model.Message = AdminConfigs.MESSAGE_REGISTER_MEMBER_ERROR;
            }

            return PartialView("RegistryAgency1", model);
        }

        [HttpPost]
        public ActionResult RegisterAgency2(Application2016.Areas.Admin.Models.RegistryAgencyModel2 model, string CaptchaInputText)
        {
            #region Lấy danh sách thông tin cần thiết
            PlaceService placeService = new PlaceService();
            model.ListCity = placeService.ListPlaceItemByParent(0);

            if (model.Regis_CityId > 0)
            {
                model.ListDistrict = placeService.ListPlaceItemByParent((int)model.Regis_CityId);
            }
            else
            {
                model.ListDistrict = new List<Item>();
            }

            if (model.Regis_DistrictId > 0)
            {
                model.ListWard = placeService.ListPlaceItemByParent((int)model.Regis_DistrictId);
            }
            else
            {
                model.ListWard = new List<Item>();
            }

            RoleService roleService = new RoleService();
            // Lấy số vùng hoạt động.
            var role = roleService.GetByCode("AgencyOnline");

            model.ListUserInRegionSend.regionNum = role.SendRegionNum;
            model.ListUserInRegionSend.ListUserInRegion = GetListUserInRegion(role.SendRegionNum, model.ListUserInRegionSend);

            model.ListUserInRegionReceive.regionNum = role.ResiveRegionNum;
            model.ListUserInRegionReceive.ListUserInRegion = GetListUserInRegion(role.ResiveRegionNum, model.ListUserInRegionReceive);
            #endregion

            #region Kiểm tra các điều kiện.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage);
                ModelState.AddModelError(string.Empty, string.Join(", ", errors));
                Application2016.Helpers.Logs.LogWrite("Lỗi đăng ký: " + string.Join(", ", errors));
                return PartialView("RegistryAgency2", model);
            }

            if (!CheckRegionAgency(model.ListUserInRegionSend))
            {
                return PartialView("RegistryAgency2", model);
            }

            if (!this.IsCaptchaValid(""))
            {
                return PartialView("RegistryAgency2", model);
            }
            #endregion

            // Cập nhật vùng hoạt động
            UpdateRegionAgency(model.Id, model.ListUserInRegionSend, model.ListUserInRegionReceive);

            UserInRolesService userInRoleService = new UserInRolesService();
            // Xóa trạng thái hiện tại
            userInRoleService.Delete(model.Id);

            // Cập nhật trạng thái mới là môi giới.
            UserInRole uir = new UserInRole();
            uir.UserId = model.Id;
            uir.RolesId = role.Id;
            uir.State = true;
            userInRoleService.Insert(uir);

            // Gửi mail thông báo
            Uri uri = Request.Url;
            string urlHost = uri.GetLeftPart(UriPartial.Authority);

            Entities.User u = userService.GetById(model.Id);
            MailHelper.MailRegisterAgency(u, urlHost, u.CodeActive, true);

            // Hiển thị message thông báo.
            model.Result = 1;
            model.Message = AdminConfigs.MESSAGE_REGISTER_AGENCY_OK;


            return PartialView("RegistryAgency2", model);
        }

        private List<Entities.UserInRegion> GetListUserInRegion(int regionNum, Application2016.Areas.Admin.Models.ListUserInRegionModel listUir)
        {
            PlaceService placeService = new PlaceService();
            List<Entities.UserInRegion> result = new List<Entities.UserInRegion>();
            Entities.UserInRegion uir;
            int[] arrCity = listUir.region_city;
            int[] arrDistrict = listUir.region_district;
            int[] arrWard = listUir.region_ward;
            for(int i=0; i < regionNum; i++)
            {
                uir = new UserInRegion();
                uir.ListCity = placeService.ListPlaceItemByParent(0);
                uir.ListDistrict = new List<Item>();
                uir.ListWard = new List<Item>();
                if (arrCity != null)
                {
                    uir.CityId = arrCity[i];
                    uir.DistrictId = arrDistrict[i];
                    uir.WardId = arrWard[i];
                    if (arrCity[i] > 0)
                    {
                        uir.ListDistrict = placeService.ListPlaceItemByParent(arrCity[i]);
                    }
                    if (arrDistrict[i] > 0)
                    {
                        uir.ListWard = placeService.ListPlaceItemByParent(arrDistrict[i]);
                    }
                }
                
                result.Add(uir);
            }
            return result;
        }

        private List<Entities.UserInRegion> GetListUserInRegion(int userId, int regionNum, int type)
        {
            List<Entities.UserInRegion> result = new List<Entities.UserInRegion>();
            Entities.UserInRegion model = new Entities.UserInRegion();
            UserInRegionService _service = new UserInRegionService();
            PlaceService placeService = new PlaceService();
            RegionService regionService = new RegionService();

            // Lấy danh sách vùng theo người dùng và kiểu vùng.
            var lst = _service.GetListByUser(userId, type);

            if (lst != null && lst.Count > 0)
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

        [HttpGet]
        public ActionResult RegisterAgency2()
        {
            Application2016.Areas.Admin.Models.RegisterModel model = new Areas.Admin.Models.RegisterModel();
            PlaceService placeService = new PlaceService();

            // Lấy danh sách tỉnh.
            model.ListCity = placeService.ListPlaceItemByParent(0);

            return PartialView("RegistryAgency2", model);
        }
        
        private bool CheckRegionAgency(Application2016.Areas.Admin.Models.ListUserInRegionModel listUir)
        {
            bool result = true;
            
            // Duyệt qua tất cả xã
            foreach (int id in listUir.region_ward)
            {
                if (id <= 0)
                {
                    // Nếu có xã chưa được chọn thì đưa ra lỗi.
                    result = false;
                    break;
                }
            }


            return result;
        }

        private void UpdateRegionAgency(int Id,
            Application2016.Areas.Admin.Models.ListUserInRegionModel listRegionSend,
            Application2016.Areas.Admin.Models.ListUserInRegionModel listRegionReceive)
        {
            UserInRegionService _service = new UserInRegionService();
            // Cập nhật danh sách vùng gửi.
            _service.DeleteByUser(Id, 1);
            UserInRegion entity = new UserInRegion();
            for (int i = 0; i < listRegionSend.region_ward.Length; i++)
            {
                if (listRegionSend.region_ward[i] > 0)
                {
                    entity = new UserInRegion();
                    entity.UserId = Id;
                    entity.RegionId = _service.GetRegionIdByWard(listRegionSend.region_ward[i]);
                    entity.Status = true;
                    entity.CityId = listRegionSend.region_city[i];
                    entity.DistrictId = listRegionSend.region_district[i];
                    entity.WardId = listRegionSend.region_ward[i];
                    entity.RegionType = 1;
                    _service.Save(entity);
                }
            }

            // Cập nhật danh sách vùng nhận.
            _service.DeleteByUser(Id, 2);
            entity = new UserInRegion();
            for (int i = 0; i < listRegionReceive.region_ward.Length; i++)
            {
                if (listRegionReceive.region_ward[i] > 0)
                {
                    entity = new UserInRegion();
                    entity.UserId = Id;
                    entity.RegionId = _service.GetRegionIdByWard(listRegionReceive.region_ward[i]);
                    entity.Status = true;
                    entity.CityId = listRegionReceive.region_city[i];
                    entity.DistrictId = listRegionReceive.region_district[i];
                    entity.WardId = listRegionReceive.region_ward[i];
                    entity.RegionType = 2;
                    _service.Save(entity);
                }
            }
        }
        #endregion

        #region Đăng nhập
        [HttpGet]
        public ActionResult Login()
        {
            Models.LoginModel model = new Models.LoginModel();
            
            return PartialView("Login_Model", model);
        }

        [HttpPost]
        public ActionResult Login(Models.LoginModel model)
        {
            if (ModelState.IsValid)
            {
                UserService userService = new UserService();
                string username = "";
                int result = userService.CheckLogin(model.Username_Model, model.Password_Model, out username);
                if (result < 0)
                {
                    switch (result)
                    {
                        case -3:
                            model.Message = "Tài khoản bạn nhập không đúng. Xin vui lòng kiểm tra lại!";
                            model.Result = -1;
                            return PartialView("Login_Model", model);
                        case -5:
                            model.Message = "Tài khoản của bạn đang bị khóa. Xin vui lòng liên lạc với ban quản trị";
                            model.Result = -1;
                            return PartialView("Login_Model", model);
                        default:
                            break;
                    }
                }
                else
                {
                    // Tạo cookie cho tài khoản.
                    var user = userService.GetById(result);
                    InitAccountInfo(user, model.Remember_Model);
                    model.Message = "Đăng nhập thành công!";
                    model.Result = 1;
                    return PartialView("Login_Model", model);
                }
            }
            return PartialView("Login_Model", model);
        }

        public void InitAccountInfo(Entities.User user, bool remember)
        {
            if (user != null)
            {
                UserInRolesService uir = new UserInRolesService();
                int roleId = uir.GetFirstRoleIdByUser(user.Id);

                int dayExpries = 1;
                if (remember)
                {
                    dayExpries = 365;
                }

                CookieHelper.Set(AdminConfigs.COOKIES_USERNAME, user.UserName, dayExpries);
                CookieHelper.Set(AdminConfigs.COOKIES_AVATAR, user.Avatar, dayExpries);
                CookieHelper.Set(AdminConfigs.COOKIES_ROLE_ID, roleId.ToString(), dayExpries);
                CookieHelper.Set(AdminConfigs.COOKIES_USER_ID, user.Id.ToString(), dayExpries);

                // Kiểm tra tài khoản có phải là Admin không.                
                bool chk = uir.CheckUserIsAdmin(user.Id);
                string isAdmin = chk ? "1" : "0";
                CookieHelper.Set(AdminConfigs.COOKIES_ADMIN, isAdmin, dayExpries);

                chk = uir.CheckUserIsAgency(user.Id);
                string isAgency = chk ? "1" : "0";
                CookieHelper.Set(AdminConfigs.COOKIES_AGENCY, isAgency, dayExpries);

                TempData[AdminConfigs.TEMP_USERNAME] = user.UserName;
                TempData[AdminConfigs.TEMP_USER_ID] = user.Id.ToString();
                TempData[AdminConfigs.TEMP_AVATAR] = user.Avatar;
            }
        }
        #endregion
    }
}
