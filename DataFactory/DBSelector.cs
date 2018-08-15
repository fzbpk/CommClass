using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataDriver;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Reflection;
using System.ComponentModel;
using LinqToDB;
using LinqToDB.Mapping;

namespace DataFactory
{
    /// <summary>
    /// 实体转T-SQL
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DBSelector<T> : IDisposable where T : class,
                  new()
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
       public DBSelector()
       { }

       /// <summary>
       /// 数据库T-SQL基本操作
       /// </summary>
       /// <param name="ConnectionType">数据库类型</param>
       /// <param name="ConnectionString">连接串</param>
       /// <param name="Timeout">超时时间，毫秒</param>
       public DBSelector(DBType ConnectionType,string ConnectionString,int Timeout=60)
       {
           connstr = ConnectionString;
           DB = ConnectionType;
           Tout = Timeout;
       }

       /// <summary>
       /// 释放资源
       /// </summary>
       ~DBSelector()
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
       /// 插入实体
       /// </summary>
       /// <param name="entity">实体</param>
       [DisplayName("Insert")]
       [Description("插入实体")]
       public virtual bool Insert(T entity)
       {
           TableAttribute[] TableAttributes = (TableAttribute[])entity.GetType().GetCustomAttributes(typeof(TableAttribute), false);
           string TableName = (TableAttributes == null ? "" : (TableAttributes.Length > 0 ? TableAttributes.ToList().First().Name.Trim() : ""));
           if (TableName.Trim() == "")
               throw new NullReferenceException("实体没有Table属性");
           string str = "insert into " + TableName + " (";
           string cols = "";
           string vals = "";
           //获得所有property的信息
           PropertyInfo[] properties = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
           //遍历每个property
           foreach (PropertyInfo p in properties)
           {
               if (p != null)
               {
                   Type PT = p.PropertyType;
                   ColumnAttribute[] ColumnAttributes = (ColumnAttribute[])p.GetCustomAttributes(typeof(ColumnAttribute), false);
                   bool Auto_Increment = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => c.IsPrimaryKey) != null);
                   bool CannotNull = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => !c.CanBeNull) != null);
                   string ColumnName = (ColumnAttributes == null ? "" : (ColumnAttributes.Length > 0 ? (ColumnAttributes.ToList().First().Name == null ? "" : ColumnAttributes.ToList().First().Name.Trim()) : ""));
                   Type t = p.PropertyType;
                   if (Auto_Increment)
                       continue;
                   if(string.IsNullOrEmpty (ColumnName))
                        continue;
                    //加入object，Binary，和XDocument， 支持sql_variant，imager 和xml等的影射。
                    if (PT.IsEnum)
                   {
                       string ss = p.GetValue(entity, null).ToString ().ToUpper ().Trim ();
                       FieldInfo[] fields = PT.GetFields();
                       if (fields != null)
                       {
                           if (fields.Length > 0)
                           {
                               foreach (var field in fields)
                                {
                                      string EnumName= field.Name .ToUpper ().Trim ();
                                      if(ss==EnumName)
                                      {
                                          cols += "" + ColumnName + ",";
                                          vals += field.GetRawConstantValue() + ",";
                                          break;
                     
                                      }
                                }
                           }
                       }
                       
                   }
                   else if (t == typeof(bool) && ColumnName.Trim() != "")
                   {
                       if (p.GetValue(entity, null) != null)
                       {
                           switch (DB)
                           { 
                               case DBType.MYSQL:
                                    cols += "" + ColumnName + ",";
                                    vals += (p.GetValue(entity, null)==null ?"0":( Convert .ToBoolean(p.GetValue(entity, null))?"1":"0"))+  ",";
                                   break;
                               case DBType.MSSQL:
                                   cols += "" + ColumnName + ",";
                                    vals +="'"+ p.GetValue(entity, null)+"',";
                                   break;
                               default :
                                    cols += "" + ColumnName + ",";
                                    vals += (p.GetValue(entity, null)==null ?"0":( Convert .ToBoolean(p.GetValue(entity, null))?"1":"0"))+  ",";
                                   break;
                           }
                       }
                       else if (!CannotNull)
                       {
                           cols += "" + ColumnName + ",";
                           vals += "null,";
                       }
                   }
                   else if ((t.IsValueType
                     || t == typeof(string) || t == typeof(System.Byte[])
                     || t == typeof(object)
                     ) && ColumnName.Trim() != "")
                   {
                       if (p.GetValue(entity, null) != null)
                       {
                           cols += "" + ColumnName + ",";
                           if (t == typeof(System.Byte[]) || PT.IsEnum
                               || t == typeof(short) || t == typeof(int) || t == typeof(long)
                               || t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong)
                               || t == typeof(double) || t == typeof(float) || t == typeof(decimal)
                               )
                               vals += p.GetValue(entity, null) + ",";
                           else
                               vals += "'" + p.GetValue(entity, null) + "',";
                       }
                       else if (!CannotNull)
                       {
                           cols += "" + ColumnName + ",";
                           vals += "null,";
                       }
                   }
                   
               }
           }
           str += cols.Substring(0, cols.Length - 1) + ") values (" + vals.Substring(0, vals.Length - 1) + ")";
           bool res = false;
           IDataBase DBOper = MakeConnection();
           if (DBOper!=null)
               res = DBOper.ExecuteNonQuery(str.ToString()) > 0;
           return res;

       }

       /// <summary>
       /// 更新实体
       /// </summary>
       /// <param name="entity">实体</param>
       [DisplayName("Update")]
       [Description("更新实体")]
       public virtual bool Update(T entity)
       {
           TableAttribute[] TableAttributes = (TableAttribute[])entity.GetType().GetCustomAttributes(typeof(TableAttribute), false);
           string TableName = (TableAttributes == null ? "" : (TableAttributes.Length > 0 ? TableAttributes.ToList().First().Name.Trim() : ""));
           if (TableName.Trim() == "")
               throw new NullReferenceException("实体没有Table属性");
           string str = "update " + TableName + "  set ";
           string cols = "";
           string vals = "";
           bool IsPrimaryKey = false;
           //获得所有property的信息
           PropertyInfo[] properties = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
           //遍历每个property
           foreach (PropertyInfo p in properties)
           {
               if (p != null)
               {
                   Type PT = p.PropertyType;
                   ColumnAttribute[] ColumnAttributes = (ColumnAttribute[])p.GetCustomAttributes(typeof(ColumnAttribute), false);
                   bool Auto_Increment = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => c.IsPrimaryKey) != null);
                   string ColumnName = (ColumnAttributes == null ? "" : (ColumnAttributes.Length > 0 ? ColumnAttributes.ToList().First().Name.Trim() : ""));
                   bool CannotNull = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => !c.CanBeNull) != null);
                   Type t = p.PropertyType;

                   if (Auto_Increment)
                   {
                       vals = ""  + ColumnName + "=" + p.GetValue(entity, null) + " ";
                       continue;
                   }
                   if (PT.IsEnum)
                   {
                       string ss = p.GetValue(entity, null).ToString().ToUpper().Trim();
                       FieldInfo[] fields = PT.GetFields();
                       if (fields != null)
                       {
                           if (fields.Length > 0)
                           {
                               foreach (var field in fields)
                               {
                                   string EnumName = field.Name.ToUpper().Trim();
                                   if (ss == EnumName)
                                   {
                                       cols += "" + ColumnName + "=" + field.GetRawConstantValue() + ",";
                                       break;

                                   }
                               }
                           }
                       }

                   }
                   else if (t == typeof(bool) && ColumnName.Trim() != "")
                   {
                       if (p.GetValue(entity, null) != null)
                       {
                           switch (DB)
                           {
                               case DBType.MYSQL:
                                   cols += ColumnName + "=" + (p.GetValue(entity, null) == null ? "0" : (Convert.ToBoolean(p.GetValue(entity, null)) ? "1" : "0")) + ",";
                                   break;
                               case DBType.MSSQL:
                                   cols += ColumnName + "='" + p.GetValue(entity, null) + "',";
                                   break;
                               default:
                                   cols += ColumnName +"="+ (p.GetValue(entity, null) == null ? "0" : (Convert.ToBoolean(p.GetValue(entity, null)) ? "1" : "0")) + ",";
                                   break;
                           }
                       }
                       else if (!CannotNull)
                       {
                           cols += "" + ColumnName + ",";
                           vals += "null,";
                       }
                   }
                   else if ((t.IsValueType
                     || t == typeof(string) || t == typeof(System.Byte[])
                     || t == typeof(object)  
                     ) && ColumnName.Trim() != "")
                   {
                       if (p.GetValue(entity, null) != null)
                       {
                           if (t == typeof(System.Byte[])
                               || t == typeof(short) || t == typeof(int) || t == typeof(long)
                               || t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong)
                               || t == typeof(double) || t == typeof(float) || t == typeof(decimal)
                               )
                           {
                               cols += "" + ColumnName + "=" + p.GetValue(entity, null) + ",";
                           }
                           else
                           {
                               cols += "" + ColumnName + "='" + p.GetValue(entity, null) + "',";
                           }
                       }
                       else if (!CannotNull)
                           cols += "" + ColumnName + "=null,";
                   }

               }
           }
           if (vals.Trim() == "")
               throw new NullReferenceException("没有主键");
           str += cols.Substring(0, cols.Length - 1) + " where " + vals;
           bool res = false;
           IDataBase DBOper = MakeConnection();
           if (DBOper != null)
               res = DBOper.ExecuteNonQuery(str.ToString()) > 0;
           return res;
       }

       /// <summary>
       /// 删除实体
       /// </summary>
       /// <param name="entity"></param>
       [DisplayName("Delete")]
       [Description("删除实体")]
       public virtual bool Delete(T entity)
       {
           TableAttribute[] TableAttributes = (TableAttribute[])entity.GetType().GetCustomAttributes(typeof(TableAttribute), false);
           string TableName = (TableAttributes == null ? "" : (TableAttributes.Length > 0 ? TableAttributes.ToList().First().Name.Trim() : ""));
           if (TableName.Trim() == "")
               throw new NullReferenceException("实体没有Table属性");
           string str = "delete from " + TableName + " ";
           string cols = "";
           string vals = "";
           //获得所有property的信息
           PropertyInfo[] properties = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
           //遍历每个property
           foreach (PropertyInfo p in properties)
           {
               if (p != null)
               {
                   Type PT = p.PropertyType;
                   ColumnAttribute[] ColumnAttributes = (ColumnAttribute[])p.GetCustomAttributes(typeof(ColumnAttribute), false);
                   bool IsPrimaryKey = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => c.IsPrimaryKey) != null);
                   if (!IsPrimaryKey)
                       continue;
                   string ColumnName = (ColumnAttributes == null ? "" : (ColumnAttributes.Length > 0 ? ColumnAttributes.ToList().First().Name.Trim() : ""));
                   Type t = p.PropertyType;
                   //加入object，Binary，和XDocument， 支持sql_variant，imager 和xml等的影射。
                   if ((t.IsValueType
                     || t == typeof(string) || t == typeof(System.Byte[])
                     || t == typeof(object) 
                     ) && ColumnName.Trim() != "")
                   {
                       if (p.GetValue(entity, null) != null)
                       {
                           if (t == typeof(System.Byte[])
                               || t == typeof(short) || t == typeof(int) || t == typeof(long)
                               || t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong)
                               || t == typeof(double) || t == typeof(float) || t == typeof(decimal)
                               )
                               vals = "" + ColumnName + "=" + p.GetValue(entity, null);
                           else
                               vals = "" + ColumnName + "='" + p.GetValue(entity, null) + "'";
                       }
                   }
               }
           }
           if (vals.Trim() == "")
               throw new NullReferenceException("没有主键");
           str += cols + " where " + vals;
           bool res = false;
           IDataBase DBOper = MakeConnection();
           if (DBOper != null)
               res = DBOper.ExecuteNonQuery(str.ToString()) > 0;
           return res;
       }
       
       /// <summary>
       /// 查询所有结果
       /// </summary>
       /// <returns>所有结果</returns>
       [DisplayName("List")]
       [Description("查询所有实体")]
       public IList<T> List()
       {
           T org = new T();
           IList<T> entities = new List<T>();
           TableAttribute[] TableAttributes = (TableAttribute[])org.GetType().GetCustomAttributes(typeof(TableAttribute), false);
           string TableName = (TableAttributes == null ? "" : (TableAttributes.Length > 0 ? TableAttributes.ToList().First().Name.Trim() : ""));
           if (TableName.Trim() == "")
               throw new NullReferenceException("实体没有Table属性");
           DataTable DT = null;
           IDataBase DBOper = MakeConnection();
           if (DBOper!=null)
               DT = DBOper.getDataTable("select * from " + TableName);
           if (DT != null)
           {
               foreach (DataRow row in DT.Rows)
               {
                   T entity = new T();
                   foreach (var item in entity.GetType().GetProperties())
                   {
                       Type PT = item.PropertyType;
                       ColumnAttribute[] ColumnAttributes = (ColumnAttribute[])item.GetCustomAttributes(typeof(ColumnAttribute), false);
                       string ColumnName = (ColumnAttributes == null ? "" : (ColumnAttributes.Length > 0 ? ColumnAttributes.ToList().First().Name.Trim() : ""));
                       if (ColumnName.Trim() != "")
                       {
                           if (PT.IsEnum)
                           {
                               if (row[ColumnName] is DBNull)
                                   continue;
                               else
                               {
                                   int val = int.Parse(row[ColumnName].ToString());
                                   var em = Enum.ToObject(PT, val);
                                   item.SetValue(entity, em, null);
                               }

                           }
                           else if (row[ColumnName] is DBNull)
                               item.SetValue(entity, null, null);
                           else
                               item.SetValue(entity, Convert.ChangeType(row[ColumnName], item.PropertyType), null);
                       }

                   }
                   entities.Add(entity);
               }
           }
           return entities;
       }

        /// <summary>
        /// 查询并分页
        /// </summary>
        /// <param name="PageIndex">当前页</param>
        /// <param name="PageSize">分页大小</param>
        /// <param name="PageCount">总页数</param>
        /// <param name="RecordCount">总记录数</param>
        /// <returns>结果</returns>
       [DisplayName("Page")]
       [Description("分页查询所有实体")]
       public IList<T> Page(int PageIndex,int PageSize,out int PageCount,out int RecordCount)
       {
           PageCount = 1;
           RecordCount = 0;
           T org = new T();
           IList<T> entities = new List<T>();
           TableAttribute[] TableAttributes = (TableAttribute[])org.GetType().GetCustomAttributes(typeof(TableAttribute), false);
           string TableName = (TableAttributes == null ? "" : (TableAttributes.Length > 0 ? TableAttributes.ToList().First().Name.Trim() : ""));
           if (TableName.Trim() == "")
               throw new NullReferenceException("实体没有Table属性");
           PropertyInfo[] properties = org.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
           string OrderID = "";
           foreach (PropertyInfo p in properties)
           {
               if (p != null)
               {
                   ColumnAttribute[] ColumnAttributes = (ColumnAttribute[])p.GetCustomAttributes(typeof(ColumnAttribute), false);
                   string ColumnName = (ColumnAttributes == null ? "" : (ColumnAttributes.Length > 0 ? ColumnAttributes.ToList().First().Name.Trim() : ""));
                   bool IsPrimaryKey = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => c.IsPrimaryKey) != null);
                   if (IsPrimaryKey && ColumnName.Trim() != "")
                   {
                       OrderID =   ColumnName  ;
                       break;
                   }
               }
           }
           DataTable DT = null;
           IDataBase DBOper = MakeConnection();
           if (DBOper != null)
               DT = DBOper.getDataTableByRam(PageIndex, PageSize, "", TableName, "", "", "", out RecordCount, out PageCount);
           if (DT != null)
           {
               foreach (DataRow row in DT.Rows)
               {
                   T entity = new T();
                   foreach (var item in entity.GetType().GetProperties())
                   {
                       Type PT = item.PropertyType;
                       ColumnAttribute[] ColumnAttributes = (ColumnAttribute[])item.GetCustomAttributes(typeof(ColumnAttribute), false);
                       string ColumnName = (ColumnAttributes == null ? "" : (ColumnAttributes.Length > 0 ? ColumnAttributes.ToList().First().Name.Trim() : ""));
                       if (ColumnName.Trim() != "")
                       {
                           if (PT.IsEnum)
                           {
                               if (row[ColumnName] is DBNull)
                                   continue;
                               else
                               {
                                   int val = int.Parse(row[ColumnName].ToString());
                                   var em = Enum.ToObject(PT, val);
                                   item.SetValue(org, em, null);
                               }

                           }
                           else if (row[ColumnName] is DBNull)
                               item.SetValue(entity, null, null);
                           else
                               item.SetValue(entity, Convert.ChangeType(row[ColumnName], item.PropertyType), null);
                       }

                   }
                   entities.Add(entity);
               }
           }
           return entities;
       }

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="ID">主键键值</param>
        /// <returns>找到为实体，找不到为NULL</returns>
       public T Find(int ID)
       {
           T org = new T();
           TableAttribute[] TableAttributes = (TableAttribute[])org.GetType().GetCustomAttributes(typeof(TableAttribute), false);
           string TableName = (TableAttributes == null ? "" : (TableAttributes.Length > 0 ? TableAttributes.ToList().First().Name.Trim() : ""));
           if (TableName.Trim() == "")
               throw new NullReferenceException("实体没有Table属性");
           PropertyInfo[] properties = org.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
           string where = "";
           foreach (PropertyInfo p in properties)
           {
               if (p != null)
               {

                   
                   ColumnAttribute[] ColumnAttributes = (ColumnAttribute[])p.GetCustomAttributes(typeof(ColumnAttribute), false);
                   string ColumnName = (ColumnAttributes == null ? "" : (ColumnAttributes.Length > 0 ? ColumnAttributes.ToList().First().Name.Trim() : ""));
                   bool IsPrimaryKey = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => c.IsPrimaryKey) != null);
                   if (IsPrimaryKey && ColumnName.Trim() != "")
                   { 
                       where = " where " + ColumnName + "=" + ID.ToString();
                       break;
                   }
               }
           }
           if (where.Trim() == "")
               throw new NullReferenceException("实体主键不存在");
           DataTable DT = null;
           IDataBase DBOper = MakeConnection();
           if (DBOper != null)
               DT = DBOper.getDataTable("select * from " + TableName + " " + where);
           if (DT != null)
           {
               if (DT.Rows.Count > 0)
               {
                   DataRow row=DT.Rows[0];
                   foreach (var item in org.GetType().GetProperties())
                   {
                       Type PT = item.PropertyType;
                       ColumnAttribute[] ColumnAttributes = (ColumnAttribute[])item.GetCustomAttributes(typeof(ColumnAttribute), false);
                       string ColumnName = (ColumnAttributes == null ? "" : (ColumnAttributes.Length > 0 ? ColumnAttributes.ToList().First().Name.Trim() : ""));
                       if (ColumnName.Trim() != "")
                       {
                           if (PT.IsEnum)
                           {
                               if (row[ColumnName] is DBNull)
                                   continue;
                               else
                               {
                                   int val = int.Parse(row[ColumnName].ToString());
                                   var em = Enum.ToObject(PT, val);
                                   item.SetValue(org, em, null);
                               }
                              
                           }
                           else if (row[ColumnName] is DBNull)
                               item.SetValue(org, null, null);
                           else
                               item.SetValue(org, Convert.ChangeType(row[ColumnName], item.PropertyType), null);
                       }
                   }
                   return org;
               }
              
           }
           return null;
       }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <returns>执行结果</returns>
       public bool CreatTable()
       {
           T org = new T();
           TableAttribute[] TableAttributes = (TableAttribute[])org.GetType().GetCustomAttributes(typeof(TableAttribute), false);
           string TableName = (TableAttributes == null ? "" : (TableAttributes.Length > 0 ? TableAttributes.ToList().First().Name.Trim() : ""));
           if (TableName.Trim() == "")
               return false;
           string sql = "";
           string Column = "";
           string PKColumn = "";
           PropertyInfo[] properties = null;
           IDataBase DBOper = null;
           switch (DB)
           {
               case  DBType.Access:
                   sql = "Create Table " + TableName + " (";
                   Column = "";
                   properties = org.GetType().GetProperties(BindingFlags.Public| BindingFlags.Instance);
                   foreach (PropertyInfo p in properties)
                   {
                       if (p != null)
                       {
                           Type PT = p.PropertyType;
                          
                           ColumnAttribute[] ColumnAttributes = (ColumnAttribute[])p.GetCustomAttributes(typeof(ColumnAttribute), false);
                           string ColumnName = (ColumnAttributes == null ? "" : (ColumnAttributes.Length > 0 ? ColumnAttributes.ToList().First().Name.Trim() : ""));
                           bool IsPrimaryKey = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => c.IsPrimaryKey) != null);
                           Type t = p.PropertyType;
                           if (ColumnName.Trim() != "")
                           {
                               Column += ",[" + ColumnName + "]";
                               if (IsPrimaryKey)
                                   Column += " Counter primary key ";
                               else if (t == typeof(string))
                                   Column += " string(255) ";
                               else if (t == typeof(char))
                                   Column += " Char  ";
                               else if (t == typeof(DateTime))
                                   Column += " Time  ";
                               else if (t == typeof(bool) || t == typeof(short) || t == typeof(int) || t == typeof(long)
                                      || t == typeof(byte) || t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong))
                               {
                                   Column += " integer ";
                               }
                               else if (t == typeof(double) || t == typeof(float) || t == typeof(decimal))
                               {
                                   Column += " double ";
                               }
                               else if (PT.IsEnum)
                               {
                                   Column += " int(11) ";
                               }
                           }
                       }
                   }
                   if (Column.StartsWith(","))
                       Column = Column.Substring(1, Column.Length - 1);
                   sql += Column + " )";
                   DBOper = new Access(connstr, Tout);
                   DBOper.ExecuteNonQuery(sql);
                    return DBOper.TableIsExist(TableName);

               case DBType.MYSQL :
                   sql = "Create Table " + TableName + " (";
                   Column = "";
                   PKColumn = "";
                   properties = org.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                   foreach (PropertyInfo p in properties)
                   {
                       if (p != null)
                       {
                           Type PT = p.PropertyType ;
                           ColumnAttribute[] ColumnAttributes = (ColumnAttribute[])p.GetCustomAttributes(typeof(ColumnAttribute), false);
                           string ColumnName = (ColumnAttributes == null ? "" : (ColumnAttributes.Length > 0 ? ColumnAttributes.ToList().First().Name.Trim() : ""));
                           bool IsPrimaryKey = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => c.IsPrimaryKey) != null);
                           bool CanBeNull = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => c.CanBeNull) != null);
                           Type t = p.PropertyType;
                           if (ColumnName.Trim() != "")
                           {
                               Column += ",`" + ColumnName + "`";
                               if (IsPrimaryKey)
                               {
                                   Column += " int(11) NOT NULL auto_increment ";
                                   PKColumn = ",PRIMARY KEY  (`" + ColumnName + "`)";
                               }
                               else if (t == typeof(string))
                                   Column += " varchar(255) " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                               else if (t == typeof(char))
                                   Column += " varchar(50) " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                               else if (t == typeof(bool))
                                   Column += " bit " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                               else if (t == typeof(DateTime))
                                   Column += " datetime " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                               else if (  t == typeof(short) || t == typeof(int) || t == typeof(long)
                                      || t == typeof(byte) || t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong))
                               {
                                   Column += " int(11) " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                               }
                               else if (t == typeof(double) || t == typeof(float) || t == typeof(decimal))
                               {
                                   Column += " float(12,4) " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                               }
                               else if (PT.IsEnum)
                               {
                                   Column += " int(11) " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                               }
                           }
                       }
                   }
                   if (Column.StartsWith(","))
                       Column = Column.Substring(1, Column.Length - 1);
                   sql += Column + PKColumn + " )";
                   DBOper = new DataDriver.MySql(connstr, Tout);
                   DBOper.ExecuteNonQuery(sql);
                    return DBOper.TableIsExist(TableName);

               case DBType.MSSQL:
                    sql = "Create Table " + TableName + " (";
                    Column = "";
                    PKColumn = "";
                    properties = org.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo p in properties)
                    {
                        if (p != null)
                        {
                            Type PT = p.PropertyType;
                            ColumnAttribute[] ColumnAttributes = (ColumnAttribute[])p.GetCustomAttributes(typeof(ColumnAttribute), false);
                            string ColumnName = (ColumnAttributes == null ? "" : (ColumnAttributes.Length > 0 ? ColumnAttributes.ToList().First().Name.Trim() : ""));
                            bool IsPrimaryKey = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => c.IsPrimaryKey) != null);
                            bool CanBeNull = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => c.CanBeNull) != null);
                            Type t = p.PropertyType;
                            if (ColumnName.Trim() != "")
                            {
                                Column += ",[" + ColumnName + "]";
                                if (IsPrimaryKey)
                                {
                                    Column += " int not null identity(1,1) ";
                                    PKColumn = ",CONSTRAINT " + TableName + "_PK PRIMARY KEY (" + ColumnName + ")";
                                }
                                else if (t == typeof(string))
                                    Column += " varchar(255) " + (CanBeNull ? "  NULL " : "  NOT NULL ");
                                else if (t == typeof(char))
                                    Column += " varchar(50) " + (CanBeNull ? "  NULL " : "  NOT NULL ");
                                else if (t == typeof(DateTime))
                                    Column += " datetime " + (CanBeNull ? "  NULL " : "  NOT NULL ");
                                else if (t == typeof(bool))
                                    Column += " BIT " + (CanBeNull ? "  NULL " : "  NOT NULL ");
                                else if ( t == typeof(short) || t == typeof(int) || t == typeof(long)
                                    || t == typeof(byte) || t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong))
                                {
                                    Column += " int " + (CanBeNull ? "  NULL " : "  NOT NULL ");
                                }
                                else if (t == typeof(double) || t == typeof(float) || t == typeof(decimal))
                                {
                                    Column += " number(12,4) " + (CanBeNull ? "  NULL " : "  NOT NULL ");
                                }
                                else if (PT.IsEnum)
                                {
                                    Column += " int " + (CanBeNull ? "  NULL " : "  NOT NULL ");
                                }
                            }
                        }
                    }
                    if (Column.StartsWith(","))
                        Column = Column.Substring(1, Column.Length - 1);
                    sql += Column + PKColumn + " )";
                    DBOper = new DataDriver.MSSql(connstr, Tout);
                    DBOper.ExecuteNonQuery(sql);
                    return DBOper.TableIsExist(TableName);

               case DBType.Oracle:
                    sql = "Create Table " + TableName + " (";
                    Column = "";
                    PKColumn = "";
                    string PKTRIGGER = "";
                    properties = org.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo p in properties)
                    {
                        if (p != null)
                        {
                            Type PT = p.PropertyType;
                            ColumnAttribute[] ColumnAttributes = (ColumnAttribute[])p.GetCustomAttributes(typeof(ColumnAttribute), false);
                            string ColumnName = (ColumnAttributes == null ? "" : (ColumnAttributes.Length > 0 ? ColumnAttributes.ToList().First().Name.Trim() : ""));
                            bool IsPrimaryKey = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => c.IsPrimaryKey) != null);
                            bool CanBeNull = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => c.CanBeNull) != null);
                            Type t = p.PropertyType;
                            if (ColumnName.Trim() != "")
                            {
                                Column += ",`" + ColumnName + "`";
                                if (IsPrimaryKey)
                                {
                                    Column += " INT NOT NULL PRIMARY KEY ";
                                    PKColumn = "create sequence " + TableName + "_seq INCREMENT BY 1 START WITH 1 NOMAXVALUE NOCYCLE CACHE 10";
                                    PKTRIGGER = "CREATE OR REPLACE TRIGGER " + TableName + "_Increase BEFORE insert ON " + TableName + " "
                                              +" FOR EACH ROW "
                                              +"begin "
                                              + "select " + TableName + "_seq.nextval into:New." + Column + " from dual "
                                              +"end";
                                }
                                else if (t == typeof(string))
                                    Column += " VARCHAR(255) " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                                else if (t == typeof(char))
                                    Column += " VARCHAR(50) " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                                else if (t == typeof(bool))
                                    Column += " char(1) check (" + ColumnName + " in(0,1))";
                                else if (t == typeof(byte[]))
                                    Column += " blob ";
                                else if (t == typeof(DateTime))
                                    Column += " date " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                                else if (t == typeof(short) || t == typeof(int) || t == typeof(long)
                                       || t == typeof(byte) || t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong))
                                {
                                    Column += " INT " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                                }
                                else if (t == typeof(double) || t == typeof(float) || t == typeof(decimal))
                                {
                                    Column += " NUMBER(12,4) " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                                }
                                else if (PT.IsEnum)
                                {
                                    Column += " INT " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                                }
                            }
                        }
                    }
                    if (Column.StartsWith(","))
                        Column = Column.Substring(1, Column.Length - 1);
                    sql += Column  + " )";
                    DBOper = new DataDriver.Oracle(connstr, Tout);
                    if (!string.IsNullOrEmpty(PKColumn) && !string.IsNullOrEmpty(PKTRIGGER))
                    {
                        DBOper.ExecuteNonQuery(PKColumn);
                        DBOper.ExecuteNonQuery(sql);
                        DBOper.ExecuteNonQuery(PKTRIGGER);
                    }
                    else
                        DBOper.ExecuteNonQuery(sql);
                    return DBOper.TableIsExist(TableName);

                case DBType.SQLite:
                    sql = "Create Table " + TableName + " (";
                    Column = "";
                    PKColumn = "";
                    properties = org.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo p in properties)
                    {
                        if (p != null)
                        {
                            Type PT = p.PropertyType;
                            ColumnAttribute[] ColumnAttributes = (ColumnAttribute[])p.GetCustomAttributes(typeof(ColumnAttribute), false);
                            string ColumnName = (ColumnAttributes == null ? "" : (ColumnAttributes.Length > 0 ? ColumnAttributes.ToList().First().Name.Trim() : ""));
                            bool IsPrimaryKey = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => c.IsPrimaryKey) != null);
                            bool CanBeNull = (ColumnAttributes == null ? false : ColumnAttributes.ToList().Find(c => c.CanBeNull) != null);
                            Type t = p.PropertyType;
                            if (ColumnName.Trim() != "")
                            {
                                Column += "," + ColumnName + " ";
                                if (IsPrimaryKey)
                                {
                                    Column += " integer PRIMARY KEY autoincrement  "; 
                                }
                                else if (t == typeof(string))
                                    Column += " varchar(255) " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                                else if (t == typeof(char))
                                    Column += " varchar(50) " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                                
                                else if (t == typeof(DateTime))
                                    Column += " datetime " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                                else if (t == typeof(short) || t == typeof(int) || t == typeof(long)
                                       || t == typeof(byte) || t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong))
                                {
                                    Column += " int(11) " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                                }
                                else if (t == typeof(double) || t == typeof(float) || t == typeof(decimal))
                                {
                                    Column += " float(12,4) " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                                }
                                else if (PT.IsEnum)
                                {
                                    Column += " int(11) " + (CanBeNull ? " default NULL " : "  NOT NULL ");
                                }
                            }
                        }
                    }
                    if (Column.StartsWith(","))
                        Column = Column.Substring(1, Column.Length - 1);
                    sql += Column+ " )";
                    DBOper = new DataDriver.SQLite(connstr, Tout);
                    DBOper.ExecuteNonQuery(sql);
                    return DBOper.TableIsExist(TableName);

            }
           return false;
       }

        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <returns>是否存在</returns>
       public bool TableIsExist()
       {
           T org = new T();
           TableAttribute[] TableAttributes = (TableAttribute[])org.GetType().GetCustomAttributes(typeof(TableAttribute), false);
           string TableName = (TableAttributes == null ? "" : (TableAttributes.Length > 0 ? TableAttributes.ToList().First().Name.Trim() : ""));
           if (TableName.Trim() == "")
               return false;
           bool res = false;
           IDataBase DBOper = MakeConnection();
           if (DBOper != null)
               res = DBOper.TableIsExist(TableName);
           return res;
       }

        /// <summary>
        /// 检测数据库连接
        /// </summary>
        /// <param name="errmsg">错误信息</param>
        /// <returns>连接结果</returns>
       public bool CheckDataBase(out string errmsg)
       {
           errmsg = "";
           bool res = false;
           IDataBase DBOper = MakeConnection();
           if (DBOper != null)
               res = DBOper.CheckConnection(out errmsg);
           return res; 
       }

       #endregion

     
    }
}
