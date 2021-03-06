using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BusinessLayer.Helpers
{
    public static class StringHelper
    {
        public static bool CheckInputStandar(string str)
        {
            if (str == "" || str == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Chuyển từ chuỗi sang list Id
        /// </summary>
        /// <param name="str"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public static List<int> ConvertStringToListInt(string str, char spec = ',')
        {
            var strSplit = str.Split(spec).ToList();
            return strSplit.Select(int.Parse).ToList();
        }

        /// <summary>
        /// Chuyển từ list id (int) sang chuỗi.
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public static string ConvertListIntToString(List<int> lst, char spec = ',')
        {
            string str = "";
            foreach (int id in lst)
            {
                str += id.ToString() + ',';
            }
            str = str.Remove(str.Length - 1);

            return str;
        }

        public static string NonUnicode(this string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",  
                                            "đ",  
                                            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",  
                                            "í","ì","ỉ","ĩ","ị",  
                                            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",  
                                            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",  
                                            "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",  
                                            "d",  
                                            "e","e","e","e","e","e","e","e","e","e","e",  
                                            "i","i","i","i","i",  
                                            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",  
                                            "u","u","u","u","u","u","u","u","u","u","u",  
                                            "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToLower(), arr2[i].ToLower());
            }
            return text;
        }
        
        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",  
                                            "đ",  
                                            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",  
                                            "í","ì","ỉ","ĩ","ị",  
                                            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",  
                                            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",  
                                            "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",  
                                            "d",  
                                            "e","e","e","e","e","e","e","e","e","e","e",  
                                            "i","i","i","i","i",  
                                            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",  
                                            "u","u","u","u","u","u","u","u","u","u","u",  
                                            "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }

        //public static string ToAlias(this string text)
        //{
        //    string temp = text.NonUnicode();
        //    temp = temp.Replace(" ", "-");
        //    temp = temp.Replace(",", "");
        //    temp = temp.Replace(".", "");
        //    return temp;
        //}

        public static string ConvertListToString(List<string> list, char seperator =';')
        {
            string result = "";
            foreach (string str in list)
            {
                result += str + seperator;
            }
            result = result.Remove(result.Length - 1);

            return result;
        }
    }
}
