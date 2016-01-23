using DAOLibrary.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAOLibrary.Service
{
    public class StoredProcedurePool
    {
        private static ConcurrentDictionary<string, DbObj> _DbProcedures = new ConcurrentDictionary<string, DbObj>(StringComparer.OrdinalIgnoreCase);
        private static object lockObj = new object();
        private static int _updateSec = 60;
        internal protected static void SetProcedureUpdateSecond(int sec)
        {
            lock (lockObj)
            {
                _updateSec = sec;
            }
        }

        public static Dictionary<string, DbObj> DbProcedures
        {
            get { return new Dictionary<string, DbObj>(_DbProcedures); }
        }

        public static void UpdateProcedure(string connectionString)
        {
            try
            {
                var oriDbObj = _DbProcedures.GetOrAdd(connectionString, (o) => { return new DbObj(); });

                if ((DateTime.Now - oriDbObj.UpdateTime).TotalSeconds > _updateSec)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        SqlDataAdapter sda = new SqlDataAdapter(Const.GET_PROCEDURE_PARAMETER, conn);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        var newDbObj = new DbObj();
                        if (dt.Rows.Count > 0)
                        {
                            string procedureKeyString = string.Empty;
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
                                    if (newDbObj.ProcedureList.ContainsKey(procedureKeyString))
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
                        }
                    }
                }
            }
            catch
            {
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
