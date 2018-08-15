using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml; 
namespace System.ClassTransform
{
    /// <summary>
    /// XML转换类
    /// </summary>
    public class XML
    {

        /// <summary>
        /// 实体转XML
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="t">实体</param>
        /// <returns>xml</returns>
        public static string ToXML<T>(T t)
        {
            Type  type =t.GetType();
            StreamReader sr = null;
            try
            {
                MemoryStream Stream = new MemoryStream();
                //创建序列化对象  
                XmlSerializer xml = new XmlSerializer(type);
                //序列化对象  
                xml.Serialize(Stream, t);
                Stream.Position = 0;
                sr = new StreamReader(Stream);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally {
                if (sr != null)
                    sr.Close();
            }

        }

        /// <summary>
        /// XML转实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="xml">xml</param>
        /// <returns>实体</returns>
        public static T FromXML<T>(string xml) where T : class, new()
        {
            var t = new T();
            Type type = t.GetType();
            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer xmldes = new XmlSerializer(type);
                return xmldes.Deserialize(sr) as T;
            }  
        }

        /// <summary>
        /// XML转DATASET
        /// </summary>
        /// <param name="xml">xml</param>
        /// <returns>DATASET</returns>
        public static Data.DataSet  FromXML(string xml)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                Data.DataSet xmlDS = new Data.DataSet();
                stream = new StringReader(xml);
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);
                return xmlDS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }


    }
}
