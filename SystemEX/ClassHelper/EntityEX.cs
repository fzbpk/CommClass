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
    /// 实体扩展
    /// </summary>
    public static  class EntityEX
    {

       /// <summary>
       /// 获取映射表名
       /// </summary>
       /// <param name="obj">实体</param>
       /// <returns>映射表名，没有则返回类名</returns>
        public static string ToTableName(this object obj)
        {
            Type ObjType = obj.GetType();
            TableAttribute[] TableAttributes = (TableAttribute[])ObjType.GetCustomAttributes(typeof(TableAttribute), false);
            string TableName = (TableAttributes == null ? ObjType.Name : (TableAttributes.Length > 0 ? TableAttributes.ToList().First().Name.Trim() : ObjType.Name));
            return TableName;
        }

        /// <summary>
        /// 获取类型的描述
        /// </summary>
        /// <param name="obj">实体</param>
        /// <returns>返回对象描述，无则显示类名</returns>
        public static string ToDescription(this object obj)
        {
            Type ObjType = obj.GetType();
            DescriptionAttribute[] EnumAttributes = (DescriptionAttribute[])ObjType.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (EnumAttributes.Length > 0)
                return EnumAttributes[0].Description;
            return obj.ToString();
        }

        /// <summary>
        /// 获取类型的显示名称
        /// </summary>
        /// <param name="obj">实体</param>
        /// <returns>返回对象描述</returns>
        public static string ToDisplayName(this object obj)
        {
            Type ObjType = obj.GetType();
            DisplayNameAttribute[] EnumAttributes = (DisplayNameAttribute[])ObjType.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            if (EnumAttributes.Length > 0)
                return EnumAttributes[0].DisplayName;
            return obj.ToString();
        }

        /// <summary>
        /// 列出类所有属性
        /// </summary>
        /// <param name="obj">实体</param>
        /// <returns>属性和值</returns>
        public static Dictionary<string, object> ToDictionary(this object obj)
        {
            Type ObjType = obj.GetType();
            Dictionary<string, object> listItems = new Dictionary<string, object>();
            //获得所有property的信息
            PropertyInfo[] properties = ObjType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //遍历每个property
            foreach (PropertyInfo p in properties)
            {
                if (p != null)
                {
                    Type t = p.PropertyType;
                    if (t.IsValueType)
                    {
                        if (p.GetValue(obj, null) != null)
                            listItems.Add(p.Name, p.GetValue(obj, null));
                        else
                            listItems.Add(p.Name, null);
                    }
                }
            }
            return listItems;
        }

        /// <summary>
        /// 实体的显示标题列
        /// </summary>
        /// <param name="obj">实体</param>
        /// <param name="NeedAttribute">是否需要有显示属性标签</param>
        /// <returns>标签列表，实体属性-显示名</returns>
        public static Dictionary<string, string> ToHeaderList(this object obj, bool NeedAttribute = true)
        {
            Type ObjType = obj.GetType();
            Dictionary<string, string> res = new Dictionary<string, string>();
            List<string> noattribute = new List<string>();
            Dictionary<string, DisplayColumnAttribute> listItems = new Dictionary<string, DisplayColumnAttribute>();
            //获得所有property的信息
            PropertyInfo[] properties = ObjType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //遍历每个property
            foreach (PropertyInfo p in properties)
            {
                if (p != null)
                {
                    DisplayColumnAttribute[] DisplayColumn = (DisplayColumnAttribute[])p.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
                    if (DisplayColumn.Length > 0)
                    {
                        if (DisplayColumn[0].CanHead)
                            listItems.Add(p.Name, DisplayColumn[0]);
                    }
                    else if (NeedAttribute)
                        noattribute.Add(p.Name);
                }
            }
            foreach (var tmp in listItems.OrderBy(c=>c.Value .Seqencing))
            { 
               if(!string.IsNullOrEmpty(tmp.Value .Name))
                   res.Add(tmp.Key, tmp.Value.Name);
               else
                   res.Add(tmp.Key, tmp.Key);
            }
            foreach (var tmp in noattribute)
                res.Add(tmp, tmp);
            return res;
        }

        /// <summary>
        /// 实体的查询列
        /// </summary>
        /// <param name="obj">实体</param>
        /// <param name="NeedAttribute">是否需要有显示属性标签</param>
        /// <returns>标签列表，实体属性-显示名</returns>
        public static Dictionary<string, string> ToSearchList(this object obj, bool NeedAttribute = true)
        {
            Type ObjType = obj.GetType();
            Dictionary<string, string> res = new Dictionary<string, string>();
            List<string> noattribute = new List<string>();
            Dictionary<string, DisplayColumnAttribute> listItems = new Dictionary<string, DisplayColumnAttribute>();
            //获得所有property的信息
            PropertyInfo[] properties = ObjType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //遍历每个property
            foreach (PropertyInfo p in properties)
            {
                if (p != null)
                {
                    DisplayColumnAttribute[] DisplayColumn = (DisplayColumnAttribute[])p.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
                    if (DisplayColumn.Length > 0)
                    {
                        if (DisplayColumn[0].CanSearch )
                            listItems.Add(p.Name, DisplayColumn[0]);
                    }
                    else if (NeedAttribute)
                        noattribute.Add(p.Name);
                }
            }
            foreach (var tmp in listItems.OrderBy(c => c.Value.Seqencing))
            {
                if (!string.IsNullOrEmpty(tmp.Value.Name))
                    res.Add(tmp.Key, tmp.Value.Name);
                else
                    res.Add(tmp.Key, tmp.Key);
            }
            foreach (var tmp in noattribute)
                res.Add(tmp, tmp);
            return res;
        }

        /// <summary>
        /// 实体的查询列
        /// </summary>
        /// <param name="obj">实体</param>
        /// <param name="NeedAttribute">是否需要有显示属性标签</param>
        /// <returns>标签列表，实体属性-显示名</returns>
        public static Dictionary<string, string> ToChoutList(this object obj, bool NeedAttribute = true)
        {
            Type ObjType = obj.GetType();
            Dictionary<string, string> res = new Dictionary<string, string>();
            List<string> noattribute = new List<string>();
            Dictionary<string, DisplayColumnAttribute> listItems = new Dictionary<string, DisplayColumnAttribute>();
            //获得所有property的信息
            PropertyInfo[] properties = ObjType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //遍历每个property
            foreach (PropertyInfo p in properties)
            {
                if (p != null)
                {
                    DisplayColumnAttribute[] DisplayColumn = (DisplayColumnAttribute[])p.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
                    if (DisplayColumn.Length > 0)
                    {
                        if (DisplayColumn[0].CanCount )
                            listItems.Add(p.Name, DisplayColumn[0]);
                    }
                    else if (NeedAttribute)
                        noattribute.Add(p.Name);
                }
            }
            foreach (var tmp in listItems.OrderBy(c => c.Value.Seqencing))
            {
                if (!string.IsNullOrEmpty(tmp.Value.Name))
                    res.Add(tmp.Key, tmp.Value.Name);
                else
                    res.Add(tmp.Key, tmp.Key);
            }
            foreach (var tmp in noattribute)
                res.Add(tmp, tmp);
            return res;
        }

        /// <summary>
        /// 实体的导入导出列
        /// </summary>
        /// <param name="obj">实体</param>
        /// <param name="NeedAttribute">是否需要有显示属性标签</param>
        /// <returns>标签列表，实体属性-显示名</returns>
        public static Dictionary<string, string> ToImpExpList(this object obj, bool NeedAttribute = true)
        {
            Type ObjType = obj.GetType();
            Dictionary<string, string> res = new Dictionary<string, string>();
            List<string> noattribute = new List<string>();
            Dictionary<string, DisplayColumnAttribute> listItems = new Dictionary<string, DisplayColumnAttribute>();
            //获得所有property的信息
            PropertyInfo[] properties = ObjType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //遍历每个property
            foreach (PropertyInfo p in properties)
            {
                if (p != null)
                {
                    DisplayColumnAttribute[] DisplayColumn = (DisplayColumnAttribute[])p.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
                    if (DisplayColumn.Length > 0)
                    {
                        if (DisplayColumn[0].CanImpExp)
                            listItems.Add(p.Name, DisplayColumn[0]);
                    }
                    else if (NeedAttribute)
                        noattribute.Add(p.Name);
                }
            }
            foreach (var tmp in listItems.OrderBy(c => c.Value.Seqencing))
            {
                if (!string.IsNullOrEmpty(tmp.Value.Name))
                    res.Add(tmp.Key, tmp.Value.Name);
                else
                    res.Add(tmp.Key, tmp.Key);
            }
            foreach (var tmp in noattribute)
                res.Add(tmp, tmp);
            return res;
        }


    }
 

}
