using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Application2016.Helpers
{
    public static class Logs
    {
        public static void LogWrite(string logMessage)
        {
            try
            {
                using (StreamWriter w = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "\\" + "Front_End_Log.txt"))
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
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),DateTime.Now.ToLongDateString());
                txtWriter.WriteLine(logMessage);
                txtWriter.WriteLine("---------------------------------------------");
            }
            catch
            {
            }
        }

        public static void LogWrite(string logMessage, string fileName)
        {
            try
            {
                using (StreamWriter w = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName))
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