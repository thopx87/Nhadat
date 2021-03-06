using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace BusinessLayer.Helpers
{
    public static class Configs
    {
        // URL
        public static string URL_LOG_FILE = AppDomain.CurrentDomain.BaseDirectory;


        // Error
        public static string ERROR_TABLE_ERROR = "Bị lỗi ở bảng {0}";
        public static string ERROR_ENTITY_EXISTS = "{0} đã tồn tại.";
        public static string ERROR_ENTITY_NOT_EXISTS = "{0} chưa tồn tại.";
        public static string ERROR_DATA_WRONG = "Dữ liệu: {0} không hợp lệ.";
        public static string ERROR_ACTION = "Lỗi xử lý: {0}";
        public static string ERROR_SEND_MAIL = "Lỗi gửi email: {0}";
    }
}
