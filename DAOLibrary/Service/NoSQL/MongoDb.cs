using DAOLibrary.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary.Service.NoSQL
{
    public class MongoDb
    {
        private MongoDatabase _db;

        public MongoDb() { }

        public MongoDb(string server, string user, string password, string dbName)
        {
            if (_db == null)
            {
                SetConnection(server, user, password, dbName);
            }
        }

        public MongoDb(string server, string dbName)
        {
            if (_db == null)
            {
                SetConnection(server, dbName);
            }
        }

        #region Connection
        public void SetConnection(string server, string user, string password, string dbName)
        {
            var connStr = _GetAuthConnectStr(server, user, password);
            _SetConnection(connStr, dbName);
        }

        public void SetConnection(string server, string dbName)
        {
            var connStr = _GetConnectStr(server);
            _SetConnection(connStr, dbName);
        }

        private void _SetConnection(string connStr, string dbName)
        {
            var client = new MongoClient(connStr);
            this._db = client.GetServer().GetDatabase(dbName);
        }

        private string _GetAuthConnectStr(string server, string user, string password)
        {
            if (server.StartsWith("mongodb://", StringComparison.CurrentCultureIgnoreCase))
            {
                server = server.Remove(0, "mongodb://".Length);
            }
            return string.Format(Const.TMP_MONGODB_AUTH_CONN_STR, user, password, server);
        }

        private string _GetConnectStr(string server)
        {
            if (server.StartsWith("mongodb://", StringComparison.CurrentCultureIgnoreCase))
            {
                server = server.Remove(0, "mongodb://".Length);
            }
            return string.Format(Const.TMP_MONGODB_CONN_STR, server);
        }
        #endregion

        public bool BulkInsert(MongoCRUDObj obj)
        {
            return Run<object, bool>((o) =>
            {
                var data = o as MongoCRUDObj;
                var c = _db.GetCollection(data.Collection);
                var bulk = c.InitializeUnorderedBulkOperation();
                foreach (var sObj in obj.BulkInsertData)
                {
                    bulk.Insert(sObj.ToBsonDocument());
                }
                bulk.Execute();
                return true;
            }, obj);
        }

        #region CRUD
        public bool Upsert(MongoCRUDObj obj)
        {
            return Run<object, bool>((o) =>
            {
                var data = o as MongoCRUDObj;
                var c = _db.GetCollection(data.Collection);
                c.Update(data.QueryFilter, data.UpdateData, UpdateFlags.Upsert);
                return true;
            }, obj);
        }

        public bool Insert(MongoCRUDObj obj)
        {
            return Run<object, bool>((o) =>
            {
                var data = o as MongoCRUDObj;
                var c = _db.GetCollection(data.Collection);
                c.Insert(data.InsertData);
                return true;
            }, obj);
        }

        public bool Update(MongoCRUDObj obj)
        {
            return Run<object, bool>((o) =>
            {
                var data = o as MongoCRUDObj;
                var c = _db.GetCollection(data.Collection);
                var result = c.Update(data.QueryFilter, data.UpdateData, UpdateFlags.Multi);
                return !result.HasLastErrorMessage;
            }, obj);
        }

        public IEnumerable<BsonDocument> QueryAndUpdate(MongoCRUDObj obj)
        {
            return Run<object, List<BsonDocument>>((o) =>
            {
                var data = o as MongoCRUDObj;
                var c = _db.GetCollection(data.Collection);
                var result = c.FindAndModify(new FindAndModifyArgs()
                {
                    Query = obj.QueryFilter,
                    SortBy = obj.SortKeys,
                    Update = obj.UpdateData,
                    VersionReturned = FindAndModifyDocumentVersion.Modified
                });
                return new List<BsonDocument>() { result.ModifiedDocument };
            }, obj);
        }

        public IEnumerable<BsonDocument> Query(MongoCRUDObj obj, int num = int.MaxValue)
        {
            return Run<object, List<BsonDocument>>((d) =>
            {
                var data = obj as MongoCRUDObj;
                var c = _db.GetCollection(data.Collection);
                if (obj.SortKeys != null)
                {
                    return c.FindAs<BsonDocument>(data.QueryFilter).SetSortOrder(obj.SortKeys).SetLimit(num).ToList();
                }
                return c.FindAs<BsonDocument>(data.QueryFilter).SetLimit(num).ToList();
            }, obj);
        }

        public bool Delete<T>(T obj)
        {
            return Run<object, bool>((d) =>
            {
                var data = obj as MongoCRUDObj;
                var c = _db.GetCollection(data.Collection);
                c.Remove(data.QueryFilter);
                return true;
            }, obj);
        }
        #endregion

        private T2 Run<T, T2>(Func<T, T2> func, T i)
        {
            if (this._db != null)
            {
                try
                {
                    return func(i);
                }
                catch { throw; }
            }
            else
            {
                throw new Exception("Set Connection First!");
            }
        }
    }
}

