using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace Application2016.Helpers
{
    public class CookieHelper
    {
        private static HttpContext context = HttpContext.Current;
        public static void Set(string key, string val, int dayExpries)
        {
            HttpContext.Current.Response.Cookies[key].Value = val;
            HttpContext.Current.Response.Cookies[key].Expires = DateTime.Now.AddDays(dayExpries);
        }

        public static void Set(string key, string val)
        {
            Set(key, val, 365);
        }

        public static string Get(string key)
        {

            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                return HttpContext.Current.Request.Cookies[key].Value;
            }
            return null;
        }


        /// <summary>
        /// Xóa cookie
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
         //   HttpContext.Current.Response.Cookies[key].Expires = DateTime.Now.AddDays(-1);
            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                var c = new HttpCookie(key);
                c.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(c);
            }
        }

        public static void RemoveAll()
        {
            foreach (string key in context.Request.Cookies.AllKeys)
            {
                Remove(key);
            }
        }
    }
}