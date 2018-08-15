using System.Text;
using System.Collections.Generic;
using LinqToDB.Mapping;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace NK.Data
{
    /// <summary>
    /// Lamda表达式扩展
    /// </summary>
    public static partial class LinqExpression
    {

        #region 内部处理函数
        private static string DealConstantExpression(ConstantExpression exp)
        {
            object vaule = exp.Value;
            string v_str = string.Empty;
            if (vaule == null)
            {
                return "NULL";
            }
            if (vaule is string)
            {
                v_str = string.Format("'{0}'", vaule.ToString());
            }
            else if (vaule is DateTime)
            {
                DateTime time = (DateTime)vaule;
                v_str = string.Format("'{0}'", time.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                v_str = vaule.ToString();
            }
            return v_str;
        }
        private static string DealBinaryExpression(BinaryExpression exp)
        {
            string left = WhereToSQL(exp.Left);
            string oper = GetOperStr(exp.NodeType);
            string right = WhereToSQL(exp.Right);

            if (exp.NodeType == ExpressionType.Constant)
            {
                if (right == "NULL")
                    right = "%%";
                else
                    right = "%" + right + "%";
            }
            else if (right == "NULL")
            {
                if (oper == "=")
                {
                    oper = " is ";
                }
                else
                {
                    oper = " is not ";
                }
            }
            return left + oper + right;
        }

        private static string DealMemberExpression(MemberExpression exp)
        {
            PropertyInfo properties = exp.Member.ReflectedType.GetProperty(exp.Member.Name);
            if (properties == null)
                return exp.Member.Name;
            else
            {
                ColumnAttribute[] ColumnAttributes = (ColumnAttribute[])properties.GetCustomAttributes(typeof(ColumnAttribute), false);
                string ColumnName = (ColumnAttributes == null ? exp.Member.Name : (ColumnAttributes.Length > 0 ? (ColumnAttributes.ToList().First().Name == null ? exp.Member.Name : ColumnAttributes.ToList().First().Name.Trim()) : exp.Member.Name));
                return ColumnName;
            }
        }
        private static string GetOperStr(ExpressionType e_type)
        {
            switch (e_type)
            {
                case ExpressionType.OrElse: return " OR ";
                case ExpressionType.Or: return "|";
                case ExpressionType.AndAlso: return " AND ";
                case ExpressionType.And: return "&";
                case ExpressionType.GreaterThan: return ">";
                case ExpressionType.GreaterThanOrEqual: return ">=";
                case ExpressionType.LessThan: return "<";
                case ExpressionType.LessThanOrEqual: return "<=";
                case ExpressionType.NotEqual: return "<>";
                case ExpressionType.Add: return "+";
                case ExpressionType.Subtract: return "-";
                case ExpressionType.Multiply: return "*";
                case ExpressionType.Divide: return "/";
                case ExpressionType.Modulo: return "%";
                case ExpressionType.Equal: return "=";
                case ExpressionType.Constant: return "like";
            }
            return "";
        }


        private static string BinarExpressionProvider(Expression left, Expression right, ExpressionType type, Dictionary<string, string> parModelList)
        {
            string sb = "(";
            //先处理左边
            string reLeftStr = ExpressionRouter(left, parModelList);
            sb += reLeftStr;

            sb += GetOperStr(type);

            //再处理右边
            string tmpStr = ExpressionRouter(right, parModelList);
            if (tmpStr == "null")
            {
                if (sb.EndsWith(" ="))
                {
                    sb = sb.Substring(0, sb.Length - 2) + " is null";
                }
                else if (sb.EndsWith("<>"))
                {
                    sb = sb.Substring(0, sb.Length - 2) + " is not null";
                }
            }
            else
            {
                //添加参数
                sb += tmpStr;
            }

            return sb += ")";
        }

        private static string ExpressionRouter(Expression exp, Dictionary<string, string> parModelList)
        {
            string sb = string.Empty;

            if (exp is BinaryExpression)
            {
                BinaryExpression be = ((BinaryExpression)exp);
                return BinarExpressionProvider(be.Left, be.Right, be.NodeType, parModelList);
            }
            else if (exp is MemberExpression)
            {
                MemberExpression me = ((MemberExpression)exp);
                if (!exp.ToString().StartsWith("value"))
                {
                    return me.Member.Name;
                }
                else
                {
                    var result = Expression.Lambda(exp).Compile().DynamicInvoke();
                    if (result == null)
                    {
                        return "null";
                    }
                    else if (result is ValueType)
                    {
                        parModelList.Add("par" + (parModelList.Count + 1), result.ToString());
                        return "@par" + parModelList.Count;
                    }
                    else if (result is string || result is DateTime || result is char)
                    {
                        parModelList.Add("par" + (parModelList.Count + 1), result.ToString());
                        return "@par" + parModelList.Count;
                    }
                    else if (result is int[])
                    {
                        var rl = result as int[];
                        StringBuilder sbIntStr = new StringBuilder();
                        foreach (var r in rl)
                        {
                            parModelList.Add("par" + (parModelList.Count + 1), result.ToString());
                            sbIntStr.Append("@par" + parModelList.Count + ",");
                        }
                        return sbIntStr.ToString().Substring(0, sbIntStr.ToString().Length - 1);
                    }
                    else if (result is string[])
                    {
                        var rl = result as string[];
                        StringBuilder sbIntStr = new StringBuilder();
                        foreach (var r in rl)
                        {
                            parModelList.Add("par" + (parModelList.Count + 1), result.ToString());
                            sbIntStr.Append("@par" + parModelList.Count + ",");
                        }
                        return sbIntStr.ToString().Substring(0, sbIntStr.ToString().Length - 1);
                    }
                }
            }
            else if (exp is NewArrayExpression)
            {
                NewArrayExpression ae = ((NewArrayExpression)exp);
                StringBuilder tmpstr = new StringBuilder();
                foreach (Expression ex in ae.Expressions)
                {
                    tmpstr.Append(ExpressionRouter(ex, parModelList));
                    tmpstr.Append(",");
                }
                //添加参数

                return tmpstr.ToString(0, tmpstr.Length - 1);
            }
            else if (exp is MethodCallExpression)
            {
                MethodCallExpression mce = (MethodCallExpression)exp;
                string par = ExpressionRouter(mce.Arguments[0], parModelList);
                if (mce.Method.Name == "Like")
                {
                    //添加参数用
                    return string.Format("({0} like {1})", par, ExpressionRouter(mce.Arguments[1], parModelList));
                }
                else if (mce.Method.Name == "NotLike")
                {
                    //添加参数用
                    return string.Format("({0} Not like {1})", par, ExpressionRouter(mce.Arguments[1], parModelList));
                }
                else if (mce.Method.Name == "In")
                {
                    //添加参数用
                    return string.Format("{0} In ({1})", par, ExpressionRouter(mce.Arguments[1], parModelList));
                }
                else if (mce.Method.Name == "NotIn")
                {
                    //添加参数用
                    return string.Format("{0} Not In ({1})", par, ExpressionRouter(mce.Arguments[1], parModelList));
                }
            }
            else if (exp is ConstantExpression)
            {
                ConstantExpression ce = ((ConstantExpression)exp);
                if (ce.Value == null)
                {
                    return "null";
                }
                else if (ce.Value is ValueType)
                {
                    parModelList.Add("par" + (parModelList.Count + 1), ce.Value.ToString());
                    return "@par" + parModelList.Count;
                }
                else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
                {
                    parModelList.Add("par" + (parModelList.Count + 1), ce.Value.ToString());
                    return "@par" + parModelList.Count;
                }

                //对数值进行参数附加
            }
            else if (exp is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp);

                return ExpressionRouter(ue.Operand, parModelList);
            }
            return null;
        }

        #endregion

        /// <summary>
        /// 排序表达式转T-SQL
        /// </summary>
        /// <param name="OrderBy">表达式</param>
        /// <returns>T-SQL</returns>
        public static string OrderbyToSql(this Expression OrderBy)
        {
            var res = "";
            var exp = OrderBy as LambdaExpression;
            if (exp.Body is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp.Body);
                Dictionary<string, string> parModelList = new Dictionary<string, string>();
                res = "order by " + ExpressionRouter(ue.Operand, parModelList).ToLower() + "";
            }
            else
            {
                MemberExpression order = ((MemberExpression)exp.Body);
                res = "order by " + order.Member.Name.ToLower();
            }
            return res;
        }

        /// <summary>
        /// 查询表达式转T-SQL
        /// </summary>
        /// <param name="Where">查询表达式</param>
        /// <returns>T-SQL </returns>
        public static string WhereToSQL(this Expression Where)
        {
            if (Where is LambdaExpression)
            {
                return WhereToSQL((Where as LambdaExpression).Body);
            }
            if (Where is BinaryExpression)
            {
                return DealBinaryExpression(Where as BinaryExpression);
            }
            if (Where is MemberExpression)
            {
                return DealMemberExpression(Where as MemberExpression);
            }
            if (Where is ConstantExpression)
            {
                return DealConstantExpression(Where as ConstantExpression);
            }
            if (Where is UnaryExpression)
            {
                return WhereToSQL((Where as UnaryExpression).Operand);
            }
            return "";
        }

        /// <summary>
        /// 新建TRUE表达式
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <returns>表达式</returns>
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        /// <summary>
        /// 新建TRUE表达式
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <returns>表达式</returns>
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        /// <summary>
        ///     true    
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="expr">表达式</param>
        /// <returns>TRUE表达式</returns>
        public static Expression<Func<T, bool>> True<T>(this Expression<Func<T, bool>> expr) { return f => true; }

        /// <summary>
        /// false
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="expr">表达式</param>
        /// <returns>False表达式</returns>
        public static Expression<Func<T, bool>> False<T>(this Expression<Func<T, bool>> expr) { return f => false; }

        /// <summary>
        /// AND连接
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="expr1">表达式1</param>
        /// <param name="expr2">表达式2</param>
        /// <returns>合并后AND表达式</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// OR 连接
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="expr1">表达式1</param>
        /// <param name="expr2">表达式2</param>
        /// <returns>合并后OR表达式</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.And(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// 表达式取反
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="expr">表达式1</param>
        /// <returns>表达式取反</returns>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
        {
            return Expression.Lambda<Func<T, bool>>
                (Expression.Not(expr));
        }

        /// <summary>
        /// 创建lambda表达式：p=>p.propertyName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Expression<Func<T, TKey>> OrderExpression<T, TKey>(string propertyName)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
            return Expression.Lambda<Func<T, TKey>>(Expression.Property(parameter, propertyName), parameter);
        }

        /// <summary>
        /// 创建lambda表达式：p=>p.propertyName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Expression<Func<T, TKey>> OrderExpression<T, TKey>(PropertyInfo property)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
            return Expression.Lambda<Func<T, TKey>>(Expression.Property(parameter, property.Name), parameter);
        }

    }
}
