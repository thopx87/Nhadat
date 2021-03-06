using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application2016.Helpers
{
    public static class StringHelperExtension
    {
        public static List<int> ConvertStringToListInt(string str, char spec=',')
        {
            var strSplit = str.Split(spec).ToList();
            return strSplit.Select(int.Parse).ToList();
        }

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

        public static string ConvertMoneyToString(this decimal number)
        {
            string s = number.ToString("#");
            string[] so = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] hang = new string[] { "", "nghìn", "triệu", "tỷ" };
            int i, j, donvi, chuc, tram;
            string str = " ";
            bool booAm = false;
            decimal decS = 0;
            //Tung addnew
            try
            {
                decS = Convert.ToDecimal(s.ToString());
            }
            catch
            {
            }
            if (decS < 0)
            {
                decS = -decS;
                s = decS.ToString();
                booAm = true;
            }
            i = s.Length;
            if (i == 0)
                str = so[0] + str;
            else
            {
                j = 0;
                while (i > 0)
                {
                    donvi = Convert.ToInt32(s.Substring(i - 1, 1));
                    i--;
                    if (i > 0)
                        chuc = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        chuc = -1;
                    i--;
                    if (i > 0)
                        tram = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        tram = -1;
                    i--;
                    if ((donvi > 0) || (chuc > 0) || (tram > 0) || (j == 3))
                        str = hang[j] + str;
                    j++;
                    if (j > 3) j = 1;
                    if ((donvi == 1) && (chuc > 1))
                        str = "một " + str;
                    else
                    {
                        if ((donvi == 5) && (chuc > 0))
                            str = "lăm " + str;
                        else if (donvi > 0)
                            str = so[donvi] + " " + str;
                    }
                    if (chuc < 0)
                        break;
                    else
                    {
                        if ((chuc == 0) && (donvi > 0)) str = "lẻ " + str;
                        if (chuc == 1) str = "mười " + str;
                        if (chuc > 1) str = so[chuc] + " mươi " + str;
                    }
                    if (tram < 0) break;
                    else
                    {
                        if ((tram > 0) || (chuc > 0) || (donvi > 0)) str = so[tram] + " trăm " + str;
                    }
                    str = " " + str;
                }
            }
            if (booAm) str = "Âm " + str;
            return str + "đồng";
        }

        public static string ConvertMoneyToStringShort(decimal number)
        {
            string s = number.ToString("#");
            string[] so = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] hang = new string[] { "", "nghìn", "triệu", "tỷ" };
            int i, j, donvi, chuc, tram;
            string str = " ";
            bool booAm = false;
            decimal decS = 0;
            //Tung addnew
            try
            {
                decS = Convert.ToDecimal(s.ToString());
            }
            catch
            {
            }
            if (decS < 0)
            {
                decS = -decS;
                s = decS.ToString();
                booAm = true;
            }
            i = s.Length;
            if (i == 0)
                str = so[0] + str;
            else
            {
                j = 0;
                while (i > 0)
                {
                    donvi = Convert.ToInt32(s.Substring(i - 1, 1));
                    i--;
                    if (i > 0)
                        chuc = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        chuc = -1;
                    i--;
                    if (i > 0)
                        tram = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        tram = -1;
                    i--;
                    if ((donvi > 0) || (chuc > 0) || (tram > 0) || (j == 3))
                        str = hang[j] + str;
                    j++;
                    if (j > 3) j = 1;
                    if ((donvi == 1) && (chuc > 1))
                        str = "một " + str;
                    else
                    {
                        if ((donvi == 5) && (chuc > 0))
                            str = "lăm " + str;
                        else if (donvi > 0)
                            str = so[donvi] + " " + str;
                    }
                    if (chuc < 0)
                        break;
                    else
                    {
                        if ((chuc == 0) && (donvi > 0)) str = "lẻ " + str;
                        if (chuc == 1) str = "mười " + str;
                        if (chuc > 1) str = so[chuc] + " mươi " + str;
                    }
                    if (tram < 0) break;
                    else
                    {
                        if ((tram > 0) || (chuc > 0) || (donvi > 0)) str = so[tram] + " trăm " + str;
                    }
                    str = " " + str;
                }
            }
            if (booAm) str = "Âm " + str;
            return str;
        }

        public static string MoneyFormat(this decimal money, int type = 2)
        {
            decimal newFormat = 0;
            string unit = "";
            switch (type)
            {
                case 1: // Nghìn đồng
                    newFormat = money / 1000;
                    unit = " (nghìn đồng)";
                    break;
                case 2: // Triệu đồng
                    newFormat = money / 1000000;
                    if (newFormat < 1)
                    {
                        return MoneyFormat(money, 1);
                    }
                    if (newFormat >= 1000)
                    {
                        return MoneyFormat(money, 3);
                    }
                    unit = " (triệu đồng)";
                    break;
                case 3: // Tỷ đồng
                    newFormat = money / 1000000000;
                    if (newFormat < 1)
                    {
                        return MoneyFormat(money, 2);
                    }
                    unit = " (tỷ đồng)";
                    break;
                default:
                    newFormat = money;
                    unit = " (đồng)";
                    break;
            }
            //return Math.Floor(newFormat).ToString("#,000") + unit;
            return Math.Round(newFormat, 2).ToString() + unit;
        }

        public static string MoneyFormat(this long money, int type = 2)
        {
            decimal newFormat = 0;
            string unit = "";
            switch (type)
            {
                case 1: // Nghìn đồng
                    newFormat = money / 1000;
                    unit = " (nghìn đồng)";
                    break;
                case 2: // Triệu đồng
                    newFormat = (decimal) money / 1000000;
                    if (newFormat < 1)
                    {
                        return MoneyFormat(money, 1);
                    }
                    if (newFormat >= 1000)
                    {
                        return MoneyFormat(money, 3);
                    }
                    unit = " (triệu đồng)";
                    break;
                case 3: // Tỷ đồng
                    newFormat = (decimal) money / 1000000000;
                    if (newFormat < 1)
                    {
                        return MoneyFormat(money, 2);
                    }
                    unit = " (tỷ đồng)";
                    break;
                default:
                    newFormat = money;
                    unit = " (đồng)";
                    break;
            }
            //return Math.Floor(newFormat).ToString("#,000") + unit;
            return Math.Round(newFormat, 2).ToString() + unit;
        }

        public static string MoneyFormatShort(this decimal money, int type = 2)
        {
            decimal newFormat = 0;
            string unit = "";
            switch (type)
            {
                case 1: // Nghìn đồng
                    newFormat = money / 1000;
                    unit = " (nghìn)";
                    break;
                case 2: // Triệu đồng
                    newFormat = money / 1000000;
                    if (newFormat < 1)
                    {
                        return MoneyFormatShort(money, 1);
                    }
                    if (newFormat > 1000)
                    {
                        return MoneyFormatShort(money, 3);
                    }
                    unit = " (triệu)";
                    break;
                case 3: // Tỷ đồng
                    newFormat = money / 1000000000;
                    if (newFormat < 1)
                    {
                        return MoneyFormatShort(money, 2);
                    }
                    unit = " (tỷ)";
                    break;
                default:
                    newFormat = money;
                    break;
            }
            //return Math.Floor(newFormat).ToString("#,000") + unit;
            return Math.Round(newFormat, 2).ToString() + unit;
        }

        public static string MoneyFormatShort(this long money, int type = 2)
        {
            decimal newFormat = 0;
            string unit = "";
            switch (type)
            {
                case 1: // Nghìn đồng
                    newFormat = (decimal)money / 1000;
                    unit = " (nghìn)";
                    break;
                case 2: // Triệu đồng
                    newFormat = (decimal)money / 1000000;
                    if (newFormat < 1)
                    {
                        return MoneyFormatShort(money, 1);
                    }
                    if (newFormat > 1000)
                    {
                        return MoneyFormatShort(money, 3);
                    }
                    unit = " (triệu)";
                    break;
                case 3: // Tỷ đồng
                    newFormat = (decimal)money / 1000000000;
                    if (newFormat < 1)
                    {
                        return MoneyFormatShort(money, 2);
                    }
                    unit = " (tỷ)";
                    break;
                default:
                    newFormat = (decimal)money;
                    break;
            }
            //return Math.Floor(newFormat).ToString("#,000") + unit;
            return Math.Round(newFormat, 3).ToString() + unit;
        }
        
        public static long MoneyExchange(this long money, char Operator = '*')
        {
            long newMoney = money;
            if (Operator == '*')
            {
                newMoney = money * 1000000; // Nhân với 1 triệu.
            }
            else if (Operator == '/')
            {
                newMoney = money / 1000000; // Chia cho 1 triệu.
            }
            return newMoney;
        }

        public static string StringFormat(string input, int type = 1)
        {
            string output = input;
            switch (type)
            {
                case 1:

                    break;
                case 2:

                    break;
                default:
                    break;
            }
            return output;
        }
        
        public static string AreaFormat(this decimal area, int type = 1)
        {
            string output = "";
            switch (type)
            {
                case 1:
                    output = Math.Round(area, 1) + " m\xB2";
                    break;
                default:
                    break;
            }
            return output;
        }

        public static string SubString(this string str, int length = 20)
        {
            if (str.Length > length + 3)
            {
                return str.Substring(0, length) + "...";
            }
            else
            {
                return str;
            }
        }

        public static string ToMoneyText(this decimal money)
        {
            string s = money.ToString("#");
            string[] so = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] hang = new string[] { "", "nghìn", "triệu", "tỷ" };
            int i, j, donvi, chuc, tram;
            string str = " ";
            bool booAm = false;
            decimal decS = 0;
            //Tung addnew
            try
            {
                decS = Convert.ToDecimal(s.ToString());
            }
            catch
            {
            }
            if (decS < 0)
            {
                decS = -decS;
                s = decS.ToString();
                booAm = true;
            }
            i = s.Length;
            if (i == 0)
                str = so[0] + str;
            else
            {
                j = 0;
                while (i > 0)
                {
                    donvi = Convert.ToInt32(s.Substring(i - 1, 1));
                    i--;
                    if (i > 0)
                        chuc = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        chuc = -1;
                    i--;
                    if (i > 0)
                        tram = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        tram = -1;
                    i--;
                    if ((donvi > 0) || (chuc > 0) || (tram > 0) || (j == 3))
                        str = hang[j] + str;
                    j++;
                    if (j > 3) j = 1;
                    if ((donvi == 1) && (chuc > 1))
                        str = "một " + str;
                    else
                    {
                        if ((donvi == 5) && (chuc > 0))
                            str = "lăm " + str;
                        else if (donvi > 0)
                            str = so[donvi] + " " + str;
                    }
                    if (chuc < 0)
                        break;
                    else
                    {
                        if ((chuc == 0) && (donvi > 0)) str = "lẻ " + str;
                        if (chuc == 1) str = "mười " + str;
                        if (chuc > 1) str = so[chuc] + " mươi " + str;
                    }
                    if (tram < 0) break;
                    else
                    {
                        if ((tram > 0) || (chuc > 0) || (donvi > 0)) str = so[tram] + " trăm " + str;
                    }
                    str = " " + str;
                }
            }
            if (booAm) str = "Âm " + str;
            return str + "đồng";
        }

        public static bool TestString(this string t)
        {
            return true;
        }

        public static string ToFriendlyUrl(this string urlType, int id, string text)
        {
            string result = "";
            switch (urlType)
            {
                case AdminConfigs.FRIENDLY_LINK_PRODUCT_DETAIL:
                    result = AdminConfigs.FRIENDLY_LINK_PRODUCT_DETAIL + "/" + id + "/" + text;
                    break;
                case AdminConfigs.FRIENDLY_LINK_ARTICLE:
                    result = AdminConfigs.FRIENDLY_LINK_ARTICLE + "/" + id + "/" + text;
                    break;
                default:
                    break;
            }
            return result;
        }

        public static string ToAlias(this string text)
        {
            string temp = text.NonUnicode();
            temp = temp.Replace(" ", "-");
            temp = temp.Replace(",", "");
            temp = temp.Replace(".", "");

            return temp;
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
    }
}