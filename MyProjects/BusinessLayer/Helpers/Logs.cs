using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace BusinessLayer.Helpers
{
    public static class Logs
    {
        public static void LogWrite(string logMessage)
        {
            try
            {
                using (StreamWriter w = File.AppendText(Configs.URL_LOG_FILE + "\\" + "Admin_error_log.txt"))
                {
                    LogWrite(logMessage, w);
                }
            }
            catch
            {
            }
        }

        public static void LogWrite(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nLog Entry : ");
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                txtWriter.WriteLine(logMessage);
                txtWriter.WriteLine("--------------------------------------------------");
            }
            catch
            {
            }
        }

        public static void LogWrite(string logMessage, string fileName)
        {
            try
            {
                using (StreamWriter w = File.AppendText(Configs.URL_LOG_FILE + "\\" + fileName))
                {
                    LogWrite(logMessage, w);
                }
            }
            catch
            {
            }
        }
    }
}
