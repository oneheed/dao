using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary.Model
{
    public class MongoCRUDObj
    {
        public string Collection { get; set; }
        public object InsertData { get; set; }
        public IMongoQuery QueryFilter { get; set; }
        public IMongoUpdate UpdateData { get; set; }
        public IMongoSortBy SortKeys { get; set; }
    }
}
