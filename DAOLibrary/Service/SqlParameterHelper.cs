﻿using DAOLibrary.Model;
using DAOLibrary.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary
{
    public class SqlParameterHelper
    {
        #region private method
        private static SqlParameter TryGetSqlParamter(ParameterObj pObj, SqlDbType sqlDbType, object parameterValue)
        {
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
            string strComment = "Exec usp_getUserDefinedTableColCount " + SpName + "," + ColumnName;
            SqlDataAdapter saCount = new SqlDataAdapter(strComment, decodeConnectionString);

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

        private static DbObj TryGetDbObj(string decodeConnectionString, string procedureKey)
        {
            StoredProcedurePool.UpdateProcedure(decodeConnectionString);
            if (StoredProcedurePool.DbProcedures == null || StoredProcedurePool.DbProcedures.Count == 0)
                throw new Exception("Dictionary尚未裝載資料！");

            var dbObj = new DbObj();

            StoredProcedurePool.DbProcedures.TryGetValue(decodeConnectionString, out dbObj);

            if (dbObj.ProcedureList == null || dbObj.ProcedureList.Count == 0 || string.IsNullOrEmpty(dbObj.ProcedureList[procedureKey].ProcedureName))
                throw new Exception(string.Format("資料表Procedure，沒有與{0}對應的預存程序！", procedureKey));

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

            DbDictionary.mappingSqlDbType().TryGetValue(sqlDbTypeKey, out sqlDbType);
            return sqlDbType;
        }
        #endregion

        #region DictionaryParameter
        private static object ConvertParamValue(Dictionary<string, object> sqlParameterValue, ParameterObj value, string decodeConnectionString, string procedureKey)
        {
            object parameterString = null;
            sqlParameterValue.TryGetValue(value.Parameter.Replace("@", ""), out parameterString);

            #region Process Json to DataTable
            if (parameterString == null || parameterString.ToString().Replace(" ", "") == "[]")
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

        private static List<SqlParameter> TryGetSqlParamterList(string decodeConnectionString, string procedureKey, ref string commandTextString, Dictionary<string, object> sqlParameterValue)
        {
            var dbObj = TryGetDbObj(decodeConnectionString, procedureKey);
            commandTextString = Convert.ToString(dbObj.ProcedureList[procedureKey].ProcedureName);
            var sqlparameterlist = new List<SqlParameter>();
            var paramsFromDb = dbObj.ProcedureList[procedureKey].ParameterObjs;
            int parameterValueLength = paramsFromDb.Where(x => x.OutputFlag == false).Count();

            if (parameterValueLength > 0)
            {
                var paramNameFromDB = paramsFromDb.Where(o => o.OutputFlag == true)
                                                  .Select((o) => { return o.Parameter.ToUpper().Replace("@", ""); }).ToList();
                var cnt = sqlParameterValue.Where(o => !paramNameFromDB.Contains(o.Key.ToUpper())).Count();
                if (parameterValueLength == cnt)
                {
                    foreach (var value in dbObj.ProcedureList[procedureKey].ParameterObjs)
                    {
                        var sqlDbType = TryGetSqlDbType(value);
                        var parameterValue = ConvertParamValue(sqlParameterValue, value, decodeConnectionString, procedureKey);
                        var sqlParam = TryGetSqlParamter(value, sqlDbType, parameterValue);
                        sqlparameterlist.Add(sqlParam);
                    }
                }
                else
                {
                    throw new Exception("參數名稱 [parameterName] 與參數內容 [parameterValue] 數量不相等 !");
                }
            }
            return sqlparameterlist;
        }

        public static List<SqlParameter> GetSqlParameter(string decodeConnectionString, string procedureKey, ref string commandTextString, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                var sqlparameterlist = TryGetSqlParamterList(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);

                return sqlparameterlist;
            }
            catch
            {
                throw;
            }
        }

        public static List<SqlParameter> GetSqlParameterWithOutput(string decodeConnectionString, string procedureKey, ref string commandTextString, Dictionary<string, object> sqlParameterValue, ref int outputCount)
        {
            try
            {
                var sqlparameterlist = TryGetSqlParamterList(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                foreach (var p in sqlparameterlist)
                {
                    if (p.Direction == ParameterDirection.Output)
                    {
                        outputCount++;
                    }
                }
                return sqlparameterlist;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region objectParameter
        private static List<SqlParameter> TryGetSqlParameterListByObject(string decodeConnectionString, string procedureKey, ref string commandTextString, object[] sqlParameterValue)
        {
            var dbObj = TryGetDbObj(decodeConnectionString, procedureKey);
            commandTextString = Convert.ToString(dbObj.ProcedureList[procedureKey].ProcedureName);

            if (sqlParameterValue == null)
                sqlParameterValue = new object[] { };

            var sqlParamList = new List<SqlParameter>();
            int paramValueLen = dbObj.ProcedureList[commandTextString].ParameterObjs.Count;
            if (paramValueLen > 0)
            {
                object[] parameterValue = new object[paramValueLen];
                sqlParameterValue.CopyTo(parameterValue, 0);

                if (paramValueLen == parameterValue.Length)
                {
                    int paramIdx = 0;

                    foreach (var value in dbObj.ProcedureList[commandTextString].ParameterObjs)
                    {
                        SqlDbType sqlDbType = TryGetSqlDbType(value);
                        object parameterString = parameterValue[paramIdx] == null ? DBNull.Value : parameterValue[paramIdx];

                        sqlParamList.Add(TryGetSqlParamter(value, sqlDbType, parameterString));

                        paramIdx++;
                    }
                }
                else
                {
                    throw new Exception("參數名稱 [parameterName] 與參數內容 [parameterValue] 數量不相等 !");
                }
            }
            return sqlParamList;
        }

        public static List<SqlParameter> GetSqlParameterWithOutput(string decodeConnectionString, string procedureKey, ref string commandTextString, object[] sqlParameterValue, ref int outputCount)
        {
            try
            {
                var sqlParamList = TryGetSqlParameterListByObject(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);

                foreach (var p in sqlParamList)
                {
                    if (p.Direction == ParameterDirection.Output)
                    {
                        outputCount++;
                    }
                }
                return sqlParamList;
            }
            catch
            {
                throw;
            }
        }

        public static List<SqlParameter> GetSqlParameter(string decodeConnectionString, string procedureKey, ref string commandTextString, object[] sqlParameterValue)
        {
            try
            {
                var sqlparameterlist = TryGetSqlParameterListByObject(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);

                return sqlparameterlist;
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