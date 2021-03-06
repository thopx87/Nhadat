using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Application2016.Helpers;
using Application2016.Enums;

namespace Application2016.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        ProductService productService = new ProductService();
        RegionService regionService = new RegionService();
        UserInRegionService uirService = new UserInRegionService();

        public ProductController()
        {
            ViewBag.ActionMenu = "ProductManager";
            ViewBag.ActionSubMenu = "Product";
        }
        
        public ActionResult Index(string type, int page, string query = "")
        {
            if (type == "") type = "List";
            if (page < 0) page = 1;
            
            // Gán lại page
            Page = page;
            // Paging
            
            // Display type
            if (type == "List")
            {
                ViewBag.DisplayType = "List";
            }
            else
            {
                ViewBag.DisplayType = "Grid";
            }

            ViewBag.BoxTitle = "Danh sách nhà";

            // Load Data
            LoadData();

            return View();
        }

        private void LoadData()
        {
            int totalRecord = 0;
            List<Entities.Product> lstEntity = productService.List();
            // Add list product
            ViewBag.ListProduct = lstEntity;
            ViewBag.NumberItem = lstEntity.Count;

            if (lstEntity.Count > 0)
            {
                totalRecord = lstEntity[0].Total.Value;
            }

            Paging(Page, totalRecord);
        }

        /// <summary>
        /// Cập nhật sản phẩm bán
        /// </summary>
        /// <param name="Id">ID sản phẩm.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateSaleProduct(int Id)
        {
            ViewBag.ActionSubMenu = "Sale";

            // Kiểm tra quyền đăng bài
            Entities.Item role = new Entities.Item();
            int roleId = CheckRolePost(ref role);
            if (roleId < 0)
            {
                return Redirect(roleId);
            }

            Models.ProductModel model = new Models.ProductModel();
            if (Id > 0)
            {
                Entities.Product e = productService.GetById(Id);
                model.MapFrom(e, ref model);
            }
            else
            {
                // set default.
                model.SellStartDate = DateTime.Now;
                model.SellEndDate = DateTime.Now.AddMonths(1);
                model.UserId = int.Parse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID));
                model.UserName = CookieHelper.Get(AdminConfigs.COOKIES_USERNAME);
            }
            LoadModel(ref model);

            // Add list product
            LoadData();

            ViewBag.ActionForm = "UpdateSaleProduct";
            ViewBag.SubmitValue = Id > 0 ? AdminConfigs.BUTTON_UPDATE : AdminConfigs.BUTTON_POST_NEW;
            return PartialView("Index", model);
        }

        [HttpPost]
        public ActionResult UpdateSaleProduct(int id, Models.ProductModel model, int[] ListRegionNeighbor)
        {
            ViewBag.ActionSubMenu = "Sale";
            int newId = 0;
            bool isSuccess = false;
            // Kiểm tra quyền đăng bài
            Entities.Item role = new Entities.Item();
            int chk = CheckRolePost(ref role);
            if (chk < 0)
            {
                Logs.LogWrite("UpdateSaleProduct Kiểm tra quyền đăng");
                return Redirect(chk);
            }

            if (ModelState.IsValid)
            {
                Entities.Product e = new Entities.Product();

                int loginUser = int.Parse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID));
                
                // Kiểm tra user có quyền chỉnh sửa hay không.
                if (id > 0)
                {
                    if (!CheckRoleUpdate(loginUser, model.UserId))
                    {
                        Logs.LogWrite("UpdateSaleProduct Kiểm tra quyền chỉnh sửa.");
                        return Redirect(-7);
                    }
                }
                else
                {
                    model.UserId = loginUser;
                }

                model.Id = id;
                model.Transaction_Type = (int)Enums.TransactionType.BAN_NHA; // Bán nhà

                // Chuyển tiền thành triệu.
                model.StandardCost = StringHelperExtension.MoneyExchange(model.StandardCost);

                model.MapFrom(model, ref e);
                if (id <= 0)
                {
                    e.UpdateBy = e.UserId;
                    e.UserName = CookieHelper.Get(AdminConfigs.COOKIES_USERNAME);

                    // Lấy thông tin role.
                    e.RoleId = role.Id;
                    e.RoleText = role.Text;
                }

                int result = 0;
                result = productService.Save(e);
                if (result < 0)
                {
                    ViewBag.Message = AdminConfigs.MESSAGE_UPDATE_ERROR;
                    ViewBag.AlertClass = AdminConfigs.CLASS_ALERT_DANGER;
                    TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_ERROR;
                } 
                else
                {
                    newId = result;
                    #region Xử lý thêm ảnh vào database
                    Models.ImageConfig[] arrImg = GetAllFileInfo(id, ImageType.PRODUCT_TEMP, e.UserName);
                    if (arrImg.Count()>0)
                    {
                        string sourceFolder = AdminConfigs.PHYSICAL_PATH + e.UserName.ToLower() + AdminConfigs.DIRSEPARATOR + AdminConfigs.FOLDER_PRODUCT_TEMPS;
                        string targetFolder = AdminConfigs.PHYSICAL_PATH + e.UserName.ToLower() + AdminConfigs.DIRSEPARATOR + AdminConfigs.FOLDER_PRODUCTS;
                        string urlImg = AdminConfigs.IMAGE_PATH + e.UserName.ToLower() + AdminConfigs.ALTDIRECTORYSEPARATORCHAR + AdminConfigs.FOLDER_PRODUCTS + AdminConfigs.ALTDIRECTORYSEPARATORCHAR;
                        string urlImgTemp = AdminConfigs.IMAGE_PATH + e.UserName.ToLower() + AdminConfigs.ALTDIRECTORYSEPARATORCHAR + AdminConfigs.FOLDER_PRODUCT_TEMPS + AdminConfigs.ALTDIRECTORYSEPARATORCHAR;
                        try
                        {
                            // Cập nhật avatar
                            productService.UpdateAvatar(result, arrImg[0].caption, urlImg + AdminConfigs.AVATAR_WIDTH + "_" + AdminConfigs.AVATAR_HEIGHT + AdminConfigs.ALTDIRECTORYSEPARATORCHAR);

                            // Cập nhật tất cả ảnh vào DB
                            Entities.ProductImage pi;
                            foreach (Models.ImageConfig img in arrImg)
                            {
                                pi = new Entities.ProductImage();
                                pi.ProductId = result;
                                pi.Text = img.caption;
                                pi.Size = img.size;
                                pi.ImageUrl = urlImg;
                                pi.DateUpload = System.DateTime.Now;
                                productService.Insert(pi);

                                FileHelper.CropImage(Server.MapPath(Url.Content(urlImgTemp + img.caption)));
                            }
                            // Xử lý chuyển ảnh từ folder temp sang folder product.
                            FileHelper.MoveFile(sourceFolder, targetFolder);
                        }
                        catch
                        {
                            
                        }
                    }
                    #endregion
                    #region Xử lý cập nhật danh sách Product - Region
                    // Cập nhật vùng cố định.
                    Entities.Product_Region pr;
                    if (id == 0)
                    {
                        pr = new Entities.Product_Region();
                        // Insert
                        pr.ProductId = result;
                        pr.RegionId = model.regionFixedId;
                        pr.Fixed = true;
                        pr.Status = true;
                        productService.Insert(pr);

                        if (ListRegionNeighbor != null)
                        {
                            foreach (int regionId in ListRegionNeighbor)
                            {
                                pr = new Entities.Product_Region();
                                pr.ProductId = result;
                                pr.RegionId = regionId;
                                pr.Fixed = false;
                                pr.Status = true;
                                productService.Insert(pr);
                            }
                        }
                    }
                    else
                    {
                        // Update vùng cố định.
                        int regionFixedDB = productService.GetRegionFixed(id);
                        // Nếu đã tồn tại thì kiểm tra sự trùng khớp với regionFixedId 
                        if (regionFixedDB > 0)
                        {
                            // Nếu có sự khác thì cập nhật.
                            if (regionFixedDB != model.regionFixedId)
                            {
                                pr = productService.GetProductRegion(id, regionFixedDB);
                                pr.RegionId = model.regionFixedId;
                                productService.Update(pr);
                            }
                        }

                        // Cập nhật vùng lân cận.
                        // 1. Lấy danh sách vùng lân cận theo product ID

                        // 2. So sánh vùng lân cận đã lưu trong DB với vùng lân cận mới
                        //  + Nếu trong DB có mà danh sách mới không có thì update Status--> false.
                        //  + Nếu trong DB không có thì thêm mới.
                        var listNeighborRegionDB = productService.ListNeighborRegion(id);
                        if (listNeighborRegionDB != null)
                        {
                            foreach (Entities.Item item in listNeighborRegionDB)
                            {
                                if (ListRegionNeighbor != null)
                                {
                                    if (!ListRegionNeighbor.Contains(item.Id))
                                    {
                                        // Update status.
                                        pr = productService.GetProductRegion(id, item.Id);
                                        pr.Status = false;
                                        productService.Update(pr);
                                    }
                                }
                                else
                                {
                                    pr = productService.GetProductRegion(id, item.Id);
                                    pr.Status = false;
                                    productService.Update(pr);
                                }
                            }

                            if (ListRegionNeighbor != null)
                            {
                                foreach (int regionId in ListRegionNeighbor)
                                {
                                    if (!listNeighborRegionDB.Exists(x => x.Id == regionId))
                                    {
                                        // Add new
                                        pr = new Entities.Product_Region();
                                        pr.ProductId = id;
                                        pr.RegionId = regionId;
                                        pr.Fixed = false;
                                        pr.Status = true;
                                        productService.Insert(pr);
                                    }
                                    else
                                    {
                                        // Update if status = false.
                                        pr = productService.GetProductRegion(id, regionId);
                                        if (pr.Status == false)
                                        {
                                            pr.Status = true;
                                            productService.Update(pr);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (ListRegionNeighbor != null)
                            {
                                foreach (int regionId in ListRegionNeighbor)
                                {
                                    // Add new
                                    pr = new Entities.Product_Region();
                                    pr.ProductId = id;
                                    pr.RegionId = regionId;
                                    pr.Fixed = false;
                                    pr.Status = true;
                                    productService.Insert(pr);
                                }
                            }
                        }
                    }

                    #endregion

                    #region Xử lý gửi mail cho môi giới
                    // Lấy thông tin host.
                    Uri uri = Request.Url;
                    string urlHost = uri.GetLeftPart(UriPartial.Authority);
                    // Link sản phẩm.
                    string link = "<a href='" + urlHost + "/" + AdminConfigs.FRIENDLY_LINK_PRODUCT_DETAIL.ToFriendlyUrl(result, e.Text.ToAlias()) + "'>" + e.Text + "</a>";
                    ArticleService articleService = new ArticleService();
                    Entities.Article article = new Entities.Article();

                    // Danh sách những email cần phải gửi thông tin.
                    List<string> ListEmail = new List<string>();

                    // Lấy form mail từ bài viết
                    article = articleService.GetByAlias("[email_thong_bao_san_pham_moi]");

                    // Xử lý các tham số trong mail.
                    // Thay tên người bán
                    article.Body = article.Body.Replace("[user]", e.UserName);
                    // Thay tên sản phẩm
                    article.Title = article.Title.Replace("[product]", e.Text);

                    article.Body = article.Body.Replace("[product]", link);

                    // Lấy danh sách email của các môi giới
                    ListEmail = productService.ListEmailAgency(result);

                    BusinessLayer.Helpers.MailHelper.MailArticle(article, ListEmail);
                    #endregion
                    TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_SUCCESS;
                    isSuccess = true;
                }

            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                ViewBag.Message = AdminConfigs.MESSAGE_UPDATE_ERROR;
                ViewBag.AlertClass = AdminConfigs.CLASS_ALERT_DANGER;
            }
            
            LoadModel(ref model);

            LoadData();

            ViewBag.ActionForm = "UpdateSaleProduct";
            ViewBag.SubmitValue = id > 0 ? AdminConfigs.BUTTON_UPDATE : AdminConfigs.BUTTON_ADD;

            if (isSuccess)
            {
                //return RedirectToAction("Index", new { type="", page=1 });
                return RedirectToAction("Detail", "Template1", new { area="", Id=newId });
            }
            else
            {
                return PartialView("Index", model);
            }
        }

        /// <summary>
        /// Cập nhật sản phẩm cho thuê
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult UpdateLeaseProduct(int Id)
        {
            ViewBag.ActionSubMenu = "Lease";
            // Kiểm tra quyền đăng bài
            Entities.Item role = new Entities.Item();
            int result = CheckRolePost(ref role);
            if (result < 0)
            {
                return Redirect(result);
            }

            Models.ProductModel model = new Models.ProductModel();
            if (Id > 0)
            {
                Entities.Product e = productService.GetById(Id);
                model.MapFrom(e, ref model);
            }
            else
            {
                // set default.
                model.SellStartDate = DateTime.Now;
                model.SellEndDate = DateTime.Now.AddMonths(1);
                model.UserId = int.Parse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID));
                model.UserName = CookieHelper.Get(AdminConfigs.COOKIES_USERNAME);
            }
            LoadModel(ref model);

            // Add list product
            List<Entities.Product> lstEntity = productService.List();
            ViewBag.ListProduct = lstEntity;
            ViewBag.NumberItem = lstEntity.Count;



            ViewBag.ActionForm = "UpdateLeaseProduct";
            ViewBag.SubmitValue = Id > 0 ? AdminConfigs.BUTTON_UPDATE : AdminConfigs.BUTTON_POST_NEW;

            return PartialView("Index", model);
        }

        [HttpPost]
        public ActionResult UpdateLeaseProduct(int id, Models.ProductModel model, int[] ListRegionNeighbor)
        {
            ViewBag.ActionSubMenu = "Lease";
            bool isSuccess = false;
            int newId = 0;
            // Kiểm tra quyền đăng bài
            Entities.Item role = new Entities.Item();
            int roleId = CheckRolePost(ref role);
            if (roleId < 0)
            {
                Logs.LogWrite("UpdateLeaseProduct Kiểm tra quyền đăng");
                return Redirect(roleId);
            }

            if (ModelState.IsValid)
            {
                Entities.Product e = new Entities.Product();
                model.Id = id;
                model.Transaction_Type = (int)Enums.TransactionType.CHO_THUE; // nhà cho thuê
                model.UserId = int.Parse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID));

                // Chuyển tiền thành triệu.
                model.StandardCost = StringHelperExtension.MoneyExchange(model.StandardCost);

                model.MapFrom(model, ref e);
                if (id <= 0)
                {
                    e.UpdateBy = e.UserId;
                    // Lấy thông tin username
                    e.UserName = CookieHelper.Get(AdminConfigs.COOKIES_USERNAME);

                    // Lấy thông tin role.
                    e.RoleId = role.Id;
                    e.RoleText = role.Text;
                }

                int result = 0;
                result = productService.Save(e);
                if (result < 0)
                {
                    ViewBag.Message = AdminConfigs.MESSAGE_UPDATE_ERROR;
                    ViewBag.AlertClass = AdminConfigs.CLASS_ALERT_DANGER;
                    TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_ERROR;
                }
                else
                {
                    newId = result;
                    #region Xử lý thêm ảnh vào database
                    Models.ImageConfig[] arrImg = GetAllFileInfo(id, ImageType.PRODUCT_TEMP, e.UserName);
                    if (arrImg.Count() >0)
                    {
                        string sourceFolder = AdminConfigs.PHYSICAL_PATH + CookieHelper.Get(AdminConfigs.COOKIES_USERNAME).ToLower() + AdminConfigs.DIRSEPARATOR + AdminConfigs.FOLDER_PRODUCT_TEMPS;
                        string targetFolder = AdminConfigs.PHYSICAL_PATH + CookieHelper.Get(AdminConfigs.COOKIES_USERNAME).ToLower() + AdminConfigs.DIRSEPARATOR + AdminConfigs.FOLDER_PRODUCTS;
                        string urlImg = AdminConfigs.IMAGE_PATH + CookieHelper.Get(AdminConfigs.COOKIES_USERNAME).ToLower() + AdminConfigs.ALTDIRECTORYSEPARATORCHAR + AdminConfigs.FOLDER_PRODUCTS + AdminConfigs.ALTDIRECTORYSEPARATORCHAR;
                        string urlImgTemp = AdminConfigs.IMAGE_PATH + CookieHelper.Get(AdminConfigs.COOKIES_USERNAME).ToLower() + AdminConfigs.ALTDIRECTORYSEPARATORCHAR + AdminConfigs.FOLDER_PRODUCT_TEMPS + AdminConfigs.ALTDIRECTORYSEPARATORCHAR;
                        try
                        {
                            // Cập nhật avatar
                            productService.UpdateAvatar(result, arrImg[0].caption, urlImg + AdminConfigs.AVATAR_WIDTH + "_" + AdminConfigs.AVATAR_HEIGHT + AdminConfigs.ALTDIRECTORYSEPARATORCHAR);

                            // Cập nhật tất cả ảnh vào DB
                            Entities.ProductImage pi;
                            foreach (Models.ImageConfig img in arrImg)
                            {
                                pi = new Entities.ProductImage();
                                pi.ProductId = result;
                                pi.Text = img.caption;
                                pi.Size = img.size;
                                pi.ImageUrl = urlImg;
                                pi.DateUpload = System.DateTime.Now;
                                productService.Insert(pi);

                                FileHelper.CropImage(Server.MapPath(Url.Content(urlImgTemp + img.caption)));
                            }
                            // Xử lý chuyển ảnh từ folder temp sang folder product.
                            FileHelper.MoveFile(sourceFolder, targetFolder);
                        }
                        catch
                        {

                        }
                    }
                    #endregion

                    #region Xử lý cập nhật danh sách Product - Region
                    // Cập nhật vùng cố định.
                    Entities.Product_Region pr;
                    if (id == 0)
                    {
                        pr = new Entities.Product_Region();
                        // Insert
                        pr.ProductId = result;
                        pr.RegionId = model.regionFixedId;
                        pr.Fixed = true;
                        pr.Status = true;
                        productService.Insert(pr);

                        if (ListRegionNeighbor != null)
                        {
                            foreach (int regionId in ListRegionNeighbor)
                            {
                                pr = new Entities.Product_Region();
                                pr.ProductId = result;
                                pr.RegionId = regionId;
                                pr.Fixed = false;
                                pr.Status = true;
                                productService.Insert(pr);
                            }
                        }
                    }
                    else
                    {
                        // Update vùng cố định.
                        int regionFixedDB = productService.GetRegionFixed(id);
                        // Nếu đã tồn tại thì kiểm tra sự trùng khớp với regionFixedId 
                        if (regionFixedDB > 0)
                        {
                            // Nếu có sự khác thì cập nhật.
                            if (regionFixedDB != model.regionFixedId)
                            {
                                pr = productService.GetProductRegion(id, regionFixedDB);
                                pr.RegionId = model.regionFixedId;
                                productService.Update(pr);
                            }
                        }

                        // Cập nhật vùng lân cận.
                        // 1. Lấy danh sách vùng lân cận theo product ID

                        // 2. So sánh vùng lân cận đã lưu trong DB với vùng lân cận mới
                        //  + Nếu trong DB có mà danh sách mới không có thì update Status--> false.
                        //  + Nếu trong DB không có thì thêm mới.
                        var listNeighborRegionDB = productService.ListNeighborRegion(id);
                        if (listNeighborRegionDB != null)
                        {
                            foreach (Entities.Item item in listNeighborRegionDB)
                            {
                                if (!ListRegionNeighbor.Contains(item.Id))
                                {
                                    // Update status.
                                    pr = productService.GetProductRegion(id, item.Id);
                                    pr.Status = false;
                                    productService.Update(pr);
                                }
                            }

                            foreach (int regionId in ListRegionNeighbor)
                            {
                                if (!listNeighborRegionDB.Exists(x => x.Id == regionId))
                                {
                                    // Add new
                                    pr = new Entities.Product_Region();
                                    pr.ProductId = id;
                                    pr.RegionId = regionId;
                                    pr.Fixed = false;
                                    pr.Status = true;
                                    productService.Insert(pr);
                                }
                                else
                                {
                                    // Update if status = false.
                                    pr = productService.GetProductRegion(id, regionId);
                                    if (pr.Status == false)
                                    {
                                        pr.Status = true;
                                        productService.Update(pr);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (int regionId in ListRegionNeighbor)
                            {
                                // Add new
                                pr = new Entities.Product_Region();
                                pr.ProductId = id;
                                pr.RegionId = regionId;
                                pr.Fixed = false;
                                pr.Status = true;
                                productService.Insert(pr);
                            }
                        }
                    }
                    #endregion

                    #region Xử lý gửi mail cho môi giới
                    // Lấy thông tin host.
                    Uri uri = Request.Url;
                    string urlHost = uri.GetLeftPart(UriPartial.Authority);
                    // Link sản phẩm.
                    string link = "<a href='" + urlHost + "/" + AdminConfigs.FRIENDLY_LINK_PRODUCT_DETAIL.ToFriendlyUrl(result, e.Text.ToAlias()) + "'>" + e.Text + "</a>";
                    ArticleService articleService = new ArticleService();
                    Entities.Article article = new Entities.Article();

                    // Danh sách những email cần phải gửi thông tin.
                    List<string> ListEmail = new List<string>();

                    // Lấy form mail từ bài viết
                    article = articleService.GetByAlias("[email_thong_bao_san_pham_moi]");

                    // Xử lý các tham số trong mail.
                    // Thay tên người bán
                    article.Body = article.Body.Replace("[user]", e.UserName);
                    // Thay tên sản phẩm
                    article.Title = article.Title.Replace("[product]", e.Text);

                    article.Body = article.Body.Replace("[product]", link);

                    // Lấy danh sách email của các môi giới
                    ListEmail = productService.ListEmailAgency(result);

                    BusinessLayer.Helpers.MailHelper.MailArticle(article, ListEmail);
                    #endregion
                    TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_SUCCESS;
                    isSuccess = true;
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                ViewBag.Message = AdminConfigs.MESSAGE_UPDATE_ERROR;
                ViewBag.AlertClass = AdminConfigs.CLASS_ALERT_DANGER;
            }

            LoadModel(ref model);

            ViewBag.ActionForm = "UpdateLeaseProduct";
            ViewBag.SubmitValue = id > 0 ? AdminConfigs.BUTTON_UPDATE : AdminConfigs.BUTTON_ADD;

            if (isSuccess)
            {
                //return RedirectToAction("Index", new { type = "", page = 1 });
                return RedirectToAction("Detail", "Template1", new { area = "", Id = newId });
            }
            else
            {
                return PartialView("Index", model);
            }
        }

        [HttpPost]
        public JsonResult UpdateImage(string username, IEnumerable<HttpPostedFileBase> ListImage)
        {
            if (ListImage != null)
            {
                foreach (var file in ListImage)
                {
                    FileHelper.UploadFile(file, username, Enums.ImageType.PRODUCT_TEMP);
                }
            }
            return Json("Success");
        }

        [HttpPost]
        public JsonResult DeleteImage(string image, int productId = 0, string username = "")
        {
            int result = 0;
            if (productId > 0)
            {
               result = productService.DeleteImage(image, productId);
            }
            if (result > 0)
            {
                FileHelper.DeleteFile(image, productId, username);
            }
            return Json("Success");
        }

        public void LoadModel(ref Models.ProductModel m)
        {
            
            PlaceService placeService = new PlaceService();
            RegionService regionService = new RegionService();
            UserInRolesService userInRoleService = new UserInRolesService();

            // Xử lý lấy ID tỉnh, thành phố; quận, huyện.
            // Trường hợp có Xã, phường
            //if (m.PlaceId > 0)
            //{
            //    var temp = placeService.GetParentItem(m.PlaceId);
            //    m.DistrictId = temp.Id;

            //    temp = placeService.GetParentItem(m.DistrictId);
            //    m.CityId = temp.Id;
            //}

            // Danh sách kiểu nhà
            m.ListProductType = productService.ListProductType();

            // Danh sách tỉnh
            m.ListCity = placeService.ListPlaceItemByParent(0);
            if (m.CityId > 0)
            {
                m.ListDistrict = placeService.ListPlaceItemByParent(m.CityId);
            }
            else
            {
                m.ListDistrict = new List<Entities.Item>();
            }

            // Lấy danh sách quận huyện
            if (m.DistrictId > 0)
            {
                m.ListWard = placeService.ListPlaceItemByParent(m.DistrictId);
            }
            else
            {
                m.ListWard = new List<Entities.Item>();
            }

            // Lấy đơn vị tính.
            m.ListCostUnit = productService.ListCostUnit();

            List<Entities.Item> lstRegion = regionService.ListItemActive();
            int id = m.Id;
            if (id > 0)
            {
                // Lấy thông tin vùng cố định
                var fixedRegion = regionService.GetFixedRegion(m.WardId);
                if (fixedRegion != null)
                {
                    m.regionFixedId = fixedRegion.Id;
                    m.regionFixedText = fixedRegion.Text;
                }
                
                // Lấy danh sách ảnh
                var lstImg = productService.GetListImage(id);
                m.ListImage = lstImg.Select(x => Url.Content(x.ImageUrl + "\\" + x.Text)).ToArray();
                string username = m.UserName;
                var imgConfig = lstImg.Select(y => new Models.ImageConfig()
                                {
                                    caption = y.Text,
                                    size = y.Size,
                                    key = y.Text,
                                    url = @Url.Action("DeleteImage", "Product", new { image = y.Text, productId = id, username = username })
                                }
                    ).ToArray();
                m.LstImageConfig = imgConfig;


                // Lấy thông tin vùng lân cận.
                List<int> regionIds = new List<int>();
                regionIds.Add(m.regionFixedId);
                lstRegion = RegionSelected(m.regionFixedId, id, ref regionIds, m.CityId);

                // Lấy chuỗi các xã phường tương ứng.
                //m.StrWards = StringPlaceByRegions(regionIds);

                // Lấy hướng dẫn chi phí cho môi giới
                switch (m.CostUnit)
                {
                    case 1:
                        m.StrTutrialCostUnit = AdminConfigs.TUTORIAL_PERCEN_HOUSE;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Lấy thông tin vùng cố định
                m.regionFixedId = 0;

                // Lấy thời gian bán mặc định.
                m.SellStartDate = System.DateTime.Now;

                // Lấy danh sách ảnh.
                m.ListImage = GetAllFileTemp(m.UserName);
                m.LstImageConfig = GetAllFileInfo(id, ImageType.PRODUCT_TEMP, m.UserName);

                m.StrTutrialCostUnit = AdminConfigs.TUTORIAL_PERCEN_HOUSE;
            }

            // Lấy danh sách vùng.
            m.ListRegion = lstRegion;

            // Kiểm tra xem người đăng có phải môi giới không.
            m.IsAgency = userInRoleService.CheckUserIsAgency(m.UserId);

            // Lấy danh sách vùng gửi cố định theo user.
            m.ListFixedSendRegion = uirService.GetListItemByUser(m.UserId, 1);

            // Lấy danh sách xã, phường theo vùng cố định khi chọn địa chỉ.
            List<int> regionByAddress = new List<int>();
            if (m.regionFixedId > 0)
            {
                m.StrWards2 = StringPlaceByRegions(m.regionFixedId);
            }

            // Lấy danh sách xã, phường theo vùng cố định theo quyền môi giới.
            foreach (var item in m.ListFixedSendRegion)
            {
                regionByAddress.Add(item.Id);
            }
            m.StrWards1 = StringPlaceByRegions(regionByAddress);
        }

        /// <summary>
        /// Lấy danh sách vùng lân cận.
        /// </summary>
        /// <param name="lstItem"></param>
        /// <returns></returns>
        public PartialViewResult ListNeighborRegion(int regionId, bool isAgency, int cityId)
        {
            List<int> regionIds = new List<int>();
            var lstItem = RegionSelected(regionId, 0, ref regionIds, cityId);
            
            ViewData["ListNeighBorRegion"] = lstItem;
            ViewData["IsAgency"] = isAgency;
            return PartialView();
        }

        public List<Entities.Item> RegionSelected(int regionId, int productId, ref List<int> regionsIds, int cityId)
        {
            List<Entities.Item> lstItem = new List<Entities.Item>();

            if (cityId > 0)
            {
                // Lấy danh sách vùng theo tỉnh.
                lstItem = regionService.ListItemActive(cityId);
            }
            else
            {
                // Lấy danh sách vùng lân cận.
                lstItem = regionService.ListItemActive();
            }
            // Xóa vùng gốc.
            lstItem.RemoveAll(x => x.Id == regionId);

            // Lấy vùng lân cận
            // Trường hợp có id sản phẩm (dùng trong cập nhật)
            if (productId > 0)
            {
                var regionChecked = productService.ListNeighborRegion(productId);
                if (regionChecked != null)
                {
                    foreach (Entities.Item item in lstItem)
                    {
                        if (regionChecked.Exists(r => r.Id == item.Id))
                        {
                            item.Checked = true;
                            regionsIds.Add(item.Id);
                        }                        
                    }
                }
            }
            else
            {
                var result = regionService.GetNeighBorRegion(regionId);

                if (result != null)
                {
                    lstItem.RemoveAll(e => e.Id == result.Id);
                    lstItem.Insert(0, result);
                    lstItem[0].Checked = true;
                }
                regionsIds = new List<int>();
            }
            return lstItem;
        }

        public string[] GetAllFileTemp(string username)
        {
            var arr = FileHelper.GetAllFileTemp(username);
            if (arr != null)
            {
                string[] result = new string[arr.Count()];
                for (int i = 0; i < arr.Count(); i++)
                {
                    result[i] = Url.Content(arr[i]);
                }
                return result;
            }
            return null;
        }

        public Models.ImageConfig[] GetAllFileInfo(int productId, Enums.ImageType type, string username )
        {
            var lstImageTemp = FileHelper.GetAllFileInfo(type, username);
            Models.ImageConfig img;
            if (lstImageTemp != null)
            {
                int count = lstImageTemp.Count();
                Models.ImageConfig[] arrImg = new Models.ImageConfig[count];
                string imgname = "";
                for (int i = 0; i < count; i++)
                {
                    imgname = lstImageTemp[i].Text;
                    img = new Models.ImageConfig();
                    img.caption = imgname;
                    img.size = lstImageTemp[i].Size;
                    img.url = @Url.Action("DeleteImage", "Product", new { image = imgname, productId = productId, username = username});
                    img.key = imgname;
                    arrImg[i] = img;
                }
                return arrImg;
            }
           
            return null;
        }

        // Chuyển trạng thái nhà sang đã bán.
        public JsonResult UpdateWasSelling(int productId)
        {
            if (productService.UpdateWasSelling(productId) > 0)
            {
                TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_SUCCESS;
            }
            return Json("Success");
        }

        #region product display
        /// <summary>
        /// Danh sách sản phẩm theo dõi
        /// </summary>
        /// <param name="userId">đối tượng cần theo dõi</param>
        /// <param name="viewType">kiểu xem (0: nhìn từ màn hình sản phẩm, 1: nhìn từ Html.Action)</param>
        /// <returns></returns>
        public ActionResult ProductFollow(int userId = 0, int viewType = 0)
        {
            if (userId == 0)
            {
                userId = int.Parse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID));
            }

            ViewBag.ActionSubMenu = "ProductFollow";
            // Get Product follow
            List<Entities.Product> lstEntity = productService.GetListProductFollow(userId);

            int totalRecord = 0;
            // Add list product
            ViewBag.ListProduct = lstEntity;
            ViewBag.NumberItem = lstEntity.Count;
            totalRecord = lstEntity.Count;
            
            Paging(Page, totalRecord);
            ViewBag.BoxTitle = "Danh sách nhà theo dõi";

            if (viewType == 1)
            {
                return PartialView("ListProduct2");
            }
            return PartialView("Index");
        }

        public ActionResult ProductSale(int userId = 0, int viewType = 0)
        {
            if (userId == 0)
            {
                userId = int.Parse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID));
            }
            // Get Product sale
            ViewBag.ActionSubMenu = "ProductSale";

            List<Entities.Product> lstEntity = productService.List(userId: userId, transactionType: 1);
            int totalRecord = 0;
            // Add list product
            ViewBag.ListProduct = lstEntity;
            ViewBag.NumberItem = lstEntity.Count;
            totalRecord = lstEntity.Count;

            Paging(Page, totalRecord);
            ViewBag.BoxTitle = "Danh sách nhà đăng bán";
            if (viewType == 1)
            {
                return PartialView("ListProduct2");
            }
            return PartialView("Index");
        }

        /// <summary> Danh sách sản phẩm nhận bán
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductReceiveSelling(int userId=0, int viewType = 0)
        {
            if (userId == 0)
            {
                userId = int.Parse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID));
            }

            // Get Product Receive selling
            ViewBag.ActionSubMenu = "ProductReceiveSelling";

            // Lấy vùng của người dùng.
            UserInRegionService userInRegionService = new UserInRegionService();
            List<Entities.UserInRegion> lst = userInRegionService.GetListByUser(userId);

            int[] regionIds = lst.Where(x=>x.Status).Select(x => x.RegionId).ToArray();
            
            // Lấy ID sản phẩm chứa vùng.
            List<int> listProductId = productService.ListProductByRegions(regionIds);

            List<Entities.Product> lstEntity = productService.GetByIds(listProductId);
            int totalRecord = 0;
            // Add list product
            ViewBag.ListProduct = lstEntity;
            ViewBag.NumberItem = lstEntity.Count;
            totalRecord = lstEntity.Count;

            // Get Max of role by user.
            RoleService roleService = new RoleService();
            Entities.Role maxRole = roleService.GetMaxRolesOfUser(userId);
            ViewBag.MaxRole = maxRole;

            Paging(Page, totalRecord);
            ViewBag.BoxTitle = "Danh sách nhà nhận bán";
            if (viewType == 1)
            {
                return PartialView("ListProduct2");
            }
            return PartialView("Index");
        }

        public ActionResult ProductWasSelling()
        {
            // Lấy danh sách nhà đã bán
            ViewBag.ActionSubMenu = "ProductWasSelling";
            List<Entities.Product> lstEntity = productService.List(state: 1);
            int totalRecord = 0;
            // Add list product
            ViewBag.ListProduct = lstEntity;
            ViewBag.NumberItem = lstEntity.Count;
            totalRecord = lstEntity.Count;
            
            Paging(Page, totalRecord);
            ViewBag.BoxTitle = "Danh sách nhà đã bán";
            return PartialView("Index");
        }
        #endregion
    }
}
