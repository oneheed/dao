using DAOLibrary.Model;
using DAOLibrary.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAOLibrary
{
    public class SqlParameterHelper
    {
        #region private method
        private static SqlParameter TryGetSqlParamter(ParameterObj pObj, SqlDbType sqlDbType, object parameterValue)
        {
            parameterValue = sqlDbType == SqlDbType.UniqueIdentifier ? Guid.Parse(parameterValue.ToString()) : parameterValue;
            if (pObj.Length > 0)
            {
                return (new SqlParameter(pObj.Parameter, sqlDbType, pObj.Length)
                {
                    Value = parameterValue,
                    Direction = pObj.OutputFlag ? ParameterDirection.Output : ParameterDirection.Input
                });
            }
            else
            {
                return (new SqlParameter(pObj.Parameter, sqlDbType)
                {
                    Value = parameterValue,
                    Direction = pObj.OutputFlag ? ParameterDirection.Output : ParameterDirection.Input
                });
            }
        }

        private static DataTable OutPutColumns(string decodeConnectionString, string SpName, string ColumnName)
        {
            string strComment = string.Format(Const.TMP_GET_USER_DEFINED_TABLE_COL_COUNT, SpName, ColumnName);
            using (SqlDataAdapter saCount = new SqlDataAdapter(strComment, decodeConnectionString))
            {

                DataTable dtCount = new DataTable();
                saCount.Fill(dtCount);

                int _procedureKey = int.Parse(dtCount.Rows[0]["Counts"].ToString());

                DataTable dt = new DataTable();

                for (int i = 0; i < _procedureKey; i++)
                {
                    dt.Columns.Add();
                }

                return dt;
            }
        }

        private static DbObj TryGetDbObj(string decodeConnectionString, string procedureKey)
        {
            var dbProcedure = StoredProcedurePool.DbProcedures.ContainsKey(decodeConnectionString);
            var dbObj = new DbObj();
            if (!dbProcedure)
            {
                StoredProcedurePool.UpdateProcedure(decodeConnectionString);
            }
            else
            {
                // 2017-10-02 Added by Lyon Wang: 等待完成指定 MainDB 或 ThirdPartyDB 載入完成 ==> total 200 milliseconds
                int retryTimes = 20;
                do
                {
                    Thread.Sleep(10);
                    StoredProcedurePool.DbProcedures.TryGetValue(decodeConnectionString, out dbObj);
                    retryTimes--;
                }
                while (!dbObj.loadComplete && retryTimes > 0);
            }

            if (!dbObj.loadComplete)
            {
                dbObj = StoredProcedurePool.GetOneDbObjFromDB(decodeConnectionString, procedureKey);
            }

            // 2017-10-02 Comment out by Lyon Wang: StoredProcedurePool 初始化就會 new 一個，所以不需要這段程式碼
            //if (StoredProcedurePool.DbProcedures == null || StoredProcedurePool.DbProcedures.Count == 0)
            //    throw new Exception(Const.CANNOT_GET_SP_LIST);

            //var dbObj = new DbObj();
            //StoredProcedurePool.DbProcedures.TryGetValue(decodeConnectionString, out dbObj);
            if (dbObj.ProcedureList == null || dbObj.ProcedureList.Count == 0 || !dbObj.ProcedureList.ContainsKey(procedureKey))
            {
                //throw new Exception(string.Format(Const.NO_SUCH_SP, procedureKey));
                dbObj = null;
            }

            return dbObj;
        }

        private static SqlDbType TryGetSqlDbType(ParameterObj value)
        {
            var sqlDbType = new SqlDbType();
            var sqlDbTypeKey = string.Empty;

            if (value.SqlType.ToUpper() == "NUMERIC")
                sqlDbTypeKey = "DECIMAL";
            else if (value.Parameter.StartsWith("@tb"))
                sqlDbTypeKey = "STRUCTURED";
            else
                sqlDbTypeKey = value.SqlType.ToUpper();

            DbDictionary.MappingSqlDbType.TryGetValue(sqlDbTypeKey, out sqlDbType);
            return sqlDbType;
        }
        #endregion

        #region DictionaryParameter
        private static object ConvertParamValue(Dictionary<string, object> sqlParameterValue, ParameterObj value, string decodeConnectionString, string procedureKey)
        {
            object parameterString = null;
            sqlParameterValue.TryGetValue(value.Parameter.Replace("@", ""), out parameterString);

            #region Process Json to DataTable
            if (parameterString == null || parameterString.ToString().Replace(" ", "") == "[]" ||
                ((parameterString.GetType() == typeof(DataTable) && ((DataTable)parameterString).Rows.Count == 0)))
            {
                if (value.Parameter.StartsWith("@tb"))
                    parameterString = OutPutColumns(decodeConnectionString, procedureKey, value.Parameter.Replace("@tbl", ""));
                else
                    parameterString = DBNull.Value;
            }
            else
            {
                if (value.Parameter.StartsWith("@tb"))
                    parameterString = (DataTable)JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(parameterString), new DataTableConverter());
            }
            #endregion
            return parameterString;
        }

        private static string TryGetProcedureName(DbObj dbObj, string procedureKey)
        {
            try { return dbObj.ProcedureList[procedureKey].ProcedureName; }
            catch { throw new Exception("Cannot get procedure name for " + procedureKey); }
        }

        private static List<SqlParameter> TryGetSqlParamterList(DbObj dbObj, string decodeConnectionString, string procedureKey, Dictionary<string, object> sqlParameterValue)
        {
            var sqlparameterlist = new List<SqlParameter>();
            var paramsFromDb = dbObj.ProcedureList[procedureKey].ParameterObjs;
            int parameterValueLength = paramsFromDb.Where(x => x.OutputFlag == false).Count();

            //if (parameterValueLength > 0)
            {
                var paramNameFromDB = paramsFromDb.Where(o => o.OutputFlag == true)
                                                  .Select((o) => { return o.Parameter.ToUpper().Replace("@", ""); }).ToList();
                var cnt = sqlParameterValue.Where(o => !paramNameFromDB.Contains(o.Key.ToUpper())).Count();
                //if (parameterValueLength == cnt)
                //{
                foreach (var value in dbObj.ProcedureList[procedureKey].ParameterObjs)
                {
                    var sqlDbType = TryGetSqlDbType(value);
                    var parameterValue = ConvertParamValue(sqlParameterValue, value, decodeConnectionString, procedureKey);
                    var sqlParam = TryGetSqlParamter(value, sqlDbType, parameterValue);
                    if (sqlParameterValue.ContainsKey(sqlParam.ParameterName.Replace("@", "")) ||
                        sqlParameterValue.ContainsKey(sqlParam.ParameterName) || sqlParam.Direction == ParameterDirection.Output)
                        sqlparameterlist.Add(sqlParam);
                }
                //}
                //else
                //{
                //    throw new Exception(Const.PARAM_NOT_MATCH);
                //}
            }
            return sqlparameterlist;
        }

        public static List<SqlParameter> GetSqlParameter(List<string> connectionStringList, string procedureKey, ref string commandTextString, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                for (int i = 0; i < connectionStringList.Count; i++)
                {
                    var dbObj = TryGetDbObj(connectionStringList[i], procedureKey);
                    if (dbObj == null)
                    {
                        if (i == connectionStringList.Count - 1)
                        {
                            throw new Exception(string.Format(Const.NO_SUCH_SP, procedureKey));
                        }
                        else
                        {
                            continue;
                        }
                    }
                    commandTextString = TryGetProcedureName(dbObj, procedureKey);

                    var sqlparameterlist = TryGetSqlParamterList(dbObj, connectionStringList[i], procedureKey, sqlParameterValue);

                    return sqlparameterlist;
                }
                return null;
            }
            catch
            {
                throw;
            }
        }

        public static List<SqlParameter> GetSqlParameterWithOutput(List<string> connectionStringList, string procedureKey, ref string commandTextString, Dictionary<string, object> sqlParameterValue, ref int outputCount)
        {
            try
            {
                for (int i = 0; i < connectionStringList.Count; i++)
                {
                    var dbObj = TryGetDbObj(connectionStringList[i], procedureKey);
                    if (dbObj == null)
                    {
                        if (i == connectionStringList.Count - 1)
                        {
                            throw new Exception(string.Format(Const.NO_SUCH_SP, procedureKey));
                        }
                        else
                        {
                            continue;
                        }
                    }
                    commandTextString = TryGetProcedureName(dbObj, procedureKey);
                    var sqlparameterlist = TryGetSqlParamterList(dbObj, connectionStringList[i], procedureKey, sqlParameterValue);
                    foreach (var p in sqlparameterlist)
                    {
                        if (p.Direction == ParameterDirection.Output)
                        {
                            outputCount++;
                        }
                    }
                    return sqlparameterlist;
                }
                return null;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region objectParameter
        private static List<SqlParameter> TryGetSqlParameterListByObject(DbObj dbObj, string commandTextString, string decodeConnectionString, string procedureKey, object[] sqlParameterValue)
        {

            if (sqlParameterValue == null)
                sqlParameterValue = new object[] { };

            var sqlParamList = new List<SqlParameter>();
            int paramValueLen = dbObj.ProcedureList[procedureKey].ParameterObjs.Count;
            if (paramValueLen > 0)
            {
                object[] parameterValue = new object[paramValueLen];
                sqlParameterValue.CopyTo(parameterValue, 0);

                if (paramValueLen == parameterValue.Length)
                {
                    int paramIdx = 0;

                    foreach (var value in dbObj.ProcedureList[procedureKey].ParameterObjs)
                    {
                        SqlDbType sqlDbType = TryGetSqlDbType(value);
                        object parameterString = parameterValue[paramIdx] == null ? DBNull.Value : parameterValue[paramIdx];
                        sqlParamList.Add(TryGetSqlParamter(value, sqlDbType, parameterString));
                        paramIdx++;
                    }
                }
                else
                {
                    throw new Exception(Const.PARAM_NOT_MATCH);
                }
            }
            return sqlParamList;
        }

        public static List<SqlParameter> GetSqlParameterWithOutput(List<string> connectionStringList, string procedureKey, ref string commandTextString, object[] sqlParameterValue, ref int outputCount)
        {
            try
            {
                for (int i = 0; i < connectionStringList.Count; i++)
                {
                    var dbObj = TryGetDbObj(connectionStringList[i], procedureKey);
                    if (dbObj == null)
                    {
                        if (i == connectionStringList.Count - 1)
                        {
                            throw new Exception(string.Format(Const.NO_SUCH_SP, procedureKey));
                        }
                        else
                        {
                            continue;
                        }
                    }
                    commandTextString = Convert.ToString(dbObj.ProcedureList[procedureKey].ProcedureName);

                    var sqlParamList = TryGetSqlParameterListByObject(dbObj, commandTextString, connectionStringList[i], procedureKey, sqlParameterValue);

                    foreach (var p in sqlParamList)
                    {
                        if (p.Direction == ParameterDirection.Output)
                        {
                            outputCount++;
                        }
                    }
                    return sqlParamList;
                }
                return null;
            }
            catch
            {
                throw;
            }
        }

        public static List<SqlParameter> GetSqlParameter(List<string> connectionStringList, string procedureKey, ref string commandTextString, object[] sqlParameterValue)
        {
            try
            {
                for (int i = 0; i < connectionStringList.Count; i++)
                {
                    var dbObj = TryGetDbObj(connectionStringList[i], procedureKey);
                    if (dbObj == null)
                    {
                        if (i == connectionStringList.Count - 1)
                        {
                            throw new Exception(string.Format(Const.NO_SUCH_SP, procedureKey));
                        }
                        else
                        {
                            continue;
                        }
                    }
                    commandTextString = Convert.ToString(dbObj.ProcedureList[procedureKey].ProcedureName);

                    var sqlparameterlist = TryGetSqlParameterListByObject(dbObj, commandTextString, connectionStringList[i], procedureKey, sqlParameterValue);

                    return sqlparameterlist;
                }
                return null;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        public static void OutputSqlParameter(List<SqlParameter> sqlParameterList, int outputCount, ref object[] outputParamValue)
        {
            outputParamValue = new object[outputCount];

            int procedureOutputValue = 0;

            foreach (SqlParameter sqlParameter in sqlParameterList)
            {
                if (sqlParameter.Direction == ParameterDirection.Output)
                {
                    outputParamValue[procedureOutputValue] = sqlParameter.Value;
                    procedureOutputValue++;
                }
            }
        }
    }
}
