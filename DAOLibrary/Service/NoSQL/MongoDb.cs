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

        #region CRUD
        /// <summary>
        /// 批次insert
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 如果資料已存在則update, 反之insert
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 塞入資料
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 更新資料
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Update(MongoCRUDObj obj)
        {
            return Run<object, bool>((o) =>
            {
                var data = o as MongoCRUDObj;
                var c = _db.GetCollection(data.Collection);
                var result = c.Update(data.QueryFilter, data.UpdateData, data.UpdateFlag);
                return !result.HasLastErrorMessage;
            }, obj);
        }

        /// <summary>
        /// 更新符合查詢條件的資料並回傳查詢結果
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 根據輸入的欲查詢筆數回傳查詢結果
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="num"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 回傳結果與符合條件的總筆數
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="idx"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<BsonDocument>, long> QueryPaging(MongoCRUDObj obj, int idx, int num)
        {
            return Run<object, Tuple<IEnumerable<BsonDocument>, long>>((d) =>
            {
                var data = obj as MongoCRUDObj;
                var c = _db.GetCollection(data.Collection);
                if (obj.SortKeys != null)
                {
                    var rSort = c.FindAs<BsonDocument>(data.QueryFilter).SetSortOrder(obj.SortKeys);
                    var rResult = rSort.SetSkip((idx - 1) * num).SetLimit(num);
                    var rSize = rSort.Count();
                    return new Tuple<IEnumerable<BsonDocument>, long>(rResult, rSize);

                }
                var r = c.FindAs<BsonDocument>(data.QueryFilter);
                var result = r.SetSkip((idx - 1) * num).SetLimit(num);
                var size = r.Count();
                return new Tuple<IEnumerable<BsonDocument>, long>(result, size);
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

