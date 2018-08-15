using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.DataProvider ;
using System.Reflection;
using System.ComponentModel;
namespace DataFactory
{
    /// <summary>
    /// Linq数据库处理
    /// </summary>
    public class DBAdaptor<T> : IDisposable where T : class,
                  new()
    {
        
        #region "定义"
        private string connstr = "";
        private  DBType  DB =  DBType.None ;
        private DataContext context = null;
        
        private int Tout = 0;
        private bool m_disposed;
        #endregion

        #region "构造函数"

       /// <summary>
        /// Linq数据库处理
       /// </summary>
       /// <param name="ConnectionType">数据库类型</param>
       /// <param name="ConnectionString">连接串</param>
       /// <param name="Timeout">超时时间，毫秒</param>
       public DBAdaptor(DBType ConnectionType,string ConnectionString,int Timeout=60)
       {
           if (Connection == null)
               throw new NullReferenceException("连接字符串为空");
           connstr = ConnectionString;
           DB = ConnectionType;
           Tout = Timeout;
           switch ( DB)
           {
               case DBType.Access:
                   context = new DataContext(new LinqToDB.DataProvider.Access.AccessDataProvider(), connstr);
                   break;
               case  DBType.MSSQL :
                   context = new DataContext(new LinqToDB.DataProvider.SqlServer.SqlServerDataProvider("", LinqToDB.DataProvider.SqlServer.SqlServerVersion.v2008), connstr);
                   break;
               case DBType.MYSQL :
                   context = new DataContext( new LinqToDB.DataProvider.MySql.MySqlDataProvider(), connstr);
                   break;
               case DBType.Oracle:
                   context = new DataContext(new LinqToDB.DataProvider.Oracle.OracleDataProvider(), connstr);
                   break;
               case DBType.SQLite:
                   context = new DataContext(new LinqToDB.DataProvider.SQLite.SQLiteDataProvider(), connstr);
                   break;
               case DBType.PostgreSQL:
                   context = new DataContext(new LinqToDB.DataProvider.PostgreSQL.PostgreSQLDataProvider(), connstr);
                   break;
               default :
                   throw new NotSupportedException("不支持的数据库");
           }
           
          
       }

       /// <summary>
       /// 释放资源
       /// </summary>
       ~DBAdaptor()
      {
        Dispose(false);
      }

       /// <summary>
       /// 释放资源
       /// </summary>
        public void Dispose()
       { 
         Dispose(true);
         GC.SuppressFinalize(this);
       }

        /// <summary>
        /// 释放连接
        /// </summary>
        /// <param name="disposing">是否释放</param>
       protected virtual void Dispose(bool disposing)
       {
           lock (this)
           {
               if (disposing && !m_disposed  )
              {
                  try
                  {
                      if (context != null)
                      {
                          
                      }
                      context = null;
                  }
                  catch { }
                  m_disposed = true; 
               }         
           }
       }

  
       #endregion

        #region "属性"

       /// <summary>
       /// 数据库连接类型
       /// </summary>
       public DBType DataBaseType
       {
           get { return DB; }
           set { DB = value ;  }
       }

       /// <summary>
       /// 数据库连接串
       /// </summary>
       public string Connection
       {
           get { return connstr; }
           set { 
               connstr = value;
               switch (DB)
               {
                   case DBType.Access:
                       context = new DataContext(new LinqToDB.DataProvider.Access.AccessDataProvider(), connstr);
                       break;
                   case DBType.MSSQL:
                       context = new DataContext(new LinqToDB.DataProvider.SqlServer.SqlServerDataProvider("", LinqToDB.DataProvider.SqlServer.SqlServerVersion.v2008), connstr);
                       break;
                   case DBType.MYSQL:
                       context = new DataContext(new LinqToDB.DataProvider.MySql.MySqlDataProvider(), connstr);
                       break;
                   case DBType.Oracle:
                       context = new DataContext(new LinqToDB.DataProvider.Oracle.OracleDataProvider(), connstr);
                       break;
                   case DBType.SQLite:
                       context = new DataContext(new LinqToDB.DataProvider.SQLite.SQLiteDataProvider(), connstr);
                       break;
                   case DBType.PostgreSQL:
                       context = new DataContext(new LinqToDB.DataProvider.PostgreSQL.PostgreSQLDataProvider(), connstr);
                       break;
                   default:
                       throw new NotSupportedException("不支持的数据库");
               }
           }
       }

       /// <summary>
       /// 数据库操作超时时间
       /// </summary>
       public int TimeOut
       {
           get { return Tout; }
           set { Tout = value; }
       }

       #endregion

        #region "方法"

       /// <summary>
       /// 根据条件查找对象
       /// </summary>
       /// <param name="Entity">条件</param>
       /// <returns></returns>
       [DisplayName("Insert")]
       [Description("插入对象")]
       public virtual void Insert(T Entity)
       {
           context.Insert(Entity);
       }

       /// <summary>
       /// 更新实体
       /// </summary>
       /// <param name="Entity">实体</param>
       [DisplayName("Update")]
       [Description("更新实体")]
       public virtual void Update(T Entity)
      {
          context.Update(Entity);
       }

        /// <summary>
       /// 删除实体
       /// </summary>
       /// <param name="Entity"></param>
       [DisplayName("Delete")]
       [Description("删除实体")]
       public virtual void Delete(T Entity)
       {
           context.Delete(Entity);
       }

       /// <summary>
       /// 根据条件查找对象
       /// </summary>
       /// <param name="whereLambda">条件</param>
       /// <returns></returns>
       [DisplayName("Find")]
       [Description("根据条件查找对象")]
       public virtual T Find(Expression<Func<T, bool>> whereLambda)
       {
           return context.GetTable<T>().FirstOrDefault(whereLambda);
       }

       /// <summary>
       /// 查询符合条件的记录
       /// </summary>
       /// <param name="whereLambda">查询条件</param>
       /// <returns>符合条件的记录</returns>
       [DisplayName("Query")]
       [Description("查询符合条件的记录")]
       public virtual IQueryable<T> Query(Expression<Func<T, bool>> whereLambda)
       {
           return context.GetTable<T>().Where(whereLambda);
       }

       /// <summary>
       /// 查询所有记录列表
       /// </summary>
       /// <returns>所有记录</returns>
       [DisplayName("List")]
       [Description("查询所有记录列表")]
       public virtual List<T> List()
       {
           return context.GetTable<T>().ToList();
       }

       /// <summary>
       /// 分页所有数据
       /// </summary>
       /// <param name="PageIndex">当前页，从1开始</param>
       /// <param name="PageSize">页面大小</param>
       /// <returns></returns>
       [DisplayName("Page")]
       [Description("分页查询数据")]
       public virtual List<T> Page(int PageIndex,int PageSize)
       {
           return context.GetTable<T>().Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList ();
       }

       /// <summary>
       /// 分页所有数据
       /// </summary>
       /// <param name="PageIndex">当前页，从1开始</param>
       /// <param name="PageSize">页面大小</param>
       /// <param name="PageCount">总页数</param>
       /// <param name="RecordCount">记录数</param>
       /// <returns></returns>
       [DisplayName("Page")]
       [Description("分页查询数据")]
       public virtual List<T> Page(int PageIndex, int PageSize, out int PageCount, out int RecordCount)
       {
           PageCount = 0;
           RecordCount = 0;
           RecordCount = context.GetTable<T>().Count();
           if (PageSize == 0)
               PageCount = (RecordCount > 0 ? 1 : 0);
           else
               PageCount = (RecordCount % PageSize == 0 ? RecordCount / PageSize : RecordCount / PageSize + 1);
           return context.GetTable<T>().Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
       }

       /// <summary>
       /// 分页查询数据
       /// </summary>
       /// <param name="whereLambda">查询条件</param>
       /// <param name="OrderBy">排序字段</param>
       /// <param name="PageIndex">当前页，从1开始</param>
       /// <param name="PageSize">页面大小</param>
       /// <returns>符合条件的记录</returns>
       [DisplayName("Page")]
       [Description("分页查询数据")]
       public virtual List<T> Select(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> OrderBy , int PageIndex, int PageSize)
       {
           return context.GetTable<T>().Where(whereLambda).OrderBy(OrderBy).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
       }

        /// <summary>
       /// 分页查询数据
       /// </summary>
       /// <param name="whereLambda">查询条件</param>
       /// <param name="OrderBy">排序字段</param>
       /// <param name="PageIndex">当前页，从1开始</param>
       /// <param name="PageSize">页面大小</param>
       /// <param name="PageCount">总页数</param>
       /// <param name="RecordCount">记录数</param>
       /// <returns>符合条件的记录</returns>
       [DisplayName("Page")]
       [Description("分页查询数据")]
       public virtual List<T> Select(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> OrderBy, int PageIndex, int PageSize, out int PageCount, out int RecordCount)
       {
           PageCount = 0;
           RecordCount = 0;
           RecordCount = context.GetTable<T>().Where(whereLambda).Count();
           if (PageSize == 0)
               PageCount = (RecordCount > 0 ? 1 : 0);
           else
               PageCount = (RecordCount % PageSize == 0 ? RecordCount / PageSize : RecordCount / PageSize + 1);
           return context.GetTable<T>().Where(whereLambda).OrderBy(OrderBy).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
       }

        #endregion

    }
}
