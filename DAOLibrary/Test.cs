using DAOLibrary.Model;
using DAOLibrary.Service;
using DAOLibrary.Service.NoSQL;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary
{
    class Test
    {
        static void Main(string[] args)
        {
            var op = new MongoDb();
            op.SetConnection("10.10.102.112", "sa", "1111", "emp");
            //op.SetConnection("10.10.102.112", "emp");

            MongoCRUDObj obj = new MongoCRUDObj()
            {
                Collection = "emp",
                //InsertData = new { name = "JJ", age = 55 },
                QueryFilter = Query.And(
                    Query.GT("age", 20)
                    //, Query.EQ("name", "Joe")
                    )
            };

            try
            {
                //var r = op.Insert<MongoCRUDObj>(obj);

                var r2 = op.Query(obj);

                foreach (var e in r2)
                    Console.WriteLine(e.ToJson());
            }
            catch (Exception e) { Console.WriteLine(e); }
        }
    }
}
