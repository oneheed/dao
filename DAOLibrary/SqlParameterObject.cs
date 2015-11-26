using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAOLibrary
{
    public class SqlParameterObject
    {
        public static List<SqlParameter> GetSqlParameter(string decodeConnectionString, string procedureKey, ref string commandTextString, object[] sqlParameterValue)
        {
            try
            {
                if (Procedure.procedureKey == null || Procedure.procedureKey.Count == 0)
                    throw new Exception("Dictionary尚未裝載資料！");

                if (sqlParameterValue == null)
                    sqlParameterValue = new object[] { };

                ProcedureStruct procStruct = new ProcedureStruct();

                Procedure.procedureKey.TryGetValue(procedureKey, out procStruct.procedureList);

                if (procStruct.procedureList == null || procStruct.procedureList.Count == 0)
                    throw new Exception(string.Format("資料表Procedure，沒有{0}的這個欄位值！", procedureKey));

                commandTextString = Convert.ToString(procStruct.procedureList.FirstOrDefault().Key);

                if (string.IsNullOrEmpty(commandTextString))
                    throw new Exception(string.Format("資料表Procedure，沒有{0}的這個預存程序！", commandTextString));

                List<SqlParameter> sqlparameterlist = new List<SqlParameter>();
                int parameterValueLenght = procStruct.procedureList[commandTextString].Count;

                if (parameterValueLenght > 0)
                {
                    object[] parameterValue = new object[parameterValueLenght];
                    sqlParameterValue.CopyTo(parameterValue, 0);

                    if (parameterValueLenght == parameterValue.Length)
                    {
                        int PROCValueArrayCount = 0;

                        foreach (DbTableStruct value in procStruct.procedureList[commandTextString])
                        {
                            SqlDbType sqlDbType = new SqlDbType();

                            if (value.sqlType.ToUpper() == "NUMERIC")
                                DbDictionary.mappingSqlDbType().TryGetValue("DECIMAL", out sqlDbType);
                            else if (value.parameter.StartsWith("@tb"))
                                sqlDbType = SqlDbType.Structured;
                            else
                                DbDictionary.mappingSqlDbType().TryGetValue(value.sqlType.ToUpper(), out sqlDbType);

                            object parameterString = parameterValue[PROCValueArrayCount] == null ? DBNull.Value : parameterValue[PROCValueArrayCount];

                            if (value.length > 0)
                                sqlparameterlist.Add(new SqlParameter(value.parameter, sqlDbType, value.length) { Value = parameterString });
                            else
                                sqlparameterlist.Add(new SqlParameter(value.parameter, sqlDbType) { Value = parameterString });

                            PROCValueArrayCount++;
                        }
                    }
                    else
                        throw new Exception("參數名稱 [parameterName] 與參數內容 [parameterValue] 數量不相等 !");
                }

                return sqlparameterlist;
            }
            catch
            {
                throw;
            }
        }
        public static List<SqlParameter> GetSqlParameter(string decodeConnectionString, string procedureKey, ref string commandTextString, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                if (Procedure.procedureKey == null || Procedure.procedureKey.Count == 0)
                    throw new Exception("Dictionary尚未裝載資料！");

                ProcedureStruct procStruct = new ProcedureStruct();

                Procedure.procedureKey.TryGetValue(procedureKey, out procStruct.procedureList);

                if (procStruct.procedureList == null || procStruct.procedureList.Count == 0)
                    throw new Exception(string.Format("資料表Procedure，沒有{0}的這個欄位值！", procedureKey));

                commandTextString = Convert.ToString(procStruct.procedureList.FirstOrDefault().Key);

                if (string.IsNullOrEmpty(commandTextString))
                    throw new Exception(string.Format("資料表Procedure，沒有{0}的這個預存程序！", commandTextString));

                List<SqlParameter> sqlparameterlist = new List<SqlParameter>();
                int ParameterValueLenght = procStruct.procedureList[commandTextString].Where(x => x.outputFlag == false).Count();

                if (ParameterValueLenght > 0)
                {
                    if (ParameterValueLenght == sqlParameterValue.Count)
                    {
                        foreach (DbTableStruct value in procStruct.procedureList[commandTextString])
                        {
                            SqlDbType sqlDbType = new SqlDbType();

                            if (value.sqlType.ToUpper() == "NUMERIC")
                                DbDictionary.mappingSqlDbType().TryGetValue("DECIMAL", out sqlDbType);
                            else if (value.parameter.StartsWith("@tb"))
                                sqlDbType = SqlDbType.Structured;
                            else
                                DbDictionary.mappingSqlDbType().TryGetValue(value.sqlType.ToUpper(), out sqlDbType);

                            object parameterString = null;
                            sqlParameterValue.TryGetValue(value.parameter.Replace("@", ""), out parameterString);

                            if (parameterString == null)
                                parameterString = DBNull.Value;
                            else
                                if (value.parameter.StartsWith("@tb"))
                                    parameterString = (DataTable)JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(parameterString), new DataTableConverter());


                            if (value.length > 0)
                            {
                                if (value.outputFlag)
                                {
                                    sqlparameterlist.Add(new SqlParameter(value.parameter, sqlDbType, value.length)
                                    {
                                        Value = parameterString,
                                        Direction = ParameterDirection.Output
                                    });
                                }
                                else
                                    sqlparameterlist.Add(new SqlParameter(value.parameter, sqlDbType, value.length) { Value = parameterString });
                            }
                            else
                            {
                                if (value.outputFlag)
                                {
                                    sqlparameterlist.Add(new SqlParameter(value.parameter, sqlDbType)
                                    {
                                        Value = parameterString,
                                        Direction = ParameterDirection.Output
                                    });
                                }
                                else
                                    sqlparameterlist.Add(new SqlParameter(value.parameter, sqlDbType) { Value = parameterString });
                            }
                        }
                    }
                    else
                        throw new Exception("參數名稱 [parameterName] 與參數內容 [parameterValue] 數量不相等 !");
                }

                return sqlparameterlist;
            }
            catch
            {
                throw;
            }
        }
        public static List<SqlParameter> GetSqlParameterWithOutput(string decodeConnectionString, string procedureKey, ref string commandTextString, object[] sqlParameterValue, ref int outputCount)
        {
            try
            {
                if (Procedure.procedureKey == null || Procedure.procedureKey.Count == 0)
                    throw new Exception("Dictionary尚未裝載資料！");

                if (sqlParameterValue == null)
                    sqlParameterValue = new object[] { };

                ProcedureStruct procStruct = new ProcedureStruct();
                Procedure.procedureKey.TryGetValue(procedureKey, out procStruct.procedureList);

                if (procStruct.procedureList == null || procStruct.procedureList.Count == 0)
                {
                    throw new Exception(string.Format("資料表Procedure，沒有{0}這個欄位值！", procedureKey));
                }

                commandTextString = Convert.ToString(procStruct.procedureList.FirstOrDefault().Key);

                if (string.IsNullOrEmpty(commandTextString))
                {
                    throw new Exception(string.Format("資料表Procedure，沒有{0}這個預存程序！", commandTextString));
                }

                List<SqlParameter> sqlparameterlist = new List<SqlParameter>();
                int ParameterValueLenght = procStruct.procedureList[commandTextString].Count;

                if (ParameterValueLenght > 0)
                {
                    object[] ParameterValue = new object[ParameterValueLenght];
                    sqlParameterValue.CopyTo(ParameterValue, 0);

                    if (ParameterValueLenght == ParameterValue.Length)
                    {
                        int PROCValueArrayCount = 0;

                        foreach (DbTableStruct value in procStruct.procedureList[commandTextString])
                        {
                            SqlDbType sqlDbType = new SqlDbType();

                            if (value.sqlType.ToUpper() == "NUMERIC")
                                DbDictionary.mappingSqlDbType().TryGetValue("DECIMAL", out sqlDbType);
                            else if (value.parameter.StartsWith("@tb"))
                                sqlDbType = SqlDbType.Structured;
                            else
                                DbDictionary.mappingSqlDbType().TryGetValue(value.sqlType.ToUpper(), out sqlDbType);

                            object parameterString = ParameterValue[PROCValueArrayCount] == null ? DBNull.Value : ParameterValue[PROCValueArrayCount];

                            if (value.length > 0)
                            {
                                if (value.outputFlag)
                                {
                                    sqlparameterlist.Add(new SqlParameter(value.parameter, sqlDbType, value.length)
                                    {
                                        Value = parameterString,
                                        Direction = ParameterDirection.Output
                                    });

                                    outputCount++;
                                }
                                else
                                    sqlparameterlist.Add(new SqlParameter(value.parameter, sqlDbType, value.length) { Value = parameterString });
                            }
                            else
                            {
                                if (value.outputFlag)
                                {
                                    sqlparameterlist.Add(new SqlParameter(value.parameter, sqlDbType)
                                    {
                                        Value = parameterString,
                                        Direction = ParameterDirection.Output
                                    });

                                    outputCount++;
                                }
                                else
                                    sqlparameterlist.Add(new SqlParameter(value.parameter, sqlDbType) { Value = parameterString });
                            }

                            PROCValueArrayCount++;
                        }
                    }
                    else
                        throw new Exception("參數名稱 [parameterName] 與參數內容 [parameterValue] 數量不相等 !");
                }

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
                if (Procedure.procedureKey == null || Procedure.procedureKey.Count == 0)
                    throw new Exception("Dictionary尚未裝載資料！");

                ProcedureStruct procStruct = new ProcedureStruct();
                Procedure.procedureKey.TryGetValue(procedureKey, out procStruct.procedureList);

                if (procStruct.procedureList == null || procStruct.procedureList.Count == 0)
                {
                    throw new Exception(string.Format("資料表Procedure，沒有{0}這個欄位值！", procedureKey));
                }

                commandTextString = Convert.ToString(procStruct.procedureList.FirstOrDefault().Key);

                if (string.IsNullOrEmpty(commandTextString))
                {
                    throw new Exception(string.Format("資料表Procedure，沒有{0}這個預存程序！", commandTextString));
                }

                List<SqlParameter> sqlparameterlist = new List<SqlParameter>();
                int ParameterValueLenght = procStruct.procedureList[commandTextString].Where(x => x.outputFlag == false).Count();

                foreach (DbTableStruct value in procStruct.procedureList[commandTextString])
                {
                    SqlDbType sqlDbType = new SqlDbType();

                    if (value.sqlType.ToUpper() == "NUMERIC")
                        DbDictionary.mappingSqlDbType().TryGetValue("DECIMAL", out sqlDbType);
                    else if (value.parameter.StartsWith("@tb"))
                        sqlDbType = SqlDbType.Structured;
                    else
                        DbDictionary.mappingSqlDbType().TryGetValue(value.sqlType.ToUpper(), out sqlDbType);

                    object parameterString = null;
                    sqlParameterValue.TryGetValue(value.parameter.Replace("@", ""), out parameterString);

                    #region Process Json to DataTable
                    if (parameterString == null || parameterString.ToString().Replace(" ", "") == "[]")
                    {
                        if (value.parameter.StartsWith("@tb"))
                            parameterString = OutPutColumns(decodeConnectionString, procedureKey, value.parameter.Replace("@tbl", ""));
                        else
                            parameterString = DBNull.Value;
                    }
                    else
                    {
                        if (value.parameter.StartsWith("@tb"))
                            parameterString = (DataTable)JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(parameterString), new DataTableConverter());
                    }
                    #endregion

                    if (value.length > 0)
                    {
                        if (value.outputFlag)
                        {
                            sqlparameterlist.Add(new SqlParameter(value.parameter, sqlDbType, value.length)
                            {
                                Value = parameterString,
                                Direction = ParameterDirection.Output
                            });

                            outputCount++;
                        }
                        else
                            sqlparameterlist.Add(new SqlParameter(value.parameter, sqlDbType, value.length) { Value = parameterString });
                    }
                    else
                    {
                        if (value.outputFlag)
                        {
                            sqlparameterlist.Add(new SqlParameter(value.parameter, sqlDbType)
                            {
                                Value = parameterString,
                                Direction = ParameterDirection.Output
                            });

                            outputCount++;
                        }
                        else
                            sqlparameterlist.Add(new SqlParameter(value.parameter, sqlDbType) { Value = parameterString });
                    }
                }

                return sqlparameterlist;
            }
            catch
            {
                throw;
            }
        }
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

        #region Convert Json to DataTable
        public static DataTable OutPutColumns(string decodeConnectionString, string SpName, string ColumnName)
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
        #endregion
    }

}
