using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
namespace System 
{
    /// <summary>
    /// 枚举成员扩展类
    /// </summary>
    public static class EnumProperty
    {
        private static Dictionary<Enum, string> dictDiscs = new Dictionary<Enum, string>();
        private static string GetDiscription(Enum myEnum)
        {
            FieldInfo fieldInfo = myEnum.GetType().GetField(myEnum.ToString());
            object[] attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                DescriptionAttribute desc = attrs[0] as DescriptionAttribute;
                if (desc != null)
                    return desc.Description;
            }
            return myEnum.ToString();
        }

        /// <summary>
        /// 显示描述
        /// </summary>
        /// <param name="myEnum"></param>
        /// <returns></returns>
        public static string ToDiscription(this Enum myEnum)
        {
            string strDisc = string.Empty;
            if (dictDiscs.ContainsKey(myEnum))
                strDisc = dictDiscs[myEnum];
            else
            {
                strDisc = GetDiscription(myEnum);
                dictDiscs.Add(myEnum, strDisc);
            }
            return strDisc;
        }
 

      
    }
}
