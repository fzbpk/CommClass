using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace LinqToDB.Mapping
{
    /// <summary>
    /// 显示字段属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DisplayColumnAttribute : Attribute
    {
        /// <summary>
        /// 显示名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示格式,TOSTRING表达式或正则表达式
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 样式
        /// </summary>
        public string CSS { get; set; }
        /// <summary>
        /// 是否用于搜索
        /// </summary>
        public bool CanSearch { get; set; }
        /// <summary>
        /// 是否用于表头显示
        /// </summary>
        public bool CanHead { get; set; }
        /// <summary>
        /// 是否用于统计
        /// </summary>
        public bool CanCount { get; set; }
        /// <summary>
        /// 是否可用于导入导出
        /// </summary>
        public bool CanImpExp { get; set; }
        /// <summary>
        /// 是否唯一
        /// </summary>
        public bool IsUnique { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Seqencing { get; set; }
    }
}
