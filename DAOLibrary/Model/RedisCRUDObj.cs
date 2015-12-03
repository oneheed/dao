using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary.Model
{
    public class RedisCRUDObj
    {
        public Dictionary<string, object> InsertData { get; set; }
        public List<string> QueryFilter { get; set; }
    }
}
