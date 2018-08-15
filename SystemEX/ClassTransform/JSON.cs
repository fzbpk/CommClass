using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;  
namespace System.ClassTransform
{
    /// <summary>
    /// JSON转换类
    /// </summary>
    public   class JSON
    {

        /// <summary>    
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串    
        /// </summary>    
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }
        /// <summary>    
        /// 将时间字符串转为Json时间    
        /// </summary>    
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }

        /// <summary>
        /// 类转JSON串
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="t">类型</param>
        /// <returns>JSON串</returns>
        public static string ToJson<T>(T t)
        {
            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ds.WriteObject(ms, t);

            string strReturn = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();

            string p = @"\\/Date\((\d+)([-+])(\d+)\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            strReturn = reg.Replace(strReturn, matchEvaluator);

            return strReturn;
        }

        /// <summary>
        /// 类转JSON串
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="t">类型</param>
        /// <returns>JSON串</returns>
        public static string ToJson<T>(List<T> t)
        {
            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(List<T>));
            MemoryStream ms = new MemoryStream();
            ds.WriteObject(ms, t);

            string strReturn = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();


            string p = @"\\/Date\((\d+)([-+])(\d+)\)\\/";  
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            strReturn = reg.Replace(strReturn, matchEvaluator);

            return strReturn;
        }

        /// <summary>
        /// JSON串转类
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="strJson">JSON串</param>
        /// <returns>类</returns>
        public static T FromJson<T>(string strJson) where T : class
        {
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            strJson = reg.Replace(strJson, matchEvaluator);  
            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJson));
            return ds.ReadObject(ms) as T;
        }

        /// <summary>
        /// JSON串转类
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="strJson">JSON串</param>
        /// <returns>类</returns>
        public static List<T> FromJsons<T>(string strJson) where T : class
        {
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            strJson = reg.Replace(strJson, matchEvaluator); 
            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(List<T>));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJson));
            return ds.ReadObject(ms) as List<T>;
        }

        /// <summary>
        /// JSON转DataSet
        /// </summary>
        /// <param name="Json">JSON串</param>
        /// <returns>DataSet</returns>
        public static Data.DataSet JsonToDataSet(string Json)
        {
            try
            {
                Data.DataSet ds = new Data.DataSet();
                JavaScriptSerializer JSS = new JavaScriptSerializer();
                object obj = JSS.DeserializeObject(Json);
                Dictionary<string, object> datajson = (Dictionary<string, object>)obj;
                foreach (var item in datajson)
                {
                    Data.DataTable dt = new Data.DataTable(item.Key);
                    object[] rows = (object[])item.Value;
                    foreach (var row in rows)
                    {
                        Dictionary<string, object> val = (Dictionary<string, object>)row;
                        Data.DataRow dr = dt.NewRow();
                        foreach (KeyValuePair<string, object> sss in val)
                        {
                            if (!dt.Columns.Contains(sss.Key))
                            {
                                dt.Columns.Add(sss.Key.ToString());
                                dr[sss.Key] = sss.Value;
                            }
                            else
                                dr[sss.Key] = sss.Value;
                        }
                        dt.Rows.Add(dr);
                    }
                    ds.Tables.Add(dt);
                }
                return ds;
            }
            catch
            {
                return null;
            }
        }


    }
}
