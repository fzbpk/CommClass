using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
namespace System.Linq.Expressions
{
    /// <summary>
    /// Lamda表达式扩展
    /// </summary>
    public static class LinqExpression
    {
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

      
    }
}
