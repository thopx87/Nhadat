using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Mail;

namespace BusinessLayer.Helpers
{
    public static class SendEmail
    {
        static Thread SendMail_Thread;
        static EmailQueueService emailQueueService = new EmailQueueService();

        public static void StartThreadSendMail()
        {
            SendMail_Thread = new Thread(new ThreadStart(SendMailbyThread));
            SendMail_Thread.Start();
        }

        public static List<Entities.EmailQueue> EmailQueueList = new List<Entities.EmailQueue>();

        public static bool SendMail(Entities.EmailQueue email, bool InsertToQueue = true)
        {
            if (InsertToQueue)
            {
                EmailQueueList.Add(email);
                return true;
            }
            else
            {
                string displayname = ConfigSettings.ReadSetting("WebsiteName");
                if (displayname == null) { displayname = "land4p.com"; }

                string email_account = ConfigSettings.ReadSetting("Email");

                string email_admin = ConfigSettings.ReadSetting("EmailRecive");
                if (email.SendTo.Length ==0)
                    email.SendTo = email_admin;
                string email_Bcc = ConfigSettings.ReadSetting("EmailBcc");
                if (email.Bcc.Length == 0)
                {
                    email.Bcc = email_Bcc;
                }

                string password_account = ConfigSettings.ReadSetting("Password");

                string host = ConfigSettings.ReadSetting("Host");

                int port = int.Parse(ConfigSettings.ReadSetting("Port"));

                bool enablessl = Convert.ToBoolean(ConfigSettings.ReadSetting("EnableSSL"));

                bool useDefaultCredentials = Convert.ToBoolean(ConfigSettings.ReadSetting("UseDefaultCredentials"));

                int timeOut = int.Parse(ConfigSettings.ReadSetting("TimeOut"));

                SmtpClient SmtpServer = new SmtpClient();
                //SmtpServer.Credentials = new System.Net.NetworkCredential(email_account, password_account);
                //SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                //SmtpServer.Port = port;
                //SmtpServer.Host = host;
                //SmtpServer.EnableSsl = enablessl;
                //SmtpServer.UseDefaultCredentials = useDefaultCredentials;
                //SmtpServer.Timeout = timeOut;

                // Test
                SmtpServer.Host = host;
                SmtpServer.Port = port;
                SmtpServer.EnableSsl = true;
                SmtpServer.Credentials = new System.Net.NetworkCredential(email_account, password_account); 

                MailMessage mail = new MailMessage();

                try
                {
                    mail.From = new MailAddress(email_account, displayname, System.Text.Encoding.UTF8);
                    mail.To.Add(email.SendTo);
                    mail.Bcc.Add(email.Bcc);
                    mail.Subject = email.Subject;
                    mail.Body = email.Body;
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    mail.Priority = MailPriority.High;
                    mail.IsBodyHtml = true;
                    
                    SmtpServer.Send(mail);

                    emailQueueService.Insert(email);
                    return true;
                }
                catch (Exception ex)
                {
                    string data =ex.Message.ToString();
                    Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data),"Email_log.txt");
                    return false;
                }
            }
        }

        static void SendMailbyThread()
        {
            while (SendMail_Thread.IsAlive)
            {
                while (EmailQueueList.Count > 0)
                {
                    try
                    {
                        var item = EmailQueueList.Where(m => !m.isFinish).FirstOrDefault();
                        if (item == null)
                            break;

                        item.isFinish = true;
                        SendMail(item, false);
                        EmailQueueList.Remove(item);
                        Thread.Sleep(2500);
                    }
                    catch
                    {

                    }
                }
                Thread.Sleep(2500);
            }

        }

        /* while (SendMail_Thread.IsAlive)
         {

             for (int i = 0; i < EmailQueueList.Count; i++)
             {
                 try
                 {
                     if (!EmailQueueList[i].is_finished)
                     {
                         SendMail(EmailQueueList[i].Email, EmailQueueList[i].Subject, EmailQueueList[i].Body, false);
                         EmailQueueList[i].is_finished = true;
                         Thread.Sleep(3000);
                     }
                 }
                 catch (Exception ex)
                 {

                 }
             }
             Thread.Sleep(10000);
         }
        
     }*/
    }
}
