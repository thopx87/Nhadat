using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application2016.Helpers
{
    public static class DateTimeHelper
    {
        public static string ToDateTime(this DateTime d)
        {
            return d.ToString("dd/MM/yyyy HH:mm:ss tt");
        }
    }
}