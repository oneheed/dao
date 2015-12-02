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

        public const string GET_PROCEDURE_PARAMETER_COUNT = "Exec usp_getProcedureParameterCount";
        public const string GET_PROCEDURE_PARAMETER = "Exec usp_getProcedureParameter";
    }
}
