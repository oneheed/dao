using DAOLibrary.Model;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibrary.Service.NoSQL
{
    public class RedisDb
    {
        private RedisClient _db;
        #region Connection
        public void SetConnection(string server, int port, string password)
        {
            var redisClient = new RedisClient(server, port, password);
            this._db = redisClient;
        }

        public void SetConnection(string server, int port)
        {
            var redisClient = new RedisClient(server, port);
            this._db = redisClient;
        }
        #endregion

        public bool Insert(RedisCRUDObj obj)
        {
            try
            {
                _db.SetValues((from o in obj.InsertData
                               select o).ToDictionary(o => o.Key, o => JsonConvert.SerializeObject(o.Value))
                               );

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Dictionary<string, string> Query(RedisCRUDObj obj)
        {
            try
            {
                return _db.GetValuesMap(obj.QueryFilter);
            }
            catch
            {
                return null;
            }
        }

        public bool Update(RedisCRUDObj obj)
        {
            return Insert(obj);
        }

        public bool Delete(RedisCRUDObj obj)
        {
            try
            {
                _db.RemoveAll(obj.QueryFilter);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
