using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace System
{
    /// <summary>
    /// 扩展类
    /// </summary>
    public static partial class ClassEX
    {

        #region 对象型

        /// <summary>
        /// 强转非空字符串
        /// </summary>
        /// <param name="obj">字符串</param>
        /// <returns>非空字符串</returns>
        public static string ToStringN(this object obj)
        {
            if (obj == null)
                return "";
            else
                return obj.ToString();
        }

        /// <summary>
        /// 对象是否为空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(this object obj)
        { return obj == null; }

        #endregion

        #region 字符串

        /// <summary>
        /// 是否空字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>是否空字符串</returns>
        public static bool IsNullOrEmpty(this string str)
       {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 是否有空格
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>是否有空格</returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 正则表达式
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="pattern">表达式</param>
        /// <returns>是否符合</returns>
        public static bool IsMatch(this string str, string pattern)
        {
            if (str.IsNullOrEmpty())
                throw new ArgumentNullException("str");
            if (pattern.IsNullOrEmpty())
                throw new ArgumentNullException("pattern");
            return Regex.IsMatch(str, pattern);
        }

        /// <summary>
        /// 字符串转INT
        /// </summary>
        /// <param name="obj">字符串</param>
        /// <returns></returns>
        public static int ToInteger(this string obj)
        {
            if (obj == null)
                return 0;
            int res = 0;
            int.TryParse(obj, out res);
            return res;
        }

        /// <summary>
        /// byte转十六进制字符串
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="SpaceSplit">是否空格分割</param>
        /// <returns>十六进制字符串</returns>
        public static string ToHex(this byte[] data, bool SpaceSplit=true)
        {
            if (data == null)
                return "";
            else if (data.Length == 0)
                return "";
            string info = "";
            for (int i = 0; i < data.Length; i++)
            {
                info += " " + (data[i].ToString("X2").Length==1?"0"+data[i].ToString("X2"):data[i].ToString("X2"));
            }
            info = info.TrimStart();
            if (!SpaceSplit)
                info = info.Replace(" ", "");
            return info;
        }

        /// <summary>
        /// 十六进制字符串转byte数组
        /// </summary>
        /// <param name="data">十六进制字符串</param>
        /// <returns>byte数组</returns>
        public static byte[] FromHex(this string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;
            data = data.Replace(" ", "");
            if ((data.Length % 2) != 0)
                data += " ";
            byte[] returnBytes = new byte[data.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(data.Substring(i * 2, 2), 16);
            return returnBytes; 
        }


        /// <summary>
        /// byte数组转Base64
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>Base64</returns>
        public static string ToBase64(this byte[] data)
        {
            if (data == null)
                return "";
            else if (data.Length == 0)
                return "";
            return  Convert.ToBase64String(data);
        }

        /// <summary>
        ///  BASE64转byte数组
        /// </summary>
        /// <param name="data">BASE64</param>
        /// <returns>byte数组</returns>
        public static byte[] FromBase64(this string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;
            return Convert.FromBase64String(data);
        }

        /// <summary>
        /// 字符串转byte[]
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="Encode">字符串编码</param>
        /// <returns>byte[]</returns>
        public static byte[] ToBytes(this string data,System.Text .Encoding Encode)
        {
            if (string.IsNullOrEmpty(data))
                return null ;
            return Encode.GetBytes(data);
        }

        /// <summary>
        /// byte数组转字符串
        /// </summary>
        /// <param name="data">byte数组</param>
        /// <param name="Encode">字符串编码</param>
        /// <returns>字符串</returns>
        public static string FormBytes(this  byte[]  data, System.Text.Encoding Encode)
        {
            if (data==null )
                return "";
            return Encode.GetString(data);
        }

        /// <summary>
        /// 字符串转时间
        /// </summary>
        /// <param name="data">时间字符串</param>
        /// <returns>时间</returns>
        public static DateTime ToDateTime(this string data)
        {
            DateTime DT = DateTime.Now;
            DateTime.TryParse(data, out DT);
            return DT;
        }

        /// <summary>
        /// 转换为时间字符串
        /// </summary>
        /// <param name="data">时间</param>
        /// <param name="Format">字符串格式</param>
        /// <returns>标准时间字符串</returns>
        public static string ToTimeString(this DateTime data, string Format = "")
        {
            if (string.IsNullOrEmpty(Format))
                return data.ToString();
            else
                return data.ToString(Format);
        }

        #endregion 

        #region 数值型

        /// <summary>
        /// int 转数组
        /// </summary>
        /// <param name="data">int</param>
        /// <returns> 数组</returns>
        public static byte[] intToBytes(this int data)
        {
            return System.BitConverter.GetBytes(data);
        }

        /// <summary>
        /// 数组转INT
        /// </summary>
        /// <param name="data">数组</param>
        /// <returns>INT</returns>
        public static int intFormByte(this byte[] data)
        { return System.BitConverter.ToInt32(data, 0); }

        #endregion

        #region DATA型

        /// <summary>
        /// DataTable转JSON
        /// </summary>
        /// <param name="DT">DataTable</param>
        /// <returns>JSON</returns>
        public static string ToJSON(this Data.DataTable DT)
        {
            return System.ClassTransform.DATATABLE.ToJSON(DT);
        }

        /// <summary>
        /// DataTable转XML
        /// </summary>
        /// <param name="DT">DataTable</param>
        /// <returns>XML</returns>
        public static string ToXML(this Data.DataTable DT)
        {
            return System.ClassTransform.DATATABLE.ToXML(DT);
        }

        #endregion

    }
}
