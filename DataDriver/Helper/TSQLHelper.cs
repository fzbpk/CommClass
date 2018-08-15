using System;
using System.Text;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinqToDB.Mapping;
using System.Reflection;
namespace NK.Data
{
    /// <summary>
    /// TSQL扩展
    /// </summary>
    public static partial class TSQLHelper
    {

        /// <summary>
        /// 字段转T-SQL
        /// </summary>
        /// <param name="Key">字段</param>
        /// <param name="Mode">运算类型</param>
        /// <param name="Value">字段值</param>
        /// <returns>T-SQL</returns>
        public static string KeyToSql(this string Key, ExpressionType Mode, object Value)
        {
            if (string.IsNullOrEmpty(Key))
                return "";
            string res = "";
            Type t = Value.GetType();
            if (Value == null)
            {
                if (Mode == ExpressionType.Equal)
                    res = " " + Key + " IS NULL ";
                else if (Mode == ExpressionType.NotEqual)
                    res = " " + Key + " IS NOT NULL ";
            }
            else
            {
                if (t == typeof(string))
                {
                    if (Mode == ExpressionType.Constant)
                        res = " '%" + Value.ToString() + "%' ";
                    else
                        res = " '" + Value.ToString() + "' ";
                }
                else if (t == typeof(bool))
                {
                    if ((bool)Value)
                        res = "1";
                    else
                        res = "0";

                }
                else if (t.IsEnum)
                {
                    string ss = Value.ToString().ToUpper().Trim();
                    FieldInfo[] fields = Value.GetType().GetFields();
                    if (fields != null)
                    {
                        if (fields.Length > 0)
                        {
                            foreach (var field in fields)
                            {
                                string EnumName = field.Name.ToUpper().Trim();
                                if (ss == EnumName)
                                {
                                    res = field.GetRawConstantValue().ToString();
                                    break;

                                }
                            }
                        }
                    }
                }
                else if (t.IsValueType)
                    res = " " + Value.ToString() + " ";
                switch (Mode)
                {
                    case ExpressionType.Equal:
                        res = " " + Key + " = " + res;
                        break;
                    case ExpressionType.NotEqual:
                        res = " " + Key + " <> " + res;
                        break;
                    case ExpressionType.GreaterThan:
                        res = " " + Key + " > " + res;
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        res = " " + Key + " >= " + res;
                        break;
                    case ExpressionType.LessThan:
                        res = " " + Key + " < " + res;
                        break;
                    case ExpressionType.LessThanOrEqual:
                        res = " " + Key + " <= " + res;
                        break;
                    case ExpressionType.Constant:
                        res = " " + Key + " Like " + res;
                        break;
                    case ExpressionType.Add:
                        res = " " + Key + " + " + res;
                        break;
                    case ExpressionType.Subtract:
                        res = " " + Key + " - " + res;
                        break;
                    case ExpressionType.Multiply:
                        res = " " + Key + " * " + res;
                        break;
                    case ExpressionType.Divide:
                        res = " " + Key + " / " + res;
                        break;
                    case ExpressionType.Modulo:
                        res = " " + Key + " % " + res;
                        break;
                    case ExpressionType.And:
                        res = " " + Key + " & " + res;
                        break;
                    case ExpressionType.Or:
                        res = " " + Key + " | " + res;
                        break;
                }
            }
            return res;
        }

        /// <summary>
        /// T-SQL合并
        /// </summary>
        /// <param name="sql1">语句1</param>
        /// <param name="Mode">运算类型</param>
        /// <param name="sql2">语句2</param>
        /// <returns></returns>
        public static string JoinToSql(this string sql1, ExpressionType Mode, string sql2)
        {
            sql1 = sql1.ToUpper();
            sql2 = sql2.ToUpper();
            if (string.IsNullOrEmpty(sql1))
                sql1 = "";
            if (string.IsNullOrEmpty(sql2))
                sql2 = "";
            switch (Mode)
            {
                case ExpressionType.AndAlso:
                    return " (" + sql1 + ") AND (" + sql2 + ") ";
                case ExpressionType.OrElse:
                    return " (" + sql1 + ") OR  (" + sql2 + ") ";
                default:
                    return "";
            }

        }

        /// <summary>
        /// T-SQL取反
        /// </summary>
        /// <param name="sql">语句</param>
        /// <returns></returns>
        public static string NotToSql(this string sql)
        {
            sql = sql.ToUpper();
            if (string.IsNullOrEmpty(sql))
                return "";
            return " NOT (" + sql + ")";
        }

        /// <summary>
        /// 条件语句合成
        /// </summary>
        /// <param name="Column">字段，字段值</param>
        /// <param name="CVMode">字段与其值的运算</param>
        /// <param name="CCMode">字段间逻辑运算</param>
        /// <returns>不带WHERE关键字的条件语句</returns>
        public static string WhereToSql(this Dictionary<string, object> Column, ExpressionType CVMode, ExpressionType CCMode)
        {
            string sql = "";
            if (Column != null)
            {
                foreach (var tmp in Column)
                {
                    if (string.IsNullOrEmpty(sql))
                        sql = tmp.Key.KeyToSql(CVMode, tmp.Value);
                    else
                        sql = sql.JoinToSql(CCMode, tmp.Key.KeyToSql(CVMode, tmp.Value));
                }
            }
            return sql;
        }

        /// <summary>
        /// 合成ORDER语句
        /// </summary>
        /// <param name="Column">字段，字段顺序倒序</param>
        /// <returns>不带ORDERBY关键字的ORDERBY语句</returns>
        public static string OrderbyToSql(this Dictionary<string, bool> Column)
        {
            string sql = "";
            if (Column != null)
            {
                foreach (var tmp in Column)
                {
                    if (string.IsNullOrEmpty(sql))
                        sql = tmp.Key + " " + (tmp.Value ? "Desc" : "");
                    else
                        sql += "," + tmp.Key + " " + (tmp.Value ? "Desc" : "");
                }
            }
            return sql;
        }

        /// <summary>
        /// 查询语句合成T-SQL
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="Column">显示字段</param>
        /// <returns>T-SQL</returns>
        public static string SelectToSql(this string TableName, List<string> Column)
        {
            if (string.IsNullOrEmpty(TableName)) return "";
            string sql = "";
            if (Column != null)
            {
                foreach (var tmp in Column)
                {
                    if (string.IsNullOrEmpty(sql))
                        sql = tmp;
                    else
                        sql += "," + tmp;
                }
            }
            if (string.IsNullOrEmpty(sql))
                sql = "*";
            return "SELECT " + sql + " FROM " + TableName;
        }

        /// <summary>
        /// 合成添加记录T-SQL
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="Column">字段，字段值</param>
        /// <returns>T-SQL</returns>
        public static string InsertToSQL(this string TableName, Dictionary<string, object> Column)
        {
            if (string.IsNullOrEmpty(TableName)) return "";
            string sql = "INSERT INTO " + TableName;
            string Col, Val;
            Col = "";
            Val = "";
            if (Column != null)
            {
                foreach (var tmp in Column)
                {
                    if (string.IsNullOrEmpty(Col))
                        Col = tmp.Key;
                    else
                        Col += "," + tmp.Key;
                    Type t = tmp.Value.GetType();
                    if (tmp.Value == null)
                    {
                        if (string.IsNullOrEmpty(Val))
                            Val = "NULL";
                        else
                            Val += ",NULL";
                    }
                    else
                    {
                        if (t == typeof(string))
                        {
                            if (string.IsNullOrEmpty(Val))
                                Val = "'" + tmp.Value.ToString() + "'";
                            else
                                Val += ",'" + tmp.Value.ToString() + "'";
                        }
                        else if (t == typeof(bool))
                        {
                            if ((bool)tmp.Value)
                            {
                                if (string.IsNullOrEmpty(Val))
                                    Val = "1";
                                else
                                    Val += ",1";
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(Val))
                                    Val = "0";
                                else
                                    Val += ",0";
                            }
                        }
                        else if (t.IsEnum)
                        {
                            string ss = tmp.Value.ToString().ToUpper().Trim();
                            FieldInfo[] fields = tmp.Value.GetType().GetFields();
                            if (fields != null)
                            {
                                if (fields.Length > 0)
                                {
                                    foreach (var field in fields)
                                    {
                                        string EnumName = field.Name.ToUpper().Trim();
                                        if (ss == EnumName)
                                        {
                                            if (string.IsNullOrEmpty(Val))
                                                Val = field.GetRawConstantValue().ToString();
                                            else
                                                Val += "," + field.GetRawConstantValue().ToString();
                                            break;

                                        }
                                    }
                                }
                            }
                        }
                        else if (t.IsValueType)
                        {
                            if (string.IsNullOrEmpty(Val))
                                Val = tmp.Value.ToString();
                            else
                                Val += "," + tmp.Value.ToString();
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(Col) || string.IsNullOrEmpty(Val))
                return "";
            sql = sql + "(" + Col + ")VALUE(" + Val + ")";
            return sql;
        }

        /// <summary>
        /// 合成更改记录T-SQL
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="Column">字段，字段值</param>
        /// <returns>T-SQL</returns>
        public static string UpdateToSQL(this string TableName, Dictionary<string, object> Column)
        {
            if (string.IsNullOrEmpty(TableName)) return "";
            string sql = "UPDATE " + TableName + " SET ";
            string ColVal = "";
            if (Column != null)
            {
                foreach (var tmp in Column)
                {
                    if (string.IsNullOrEmpty(ColVal))
                        ColVal = tmp.Key + "=";
                    else
                        ColVal += "," + tmp.Key + "=";
                    Type t = tmp.Value.GetType();
                    if (tmp.Value == null)
                        ColVal += "NULL";
                    else
                    {
                        if (t == typeof(string))
                            ColVal += "'" + tmp.Value.ToString() + "'";
                        else if (t == typeof(bool))
                        {
                            if ((bool)tmp.Value)
                                ColVal += "1";
                            else
                                ColVal += "0";
                        }
                        else if (t.IsEnum)
                        {
                            string ss = tmp.Value.ToString().ToUpper().Trim();
                            FieldInfo[] fields = tmp.Value.GetType().GetFields();
                            if (fields != null)
                            {
                                if (fields.Length > 0)
                                {
                                    foreach (var field in fields)
                                    {
                                        string EnumName = field.Name.ToUpper().Trim();
                                        if (ss == EnumName)
                                            ColVal += field.GetRawConstantValue().ToString();
                                    }
                                }
                            }
                        }
                        else if (t.IsValueType)
                            ColVal += tmp.Value.ToString();
                    }
                }
            }
            sql = sql + ColVal;
            return sql;
        }

        /// <summary>
        /// 合成删除记录T-SQL
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns>T-SQL</returns>
        public static string DeleteToSQL(this string TableName)
        {
            if (string.IsNullOrEmpty(TableName)) return "";
            string sql = "DELETE FROM " + TableName;
            return sql;
        }
         
    }
}
