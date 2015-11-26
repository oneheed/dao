using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace DAOLibrary
{
    public struct DbTableStruct
    {
        public string parameter;
        public string sqlType;
        public int length;
        public bool outputFlag;
        public byte parameterIndex;
        public string defaultValue;
    }

    public struct ProcedureStruct
    {
        public Dictionary<string, List<DbTableStruct>> procedureList;
        public List<DbTableStruct> procedureValueArray;
    }

    public static class DbDictionary
    {
        public static Dictionary<string, SqlDbType> dic = new Dictionary<string, SqlDbType>();
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

    public static class Procedure
    {
        public static Dictionary<string, Dictionary<string, List<DbTableStruct>>> procedureKey = new Dictionary<string, Dictionary<string, List<DbTableStruct>>>(StringComparer.OrdinalIgnoreCase);
        public static void GetProcedure(string connectionString)
        {
            try
            {
                //procedureKey.Clear();
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    SqlDataAdapter saCount = new SqlDataAdapter("Exec usp_getProcedureParameterCount", cn);
                    DataTable dtCount = new DataTable();
                    saCount.Fill(dtCount);

                    int _procedureKey = int.Parse(dtCount.Rows[0]["Counts"].ToString());

                    if (_procedureKey == procedureKey.Count)
                    {
                        return;
                    }

                    procedureKey.Clear();
                    SqlDataAdapter sa = new SqlDataAdapter("Exec usp_getProcedureParameter", cn);
                    DataTable dt = new DataTable();
                    sa.Fill(dt);

                    string procedureKeyString = string.Empty;

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (procedureKeyString != dr["ProcedureKey"].ToString())
                            {
                                procedureKeyString = dr["ProcedureKey"].ToString();
                                var parameterQuery = (from a in dt.AsEnumerable()
                                                      where a.Field<string>("ProcedureKey") == procedureKeyString
                                                      && a.Field<string>("Parameter") != null
                                                      orderby a.Field<byte?>("ParameterIndex") ascending
                                                      select a).ToList();

                                Dictionary<string, List<DbTableStruct>> procedureValue = new Dictionary<string, List<DbTableStruct>>();
                                List<DbTableStruct> tableStructList = new List<DbTableStruct>();
                                int Count = parameterQuery.Count();

                                if (Count > 0)
                                {
                                    for (int i = 0; i < Count; i++)
                                    {
                                        DbTableStruct tableStruct = new DbTableStruct();
                                        tableStruct.parameter = parameterQuery[i]["Parameter"].ToString();
                                        tableStruct.sqlType = parameterQuery[i]["SqlType"].ToString().ToUpper();
                                        tableStruct.length = int.Parse(parameterQuery[i]["Length"].ToString());
                                        tableStruct.outputFlag = bool.Parse(parameterQuery[i]["OutputFlag"].ToString());
                                        tableStruct.defaultValue = parameterQuery[i]["DefaultValue"].ToString();
                                        tableStructList.Add(tableStruct);
                                    }
                                }

                                procedureValue.Add(dr["Name"].ToString(), tableStructList);
                                procedureKey.Add(procedureKeyString, procedureValue);
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }

    public abstract class IDataAccess
    {
        protected string decodeConnectionString = string.Empty;
        public IDataAccess(string connectionString)
        {
            decodeConnectionString = Cryptography.DESDecode(connectionString);
        }
        public abstract int ExecuteSp(string procedureKey, params object[] sqlParameterValue);
        public abstract int ExecuteSp(string procedureKey, ref object[] outputParameterValue, params object[] sqlParameterValue);
        public abstract int ExecuteSp(string procedureKey, Dictionary<string, object> sqlParameterValue);
        public abstract int ExecuteSp(string procedureKey, ref object[] outputParameterValue, Dictionary<string, object> sqlParameterValue);
        public abstract DataSet GetDataSetFromSp(string procedureKey, params object[] sqlParameterValue);
        public abstract DataSet GetDataSetFromSp(string procedureKey, int timeout, params object[] sqlParameterValue);
        public abstract DataSet GetDataSetFromSp(string procedureKey, ref object[] outputParameterValue, params object[] sqlParameterValue);
        public abstract DataSet GetDataSetFromSp(string procedureKey, int timeout, ref object[] outputParameterValue, params object[] sqlParameterValue);
        public abstract DataSet GetDataSetFromSp(string procedureKey, Dictionary<string, object> sqlParameterValue);
        public abstract DataSet GetDataSetFromSp(string procedureKey, int timeout, Dictionary<string, object> sqlParameterValue);
        public abstract DataSet GetDataSetFromSp(string procedureKey, ref object[] outputParameterValue, Dictionary<string, object> sqlParameterValue);
        public abstract DataSet GetDataSetFromSp(string procedureKey, int timeout, ref object[] outputParameterValue, Dictionary<string, object> sqlParameterValue);
        public abstract List<O> GetListFromSp<O>(string procedureKey, params object[] sqlParameterValue) where O : new();
        public abstract List<O> GetListFromSp<O>(string procedureKey, int timeout, params object[] sqlParameterValue) where O : new();
        public abstract List<O> GetListFromSp<O>(string procedureKey, ref object[] outputParameterValue, params object[] sqlParameterValue) where O : new();
        public abstract List<O> GetListFromSp<O>(string procedureKey, int timeout, ref object[] outputParameterValue, params object[] sqlParameterValue) where O : new();
        public abstract List<O> GetListFromSp<O>(string procedureKey, Dictionary<string, object> sqlParameterValue) where O : new();
        public abstract List<O> GetListFromSp<O>(string procedureKey, int timeout, Dictionary<string, object> sqlParameterValue) where O : new();
        public abstract List<O> GetListFromSp<O>(string procedureKey, ref object[] outputParameterValue, Dictionary<string, object> sqlParameterValue) where O : new();
        public abstract List<O> GetListFromSp<O>(string procedureKey, int timeout, ref object[] outputParameterValue, Dictionary<string, object> sqlParameterValue) where O : new();
    }

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
