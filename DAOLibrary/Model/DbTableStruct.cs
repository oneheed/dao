using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary
{
    public struct DbTableStruct
    {
        public string parameter;
        public string sqlType;
        public int length;
        public bool outputFlag;
        public byte parameterIndex;
        public string defaultValue;
    }
}
