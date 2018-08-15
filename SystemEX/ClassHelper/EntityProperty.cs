using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using LinqToDB.Mapping;
namespace System
{
    /// <summary>
    /// 实体属性扩展
    /// </summary>
    public static class EntityProperty
    {
        /// <summary>
        /// 获取属性类型的描述
        /// </summary>
        /// <param name="entity">类</param>
        /// <param name="filed">属性字段</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>返回对象描述，没有字段属性则显示属性名</returns>
        public static string ToDescripfiled(this object entity, string filed, bool IgnoreCase=true)
        {
            System.Type obj = entity.GetType();
            PropertyInfo[] propertys = obj.GetProperties();
            PropertyInfo property = propertys.FirstOrDefault(c => (IgnoreCase && c.Name.ToUpper() == filed.ToUpper()) || (!IgnoreCase && c.Name== filed));
            if (property == null)
                return "";
            DescriptionAttribute[] EnumAttributes = (DescriptionAttribute[])property.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (EnumAttributes.Length > 0)
                return EnumAttributes[0].Description;
            return property.Name;
        }


        /// <summary>
        /// 获取属性类型的显示名
        /// </summary>
        /// <param name="entity">类</param>
        /// <param name="filed">属性字段</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>返回属性类型的显示名，没有字段属性则显示属性名</returns>
        public static string ToDisplayfiled(this object entity, string filed, bool IgnoreCase = true)
        {
            System.Type obj = entity.GetType();
            PropertyInfo[] propertys = obj.GetProperties();
            PropertyInfo property = propertys.FirstOrDefault(c => (IgnoreCase && c.Name.ToUpper() == filed.ToUpper()) || (!IgnoreCase && c.Name == filed));
            if (property == null)
                return "";
            DisplayNameAttribute[] EnumAttributes = (DisplayNameAttribute[])property.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            if (EnumAttributes.Length > 0)
                return EnumAttributes[0].DisplayName;
            return property.Name;
        }

        /// <summary>
        /// 获取属性类型的映射表中的字段
        /// </summary>
        /// <param name="entity">类</param>
        /// <param name="filed">属性字段</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>映射表中的字段,空为找不到属性，没有字段属性则显示属性名</returns>
        public static string ToColumnName(this object entity, string filed, bool IgnoreCase = true)
        {
            System.Type obj = entity.GetType();
            PropertyInfo[] propertys = obj.GetProperties();
            PropertyInfo property = propertys.FirstOrDefault(c => (IgnoreCase && c.Name.ToUpper() == filed.ToUpper()) || (!IgnoreCase && c.Name == filed));
            if (property == null)
                return "";
            ColumnAttribute[] EnumAttributes = (ColumnAttribute[])property.GetCustomAttributes(typeof(ColumnAttribute), false);
            if (EnumAttributes.Length > 0)
                return EnumAttributes[0].Name;
            return property.Name;
        }

        /// <summary>
        /// 获取属性类型是否主键
        /// </summary>
        /// <param name="entity">类</param>
        /// <param name="filed">属性字段</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>是否主键</returns>
        public static bool IsPrimaryKey(this object entity, string filed, bool IgnoreCase = true)
        {
            System.Type obj = entity.GetType();
            PropertyInfo[] propertys = obj.GetProperties();
            PropertyInfo property = propertys.FirstOrDefault(c => (IgnoreCase && c.Name.ToUpper() == filed.ToUpper()) || (!IgnoreCase && c.Name == filed));
            if (property == null)
                return false ;
            ColumnAttribute[] EnumAttributes = (ColumnAttribute[])property.GetCustomAttributes(typeof(ColumnAttribute), false);
            if (EnumAttributes.Length > 0)
                return EnumAttributes[0].IsPrimaryKey;
            return false;
        }

        /// <summary>
        /// 获取属性类型是否可空
        /// </summary>
        /// <param name="entity">类</param>
        /// <param name="filed">属性字段</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>是否可空</returns>
        public static bool CanBeNull(this object entity, string filed, bool IgnoreCase = true)
        {
            System.Type obj = entity.GetType();
            PropertyInfo[] propertys = obj.GetProperties();
            PropertyInfo property = propertys.FirstOrDefault(c => (IgnoreCase && c.Name.ToUpper() == filed.ToUpper()) || (!IgnoreCase && c.Name == filed));
            if (property == null)
                return false;
            ColumnAttribute[] EnumAttributes = (ColumnAttribute[])property.GetCustomAttributes(typeof(ColumnAttribute), false);
            if (EnumAttributes.Length > 0)
                return EnumAttributes[0].CanBeNull;
            return true;
        }

        /// <summary>
        /// 获取属性显示字段
        /// </summary>
        /// <param name="entity">类</param>
        /// <param name="filed">属性字段</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>显示字段,空为找不到属性，没有字段属性则显示属性名</returns>
        public static string ToDisplayColumn(this object entity, string filed, bool IgnoreCase = true)
        {
            System.Type obj = entity.GetType();
            PropertyInfo[] propertys = obj.GetProperties();
            PropertyInfo property = propertys.FirstOrDefault(c => (IgnoreCase && c.Name.ToUpper() == filed.ToUpper()) || (!IgnoreCase && c.Name == filed));
            if (property == null)
                return "";
            DisplayColumnAttribute[] EnumAttributes = (DisplayColumnAttribute[])property.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
            if (EnumAttributes.Length > 0)
                return (!string.IsNullOrEmpty(EnumAttributes[0].Name)?EnumAttributes[0].Name:property.Name);
            return property.Name;
        }

        /// <summary>
        /// 获取属性显示字段格式
        /// </summary>
        /// <param name="entity">类</param>
        /// <param name="filed">属性字段</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>显示字段格式</returns>
        public static string ToDisplayColumnFormat(this object entity, string filed, bool IgnoreCase = true)
        {
            System.Type obj = entity.GetType();
            PropertyInfo[] propertys = obj.GetProperties();
            PropertyInfo property = propertys.FirstOrDefault(c => (IgnoreCase && c.Name.ToUpper() == filed.ToUpper()) || (!IgnoreCase && c.Name == filed));
            if (property == null)
                return "";
            DisplayColumnAttribute[] EnumAttributes = (DisplayColumnAttribute[])property.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
            if (EnumAttributes.Length > 0)
                return (!string.IsNullOrEmpty(EnumAttributes[0].Format) ? EnumAttributes[0].Format : "");
            return "";
        }

        /// <summary>
        /// 获取属性显示样式
        /// </summary>
        /// <param name="entity">类</param>
        /// <param name="filed">属性字段</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>显示样式</returns>
        public static string ToDisplayCSS(this object entity, string filed, bool IgnoreCase = true)
        {
            System.Type obj = entity.GetType();
            PropertyInfo[] propertys = obj.GetProperties();
            PropertyInfo property = propertys.FirstOrDefault(c => (IgnoreCase && c.Name.ToUpper() == filed.ToUpper()) || (!IgnoreCase && c.Name == filed));
            if (property == null)
                return "";
            DisplayColumnAttribute[] EnumAttributes = (DisplayColumnAttribute[])property.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
            if (EnumAttributes.Length > 0)
                return (!string.IsNullOrEmpty(EnumAttributes[0].CSS) ? EnumAttributes[0].CSS :"");
            return property.Name;
        }

        /// <summary>
        /// 获取属性显示是否用于表头
        /// </summary>
        /// <param name="entity">类</param>
        /// <param name="filed">属性字段</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>是否用于表头</returns>
        public static bool IsCanHead(this object entity, string filed, bool IgnoreCase = true)
        {
            System.Type obj = entity.GetType();
            PropertyInfo[] propertys = obj.GetProperties();
            PropertyInfo property = propertys.FirstOrDefault(c => (IgnoreCase && c.Name.ToUpper() == filed.ToUpper()) || (!IgnoreCase && c.Name == filed));
            if (property == null)
                return false ;
            DisplayColumnAttribute[] EnumAttributes = (DisplayColumnAttribute[])property.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
            if (EnumAttributes.Length > 0)
                return EnumAttributes[0].CanHead  ;
            return false ;
        }

        /// <summary>
        /// 获取属性显示是否用于搜索
        /// </summary>
        /// <param name="entity">类</param>
        /// <param name="filed">属性字段</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>是否用于表头</returns>
        public static bool IsCanSearch(this object entity, string filed, bool IgnoreCase = true)
        {
            System.Type obj = entity.GetType();
            PropertyInfo[] propertys = obj.GetProperties();
            PropertyInfo property = propertys.FirstOrDefault(c => (IgnoreCase && c.Name.ToUpper() == filed.ToUpper()) || (!IgnoreCase && c.Name == filed));
            if (property == null)
                return false;
            DisplayColumnAttribute[] EnumAttributes = (DisplayColumnAttribute[])property.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
            if (EnumAttributes.Length > 0)
                return EnumAttributes[0].CanSearch;
            return false;
        }

        /// <summary>
        /// 获取属性显示是否用于统计
        /// </summary>
        /// <param name="entity">类</param>
        /// <param name="filed">属性字段</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>是否用于统计</returns>
        public static bool IsCanCount(this object entity, string filed, bool IgnoreCase = true)
        {
            System.Type obj = entity.GetType();
            PropertyInfo[] propertys = obj.GetProperties();
            PropertyInfo property = propertys.FirstOrDefault(c => (IgnoreCase && c.Name.ToUpper() == filed.ToUpper()) || (!IgnoreCase && c.Name == filed));
            if (property == null)
                return false;
            DisplayColumnAttribute[] EnumAttributes = (DisplayColumnAttribute[])property.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
            if (EnumAttributes.Length > 0)
                return EnumAttributes[0].CanCount ;
            return false;
        }

        /// <summary>
        /// 获取属性显示是否用于导入导出
        /// </summary>
        /// <param name="entity">类</param>
        /// <param name="filed">属性字段</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>是否用于表头</returns>
        public static bool IsCanImpExp(this object entity, string filed, bool IgnoreCase = true)
        {
            System.Type obj = entity.GetType();
            PropertyInfo[] propertys = obj.GetProperties();
            PropertyInfo property = propertys.FirstOrDefault(c => (IgnoreCase && c.Name.ToUpper() == filed.ToUpper()) || (!IgnoreCase && c.Name == filed));
            if (property == null)
                return false;
            DisplayColumnAttribute[] EnumAttributes = (DisplayColumnAttribute[])property.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
            if (EnumAttributes.Length > 0)
                return EnumAttributes[0].CanImpExp;
            return false;
        }

        /// <summary>
        /// 获取属性显示是否用于导入导出
        /// </summary>
        /// <param name="entity">类</param>
        /// <param name="filed">属性字段</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>是否用于表头</returns>
        public static bool IsCanUnique(this object entity, string filed, bool IgnoreCase = true)
        {
            System.Type obj = entity.GetType();
            PropertyInfo[] propertys = obj.GetProperties();
            PropertyInfo property = propertys.FirstOrDefault(c => (IgnoreCase && c.Name.ToUpper() == filed.ToUpper()) || (!IgnoreCase && c.Name == filed));
            if (property == null)
                return false;
            DisplayColumnAttribute[] EnumAttributes = (DisplayColumnAttribute[])property.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
            if (EnumAttributes.Length > 0)
                return EnumAttributes[0].IsUnique;
            return false;
        }

    }
}
