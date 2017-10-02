using DAOLibrary.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAOLibrary.Service
{
    /// <summary>
    /// SP 清單池
    /// </summary>
    public class StoredProcedurePool
    {
        private static ConcurrentDictionary<string, DbObj> _DbProcedures = new ConcurrentDictionary<string, DbObj>(StringComparer.OrdinalIgnoreCase);
        private static object lockObj = new object();
        private static int _updateSec = 86400;
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

        public static IDictionary<string, DbObj> DbProcedures
        {
            get { return _DbProcedures; }
        }

        /// <summary>
        /// 更新SP清單
        /// </summary>
        /// <param name="connectionString"></param>
        public static void UpdateProcedure(string connectionString)
        {
            try
            {
                var oriDbObj = _DbProcedures.GetOrAdd(connectionString, (o) => { return new DbObj(); });

                if ((DateTime.Now - oriDbObj.UpdateTime).TotalSeconds > _updateSec)
                {
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

                                            if (newDbObj.ProcedureList.ContainsKey(procedureKeyString) || 
                                                _DbProcedures.Where(o => o.Value.ProcedureList.ContainsKey(procedureKeyString)).Count() > 0)
                                            {
                                                throw new Exception(string.Format("Duplicate Procedure: {0}", procedureKeyString));
                                            }

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
                                    _DbProcedures.TryUpdate(connectionString, newDbObj, oriDbObj);
                                    newDbObj.loadComplete = true;
                                }
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

        /// <summary>
        ///當SP清單尚未備妥之前直接查詢DB，取得指定 SP 的參數清單
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="procedureKey"></param>
        public static DbObj GetOneDbObjFromDB(string connectionString, string procedureKey)
        {
            DbObj dbObj = new DbObj();
            if(String.IsNullOrEmpty(connectionString.Trim()) || String.IsNullOrEmpty(procedureKey.Trim()))
            {
                return null;
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string strCommandText = string.Format(Const.GET_ONE_PROCEDURE_PARAMETER, procedureKey);
                using (SqlDataAdapter sda = new SqlDataAdapter(strCommandText, conn))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
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

                                    if (dbObj.ProcedureList.ContainsKey(procedureKeyString) ||
                                        _DbProcedures.Where(o => o.Value.ProcedureList.ContainsKey(procedureKeyString)).Count() > 0)
                                    {
                                        throw new Exception(string.Format("Duplicate Procedure: {0}", procedureKeyString));
                                    }

                                    try
                                    {
                                        dbObj.ProcedureList.Add(procedureKeyString, new ProcedureObj()
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
                            dbObj.UpdateTime = DateTime.Now;
                            dbObj.loadComplete = true;
                        }
                    }
                }
            }
            return dbObj;
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
