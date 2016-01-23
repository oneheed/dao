using DAOLibrary.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace DAOLibrary
{
    public class SqlDataAccess : IDataAccess
    {
        public SqlDataAccess(string connectionString, bool isTest = false, int procUpdateSec = 60)
            : base(connectionString, isTest, procUpdateSec) { }

        private static List<T> SqlDataReaderToObjectList<T>(ref T outputObject, SqlDataReader sqlDataReader) where T : new()
        {
            List<T> listData = new List<T>();
            Type objectType = typeof(T);
            if (sqlDataReader.HasRows)
            {
                while (sqlDataReader.Read())
                {
                    T dataStructure = new T();

                    for (int i = 0; i < sqlDataReader.FieldCount; i++)
                    {
                        try
                        {
                            string sqlDataReaderFieldName = sqlDataReader.GetName(i);
                            object objectValue = sqlDataReader.GetValue(i);

                            if (!string.IsNullOrEmpty(sqlDataReaderFieldName) && objectValue != null && !(objectValue is DBNull))
                            {
                                FieldInfo fieldInfo = objectType.GetField(sqlDataReaderFieldName);
                                PropertyInfo propertyInfo = objectType.GetProperty(sqlDataReaderFieldName);

                                if (fieldInfo != null)
                                {
                                    if (objectValue is Guid)
                                    {
                                        Guid guidOutputParameter = Guid.Empty;
                                        if (Guid.TryParse(objectValue.ToString(), out guidOutputParameter))
                                            fieldInfo.SetValue(dataStructure, guidOutputParameter);
                                    }
                                    else
                                        fieldInfo.SetValue(dataStructure, Convert.ChangeType(objectValue, fieldInfo.FieldType));
                                }

                                if (propertyInfo != null)
                                {
                                    if (objectValue is Guid && propertyInfo.PropertyType.Name == "String")
                                        propertyInfo.SetValue(dataStructure, ((Guid)objectValue).ToString(), null);
                                    else if (objectValue is Guid)
                                    {
                                        Guid guidOutputParameter = Guid.Empty;

                                        if (Guid.TryParse(objectValue.ToString(), out guidOutputParameter))
                                            propertyInfo.SetValue(dataStructure, guidOutputParameter, null);
                                    }
                                    else if (objectValue is DateTime && propertyInfo.PropertyType.Name == "String")
                                        propertyInfo.SetValue(dataStructure, ((DateTime)objectValue).ToString("yyyy/MM/dd HH:mm:ss"), null);
                                    else
                                        propertyInfo.SetValue(dataStructure, Convert.ChangeType(objectValue, propertyInfo.PropertyType), null);
                                }
                            }
                        }
                        catch
                        {
                            throw;
                        }
                    }

                    listData.Add(dataStructure);
                }
            }

            if (!sqlDataReader.IsClosed)
                sqlDataReader.Close();

            return listData;
        }
        private int Execute(List<SqlParameter> sqlParameterList, string procedureKey, string cmdStr, int timeout = 30)
        {
            using (SqlConnection conn = new SqlConnection(GetSqlConnectionStr(procedureKey)))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = cmdStr;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = timeout > 0 ? timeout : 30;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    conn.Open();
                    int Count = cmd.ExecuteNonQuery();
                    return Count;
                }
            }
        }
        private DataSet GetDataSet(List<SqlParameter> sqlParameterList, string procedureKey, string cmdStr, int timeout = 30)
        {
            using (SqlConnection conn = new SqlConnection(GetSqlConnectionStr(procedureKey)))
            {
                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = timeout > 0 ? timeout : 30;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    SqlDataAdapter sa = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sa.Fill(ds);
                    return ds;
                }
            }
        }
        private List<T> GetDataStructList<T>(List<SqlParameter> sqlParameterList, string procedureKey, string cmdStr, int timeout = 30) where T : new()
        {
            using (SqlConnection conn = new SqlConnection(GetSqlConnectionStr(procedureKey)))
            {
                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cmd.CommandTimeout = timeout > 0 ? timeout : 30;
                    conn.Open();
                    SqlDataReader sr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    T outputDataStruct = new T();
                    List<T> dataStructList = SqlDataAccess.SqlDataReaderToObjectList<T>(ref outputDataStruct, sr);
                    return dataStructList;
                }
            }
        }

        public override int ExecuteSp(string procedureKey, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var sqlParameterList = SqlParameterHelper.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                return Execute(sqlParameterList, procedureKey, commandTextString);
            }
            catch
            {
                throw;
            }
        }
        public override int ExecuteSp(string procedureKey, ref object[] outputParameterValue, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var outputCount = 0;
                var sqlParameterList = SqlParameterHelper.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                var result = Execute(sqlParameterList, procedureKey, commandTextString);
                SqlParameterHelper.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);

                return result;
            }
            catch
            {
                throw;
            }
        }
        public override DataSet GetDataSetFromSp(string procedureKey, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var sqlParameterList = SqlParameterHelper.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                return GetDataSet(sqlParameterList, procedureKey, commandTextString);
            }
            catch
            {
                throw;
            }
        }
        public override DataSet GetDataSetFromSp(string procedureKey, int timeout, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var sqlParameterList = SqlParameterHelper.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                return GetDataSet(sqlParameterList, procedureKey, commandTextString, timeout);
            }
            catch
            {
                throw;
            }
        }
        public override DataSet GetDataSetFromSp(string procedureKey, ref object[] outputParameterValue, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var outputCount = 0;
                var sqlParameterList = SqlParameterHelper.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                var result = GetDataSet(sqlParameterList, procedureKey, commandTextString);
                SqlParameterHelper.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                return result;
            }
            catch
            {
                throw;
            }
        }
        public override DataSet GetDataSetFromSp(string procedureKey, int timeout, ref object[] outputParameterValue, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var outputCount = 0;
                var sqlParameterList = SqlParameterHelper.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                var result = GetDataSet(sqlParameterList, procedureKey, commandTextString, timeout);
                SqlParameterHelper.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);

                return result;
            }
            catch
            {
                throw;
            }
        }
        public override List<T> GetListFromSp<T>(string procedureKey, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var sqlParameterList = SqlParameterHelper.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                return GetDataStructList<T>(sqlParameterList, procedureKey, commandTextString);
            }
            catch
            {
                throw;
            }
        }
        public override List<T> GetListFromSp<T>(string procedureKey, int timeout, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var sqlParameterList = SqlParameterHelper.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                return GetDataStructList<T>(sqlParameterList, procedureKey, commandTextString, timeout);
            }
            catch
            {
                throw;
            }
        }
        public override List<T> GetListFromSp<T>(string procedureKey, ref object[] outputParameterValue, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var outputCount = 0;
                var sqlParameterList = SqlParameterHelper.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                var result = GetDataStructList<T>(sqlParameterList, procedureKey, commandTextString);
                SqlParameterHelper.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                return result;
            }
            catch
            {
                throw;
            }
        }
        public override List<T> GetListFromSp<T>(string procedureKey, int timeout, ref object[] outputParameterValue, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var outputCount = 0;
                var sqlParameterList = SqlParameterHelper.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                var result = GetDataStructList<T>(sqlParameterList, procedureKey, commandTextString, timeout);
                SqlParameterHelper.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);

                return result;
            }
            catch
            {
                throw;
            }
        }

        //belows are objects
        public override int ExecuteSp(string procedureKey, params object[] sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var sqlParameterList = SqlParameterHelper.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                return Execute(sqlParameterList, procedureKey, commandTextString);
            }
            catch
            {
                throw;
            }
        }
        public override int ExecuteSp(string procedureKey, ref object[] outputParameterValue, params object[] sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var outputCount = 0;
                var sqlParameterList = SqlParameterHelper.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);

                var result = Execute(sqlParameterList, procedureKey, commandTextString);
                SqlParameterHelper.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                return result;
            }
            catch
            {
                throw;
            }
        }
        public override DataSet GetDataSetFromSp(string procedureKey, params object[] sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var sqlParameterList = SqlParameterHelper.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                return GetDataSet(sqlParameterList, procedureKey, commandTextString);
            }
            catch
            {
                throw;
            }
        }
        public override DataSet GetDataSetFromSp(string procedureKey, int timeout, params object[] sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var sqlParameterList = SqlParameterHelper.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                return GetDataSet(sqlParameterList, procedureKey, commandTextString, timeout);
            }
            catch
            {
                throw;
            }
        }
        public override DataSet GetDataSetFromSp(string procedureKey, ref object[] outputParameterValue, params object[] sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var outputCount = 0;
                var sqlParameterList = SqlParameterHelper.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);

                var result = GetDataSet(sqlParameterList, procedureKey, commandTextString);
                SqlParameterHelper.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                return result;
            }
            catch
            {
                throw;
            }
        }
        public override DataSet GetDataSetFromSp(string procedureKey, int timeout, ref object[] outputParameterValue, params object[] sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var outputCount = 0;
                var sqlParameterList = SqlParameterHelper.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);

                var result = GetDataSet(sqlParameterList, procedureKey, commandTextString, timeout);
                SqlParameterHelper.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                return result;
            }
            catch
            {
                throw;
            }
        }
        public override List<T> GetListFromSp<T>(string procedureKey, params object[] sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var sqlParameterList = SqlParameterHelper.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                return GetDataStructList<T>(sqlParameterList, procedureKey, commandTextString);
            }
            catch
            {
                throw;
            }
        }
        public override List<T> GetListFromSp<T>(string procedureKey, int timeout, params object[] sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var sqlParameterList = SqlParameterHelper.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                return GetDataStructList<T>(sqlParameterList, procedureKey, commandTextString, timeout);
            }
            catch
            {
                throw;
            }
        }
        public override List<T> GetListFromSp<T>(string procedureKey, ref object[] outputParameterValue, params object[] sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var outputCount = 0;
                var sqlParameterList = SqlParameterHelper.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                var result = GetDataStructList<T>(sqlParameterList, procedureKey, commandTextString);
                SqlParameterHelper.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                return result;
            }
            catch
            {
                throw;
            }
        }
        public override List<T> GetListFromSp<T>(string procedureKey, int timeout, ref object[] outputParameterValue, params object[] sqlParameterValue)
        {
            try
            {
                var commandTextString = string.Empty;
                var outputCount = 0;
                var sqlParameterList = SqlParameterHelper.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                var result = GetDataStructList<T>(sqlParameterList, procedureKey, commandTextString, timeout);
                SqlParameterHelper.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public override List<string> GetSqlParameterNames(string procedureKey)
        {
            try
            {
                if (!StoredProcedurePool.DbProcedures.ContainsKey(decodeConnectionString))
                {
                    StoredProcedurePool.UpdateProcedure(decodeConnectionString);
                }

                if (!StoredProcedurePool.DbProcedures[decodeConnectionString].ProcedureList.ContainsKey(procedureKey))
                {
                    return null;
                }

                var paramList = StoredProcedurePool.DbProcedures[decodeConnectionString].ProcedureList[procedureKey].ParameterObjs;
                return (from p in paramList
                        select p.Parameter).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
