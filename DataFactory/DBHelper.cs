using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data ;
using System.Web.UI.WebControls;
using DataDriver;
namespace DataFactory
{
    /// <summary>
    /// T-SQL数据绑定到控件
    /// </summary>
    public class DBHelper : IDisposable
    {
       #region "定义"
        private string connstr = "";
        private  DBType  DB =  DBType.None ;
        private int Tout = 0;
        private bool m_disposed;
        #endregion

       #region "构造函数"

        /// <summary>
        /// T-SQL数据绑定到控件
        /// </summary>
       public DBHelper()
       { }

       /// <summary>
       /// T-SQL数据绑定到控件
       /// </summary>
       /// <param name="ConnectionType">数据库类型</param>
       /// <param name="ConnectionString">连接串</param>
       /// <param name="Timeout">超时时间，毫秒</param>
       public DBHelper(DBType ConnectionType,string ConnectionString,int Timeout=60)
       {
           connstr = ConnectionString;
           DB = ConnectionType;
           Tout = Timeout;
       }

       /// <summary>
       /// 释放资源
       /// </summary>
       ~DBHelper()
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

       public DBType DataBaseType
       {
           get { return DB; }
           set { DB = value ;  }
       }

       public string Connection
       {
           get { return connstr; }
           set { connstr = value; }
       }

       public int TimeOut
       {
           get { return Tout; }
           set { Tout = value; }
       }

       #endregion

        #region "方法"

       private DataTable getDataTable(string sql)
       {
           switch (DB)
           {
               case DBType.Access:
                   Access access = new Access(connstr, Tout);
                   return access.getDataTable(sql);
               case DBType.MYSQL:
                   DataDriver.MySql mysql = new DataDriver.MySql(connstr, Tout);
                   return mysql.getDataTable(sql);
               case DBType.MSSQL:
                   DataDriver.MSSql mssql = new DataDriver.MSSql(connstr, Tout);
                   return mssql.getDataTable(sql);
               case DBType.SQLite:
                   DataDriver.SQLite sqlite = new DataDriver.SQLite(connstr, Tout);
                   return sqlite.getDataTable(sql);
               case DBType.PostgreSQL:
                   DataDriver.PostgreSQL pgsql = new DataDriver.PostgreSQL(connstr, Tout);
                   return pgsql.getDataTable(sql);
               default:
                   return null;
           }
       }

       /// <summary>
       /// 绑定数据到CheckBoxList
       /// </summary>
       /// <param name="sql">T-SQL</param>
       /// <param name="ClassName">控件</param>
       /// <param name="TextField">显示列</param>
       /// <param name="ValueField">值列</param>
       public void bindCheckButtonList(string sql, CheckBoxList ClassName, string TextField, string ValueField, string defaultValue = "")
       {
           DataTable dt = getDataTable(sql);
           if (dt != null)
           {
               ClassName.DataSource = dt;
               ClassName.DataTextField = TextField;
               ClassName.DataValueField = ValueField;
               ClassName.DataBind();
           }
           if (string.IsNullOrEmpty(defaultValue))
               ClassName.Items.Insert(0, new ListItem("请选择", ""));
           else
           {
               try
               { ClassName.Items.FindByValue(defaultValue).Selected = true; }
               catch { }
           }
       }

       /// <summary>
       /// 绑定数据到HTML DropDownList
       /// </summary>
       /// <param name="sql">T-SQL</param>
       /// <param name="ClassName">控件</param>
       /// <param name="TextField">显示列</param>
       /// <param name="ValueField">值列</param>
       public string bindDropDownList(string sql, string ClassName, string TextField, string ValueField, string defaultValue = "", string CSS = "")
       {
           string HTML = "<select name=\"" + ClassName + "\" id=\"" + ClassName + "\" class=\"" + CSS + "\" >";
           DataTable dt = getDataTable(sql);
           if (dt != null)
           {
               for (int i = 0; i < dt.Rows.Count;i++ )
               {
                   if (dt.Rows[i][ValueField].ToString() == defaultValue)
                       HTML += "<option value=\"" + dt.Rows[i][ValueField].ToString() + "\" selected>" + dt.Rows[i][TextField].ToString() + "</option>";
                   else
                       HTML += "<option value=\"" + dt.Rows[i][ValueField].ToString() + "\" >" + dt.Rows[i][TextField].ToString() + "</option>";
               }
           }
           if (string.IsNullOrEmpty(defaultValue))
               HTML += "<option value='' selected>请选择</option>";
           HTML += "</select>";
           return HTML;
       }

       /// <summary>
       /// 绑定数据到RadioButtonList
       /// </summary>
       /// <param name="sql">T-SQL</param>
       /// <param name="ClassName">控件</param>
       /// <param name="TextField">显示列</param>
       /// <param name="ValueField">值列</param>
       public void bindRadioButtonList(string sql, RadioButtonList ClassName, string TextField, string ValueField, string defaultValue = "")
       {
           DataTable dt = getDataTable(sql);
           if (dt != null)
           {
               ClassName.DataSource = dt;
               ClassName.DataTextField = TextField;
               ClassName.DataValueField = ValueField;
               ClassName.DataBind();
           }
           if (string.IsNullOrEmpty(defaultValue))
               ClassName.Items.Insert(0, new ListItem("请选择", ""));
           else
           {
               try
               { ClassName.Items.FindByValue(defaultValue).Selected = true; }
               catch { }
           }
       }

       /// <summary>
       /// 绑定数据到HTML DropDownList
       /// </summary>
       /// <param name="sql">T-SQL</param>
       /// <param name="ClassName">控件</param>
       /// <param name="TextField">显示列</param>
       /// <param name="ValueField">值列</param>
       public string bindRadioButtonList(string sql, string ClassName, string TextField, string ValueField, string defaultValue = "", string CSS = "")
       {
           string HTML = "";
           DataTable dt = getDataTable(sql);
           if (dt != null)
           {
               bool sel = false;
               for (int i = 0; i < dt.Rows.Count; i++)
               {
                   if (dt.Rows[i][ValueField].ToString() == defaultValue)
                   {
                       sel = true;
                       HTML += " <input type=\"radio\" name=\"" + ClassName + "\" id=\"" + ClassName + "_" + i.ToString() + "\" value=\"" + dt.Rows[i][ValueField].ToString() + "\" checked=\"checked\" />" + dt.Rows[i][TextField].ToString();
                   }
                   else
                   {
                       if (i == (dt.Rows.Count - 1) && !sel)
                            HTML += " <input type=\"radio\" name=\"" + ClassName + "\" id=\"" + ClassName + "_" + i.ToString() + "\" value=\"" + dt.Rows[i][ValueField].ToString() + "\" />" + dt.Rows[i][TextField].ToString();
                       else
                           HTML += " <input type=\"radio\" name=\"" + ClassName + "\" id=\"" + ClassName + "_" + i.ToString() + "\" value=\"" + dt.Rows[i][ValueField].ToString() + "\" />" + dt.Rows[i][TextField].ToString(); 
                   }
               }
           }
           return HTML;
       }

        #endregion

    }
}
