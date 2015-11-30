using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary
{
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
                    SqlDataAdapter saCount = new SqlDataAdapter(Const.GET_PROCEDURE_PARAMETER_COUNT, cn);
                    DataTable dtCount = new DataTable();
                    saCount.Fill(dtCount);

                    int _procedureKey = int.Parse(dtCount.Rows[0]["Counts"].ToString());

                    if (_procedureKey == procedureKey.Count)
                    {
                        return;
                    }

                    procedureKey.Clear();
                    SqlDataAdapter sa = new SqlDataAdapter(Const.GET_PROCEDURE_PARAMETER, cn);
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
}
