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
        private static readonly int _UpdateSeconds = 3;

        public static Dictionary<string, DbObj> DbProcedures
        {
            get { return new Dictionary<string, DbObj>(_DbProcedures); }
        }

        public static void UpdateProcedure(string connectionString)
        {
            try
            {
                var oriDbObj = _DbProcedures.GetOrAdd(connectionString, (o) => { return new DbObj(); });

                if ((DateTime.Now - oriDbObj.UpdateTime).TotalSeconds > _UpdateSeconds)
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

                                    newDbObj.ProcedureList.Add(procedureKeyString, new ProcedureObj() { ProcedureName = dr["Name"].ToString(), ParameterObjs = parameterObjs });
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
    }
}
