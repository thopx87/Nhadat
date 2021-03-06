using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BusinessLayer.Enums;

namespace BusinessLayer.Helpers
{
    public static class MailHelper
    {
        private static string className { get { return "MailHelper"; } }
        private static string websiteName { get { return ConfigSettings.ReadSetting("WebsiteName"); } }
        

        private const string Sign = " <br /><br /> " +
            " Best and Regards <br />" +
            " Ban quản trị website"
            ;

        public const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
             + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				        [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
             + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				        [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
             + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";
        /// <summary>
        /// Checks whether the given Email-Parameter is a valid E-Mail address.
        /// </summary>
        /// <param name="email">Parameter-string that contains an E-Mail address.</param>
        /// <returns>True, when Parameter-string is not null and 
        /// contains a valid E-Mail address;
        /// otherwise false.</returns>
        public static bool IsEmail(string email)
        {
            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            else return false;
        }

        public static bool MailRegister(Entities.User u, string host, string code)
        {
            string urlLink = host + "/Admin/User/ConfirmRegister/" + u.Id + "?code=" + code;
            Entities.EmailQueue email = new Entities.EmailQueue();
            string body = "Xin chào " + u.UserName + "<br /><br /> Xin cám ơn đã đăng ký vào website " + websiteName;
            body += "<br />Hãy click vào link dưới để kích hoạt tài khoản của bạn. <br />";
            body += "<a href=\"" + urlLink + "\" target=\"_blank\">"+urlLink+"</a>";
            body += Sign;

            email.Subject = "[Xác nhận đăng ký] vào website " + websiteName;
            email.Body = body;
            email.SendTo = u.Email;
            email.isFinish = false;
            try
            {
                if (!SendEmail.SendMail(email))
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Mail xác nhận đăng ký làm môi giới
        /// </summary>
        /// <param name="u">Tài khoản đăng ký</param>
        /// <param name="host">hosting hệ thống</param>
        /// <param name="code">mã kích hoạt</param>
        /// <param name="isActive">trạng thái kích hoạt</param>
        /// <returns></returns>
        public static bool MailRegisterAgency(Entities.User u, string host = "", string code = "", bool isActive = false)
        {
            Entities.EmailQueue email = new Entities.EmailQueue();
            string body = "Xin chào " + u.UserName + "<br /><br /> Xin chúc mừng bạn đã đăng ký môi giới Online thành công";
            if (isActive)
            {
                body += "<br />";
                body += "Bây giờ bạn có thể dùng các chức năng của môi giới";
            }
            else
            {
                string urlLink = host + "/Admin/User/ConfirmRegister/" + u.Id + "?code=" + code;
                body += "<br />Hãy click vào link dưới để kích hoạt tài khoản của bạn. <br />";
                body += "<a href=\"" + urlLink + "\" target=\"_blank\">" + urlLink + "</a>";
            }
            body += Sign;

            email.Subject = "[Xác nhận đăng ký môi giới] vào website " + websiteName;
            email.Body = body;
            email.SendTo = u.Email;
            email.isFinish = false;
            try
            {
                if (!SendEmail.SendMail(email))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Mail lấy lại mật khẩu
        /// </summary>
        /// <param name="u">user</param>
        /// <param name="host">hosting</param>
        /// <param name="code">mã lấy mật khẩu</param>
        /// <returns></returns>
        public static bool MailPasswordRetrieval(Entities.User u, string host)
        {
            Entities.EmailQueue email = new Entities.EmailQueue();
            string urlLink = host + "/Admin/User/PasswordRetrieval/" + u.Id + "?code=" + u.CodeActive;
            string subject = "[Quên mật khẩu] với tài khoản " + u.Email;
            string body = "Chào bạn! <br /> <br /> Để lấy lại mật khẩu hãy làm theo các bước sau: <br />";
            body += "(1)   Hãy click vào link dưới để tạo mật khẩu mới. <br />";
            body += "<a href=\"" + urlLink + "\" target=\"_blank\">" + urlLink + "</a> <br />";
            body += "(2)   Tại ô yêu cầu nhập key, bạn hãy nhập key: <strong>" + u.Id + "</strong>";
            body += Sign;

            email.Subject = "[Quên mật khẩu] với tài khoản " + u.Email;
            email.Body = body;
            email.SendTo = u.Email;
            email.isFinish = false;
            try
            {
                if (!SendEmail.SendMail(email))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return false;
            }
            return true;
        }

        public static bool MailArticle(Entities.Article a, string listEmail)
        {
            Entities.EmailQueue email = new Entities.EmailQueue();
            string body = a.Body;
            body += Sign;

            email.Subject = a.Title;
            email.Body = body;
            email.SendTo = "";
            email.Bcc = listEmail;
            email.isFinish = false;
            try
            {
                if (!SendEmail.SendMail(email))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return false;
            }
            return true;
        }

        public static bool MailArticle(Entities.Article a, List<string> listEmail)
        {
            string strMail = "";
            foreach (string str in listEmail)
            {
                strMail += str + ",";
            }
            if (strMail.Length > 0)
            {
                strMail = strMail.Remove(strMail.Length - 1);
            }

            Entities.EmailQueue email = new Entities.EmailQueue();
            string body = a.Body;
            body += Sign;

            email.Subject = a.Title;
            email.Body = body;
            email.SendTo = "";
            email.Bcc = strMail;
            email.isFinish = false;
            try
            {
                if (!SendEmail.SendMail(email))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return false;
            }
            return true;
        }

        public static bool TestConnect()
        {
            Entities.EmailQueue email = new Entities.EmailQueue();
            email.Subject = "Test Connect Email";
            email.Body = "Đây là mail test kết nối!";
            email.SendTo = "thopx.87@gmail.com";
            email.isFinish = false;
            return SendEmail.SendMail(email);
        }
    }
}
