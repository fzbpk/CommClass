using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace NK.Data
{
    public partial class OleDb : IDisposable, iDataBase
    {
 
        #region 定义

        private OleDbConnection STConn = null;
        private OleDbTransaction ST = null;
        private bool m_disposed;
        private string ClassName = "";

        #endregion

        #region 构造函数

        public OleDb(string connection = "", int Timeouts = 60)
        {
            this.Connection = connection;
            this.Timeout = Timeouts;
            ClassName = this.GetType().ToString();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        ~OleDb()
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
                if (disposing && !m_disposed)
                {
                    if (ST != null)
                    {
                        try
                        {
                            ST.Rollback();
                            ST.Dispose();
                            ST = null;
                        }
                        catch
                        { }
                    }
                    if (STConn != null)
                    {
                        try
                        {
                            if (STConn.State != ConnectionState.Closed)
                                STConn.Close();
                            STConn = null;
                        }
                        catch
                        { }
                    }
                    m_disposed = true;
                }
            }
        }

        #endregion

        #region 属性
         
        public string Connection { get; set; }
        public int Timeout { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        /// <returns>数据库连接</returns>
        public DbConnection GetConnection()
        {
            try
            {
                DbConnection conn = new OleDbConnection(this.Connection);
                return conn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <param name="conn">数据库连接</param>
        public void CloseConnection(DbConnection conn)
        {
            try
            {
                conn.Close();
                conn.Dispose();
                conn = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检查数据库连接是否连接成功
        /// </summary>
        /// <param name="ErrMsg">错误信息</param>
        /// <returns>是否连接成功</returns>
        public bool CheckConnection(out string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                DbConnection conn = GetConnection();
                CloseConnection(conn);
                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 数据库Insert,update,delete带返回执行数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>影响数量</returns>
        public int ExecuteNonQuery(string sql)
        {
            try
            {
                OleDbConnection conn = (OleDbConnection)GetConnection();
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.CommandTimeout = this.Timeout * 1000;
                int res = cmd.ExecuteNonQuery();
                CloseConnection(conn);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取查询数据
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public DataTable getDataTable(string sql)
        {
            int RecCount = 0;
            int PageSize = 0;
            int PageCount = 0;
            return getDataTable(sql, PageSize, out RecCount, out PageCount);
        }

        /// <summary>
        /// 获取查询数据
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="RecCount">返回记录数</param>
        /// <returns></returns>
        public DataTable getDataTable(string sql, out int RecCount)
        {
            int PageSize = 0;
            int PageCount = 0;
            return getDataTable(sql, PageSize, out RecCount, out PageCount);
        }

        /// <summary>
        /// 获取查询数据
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="PageSize">分页大小</param>
        /// <param name="PageCount">返回总页数</param>
        /// <returns></returns>
        public DataTable getDataTable(string sql, int PageSize, out int PageCount)
        {
            int RecCount = 0;
            return getDataTable(sql, PageSize, out RecCount, out PageCount);
        }

        /// <summary>
        /// 获取查询数据
        /// </summary>
        /// <param name="sql">查询SQL</param>
        /// <param name="PageSize">分页大小</param>
        /// <param name="RecCount">返回查询记录数</param>
        /// <param name="PageCount">返回总页数</param>
        /// <param name="TableName">表名</param>
        /// <returns>查询结果</returns>
        public DataTable getDataTable(string sql, int PageSize, out int RecCount, out int PageCount, string TableName = "")
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                OleDbConnection conn = (OleDbConnection)GetConnection();
                OleDbDataAdapter da = new OleDbDataAdapter();
                da.SelectCommand = new OleDbCommand();
                da.SelectCommand.Connection = conn;
                da.SelectCommand.CommandTimeout = this.Timeout * 1000;
                da.SelectCommand.CommandText = sql;
                if (string.IsNullOrEmpty(TableName))
                    da.Fill(ds);
                else
                    da.Fill(ds, TableName);
                if (ds.Tables.Count > 0)
                {
                    RecCount = ds.Tables[0].Rows.Count;
                    dt = ds.Tables[0];
                }
                else
                {
                    RecCount = 0;
                    dt = null;
                }
                CloseConnection(conn);
                if (PageSize == 0)
                    PageCount = RecCount;
                else if (RecCount % PageSize == 0)
                    PageCount = RecCount / PageSize;
                else
                    PageCount = (RecCount / PageSize) + 1;
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 判断是否存在记录
        /// </summary>
        /// <param name="sql">查询数据库</param>
        /// <returns>Boolean,TRUE为存在，False为不存在</returns>
        public bool IsExist(string sql)
        {
            try
            {
                OleDbConnection conn = (OleDbConnection)GetConnection();
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.CommandTimeout = this.Timeout * 1000;
                OleDbDataReader da = cmd.ExecuteReader();
                bool res = da.Read();
                da.Close();
                CloseConnection(conn);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查询数据记录
        /// </summary>
        /// <param name="sql">查询SQL</param>
        /// <returns>返回第一行查询数据</returns>
        public Dictionary<string, object> Find(string sql)
        {
            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                OleDbConnection conn = (OleDbConnection)GetConnection();
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.CommandTimeout = this.Timeout * 1000;
                OleDbDataReader da = cmd.ExecuteReader();
                if (da.Read())
                {
                    if (da.FieldCount > 0)
                    {
                        for (int i = 0; i < da.FieldCount; i++)
                        {
                            if (da.IsDBNull(i))
                                res.Add(da.GetName(i), null);
                            else
                                res.Add(da.GetName(i), da[i]);
                        }
                    }
                }
                da.Close();
                CloseConnection(conn);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="sql">查询SQL</param>
        /// <returns>返回第一行第一列数据</returns>
        public object ExecuteScalar(string sql)
        {
            try
            {
                OleDbConnection conn = (OleDbConnection)GetConnection();
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.CommandTimeout = this.Timeout * 1000;
                object res = null;
                try
                {
                    res = cmd.ExecuteScalar();
                    if (res is DBNull)
                        res = null;
                }
                catch
                { }
                CloseConnection(conn);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查询表是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool TableIsExist(string TableName)
        {
            if (string.IsNullOrEmpty(TableName))
                return false;
            try
            {
                bool res = false;
                OleDbConnection conn = (OleDbConnection)GetConnection();
                OleDbCommand cmd = new OleDbCommand("select count(1) from " + TableName + "", conn);
                cmd.CommandTimeout = this.Timeout * 1000;
                CloseConnection(conn);
                return res;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// 利用内存分页因此适合数据小但SQL语句复杂的查询
        /// </summary>
        /// <param name="PageIndex">当前页</param>
        /// <param name="PageSize">分页大小</param>
        /// <param name="DisplayField">字段列，每个字段用,分开</param>
        /// <param name="TableName">表名，支持（） k 视图方式</param>
        /// <param name="Where">查询条件，不带关键字WHERE</param>
        /// <param name="OrderBy">排序语句，带order by</param>
        /// <param name="GroupBy">GROUP BY 字段，不带关键字GROUP BY</param>
        /// <param name="RecodeCount">返回总记录数</param>
        /// <param name="PageCount">返回总记录数</param>
        /// <returns>查询结果</returns>
        public DataTable getDataTableByRam(int PageIndex, int PageSize, string DisplayField, string TableName, string Where, string OrderBy, string GroupBy, out int RecodeCount, out int PageCount)
        {
            string Sql = "";
            string CountSql = "";
            if (!string.IsNullOrEmpty(Where))
            {
                if (!Where.ToLower().Trim().Contains("where"))
                    Where = " WHERE " + Where;
            }
            if (string.IsNullOrEmpty(DisplayField))
                DisplayField = "*";
            if (!string.IsNullOrEmpty(OrderBy))
            {
                if (!OrderBy.ToLower().Trim().Contains("order"))
                    OrderBy = " ORDER BY " + OrderBy;
            }
            if (!string.IsNullOrEmpty(GroupBy))
            {
                if (!GroupBy.ToLower().Trim().Contains("group"))
                    GroupBy = " GROUP BY " + GroupBy;
                CountSql = "select count(1) as num from ( select count(1) as xx from " + TableName + " " + Where + " " + GroupBy + " ) DERIVEDTBL";
            }
            else
                CountSql = "select count(1) as num from  " + TableName + " " + Where;
            Sql = "SELECT " + DisplayField + " FROM  " + TableName + " " + Where + " " + GroupBy + " " + OrderBy;

            DataTable dt = new DataTable();
            DataSet ds = new DataSet();

            try
            {
                OleDbConnection conn = (OleDbConnection)GetConnection();
                OleDbCommand cmd = new OleDbCommand(CountSql, conn);
                cmd.CommandTimeout = this.Timeout * 1000;
                RecodeCount = Convert.ToInt32(cmd.ExecuteScalar());
                if (PageSize == 0)
                    PageCount = RecodeCount;
                else if (RecodeCount % PageSize == 0)
                    PageCount = RecodeCount / PageSize;
                else
                    PageCount = (RecodeCount / PageSize) + 1;
                OleDbDataAdapter da = new OleDbDataAdapter();
                da.SelectCommand = new OleDbCommand();
                da.SelectCommand.Connection = conn;
                da.SelectCommand.CommandTimeout = this.Timeout * 1000;
                da.SelectCommand.CommandText = Sql;
                da.Fill(ds, (PageIndex - 1) * PageSize + 1, PageSize, TableName);
                if (ds.Tables.Count > 0)
                    dt = ds.Tables[0];
                else
                    dt = null;
                CloseConnection(conn);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 利用数据库分页因此适合数据小但SQL语句复杂的查询
        /// </summary>
        /// <param name="PageIndex">当前页</param>
        /// <param name="PageSize">分页大小</param>
        /// <param name="DisplayField">字段列，每个字段用,分开</param>
        /// <param name="TableName">表名，支持（） k 视图方式</param>
        /// <param name="Where">查询条件，不带关键字WHERE</param>
        /// <param name="OrderBy">排序语句，带order by</param>
        /// <param name="OrderField">排序字段</param>
        /// <param name="GroupBy">GROUP BY 字段，不带关键字GROUP BY</param>
        /// <param name="RecodeCount">返回总记录数</param>
        /// <param name="PageCount">返回总记录数</param>
        /// <returns>查询结果</returns>
        public DataTable getDataTableByDB(int PageIndex, int PageSize, string DisplayField, string TableName, string Where, string OrderField, string OrderBy, string GroupBy, out int RecodeCount, out int PageCount)
        {
            string Sql = "";
            string CountSql = "";
            if (!string.IsNullOrEmpty(Where))
            {
                if (!Where.ToLower().Trim().Contains("where"))
                    Where = " WHERE " + Where;
            }
            if (string.IsNullOrEmpty(DisplayField))
                DisplayField = "*";
            if (!string.IsNullOrEmpty(OrderBy))
            {
                if (!OrderBy.ToLower().Trim().Contains("order"))
                    OrderBy = " ORDER BY " + OrderBy;
            }
            if (!string.IsNullOrEmpty(GroupBy))
            {
                if (!GroupBy.ToLower().Trim().Contains("group"))
                    GroupBy = " GROUP BY " + GroupBy;
                CountSql = "select count(1) as num from ( select count(1) as xx from " + TableName + " " + Where + " " + GroupBy + " ) DERIVEDTBL";
            }
            else
                CountSql = "select count(1) as num from  " + TableName + " " + Where;
            Sql = "select top " + PageSize.ToString() + " * from " + TableName + "  where " + OrderField + " not in(select top " + ((PageIndex - 1) * PageSize).ToString() + " " + OrderField + " from " + TableName + "  " + OrderBy + ") " + OrderBy + "";

            DataTable dt = new DataTable();
            DataSet ds = new DataSet();

            try
            {
                OleDbConnection conn = (OleDbConnection)GetConnection();
                OleDbCommand cmd = new OleDbCommand(CountSql, conn);
                cmd.CommandTimeout = this.Timeout * 1000;
                RecodeCount = Convert.ToInt32(cmd.ExecuteScalar());
                if (PageSize == 0)
                    PageCount = RecodeCount;
                else if (RecodeCount % PageSize == 0)
                    PageCount = RecodeCount / PageSize;
                else
                    PageCount = (RecodeCount / PageSize) + 1;
                OleDbDataAdapter da = new OleDbDataAdapter();
                da.SelectCommand = new OleDbCommand();
                da.SelectCommand.Connection = conn;
                da.SelectCommand.CommandTimeout = this.Timeout * 1000;
                da.SelectCommand.CommandText = Sql;
                da.Fill(ds, 0, PageSize, TableName);
                if (ds.Tables.Count > 0)
                    dt = ds.Tables[0];
                else
                    dt = null;
                CloseConnection(conn);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

     

        /// <summary>
        /// 查询字段属性
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="Field">字段名</param>
        /// <param name="FieldType">字段类型</param>
        /// <returns>true 字段存在，false 字段不存在</returns>
        public bool CheckField(string TableName, string Field, out System.Type FieldType, out bool CanBeNull, out bool IsPrimaryKey)
        {
            FieldType = typeof(object);
            CanBeNull = false;
            IsPrimaryKey = false;
            if (string.IsNullOrEmpty(TableName))
            {  throw new NullReferenceException("TableName Is  Null Or Empty");
                
            }
            else if (string.IsNullOrEmpty(Field))
            {
                throw new NullReferenceException("Field Is  Null Or Empty");
               
            }
            try
            {
                OleDbConnection conn = (OleDbConnection)GetConnection();
                bool res = false;
                DataTable dt = conn.GetSchema("Columns", new string[] { null, null, TableName });
                int m = dt.Columns.IndexOf("COLUMN_NAME");
                int n = dt.Columns.IndexOf("ISNULLABLE");
                int o = dt.Columns.IndexOf("COLUMN_KEY");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    if (dr.ItemArray.GetValue(m).ToString().ToUpper() == Field.ToUpper())
                    {
                        CanBeNull = dr.ItemArray.GetValue(n).ToString().ToUpper().Contains("YES");
                        IsPrimaryKey = dr.ItemArray.GetValue(o).ToString().ToUpper().Contains("PRI");
                        res = true;
                        break;
                    }
                }
                if (res)
                {
                    OleDbCommand cmd = new OleDbCommand("select " + Field + " from " + TableName + "", conn);
                    cmd.CommandTimeout = this.Timeout * 1000;
                    OleDbDataReader da = cmd.ExecuteReader();
                    FieldType = da.GetFieldType(0);
                }
                CloseConnection(conn);
                return res;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取数据库所有表
        /// </summary>
        public List<string> Tables
        {
            get
            {
                List<string> res = new List<string>();
                try
                {
                    OleDbConnection conn = (OleDbConnection)GetConnection();
                    DataTable dt = conn.GetSchema("Tables");
                    int m = dt.Columns.IndexOf("TABLE_NAME");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        res.Add(dr.ItemArray.GetValue(m).ToString());
                    }
                    CloseConnection(conn);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return res;
            }
        }

        /// <summary>
        /// 获取所有视图
        /// </summary>
        public List<string> Views
        {
            get
            {
                List<string> res = new List<string>();
                try
                {
                    OleDbConnection conn = (OleDbConnection)GetConnection();
                    DataTable dt = conn.GetSchema("Views");
                    int m = dt.Columns.IndexOf("TABLE_NAME");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        res.Add(dr.ItemArray.GetValue(m).ToString());
                    }
                    CloseConnection(conn);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return res;
            }
        }

        /// <summary>
        /// 表结构
        /// </summary>
        /// <param name="TableName">表</param>
        /// <returns>字段</returns>
        public List<string> Columns(string TableName)
        {
            List<string> res = new List<string>();
            if (string.IsNullOrEmpty(TableName))
            {
                 throw new NullReferenceException("TableName Is  Null Or Empty");
                
            }
            else
            {
                try
                {
                    OleDbConnection conn = (OleDbConnection)GetConnection();
                    DataTable dt = conn.GetSchema("Columns", new string[] { null, null, TableName });
                    int m = dt.Columns.IndexOf("COLUMN_NAME");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        res.Add(dr.ItemArray.GetValue(m).ToString());
                    }
                    CloseConnection(conn);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return res;
            }
        }

        #endregion

        #region 事务

        public void Transaction()
        {
            try
            {
                STConn = new OleDbConnection(this.Connection);
                STConn.Open();
                ST = STConn.BeginTransaction();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 执行事务并完成事务，当出错后自动回滚。再次执行需要执行Transaction
        /// </summary>
        public void SaveChange(bool Rollback = true)
        {
            try
            {
                if (ST != null)
                {
                    try
                    { ST.Commit(); }
                    catch (Exception ex)
                    {
                        if (Rollback)
                            ST.Rollback();
                          throw ex;
                    }
                    finally
                    { ST.Dispose(); }
                    ST = null;
                }
                if (STConn != null)
                {
                    try
                    {
                        if (STConn.State != ConnectionState.Closed)
                            STConn.Close();
                        STConn.Dispose();
                    }
                    catch
                    { }
                    STConn = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 回滚后完成事务。再次执行需要执行Transaction
        /// </summary>
        public void Cancel()
        {
            try
            {
                if (ST != null)
                {
                    try
                    { ST.Rollback(); }
                    catch
                    { }
                    finally
                    { ST.Dispose(); }
                    ST = null;
                }
                if (STConn != null)
                {
                    try
                    {
                        if (STConn.State != ConnectionState.Closed)
                            STConn.Close();
                        STConn.Dispose();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    STConn = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 事务SQL
        /// </summary>
        /// <param name="sql">SQL语句</param>
        public void TExecuteNonQuery(string sql)
        {
            try
            {
                if (STConn != null)
                {
                    OleDbCommand cmd = STConn.CreateCommand();
                    cmd.Transaction = ST;
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = this.Timeout * 1000;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


    }
}
