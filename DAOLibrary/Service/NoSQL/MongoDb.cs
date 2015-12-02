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
    public class MongoDb
    {
        private MongoDatabase _db;

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
            return string.Format(Const.TMP_MONGODB_AUTH_CONN_STR, user, password, server);
        }

        private string _GetConnectStr(string server)
        {
            return string.Format(Const.TMP_MONGODB_CONN_STR, server);
        }
        #endregion

        #region CRUD
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

        public MongoCursor<BsonDocument> Update(MongoCRUDObj obj)
        {
            return Run<object, MongoCursor<BsonDocument>>((o) =>
            {
                var data = o as MongoCRUDObj;
                var c = _db.GetCollection(data.Collection);
                c.Update(data.QueryFilter, data.UpdateData);
                return Query(obj);
            }, obj);
        }

        public MongoCursor<BsonDocument> Query(MongoCRUDObj obj)
        {
            return Run<object, MongoCursor<BsonDocument>>((d) =>
            {
                var data = obj as MongoCRUDObj;
                var c = _db.GetCollection(data.Collection);
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

