using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Application2016.Areas.Admin.Controllers;
using BusinessLayer;
using Application2016.Areas.Admin.Models;
using Application2016.Helpers;
using Microsoft.CSharp.RuntimeBinder;

namespace Application2016
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Khởi tạo luồng để xử lý gửi mail.
            BusinessLayer.Helpers.SendEmail.StartThreadSendMail();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            SetGlobalVariable();
        }

        protected void Application_EndRequest()
        {
            bool error = false;
            var rd = new RouteData();
            rd.DataTokens["area"] = "Admin"; // In case controller is in another area
            rd.Values["controller"] = "Errors";
            switch (Context.Response.StatusCode)
            {
                case 404:
                    rd.Values["action"] = "NotFound";
                    error = true;
                    break;
                case 500:
                    rd.Values["action"] = "ServerError";
                    error = true;
                    break;
            }
            if (error)
            {
                Response.Clear();
                IController c = new ErrorsController();
                c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
            }
        }

        private void SetGlobalVariable()
        {            
            SettingService _service = new SettingService();
            // Get all setting
            var allSetting = _service.List();

            // Get all website setting
            var lstWebsite = allSetting.Where(e => e.SettingName == AdminConfigs.SETTING_WEBSITE).ToList();

            List<SettingCommonModel> lstSettingModel = new List<SettingCommonModel>();
            foreach (Entities.Setting s in lstWebsite)
            {
                SettingCommonModel m = new SettingCommonModel();
                m.MapFrom(s, ref m);
                lstSettingModel.Add(m);
            }
            Application[AdminConfigs.SETTING_WEBSITE] = lstSettingModel;

            // Get all social setting
            var lstSocial = allSetting.Where(e => e.SettingName == AdminConfigs.SETTING_SOCIAL_NETWORK).ToList();
            lstSettingModel = new List<SettingCommonModel>();
            foreach (Entities.Setting s in lstSocial)
            {
                SettingCommonModel m = new SettingCommonModel();
                m.MapFrom(s, ref m);
                lstSettingModel.Add(m);
            }
            Application[AdminConfigs.SETTING_SOCIAL_NETWORK] = lstSettingModel;
        }
    }
}