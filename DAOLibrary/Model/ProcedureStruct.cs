using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary
{
    public struct ProcedureStruct
    {
        public Dictionary<string, List<DbTableStruct>> procedureList;
        public List<DbTableStruct> procedureValueArray;
    }
}
