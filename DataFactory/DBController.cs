using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data ;
using DataDriver;
namespace DataFactory
{
    /// <summary>
    /// 数据库T-SQL基本操作
    /// </summary>
    public class DBController : IDisposable
    {

       #region "定义"
        private string connstr = "";
        private  DBType  DB =  DBType.None ;
        private int Tout = 0;
        private bool m_disposed;
        #endregion

       #region "构造函数"

        /// <summary>
        /// 数据库T-SQL基本操作
        /// </summary>
       public DBController()
       { }

       /// <summary>
       /// 数据库T-SQL基本操作
       /// </summary>
       /// <param name="ConnectionType">数据库类型</param>
       /// <param name="ConnectionString">连接串</param>
       /// <param name="Timeout">超时时间，毫秒</param>
       public DBController(DBType ConnectionType,string ConnectionString,int Timeout=60)
       {
           connstr = ConnectionString;
           DB = ConnectionType;
           Tout = Timeout;
       }

        /// <summary>
        /// 释放资源
        /// </summary>
       ~DBController()
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
           set { connstr = value; }
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

       private IDataBase MakeConnection()
       {
           IDataBase DBOper = null;
           switch (DB)
           {
               case DBType.Access:
                   DBOper = new Access(connstr, Tout);
                   break;
               case DBType.MYSQL:
                   DBOper = new DataDriver.MySql(connstr, Tout);
                   break;
               case DBType.MSSQL:
                   DBOper = new DataDriver.MSSql(connstr, Tout);
                   break;
               case DBType.Oracle:
                   DBOper = new DataDriver.Oracle(connstr, Tout);
                   break;
               case DBType.SQLite:
                   DBOper = new DataDriver.SQLite(connstr, Tout);
                   break;
               case DBType.PostgreSQL:
                   DBOper = new DataDriver.PostgreSQL(connstr, Tout);
                   break;
               default:
                   DBOper = null;
                   break;
           }
           return DBOper;
       }

        /// <summary>
        /// 检查数据库连接
        /// </summary>
        /// <param name="ErrorMessage">错误信息</param>
        /// <returns>是否连接成功</returns>
       public bool CheckConnection(out string ErrorMessage)
       {
           ErrorMessage = "";
           IDataBase DBOper = MakeConnection();
           if (DBOper != null)
               return DBOper.CheckConnection(out ErrorMessage);
           else
               ErrorMessage = "不支持的数据库";
           return false;

       }

       /// <summary>
       /// 查询表是否存在
       /// </summary>
       /// <param name="TableName">表名称</param>
       /// <returns>是否存在</returns>
       public bool TableIsExist(string TableName)
       {
           IDataBase DBOper = MakeConnection();
           if (DBOper != null)
               return DBOper.TableIsExist(TableName);
           return false;
       }

       /// <summary>
       /// 数据库Insert,update,delete带返回执行数
       /// </summary>
       /// <param name="sql">SQL语句</param>
       /// <returns>影响数量</returns>
       public int ExecuteNonQuery(string sql)
       {
           IDataBase DBOper = MakeConnection();
           if (DBOper != null)
               return DBOper.ExecuteNonQuery(sql);
           return -1;
       }

       /// <summary>
       /// 判断是否存在记录
       /// </summary>
       /// <param name="sql">T-SQL</param>
       /// <returns>TRUE为存在，False为不存在</returns>
       public bool IsExist(string sql)
       {
           IDataBase DBOper = MakeConnection();
           if (DBOper != null)
               return DBOper.IsExist(sql);
           return false;
       }

       /// <summary>
       /// 查询数据记录
       /// </summary>
       /// <param name="sql">T-SQL</param>
       /// <returns>TRUE为存在，False为不存在</returns>
       public Dictionary<string, object> Find(string sql)
       {
           IDataBase DBOper = MakeConnection();
           if (DBOper != null)
               return DBOper.Find(sql);
           return null;
       }

       /// <summary>
       /// 返回第一行第一列数据
       /// </summary>
       /// <param name="sql">T-SQL</param>
       /// <returns></returns>
       public object ExecuteScalar(string sql)
       {
           IDataBase DBOper = MakeConnection();
           if (DBOper != null)
               return DBOper.ExecuteScalar(sql);
           return null;
       }

       /// <summary>
       /// 获取查询数据
       /// </summary>
       /// <param name="sql">T-SQL</param>
       /// <returns>DataTable</returns>
       public DataTable getDataTable(string sql)
       {
           int PageSize = 0;
           int RecCount, PageCount;
           return getDataTable(sql, PageSize, out RecCount, out PageCount);
       }

       /// <summary>
       /// 获取查询数据
       /// </summary>
       /// <param name="sql">T-SQL</param>
       /// <param name="PageSize">分页大小</param>
       /// <param name="RecCount">返回查询记录数</param>
       /// <param name="PageCount">返回总页数</param>
       /// <param name="TableName">表名</param>
       /// <returns>DataTable</returns>
       public DataTable getDataTable(string sql, int PageSize, out int RecCount, out int PageCount, string TableName = "Query")
       {
           RecCount = 0;
           PageCount = 0;
           IDataBase DBOper = MakeConnection();
           if (DBOper != null)
               return DBOper.getDataTable(sql,PageSize , out RecCount, out PageCount, TableName);
           return null;
       }

      /// <summary>
      /// 内存分页
      /// </summary>
       /// <param name="PageIndex">当前页</param>
       /// <param name="PageSize">分页大小</param>
       /// <param name="DisplayField">字段列，每个字段用,分开</param>
       /// <param name="TableName">表名，支持（） k 视图方式</param>
       /// <param name="Where">查询条件</param>
       /// <param name="OrderBy">排序语句</param>
       /// <param name="GroupBy">GROUP BY 字段</param>
       /// <param name="RecodeCount">返回记录数</param>
       /// <param name="PageCount">返回页数</param>
      /// <returns>查询结果</returns>
       public DataTable getDataTableByRam(int PageIndex, int PageSize, string DisplayField, string TableName, string Where, string OrderBy, string GroupBy, out int RecodeCount, out int PageCount)
       {
           RecodeCount = 0;
           PageCount = 0;
           IDataBase DBOper = MakeConnection();
           if (DBOper != null)
               return DBOper.getDataTableByRam(PageIndex, PageSize, DisplayField, TableName, Where, OrderBy, GroupBy, out RecodeCount, out PageCount);
           return null;
       }

       /// <summary>
       /// 数据库分页
       /// </summary>
       /// <param name="PageIndex">当前页</param>
       /// <param name="PageSize">分页大小</param>
       /// <param name="DisplayField">字段列，每个字段用,分开</param>
       /// <param name="TableName">表名，支持（） k 视图方式</param>
       /// <param name="Where">查询条件</param>
       /// <param name="OrderField">排序字段</param>
       /// <param name="OrderBy">排序语句</param>
       /// <param name="GroupBy">GROUP BY 字段</param>
       /// <param name="RecodeCount">返回记录数</param>
       /// <param name="PageCount">返回页数</param>
       /// <returns>查询结果</returns>
       public DataTable getDataTableByDB(int PageIndex, int PageSize, string DisplayField, string TableName, string Where, string OrderField, string OrderBy, string GroupBy, out int RecodeCount, out int PageCount)
       {
           RecodeCount = 0;
           PageCount = 0;
           IDataBase DBOper = MakeConnection();
           if (DBOper != null)
               return DBOper.getDataTableByDB(PageIndex, PageSize, DisplayField, TableName, Where, OrderField, OrderBy, GroupBy, out RecodeCount, out PageCount);
           return null;
       }

        #endregion


    }
}
