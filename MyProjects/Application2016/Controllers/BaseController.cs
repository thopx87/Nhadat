using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application2016.Areas.Admin.Models;
using BusinessLayer;
using Application2016.Helpers;
using Application2016.Models;
namespace Application2016.Controllers
{
    public class BaseController : Controller
    {
        private ProductService productService = new ProductService();
        private PlaceService placeService = new PlaceService();

        public BaseController()
        {
    
        }

        // Phân trang.
        public void Paging(int page, int totalRecord, int pageSize)
        {
            ViewBag.page = page;
            ViewBag.pageSize = pageSize;

            ViewBag.MaxPage = (int)Math.Ceiling((double)totalRecord / pageSize);
            ViewBag.PageShow = AdminConfigs.PAGE_SHOW;
            ViewBag.PagePreview = AdminConfigs.PAGE_PREVIEW;
            ViewBag.TotalRecord = totalRecord;
        }

        public ActionResult ListProduct()
        {

            return PartialView();
        }

        public ActionResult BoxFeature()
        {
            return PartialView();
        }

        public ActionResult BoxLastestArticle()
        {
            return PartialView();
        }

        public ActionResult BoxBlandLogo()
        {
            return PartialView();
        }

        public ActionResult BoxContact()
        {
            ViewBag.WebsiteInfo = HttpContext.Application[AdminConfigs.SETTING_WEBSITE];
            return PartialView();
        }

        public ActionResult BoxGuide()
        {
            return PartialView();
        }

        public ActionResult BoxInformation()
        {
            return PartialView();
        }

        public ActionResult BoxSocialNetwork()
        {
            return PartialView();
        }

        public ActionResult BoxHelpSlider()
        {
            ViewBag.WebsiteInfo = HttpContext.Application[AdminConfigs.SETTING_WEBSITE];
            return PartialView();
        }

        public ActionResult BoxService()
        {
            return PartialView();
        }

        public ActionResult BoxBanner()
        {
            return PartialView();
        }

        public ActionResult BoxSearch()
        {
            return PartialView();
        }

        /// <summary>
        /// Box các sản phẩm liên quan
        /// </summary>
        /// <returns></returns>
        public ActionResult BoxRelated()
        {
            var lstTemp = productService.List();
            ViewBag.ListProduct = lstTemp;
            return PartialView();
        }

        public ActionResult BoxRating()
        {
            return PartialView();
        }

        public ActionResult BoxSlideShow()
        {
            return PartialView();
        }

        public ActionResult BoxBreadCrumb()
        {
            return PartialView();
        }

        public ActionResult BoxProduct(ProductConditions condition)
        {
            if (condition.displayType == "Grid")
            {
                return BoxProductGrid(condition);
            }
            else
            {
                return BoxProductList(condition);
            }
        }

        /// <summary>
        /// Hiển thị danh sách sản phẩm dưới dạng List
        /// </summary>
        /// <returns></returns>
        public ActionResult BoxProductList(ProductConditions condition)
        {
            // Get List City
            condition.ListCity = placeService.List(0);
            
            // Get List District
            if (condition.CityId > 0)
            {
                condition.ListDistrict = placeService.List(condition.CityId);
            }
            else
            {
                condition.DistrictId = 0;
                //condition.ListDistrict = placeService.List(1);
            }
            

            // Get List Ward
            if (condition.DistrictId > 0)
            {
                condition.ListWard = placeService.List(condition.DistrictId);
            }
            else
            {
                condition.WardId = 0;
                //condition.ListWard = placeService.List(condition.ListDistrict[0].Id);
            }
            condition.ListCity.Insert(0, new Entities.Place() { Text = "Chọn tỉnh/thành phố", Id = 0 });
            condition.ListDistrict.Insert(0, new Entities.Place() { Text = "Chọn quận/huyện", Id = 0 });
            condition.ListWard.Insert(0, new Entities.Place() { Text = "Chọn xã/phường", Id = 0 });


            Entities.ProductCondtions e = new Entities.ProductCondtions();
            condition.MapFrom(condition, ref e);


            var lstTemp = productService.List(e);
            int totalRecord = 0;
            if (lstTemp.Count > 0)
            {
                totalRecord = lstTemp[0].Total.Value;
                Paging(condition.Page, totalRecord, condition.Limit);
            }

            ViewBag.ListProduct = lstTemp;
            return PartialView("BoxProductList", condition);
        }

        /// <summary>
        /// Hiển thị danh sách sản phẩm dưới dạng Grid
        /// </summary>
        /// <returns></returns>
        public ActionResult BoxProductGrid(ProductConditions condition)
        {            
            // Get List City
            condition.ListCity = placeService.List(0);
            
            // Get List District
            if (condition.CityId > 0)
            {
                condition.ListDistrict = placeService.List(condition.CityId);
            }
            else
            {
                condition.DistrictId = 0;
                //condition.ListDistrict = placeService.List(1);
            }
            

            // Get List Ward
            if (condition.DistrictId > 0)
            {
                condition.ListWard = placeService.List(condition.DistrictId);
            }
            else
            {
                condition.WardId = 0;
                //condition.ListWard = placeService.List(condition.ListDistrict[0].Id);
            }
            condition.ListCity.Insert(0, new Entities.Place() { Text = "Chọn tỉnh/thành phố", Id = 0 });
            condition.ListDistrict.Insert(0, new Entities.Place() { Text = "Chọn quận/huyện", Id = 0 });
            condition.ListWard.Insert(0, new Entities.Place() { Text = "Chọn xã/phường", Id = 0 });


            Entities.ProductCondtions e = new Entities.ProductCondtions();
            condition.MapFrom(condition, ref e);


            var lstTemp = productService.List(e);
            int totalRecord = 0;
            if (lstTemp.Count > 0)
            {
                totalRecord = lstTemp[0].Total.Value;
                Paging(condition.Page, totalRecord, condition.Limit);
            }

            ViewBag.ListProduct = lstTemp;
            return PartialView("BoxProductGrid",condition);
        }

        public ActionResult BoxProductFeature()
        {
            var lstTemp = productService.List();
            ViewBag.ListProduct = lstTemp;
            return PartialView();
        }

        public ActionResult BoxProductSameKind(int productType = 0)
        {
            var lstTemp = productService.List(productType:productType);
            ViewBag.ListProduct = lstTemp;
            return PartialView();
        }

        public ActionResult BoxUpSellProduct()
        {
            var lstTemp = productService.List();
            ViewBag.ListProduct = lstTemp;
            return PartialView();
        }
    }
}
