using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary.Model
{
    public class ParameterObj
    {
        public string Parameter { get; set; }
        public string SqlType { get; set; }
        public int Length { get; set; }
        public bool OutputFlag { get; set; }
        public byte ParameterIndex { get; set; }
        public string DefaultValue { get; set; }
    }
}
