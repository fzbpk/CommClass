using LinqToDB;
using System;
namespace   LinqToDB.Mapping
{
    /// <summary>
    ///  ColumnAttribute扩展类型
    /// </summary>
    public static partial class ColumnAttributeEX
    {
        /// <summary>
        /// ColumnAttribute类中的DataType转TYPE
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Type ToSystemType(this DataType obj)
        {
            switch (obj)
            {
                case LinqToDB.DataType.Binary:
                case LinqToDB.DataType.VarBinary:
                case LinqToDB.DataType.Blob:
                    return typeof(byte[]);
                case LinqToDB.DataType.Boolean:
                    return typeof(bool);
                case LinqToDB.DataType.Byte:
                case LinqToDB.DataType.Char:
                    return typeof(char);
                case LinqToDB.DataType.NChar:
                    return typeof(char);
                case LinqToDB.DataType.Date:
                case LinqToDB.DataType.DateTime:
                case LinqToDB.DataType.DateTime2:
                case LinqToDB.DataType.DateTimeOffset:
                case LinqToDB.DataType.Time:
                case LinqToDB.DataType.Timestamp:
                    return typeof(DateTime);
                case LinqToDB.DataType.Decimal:
                case LinqToDB.DataType.Single:
                case LinqToDB.DataType.Double:
                    return typeof(double);
                case LinqToDB.DataType.Money:
                case LinqToDB.DataType.SmallMoney:
                    return typeof(float);
                case LinqToDB.DataType.Int16:
                case LinqToDB.DataType.Int32:
                case LinqToDB.DataType.Int64:
                case LinqToDB.DataType.UInt16:
                case LinqToDB.DataType.UInt32:
                case LinqToDB.DataType.UInt64:
                    return typeof(long);
                case LinqToDB.DataType.VarChar:
                case LinqToDB.DataType.Text:
                    return typeof(string);
                case LinqToDB.DataType.NVarChar:
                case LinqToDB.DataType.NText:
                    return typeof(string);
                default:
                    return null;
            }
        }

        /// <summary>
        ///TYPE转 ColumnAttribute类中的DataType
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DataType ToDataType(this Type obj)
        {
            if (obj == typeof(string))
                return DataType.VarChar;
            else if (obj.IsEnum)
                return DataType.Int16;
            else if (obj == typeof(int) || obj == typeof(short) || obj == typeof(long))
                return DataType.Int64;
            else if (obj == typeof(byte) || obj == typeof(char))
                return DataType.Char;
            else if (obj == typeof(uint) || obj == typeof(ushort) || obj == typeof(ulong))
                return DataType.UInt64;
            else if (obj == typeof(byte[]))
                return DataType.Binary;
            else if (obj == typeof(DateTime))
                return DataType.DateTime;
            else
                return DataType.Undefined;

        }
    }
}
