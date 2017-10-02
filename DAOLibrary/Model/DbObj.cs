using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary.Model
{
    public class DbObj
    {
        public Dictionary<string, ProcedureObj> ProcedureList { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool loadComplete { get; set; }

        public DbObj()
        {
            this.loadComplete = false;
            UpdateTime = DateTime.Now.AddYears(-1);
            ProcedureList = new Dictionary<string, ProcedureObj>();
        }
    }
}
