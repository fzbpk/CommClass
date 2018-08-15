using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
namespace DataFactory
{
    /// <summary>
    /// 支持的数据库类型
    /// </summary>
    [Description("支持的数据库类型")]
    public enum DBType : short
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        None = 0,
        /// <summary>
        /// MSSQL,支持从2000,2005,2008,2012
        /// </summary>
        [Description("微软SQL Server，支持从2000,2005,2008,2012")]
        MSSQL,
        /// <summary>
        /// Access
        /// </summary>
        [Description("微软Access，支持mdb和accdb")]
        Access,
        /// <summary>
        /// MYSQL,支持5.0及以上版本
        /// </summary>
        [Description("属于Oracle公司的一个关系型数据库,支持5.0及以上版本")]
        MYSQL,
        /// <summary>
        /// Oracle,支持10g及以上版本
        /// </summary>
        [Description("属于Oracle公司的一个关系型数据库,支持10g及以上版本")]
        Oracle,
        /// <summary>
        /// SQLite
        /// </summary>
        [Description("轻型数据库系统")]
        SQLite,
        /// <summary>
        /// PostgreSQL
        /// </summary>
        [Description("高效率数据库系统")]
        PostgreSQL,
    }

    /// <summary>
    /// 
    /// </summary>
    [Description("数据库配置")]
    public class DBInfo
    {
        /// <summary>
        /// 启用配置
        /// </summary>
        [Description("启用配置")]
        public bool Enable { get; set; }
        /// <summary>
        /// 启用配置
        /// </summary>
        [Description("数据库类型")]
        public DBType Mode { get; set; }
        /// <summary>
        /// 数据库地址
        /// </summary>
        [Description("数据库地址")]
        public string Url { get; set; }
        /// <summary>
        /// 数据库端口
        /// </summary>
        [Description("数据库端口")]
        public int Port { get; set; }
        /// <summary>
        /// 数据库账号
        /// </summary>
        [Description("数据库账号")]
        public string User { get; set; }
        /// <summary>
        /// 数据库密码
        /// </summary>
        [Description("数据库密码")]
        public string Password { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        [Description("数据库名称")]
        public string DataBaseName { get; set; }
        /// <summary>
        /// 超时时间，ms
        /// </summary>
        [Description("超时时间，ms")]
        public int TimeOut { get; set; }
        /// <summary>
        /// 字符编码
        /// </summary>
        [Description("字符编码")]
        public string Charset { get; set; }
    }

}
