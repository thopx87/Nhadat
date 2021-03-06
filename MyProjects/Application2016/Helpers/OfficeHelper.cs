using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Data.OleDb;

namespace Application2016.Helpers
{
    public static class ExcelHelper
    {
        public static DataSet ReadExcelFile(string urlFile)
        {
            string extend = Path.GetExtension(urlFile);
            string strConn = "";
            if (extend == ".xls")
            {
                strConn = "Provider= Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + urlFile + "; " + "Extended Properties=Excel 8.0;";
            }
            else if (extend == ".xlsx")
            {

            }
            else
            {
                return null;
            }            
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel="select * from [Sheet1$]";
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds,"table1");
            return ds;
        }
    }
}