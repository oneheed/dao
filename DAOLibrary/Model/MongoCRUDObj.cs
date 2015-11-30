using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary.Model
{
    class MongoCRUDObj
    {
        public string Collection { get; set; }
        public object InsertData { get; set; }
        public IMongoQuery QueryFilter { get; set; }
        public IMongoUpdate UpdateData { get; set; }
    }
}
