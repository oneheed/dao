using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary
{

    public static class DbDictionary
    {
        private static Dictionary<string, SqlDbType> dic = new Dictionary<string, SqlDbType>();
        public static Dictionary<string, SqlDbType> mappingSqlDbType()
        {
            if (dic.Count > 0)
                return dic;

            dic.Add("BIGINT", SqlDbType.BigInt);
            dic.Add("BINARY", SqlDbType.VarBinary);
            dic.Add("BIT", SqlDbType.Bit);
            dic.Add("CHAR", SqlDbType.Char);
            dic.Add("DATE", SqlDbType.Date);
            dic.Add("DATETIME", SqlDbType.DateTime);
            dic.Add("DATETIME2", SqlDbType.DateTime2);
            dic.Add("DATETIMEOFFSET", SqlDbType.DateTimeOffset);
            dic.Add("DECIMAL", SqlDbType.Decimal);
            dic.Add("FLOAT", SqlDbType.Float);
            dic.Add("IMAGE", SqlDbType.Image);
            dic.Add("INT", SqlDbType.Int);
            dic.Add("MONEY", SqlDbType.Money);
            dic.Add("NCHAR", SqlDbType.NChar);
            dic.Add("NTEXT", SqlDbType.NText);
            dic.Add("NVARCHAR", SqlDbType.NVarChar);
            dic.Add("REAL", SqlDbType.Real);
            dic.Add("ROWVERSION", SqlDbType.Timestamp);
            dic.Add("SMALLDATETIME", SqlDbType.SmallDateTime);
            dic.Add("SMALLINT", SqlDbType.SmallInt);
            dic.Add("SMALLMONEY", SqlDbType.SmallMoney);
            dic.Add("STRUCTURED", SqlDbType.Structured);
            dic.Add("TEXT", SqlDbType.Text);
            dic.Add("TIME", SqlDbType.Time);
            dic.Add("TIMESTAMP", SqlDbType.Timestamp);
            dic.Add("TINYINT", SqlDbType.TinyInt);
            dic.Add("UDT", SqlDbType.Udt);
            dic.Add("UNIQUEIDENTIFIER", SqlDbType.UniqueIdentifier);
            dic.Add("VARBINARY", SqlDbType.VarBinary);
            dic.Add("VARCHAR", SqlDbType.VarChar);
            dic.Add("VARIANT", SqlDbType.Variant);
            dic.Add("XML", SqlDbType.Xml);

            return dic;
        }
    }
}

