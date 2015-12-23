using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary.Model
{
    public class ProcedureObj
    {
        public string ProcedureName { get; set; }
        public string DBName { get;set; }
        public string DBServer { get; set; }
        public List<ParameterObj> ParameterObjs { get; set; }
    }
}
