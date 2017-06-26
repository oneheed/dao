using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary
{
    public class Const
    {
        public const string TMP_MONGODB_AUTH_CONN_STR = "mongodb://{0}:{1}@{2}/admin";
        public const string TMP_MONGODB_CONN_STR = "mongodb://{0}";

        public const string TMP_MSSQL_CONN_STR = @"Data Source={0};Initial Catalog={1};user={2};password={3}";
        public const string TMP_MSSQL_CONN_STR_WITH_APPNAME = @"Data Source={0};Initial Catalog={1};user={2};password={3};Application Name={4}";

        public const string TMP_GET_USER_DEFINED_TABLE_COL_COUNT = "Exec usp_getUserDefinedTableColCount {0},{1}";
        public const string GET_PROCEDURE_PARAMETER_COUNT = "Exec usp_getProcedureParameterCount";
        public const string GET_PROCEDURE_PARAMETER = "Exec usp_getProcedureParameter";

        public const string NO_SUCH_SP = "資料表Procedure，沒有與{0}對應的預存程序！";
        public const string PARAM_NOT_MATCH = "參數名稱[parameterName] 與參數內容[parameterValue] 數量不相等 !";
        public const string CANNOT_GET_SP_LIST = "Dictionary尚未裝載資料！";

    }
}
