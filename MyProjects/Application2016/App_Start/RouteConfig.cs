using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Application2016.Helpers;

namespace Application2016
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Product_Detail",
                url: AdminConfigs.FRIENDLY_LINK_PRODUCT_DETAIL + "/{id}/{metatitle}",
                defaults: new { controller = "Template1", action = "Detail", id = UrlParameter.Optional},
                namespaces: new string[] { "Application2016.Controllers" }
                );

            routes.MapRoute(
                name: "Article",
                url: AdminConfigs.FRIENDLY_LINK_ARTICLE + "/{id}/{metatitle}",
                defaults: new { controller = "Template1", action = "Article", id = UrlParameter.Optional },
                namespaces: new string[] { "Application2016.Controllers" }
                );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Template1", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Application2016.Controllers" }
                );
        }
    }
}