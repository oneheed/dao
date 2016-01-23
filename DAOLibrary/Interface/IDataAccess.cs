using DAOLibrary.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary
{
    public abstract class IDataAccess
    {
        protected string decodeConnectionString = string.Empty;
        private bool _isTest = false;
        public IDataAccess(string connectionString, bool isTest, int procUpdateSec)
        {
            decodeConnectionString = Cryptography.DESDecode(connectionString);
            _isTest = isTest;
            StoredProcedurePool.SetProcedureUpdateSecond(procUpdateSec);
        }

        protected string GetSqlConnectionStr(string procedureKey)
        {
            foreach (var connStr in StoredProcedurePool.DbProcedures.Keys)
            {
                foreach (var proc in StoredProcedurePool.DbProcedures[connStr].ProcedureList)
                {
                    if (proc.Key.Equals(procedureKey))
                    {
                        return string.Format(Const.TMP_MSSQL_CONN_STR, proc.Value.DBServer, proc.Value.DBName, !_isTest, "nickchen", "just4nick");
                    }
                }
            }
            throw new Exception(string.Format("Cannot get db server for {0}", procedureKey));
        }


        public abstract List<string> GetSqlParameterNames(string procedureKey);
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
        public abstract List<T> GetListFromSp<T>(string procedureKey, params object[] sqlParameterValue) where T : new();
        public abstract List<T> GetListFromSp<T>(string procedureKey, int timeout, params object[] sqlParameterValue) where T : new();
        public abstract List<T> GetListFromSp<T>(string procedureKey, ref object[] outputParameterValue, params object[] sqlParameterValue) where T : new();
        public abstract List<T> GetListFromSp<T>(string procedureKey, int timeout, ref object[] outputParameterValue, params object[] sqlParameterValue) where T : new();
        public abstract List<T> GetListFromSp<T>(string procedureKey, Dictionary<string, object> sqlParameterValue) where T : new();
        public abstract List<T> GetListFromSp<T>(string procedureKey, int timeout, Dictionary<string, object> sqlParameterValue) where T : new();
        public abstract List<T> GetListFromSp<T>(string procedureKey, ref object[] outputParameterValue, Dictionary<string, object> sqlParameterValue) where T : new();
        public abstract List<T> GetListFromSp<T>(string procedureKey, int timeout, ref object[] outputParameterValue, Dictionary<string, object> sqlParameterValue) where T : new();
    }
}
