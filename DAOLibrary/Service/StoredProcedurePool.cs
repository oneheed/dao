using DAOLibrary.Model;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace DAOLibrary.Service
{
    /// <summary>
    /// SP 清單池
    /// </summary>
    public class StoredProcedurePool
    {
        private static ConcurrentDictionary<long, ConcurrentDictionary<string, DbObj>> verProcedure = new ConcurrentDictionary<long, ConcurrentDictionary<string, DbObj>>();
        private static object lockObj = new object();
        private static int _updateSec = 86400;

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 設定多久更新記憶體中的SP清單
        /// </summary>
        /// <param name="sec"></param>
        internal protected static void SetProcedureUpdateSecond(int sec)
        {
            lock (lockObj)
            {
                _updateSec = sec;
            }
        }

        /// <summary>
        /// 目前最新版SP清單
        /// </summary>
        public static ConcurrentDictionary<string, DbObj> DbProcedures
        {
            get {
                _wait_first_loading.WaitOne();
                return verProcedure[_current_cache_version];
            }
        }





        private static long _current_cache_version = 0;
        private static long _current_loading_version = 0;


        private static ManualResetEvent _wait_first_loading = new ManualResetEvent(false);

        /// <summary>
        /// 更新SP清單
        /// </summary>
        /// <param name="connectionStringList"></param>
        //public static void UpdateProcedure(List<string> connectionStringList string connectionString)
        public static void UpdateProcedure(List<string> connectionStringList)
        {
            long expected_version = _current_loading_version + 1;
            if (Interlocked.Increment(ref _current_loading_version) > expected_version)
            {
                return;
            }

            try
            {
                _logger.Info(String.Format("Begin UpdateProcedure: current_ver => {0}, loading_ver => {1}", _current_cache_version, _current_loading_version));
                DateTime beginTime = DateTime.Now;

                // New 空的新版
                ConcurrentDictionary<string, DbObj> _new_DbProcedures = new ConcurrentDictionary<string, DbObj>(StringComparer.OrdinalIgnoreCase);
                foreach (string connectionString in connectionStringList)
                {
                    // 開始載入新版
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(Const.GET_PROCEDURE_PARAMETER, conn))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                var newDbObj = new DbObj();
                                if (dt.Rows.Count > 0)
                                {
                                    string procedureKeyString = string.Empty;
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        if (procedureKeyString != dr["ProcedureKey"].ToString().ToLower())
                                        {
                                            procedureKeyString = dr["ProcedureKey"].ToString().ToLower();
                                            var parameterQuery = (from a in dt.AsEnumerable()
                                                                  where a.Field<string>("ProcedureKey").ToLower() == procedureKeyString
                                                                  && a.Field<string>("Parameter") != null
                                                                  orderby a.Field<byte?>("ParameterIndex") ascending
                                                                  select a).ToList();

                                            var parameterObjs = new List<ParameterObj>();

                                            foreach (var param in parameterQuery)
                                            {
                                                var pObj = new ParameterObj();
                                                pObj.Parameter = param["Parameter"].ToString();
                                                pObj.SqlType = param["SqlType"].ToString().ToUpper();
                                                pObj.Length = int.Parse(param["Length"].ToString());
                                                pObj.OutputFlag = bool.Parse(param["OutputFlag"].ToString());
                                                pObj.DefaultValue = param["DefaultValue"].ToString();
                                                parameterObjs.Add(pObj);
                                            }

                                            //if (newDbObj.ProcedureList.ContainsKey(procedureKeyString) ||
                                            //    DbProcedures.Where(o => o.Value.ProcedureList.ContainsKey(procedureKeyString)).Count() > 0)
                                            //{
                                            //    throw new Exception(string.Format("Duplicate Procedure: {0}", procedureKeyString));
                                            //}

                                            try
                                            {
                                                newDbObj.ProcedureList.Add(procedureKeyString, new ProcedureObj()
                                                {
                                                    ProcedureName = dr["Name"].ToString(),
                                                    DBName = dr["DBName"].ToString(),
                                                    DBServer = dr["ServerIP"].ToString(),
                                                    ParameterObjs = parameterObjs
                                                });
                                            }
                                            catch (Exception e)
                                            {
                                                throw new Exception(string.Format("{0}: {1}", e.Message, procedureKeyString));
                                            }
                                        }
                                    }
                                    newDbObj.UpdateTime = DateTime.Now;
                                    _new_DbProcedures.TryAdd(connectionString, newDbObj);
                                }
                            }
                        }
                    }
                }
                // Add new
                verProcedure.TryAdd(_current_loading_version, _new_DbProcedures);
                // Remove old
                if (verProcedure.Count() > 1)
                {
                    ConcurrentDictionary<string, DbObj> _old_DbProcedures = new ConcurrentDictionary<string, DbObj>();
                    verProcedure.TryRemove(_current_cache_version, out _old_DbProcedures);
                }
                _current_cache_version = _current_loading_version;
                _wait_first_loading.Set();
                _logger.Info(String.Format("End UpdateProcedure: {0} milliseconds, current_ver => {1}, loading_ver => {2}",
                        (DateTime.Now - beginTime).TotalMilliseconds, _current_cache_version, _current_loading_version));
            }
            catch (Exception e)
            {
                _logger.Debug(String.Format("Begin UpdateProcedure: current_ver => {0}, loading_ver => {1}", _current_cache_version, _current_loading_version), e);
                _current_loading_version = _current_cache_version;
                throw;
            }

        }

        //private static void RenewDbProcedure(string connectionString)
        //{
        //    lock (lockObj)
        //    {
        //        try
        //        {
        //            using (SqlConnection conn = new SqlConnection(connectionString))
        //            {
        //                using (SqlCommand cmd = new SqlCommand(Const.RENEW_PROCEDURE_PARAMETER, conn))
        //                {
        //                    conn.Open();
        //                    cmd.ExecuteNonQuery();
        //                }
        //            }
        //        }
        //        catch { }
        //    }
        //}
    }
}
