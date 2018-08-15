using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
namespace System
{
    /// <summary>
    /// 枚举类
    /// </summary>
    public static class EnumEx
    {
        /// <summary>
        /// 列出枚举及值
        /// </summary>
        /// <param name="em">枚举</param>
        /// <returns>名称和值</returns>
        public static Dictionary<string, int> EnumToList( Enum em)
        {
            Dictionary<string, int> listItems = new Dictionary<string, int>();
            Array array = Enum.GetValues(em.GetType());
            foreach (int val in array)
            {
                string EnumName = Enum.GetName(em.GetType(), val);
                FieldInfo EnumInfo = em.GetType().GetField(EnumName);
                if (EnumInfo != null)
                {
                    DescriptionAttribute[] EnumAttributes = (DescriptionAttribute[])EnumInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (EnumAttributes.Length > 0)
                        listItems.Add(EnumAttributes[0].Description, val);
                }
            }
            return listItems;
           
        }

        /// <summary>
        ///  列出枚举描述及值
        /// </summary>
        /// <param name="em">枚举</param>
        /// <returns>枚举描述及值</returns>
        public static Dictionary<string, int> EnumDescToList( Enum  em)
        {
 
            Dictionary<string, int> listItems = new Dictionary<string, int>();
            Array array = Enum.GetValues(em.GetType());
            foreach (int val in array)
            {
                string EnumName = Enum.GetName(em.GetType(), val);
                FieldInfo EnumInfo = em.GetType().GetField(EnumName);
                if (EnumInfo != null)
                {
                    DescriptionAttribute[] EnumAttributes = (DescriptionAttribute[])EnumInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (EnumAttributes.Length > 0)
                        listItems.Add(EnumAttributes[0].Description, val);
                }
            }
            return listItems;

        }




    }
}
