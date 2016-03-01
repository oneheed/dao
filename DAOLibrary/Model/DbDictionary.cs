using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary
{
    /// <summary>
    /// MS-SQL and C# 的設定對應
    /// </summary>
    public static class DbDictionary
    {
        /// <summary>
        /// SqlDbType的對應
        /// </summary>
        public static readonly Dictionary<string, SqlDbType> MappingSqlDbType = new Dictionary<string, SqlDbType>()
        {
            {"BIGINT", SqlDbType.BigInt},
            {"BINARY", SqlDbType.VarBinary},
            {"BIT", SqlDbType.Bit},
            {"CHAR", SqlDbType.Char},
            {"DATE", SqlDbType.Date},
            {"DATETIME", SqlDbType.DateTime},
            {"DATETIME2", SqlDbType.DateTime2},
            {"DATETIMEOFFSET", SqlDbType.DateTimeOffset},
            {"DECIMAL", SqlDbType.Decimal},
            {"FLOAT", SqlDbType.Float},
            {"IMAGE", SqlDbType.Image},
            {"INT", SqlDbType.Int},
            {"MONEY", SqlDbType.Money},
            {"NCHAR", SqlDbType.NChar},
            {"NTEXT", SqlDbType.NText},
            {"NVARCHAR", SqlDbType.NVarChar},
            {"REAL", SqlDbType.Real},
            {"ROWVERSION", SqlDbType.Timestamp},
            {"SMALLDATETIME", SqlDbType.SmallDateTime},
            {"SMALLINT", SqlDbType.SmallInt},
            {"SMALLMONEY", SqlDbType.SmallMoney},
            {"STRUCTURED", SqlDbType.Structured},
            {"TEXT", SqlDbType.Text},
            {"TIME", SqlDbType.Time},
            {"TIMESTAMP", SqlDbType.Timestamp},
            {"TINYINT", SqlDbType.TinyInt},
            {"UDT", SqlDbType.Udt},
            {"UNIQUEIDENTIFIER", SqlDbType.UniqueIdentifier},
            {"VARBINARY", SqlDbType.VarBinary},
            {"VARCHAR", SqlDbType.VarChar},
            {"VARIANT", SqlDbType.Variant},
            {"XML", SqlDbType.Xml}
        };
    }
}

