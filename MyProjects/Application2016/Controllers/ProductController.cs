using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Application2016.Models;
using Application2016.Helpers;
using BusinessLayer.Enums;
//using BusinessLayer.Helpers;

namespace Application2016.Controllers
{
    public class ProductController : Controller
    {
        private ProductService _service;

        public ProductController()
        {
            _service = new ProductService();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Detail(int Id)
        {
            ProductService _service = new ProductService();
            Entities.Product product = _service.GetById(Id);
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

                // Lấy danh sách ảnh sản phẩm.
                model.ListImage = _service.GetListImage(Id);

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
                model.NewCost = _service.GetCostChangeByOwn(Id, product.UserId);
                
                // Lấy danh sách giá đặt mua
                List<Entities.Product_ChangeCost> lstChangeCost = _service.GetListProductChangeCost(Id, product.UserId);
                model.ListChangeCost = lstChangeCost;

            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Preview(int Id)
        {
            Entities.Product product = _service.GetById(Id);
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

                // Lấy danh sách ảnh sản phẩm.
                model.ListImage = _service.GetListImage(Id);

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
                model.NewCost = _service.GetCostChangeByOwn(Id, product.UserId);

                // Lấy danh sách giá đặt mua
                List<Entities.Product_ChangeCost> lstChangeCost = _service.GetListProductChangeCost(Id, product.UserId);
                model.ListChangeCost = lstChangeCost;

            }
            return View(model);
        }

        [HttpPost]
        public JsonResult DoChangeCost(int productId, long money)
        {
            SelectListItem item = new SelectListItem();
            int userId = 0; // UserId

            // Kiểm tra đăng nhập
            if (CookieHelper.Get(AdminConfigs.COOKIES_USER_ID) == null)
            {
                item.Text = AdminConfigs.ERROR_NOT_LOGIN;
                item.Value = ((int)Errors.NOT_LOGIN).ToString();
                return Json(item);
            }
            else
            {
                userId = int.Parse(CookieHelper.Get(AdminConfigs.COOKIES_USER_ID));
            }
            Entities.Product product = _service.GetById(productId);

            UserService _userService = new UserService();
            
            // Người đăng nhập
            Entities.User user = _userService.GetById(userId);
            
            // Đổi số tiền sang đơn vị triệu
            money = StringHelperExtension.MoneyExchange(money);

            // Kiểm tra số lần đổi giá.
            
            // Kiểm tra số tiền nhập vào.
            // Số tiền phải nhỏ hơn giá hiện tại
            // Lớn hơn 1/10 giá trị ngôi nhà.
            item = CheckInputCost(_service, product, money);
            if (int.Parse(item.Value) < 0)
            {
                TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_ERROR;
                return Json(item);
            }
            
            // Lấy thông tin thay đổi giá
            Entities.Product_ChangeCost productChangeCost = new Entities.Product_ChangeCost();
            productChangeCost.ProductId = productId;
            productChangeCost.UserId = userId;
            productChangeCost.UserName = user.UserName;
            productChangeCost.PhoneNumber = user.Phone;
            productChangeCost.Cost = money;

            // Lấy danh sách những người đang theo dõi ngoại trừ người thực hiện thao tác này.
            List<int> ListUserIdFollow = _service.ListUserIdProductFollow(productId);
            ListUserIdFollow.Remove(userId);

            // Danh sách những email cần phải gửi thông tin.
            List<string> ListEmail = new List<string>();

            // Lấy thông tin lần cập nhật cuối cùng
            Entities.Product_ChangeCost lastChangeCost = _service.GetLastChangeCost(productId, productChangeCost.UserId);

            int result = _service.UpdateProductChangeCost(productChangeCost);
            if (result < 0)
            {
                item = new SelectListItem();
                item.Text = AdminConfigs.ERROR_PRODUCT_NOT_KNOW;
                item.Value = ((int)ProductCase.NO).ToString();
            }
            else
            {
                // Chuyển thành trạng thái theo dõi.
                _service.ChangeFollow(productChangeCost.ProductId, productChangeCost.UserId, true);

                // Lấy thông tin host.
                Uri uri = Request.Url;
                string urlHost = uri.GetLeftPart(UriPartial.Authority);
                // Link sản phẩm.
                string link = "<a href='" + urlHost + "/" + AdminConfigs.FRIENDLY_LINK_PRODUCT_DETAIL.ToFriendlyUrl(product.Id, product.Text.ToAlias()) + "'>" + product.Text + "</a>";
                ArticleService articleService = new ArticleService();
                Entities.Article article = new Entities.Article();

                // Cập nhật vào bảng sản phẩm trong trường hợp chính chủ.
                if (product.UserId == userId)
                {
                    product.UpdateCost = money;
                    product.UpdateBy = userId;
                    result = _service.UpdateCost(product);

                    Entities.ProductMessage message = new Entities.ProductMessage();
                    if (result > 0)
                    {
                        // Lấy danh sách những người đã đặt giá
                        List<Entities.Product_ChangeCost> lstChangeCost = _service.GetListProductChangeCost(product.Id, product.UserId, 10000);
                        if (lstChangeCost.Count > 0)
                        {
                            foreach (var obj in lstChangeCost)
                            {
                                // Nếu giá của người muốn mua > giá muốn bán thì sẽ cập nhật vào bảng muốn bán sản phẩm
                                if (obj.Cost >= product.UpdateCost)
                                {
                                    Entities.Product_WantSell pws = new Entities.Product_WantSell();
                                    pws.ProductId = product.Id;
                                    pws.Text = product.Text;
                                    pws.CostSell = (int)product.UpdateCost;
                                    pws.BuyerId = obj.UserId;
                                    pws.CostBuy = (int)obj.Cost;
                                    pws.UpdateTime = DateTime.Now;
                                    pws.Times = 1;
                                    pws.IsChecked = false;
                                    _service.InsertProductWantSell(pws);

                                    // Cập nhật vào bảng thông báo cho người này
                                    message = new Entities.ProductMessage();
                                    message.ProductId = product.Id;
                                    message.ProductText = product.Text;
                                    message.Read_Flag = false;
                                    message.Delete_Flag = false;
                                    message.CreateDate = DateTime.Now;
                                    message.From = product.UserId;
                                    message.To = obj.UserId;
                                    // người bán [user] muốn bán sản phẩm [product] cho bạn. Hãy gọi điện tới số [phone] để có thể mua sản phẩm này"
                                    message.Content = string.Format(AdminConfigs.MESSAGE_PRODUCT_04);

                                    _service.Insert(message);

                                    #region Gửi mail cho những người liên quan.
                                    // Lấy form mail từ bài viết
                                    article = articleService.GetByAlias("[email_thong_bao_muon_ban_san_pham]");

                                    // Xử lý các tham số trong mail.
                                    // Thay tên người bán
                                    article.Body = article.Body.Replace("[user]", user.UserName);
                                    // Thay tên sản phẩm
                                    article.Title = article.Title.Replace("[product]", product.Text);

                                    article.Body = article.Body.Replace("[product]", link);

                                    // Lấy số phone người bán
                                    article.Body = article.Body.Replace("[phone]", user.Phone);

                                    // Lấy email của người mua.
                                    ListEmail = new List<string>();
                                    ListEmail.Add(obj.Email);
                                    var temp = _service.ListEmailAgency(product.Id);
                                    if (temp.Count > 0)
                                    {
                                        ListEmail.AddRange(temp);
                                    }

                                    BusinessLayer.Helpers.MailHelper.MailArticle(article, ListEmail);
                                    #endregion
                                }
                                else
                                {
                                    // Cập nhật vào danh sách những người cần được thông báo
                                    if (!ListUserIdFollow.Contains(obj.UserId))
                                    {
                                        ListUserIdFollow.Add(obj.UserId);
                                    }
                                }
                            }
                        }
                    }

                    // Cập nhật thông báo cho những người đang theo dõi & những người đã đặt giá.
                    message = new Entities.ProductMessage();
                    message.ProductId = product.Id;
                    message.ProductText = product.Text;
                    message.Read_Flag = false;
                    message.Delete_Flag = false;
                    message.CreateDate = DateTime.Now;
                    message.From = product.UserId;
                    //message.To = obj.UserId;
                    // người bán [user] đã thay đổi giá bán sản phẩm [product] thành {0}";
                    message.Content = string.Format(AdminConfigs.MESSAGE_PRODUCT_03, money.MoneyFormat());
                    _service.InsertList(message, ListUserIdFollow);
                    //_service.Insert(message);

                    #region Gửi mail cho những người liên quan.
                    // Lấy form mail từ bài viết
                    article = articleService.GetByAlias("[email_thong_bao_sua_gia_ban]");

                    // Xử lý các tham số trong mail.
                    // Thay số tiền
                    article.Body = article.Body.Replace("[gia_ban_moi]", money.MoneyFormat());
                    // Thay tên người bán
                    article.Body = article.Body.Replace("[user]", user.UserName);
                    // Thay tên sản phẩm
                    article.Title = article.Title.Replace("[product]", product.Text);

                    
                    article.Body = article.Body.Replace("[product]", link);

                    // Lấy danh sách email những người đang theo dõi.
                    ListEmail = new List<string>();
                    ListEmail = _service.ListUserEmailFollow(product.Id, userId);

                    // Lấy danh sách email của các môi giới
                    var temp2 = _service.ListEmailAgency(product.Id);
                    if (temp2.Count > 0)
                    {
                        ListEmail.AddRange(temp2);
                    }
                    
                    BusinessLayer.Helpers.MailHelper.MailArticle(article, ListEmail);
                    #endregion
                }
                else
                {
                    Entities.ProductMessage message = new Entities.ProductMessage();
                    message.ProductId = product.Id;
                    message.ProductText = product.Text;
                    message.Read_Flag = false;
                    message.Delete_Flag = false;
                    message.CreateDate = DateTime.Now;
                    message.From = productChangeCost.UserId;
                    //message.To = product.UserId;
                    if (!ListUserIdFollow.Contains(product.UserId))
                    {
                        ListUserIdFollow.Add(product.UserId);
                    }

                    if (lastChangeCost != null)
                    {
                        // người mua [user] đã thay đổi giá mua sản phẩm [product] từ {0} thành {1}";
                        message.Content = string.Format(AdminConfigs.MESSAGE_PRODUCT_02, lastChangeCost.Cost.MoneyFormat(), money.MoneyFormat());

                        _service.InsertList(message, ListUserIdFollow);

                        #region Gửi mail cho những người liên quan.
                        // Lấy form mail từ bài viết
                        article = articleService.GetByAlias("[email_thong_bao_sua_gia_mua]");

                        // Xử lý các tham số trong mail.
                        // Thay số tiền
                        article.Body = article.Body.Replace("[gia_cu]", lastChangeCost.Cost.MoneyFormat());
                        article.Body = article.Body.Replace("[gia_moi]", money.MoneyFormat());
                        // Thay tên người mua
                        article.Body = article.Body.Replace("[user]", user.UserName);
                        // Thay tên sản phẩm
                        article.Title = article.Title.Replace("[product]", product.Text);

                        article.Body = article.Body.Replace("[product]", link);

                        // Lấy danh sách những người đang theo dõi sản phẩm
                        ListEmail = new List<string>();
                        ListEmail = _service.ListUserEmailFollow(product.Id, userId);
                        
                        // Lấy email người bán sản phẩm.
                        string email = _service.GetEmailByProductId(product.Id);
                        ListEmail.Add(email);

                        // Lấy danh sách email của các môi giới
                        var temp = _service.ListEmailAgency(product.Id);
                        if (temp.Count > 0)
                        {
                            ListEmail.AddRange(temp);
                        }

                        BusinessLayer.Helpers.MailHelper.MailArticle(article, ListEmail);
                        #endregion
                    }
                    else
                    {
                        // người mua [user] đã đặt mua sản phẩm [product] với giá {0}";
                        message.Content = string.Format(AdminConfigs.MESSAGE_PRODUCT_01, money.MoneyFormat());

                        _service.InsertList(message, ListUserIdFollow);

                        #region Gửi mail cho những người liên quan.
                        // Lấy form mail từ bài viết
                        article = articleService.GetByAlias("[email_thong_bao_dat_gia_mua]");

                        // Xử lý các tham số trong mail.
                        // Thay số tiền
                        article.Body = article.Body.Replace("[gia_mua]", money.MoneyFormat());
                        // Thay tên người mua
                        article.Body = article.Body.Replace("[user]", user.UserName);
                        // Thay tên sản phẩm
                        article.Title = article.Title.Replace("[product]", product.Text);

                        article.Body = article.Body.Replace("[product]", link);

                        // Lấy email người bán sản phẩm.
                        ListEmail = new List<string>();
                        string email = _service.GetEmailByProductId(product.Id);
                        ListEmail.Add(email);

                        // Lấy danh sách email của các môi giới
                        var temp = _service.ListEmailAgency(product.Id);
                        if (temp.Count > 0)
                        {
                            ListEmail.AddRange(temp);
                        }
                        if (temp.Count > 0)
                        {
                            ListEmail.AddRange(temp);
                        }

                        BusinessLayer.Helpers.MailHelper.MailArticle(article, ListEmail);
                        #endregion
                    }
                }
                TempData[AdminConfigs.TEMP_MESSAGE] = AdminConfigs.MESSAGE_UPDATE_SUCCESS;
                item.Text = AdminConfigs.MESSAGE_PRODUCT_ORDER_OK;
                item.Value = "1";
            }
            return Json(item);
        }

        /// <summary>
        /// Update Product Follow
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateProductFollow(int productId)
        {
            SelectListItem item = new SelectListItem();
            string strUserId = CookieHelper.Get(AdminConfigs.COOKIES_USER_ID);
            if (strUserId == null)
            {
                item.Text = AdminConfigs.ERROR_NOT_LOGIN;
                item.Value = ((int)Errors.NOT_LOGIN).ToString();
                return Json(item);
            }
            int result = _service.ChangeFollow(productId, int.Parse(strUserId), true);
            item.Value = result.ToString();
            if (result > 0)
            {
                item.Text = AdminConfigs.MESSAGE_PRODUCT_FOLLOW_OK;
            }
            else
            {
                item.Text = AdminConfigs.MESSAGE_UPDATE_ERROR;
            }
            return Json(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_service"></param>
        /// <param name="productId"></param>
        /// <param name="newCost"></param>
        /// <returns></returns>
        private SelectListItem CheckInputCost(ProductService _service, Entities.Product product, decimal newCost)
        {
            SelectListItem item = new SelectListItem();
            if (product == null)
            {
                item.Text = AdminConfigs.ERROR_PRODUCT_NOT_EXISTS;
                item.Value = ((int)ProductCase.NOT_EXISTS).ToString();
                return item;
            } 

            decimal costUpdate = _service.GetCostChangeByOwn(product.Id, product.UserId);
            if (costUpdate == 0)
            {
                costUpdate = product.StandardCost;
            }

            // Kiểm tra số tiền mới có nhỏ hơn tiền hiện tại hay không.
            if (costUpdate <= newCost)
            {
                item.Text = AdminConfigs.ERROR_PRODUCT_LARGER;
                item.Value = ((int)ProductCase.LARGER).ToString();
                return item;
            }

            // Kiểm tra số tiền mới có lớn hơn 1/10 số tiền giá bán hay không.
            if (costUpdate / 10 > newCost)
            {
                item.Text = AdminConfigs.ERROR_PRODUCT_SMALL;
                item.Value = ((int)ProductCase.SMALL).ToString();
                return item;
            }

            if (CookieHelper.Get(AdminConfigs.COOKIES_USER_ID) == product.UserId.ToString())
            {
                item.Text = AdminConfigs.MESSAGE_PRODUCT_CHANGE_COST_OK;
            }
            else
            {
                item.Text = AdminConfigs.MESSAGE_PRODUCT_ORDER_OK;
            }
            item.Value = ((int)ProductCase.OK).ToString();
            return item;
        }
    }
}
