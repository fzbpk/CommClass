using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Reflection;
using System.Data.Linq.Mapping;
using System.Xml.Serialization;
using System.Xml; 
namespace System.ClassTransform
{
    /// <summary>
    /// DataTable  转换类
    /// </summary>
    public class DATATABLE 
    {

        /// <summary>
        /// DATATABLE转实体
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="DT">DATATABLE</param>
        /// <param name="NeedAttribute">是否需要通过数据属性标识转换，默认是</param>
        /// <returns>实体列表</returns>
        public static IList<T> ToEntity<T>(DataTable DT,bool NeedAttribute=true) where T : class, new()
        {
           T org = new T();
           IList<T> entities = new List<T>();
           if (DT != null)
           {
               foreach (DataRow row in DT.Rows)
               {
                   T entity = new T();
                   foreach (var item in entity.GetType().GetProperties())
                   {
                       Type PT = item.PropertyType;
                       DataAttribute[] DataAttributes = (DataAttribute[])item.GetCustomAttributes(typeof(DataAttribute), false);
                       string ColumnName = (DataAttributes == null ? "" : (DataAttributes.Length > 0 ? DataAttributes.ToList().First().Name.Trim() : ""));
                       if(!NeedAttribute && string.IsNullOrEmpty (ColumnName))
                           ColumnName=PT.Name;
                       if (ColumnName.Trim() != "")
                       {
                           try
                           {
                             if (PT.IsEnum)
                           {
                               if (row[ColumnName] is DBNull)
                                   continue;
                               else
                               {
                                   int val = int.Parse(row[ColumnName].ToString());
                                   var em = Enum.ToObject(PT, val);
                                   item.SetValue(org, em, null);
                               }

                           }
                           else if (row[ColumnName] is DBNull)
                               item.SetValue(entity, null, null);
                           else
                               item.SetValue(entity, Convert.ChangeType(row[ColumnName], item.PropertyType), null);
                           }
                           catch
                           {}
                         
                       }

                   }
                   entities.Add(entity);
               }
           }
           return entities;
       }

        /// <summary>
        /// 实体转DT
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="LS">实体列表</param>
        /// <returns>DATATABLE</returns>
        public static DataTable FromEntity<T>(IList<T> LS) where T : class, new()
        {
            if (LS == null)
                return null;
            T entityType = new T();
            PropertyInfo[] entityProperties = entityType.GetType().GetProperties();
            DataTable dt = new DataTable();
            for (int i = 0; i < entityProperties.Length; i++)
                dt.Columns.Add(entityProperties[i].Name);
            foreach (object entity in LS)
            {
                object[] entityValues = new object[entityProperties.Length];
                for (int i = 0; i < entityProperties.Length; i++)
                   entityValues[i] = entityProperties[i].GetValue(entity, null);
                dt.Rows.Add(entityValues);
            }
            return dt;
        }

        /// <summary>
        /// DataTable转XML
        /// </summary>
        /// <param name="DT">DATATABLE</param>
        /// <returns>XML</returns>
        public static string ToXML(DataTable DT)
        {
            MemoryStream stream = null;
            XmlTextWriter writer = null;
            try
            {
                stream = new MemoryStream();
                //从stream装载到XmlTextReader
                writer = new XmlTextWriter(stream, Encoding.Unicode);
                //用WriteXml方法写入文件.
                DT.WriteXml(writer);
                int count = (int)stream.Length;
                byte[] arr = new byte[count];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(arr, 0, count);
                UnicodeEncoding utf = new UnicodeEncoding();
                return utf.GetString(arr).Trim();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// DataTable转JSON
        /// </summary>
        /// <param name="DT">DataTable</param>
        /// <returns>JSON</returns>
        public static string ToJSON(DataTable DT)
        { 
           StringBuilder jsonBuilder = new StringBuilder();
           jsonBuilder.Append("{\"");
            jsonBuilder.Append(DT.TableName.ToString());
            jsonBuilder.Append("\":[");
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < DT.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(DT.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(DT.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

    }
}
