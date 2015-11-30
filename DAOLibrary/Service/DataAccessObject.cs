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
        public SqlDataAccess(string connectionString) : base(connectionString) { }
        public override int ExecuteSp(string procedureKey, params object[] sqlParameterValue)
        {
            try
            {
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    sqlParameterList = SqlParameterObject.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cn.Open();
                    int Count = cmd.ExecuteNonQuery();
                    return Count;
                }
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
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    int outputCount = 0;
                    sqlParameterList = SqlParameterObject.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cn.Open();
                    int Count = cmd.ExecuteNonQuery();
                    SqlParameterObject.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                    return Count;
                }
            }
            catch
            {
                throw;
            }
        }
        public override int ExecuteSp(string procedureKey, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    sqlParameterList = SqlParameterObject.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cn.Open();
                    int Count = cmd.ExecuteNonQuery();
                    return Count;
                }
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
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    int outputCount = 0;
                    sqlParameterList = SqlParameterObject.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cn.Open();
                    int Count = cmd.ExecuteNonQuery();
                    SqlParameterObject.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                    return Count;
                }
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
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    sqlParameterList = SqlParameterObject.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    SqlDataAdapter sa = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sa.Fill(ds);
                    return ds;
                }
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
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    sqlParameterList = SqlParameterObject.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandTimeout = timeout > 0 ? timeout : 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    SqlDataAdapter sa = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sa.Fill(ds);
                    return ds;
                }
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
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    int outputCount = 0;
                    sqlParameterList = SqlParameterObject.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    SqlDataAdapter sa = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sa.Fill(ds);
                    SqlParameterObject.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                    return ds;
                }
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
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    int outputCount = 0;
                    sqlParameterList = SqlParameterObject.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandTimeout = timeout > 0 ? timeout : 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    SqlDataAdapter sa = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sa.Fill(ds);
                    SqlParameterObject.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                    return ds;
                }
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
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    sqlParameterList = SqlParameterObject.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    SqlDataAdapter sa = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sa.Fill(ds);
                    return ds;
                }
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
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    sqlParameterList = SqlParameterObject.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandTimeout = timeout > 0 ? timeout : 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    SqlDataAdapter sa = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sa.Fill(ds);
                    return ds;
                }
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
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    int outputCount = 0;
                    sqlParameterList = SqlParameterObject.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    SqlDataAdapter sa = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sa.Fill(ds);
                    SqlParameterObject.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                    return ds;
                }
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
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    int outputCount = 0;
                    sqlParameterList = SqlParameterObject.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandTimeout = timeout > 0 ? timeout : 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    SqlDataAdapter sa = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sa.Fill(ds);
                    SqlParameterObject.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                    return ds;
                }
            }
            catch
            {
                throw;
            }
        }
        public override List<O> GetListFromSp<O>(string procedureKey, params object[] sqlParameterValue)
        {
            try
            {
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    sqlParameterList = SqlParameterObject.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cn.Open();
                    SqlDataReader sr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    O outputDataStruct = new O();
                    List<O> dataStructList = SqlDataAccess.SqlDataReaderToObjectList<O>(ref outputDataStruct, sr);
                    return dataStructList;
                }
            }
            catch
            {
                throw;
            }
        }
        public override List<O> GetListFromSp<O>(string procedureKey, int timeout, params object[] sqlParameterValue)
        {
            try
            {
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    sqlParameterList = SqlParameterObject.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandTimeout = timeout > 0 ? timeout : 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cn.Open();
                    SqlDataReader sr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    O outputDataStruct = new O();
                    List<O> dataStructList = SqlDataAccess.SqlDataReaderToObjectList<O>(ref outputDataStruct, sr);
                    return dataStructList;
                }
            }
            catch
            {
                throw;
            }
        }
        public override List<O> GetListFromSp<O>(string procedureKey, ref object[] outputParameterValue, params object[] sqlParameterValue)
        {
            try
            {
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    int outputCount = 0;
                    sqlParameterList = SqlParameterObject.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cn.Open();
                    SqlDataReader sr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    O outputDataStruct = new O();
                    List<O> dataStructList = SqlDataAccess.SqlDataReaderToObjectList<O>(ref outputDataStruct, sr);
                    SqlParameterObject.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                    return dataStructList;
                }
            }
            catch
            {
                throw;
            }
        }
        public override List<O> GetListFromSp<O>(string procedureKey, int timeout, ref object[] outputParameterValue, params object[] sqlParameterValue)
        {
            try
            {
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    int outputCount = 0;
                    sqlParameterList = SqlParameterObject.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandTimeout = timeout > 0 ? timeout : 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cn.Open();
                    SqlDataReader sr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    O outputDataStruct = new O();
                    List<O> dataStructList = SqlDataAccess.SqlDataReaderToObjectList<O>(ref outputDataStruct, sr);
                    SqlParameterObject.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                    return dataStructList;
                }
            }
            catch
            {
                throw;
            }
        }
        public override List<O> GetListFromSp<O>(string procedureKey, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    sqlParameterList = SqlParameterObject.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cn.Open();
                    SqlDataReader sr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    O outputDataStruct = new O();
                    List<O> dataStructList = SqlDataAccess.SqlDataReaderToObjectList<O>(ref outputDataStruct, sr);
                    return dataStructList;
                }
            }
            catch
            {
                throw;
            }
        }
        public override List<O> GetListFromSp<O>(string procedureKey, int timeout, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    sqlParameterList = SqlParameterObject.GetSqlParameter(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandTimeout = timeout > 0 ? timeout : 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cn.Open();
                    SqlDataReader sr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    O outputDataStruct = new O();
                    List<O> dataStructList = SqlDataAccess.SqlDataReaderToObjectList<O>(ref outputDataStruct, sr);
                    return dataStructList;
                }
            }
            catch
            {
                throw;
            }
        }
        public override List<O> GetListFromSp<O>(string procedureKey, ref object[] outputParameterValue, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    int outputCount = 0;
                    sqlParameterList = SqlParameterObject.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cn.Open();
                    SqlDataReader sr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    O outputDataStruct = new O();
                    List<O> dataStructList = SqlDataAccess.SqlDataReaderToObjectList<O>(ref outputDataStruct, sr);
                    SqlParameterObject.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                    return dataStructList;
                }
            }
            catch
            {
                throw;
            }
        }
        public override List<O> GetListFromSp<O>(string procedureKey, int timeout, ref object[] outputParameterValue, Dictionary<string, object> sqlParameterValue)
        {
            try
            {
                Procedure.GetProcedure(decodeConnectionString);

                using (SqlConnection cn = new SqlConnection(decodeConnectionString))
                {
                    string commandTextString = string.Empty;
                    List<SqlParameter> sqlParameterList = new List<SqlParameter>();
                    int outputCount = 0;
                    sqlParameterList = SqlParameterObject.GetSqlParameterWithOutput(decodeConnectionString, procedureKey, ref commandTextString, sqlParameterValue, ref outputCount);
                    SqlCommand cmd = new SqlCommand(commandTextString, cn);
                    cmd.CommandTimeout = timeout > 0 ? timeout : 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cn.Open();
                    SqlDataReader sr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    O outputDataStruct = new O();
                    List<O> dataStructList = SqlDataAccess.SqlDataReaderToObjectList<O>(ref outputDataStruct, sr);
                    SqlParameterObject.OutputSqlParameter(sqlParameterList, outputCount, ref outputParameterValue);
                    return dataStructList;
                }
            }
            catch
            {
                throw;
            }
        }
        public static List<T> SqlDataReaderToObjectList<T>(ref T outputObject, SqlDataReader sqlDataReader) where T : new()
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
    }
}
