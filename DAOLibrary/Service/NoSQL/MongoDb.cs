using DAOLibrary.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary.Service.NoSQL
{
    class MongoDb
    {
        private MongoDatabase _db;

        #region Connection
        internal void SetConnection(string server, string user, string password, string dbName)
        {
            var connStr = GetAuthConnectStr(server, user, password);
            var client = new MongoClient(connStr);
            this._db = client.GetServer().GetDatabase(dbName);
        }

        internal void SetConnection(string server, string dbName)
        {
            var connStr = GetConnectStr(server);
            var client = new MongoClient(connStr);
            this._db = client.GetServer().GetDatabase(dbName);
        }

        private string GetAuthConnectStr(string server, string user, string password)
        {
            return string.Format(Const.TMP_MONGODB_AUTH_CONN_STR, user, password, server);
        }

        private string GetConnectStr(string server)
        {
            return string.Format(Const.TMP_MONGODB_CONN_STR, server);
        }
        #endregion

        #region CRUD
        public bool Insert<T>(T obj)
        {
            return Run<object, bool>((o) =>
            {
                var data = o as MongoCRUDObj;
                var c = _db.GetCollection<T>(data.Collection);
                c.Insert(data.InsertData);
                return true;
            }, obj);
        }

        public MongoCursor<BsonDocument> Update<T>(T obj)
        {
            return Run<object, MongoCursor<BsonDocument>>((o) =>
            {
                var data = o as MongoCRUDObj;
                var c = _db.GetCollection<T>(data.Collection);
                c.Update(data.QueryFilter, data.UpdateData);
                return Query<T>(obj);
            }, obj as MongoCRUDObj);
        }

        public MongoCursor<BsonDocument> Query<T>(T obj)
        {
            return Run<object, MongoCursor<BsonDocument>>((d) =>
            {
                var data = obj as MongoCRUDObj;
                var c = _db.GetCollection<T>(data.Collection);
                return c.FindAs<BsonDocument>(data.QueryFilter);
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

