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
