using Core.Caching;
using Core.Configuration;
using Core.Infrastructure;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Caching
{
    /// <summary>
    /// redi缓存
    /// </summary>
    public partial class RedisCacheManager : ICacheManager
    {
        #region Field

        private readonly ConnectionMultiplexer _muxer;

        private readonly IDatabase _db;

        private readonly ICacheManager _preRequestCacheManager;

        #endregion

        public RedisCacheManager(NopConfig config)
        {
            if (string.IsNullOrEmpty(config.RedisCachingConnectionString))
                throw new Exception("没有提供相关的redis连接字符串");

            this._muxer = ConnectionMultiplexer.Connect(config.RedisCachingConnectionString);
            this._db = _muxer.GetDatabase();
            this._preRequestCacheManager = EngineContext.Current.Resolve<ICacheManager>();
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual byte[] Serialize(object item)
        {
            var jsonString = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(jsonString);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        protected virtual T Deserialize<T>(byte[] serializedObject)
        {
            if (serializedObject == null)
                return default(T);
            var jsonString = Encoding.UTF8.GetString(serializedObject);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public void Clear()
        {
            foreach (var ep in _muxer.GetEndPoints())
            {
                var server = _muxer.GetServer(ep);
                var keys = server.Keys();
                foreach (var key in keys)
                    _db.KeyDelete(key);
            }
        }

        public void Dispose()
        {
            if (_muxer != null)
                _muxer.Dispose();
        }

        public T Get<T>(string key)
        {
            if (_preRequestCacheManager.IsSet(key))
                return _preRequestCacheManager.Get<T>(key);

            var rValue = _db.StringGet(key);
            if (!rValue.HasValue)
                return default(T);
            var result = Deserialize<T>(rValue);

            _preRequestCacheManager.Set(key, result, 0);
            return result;
        }

        public bool IsSet(string key)
        {
            if (_preRequestCacheManager.IsSet(key))
                return true;

            return _db.KeyExists(key);
        }

        public void Remove(string key)
        {
            _db.KeyDelete(key);
        }

        public void RemoveByPattern(string pattern)
        {
            foreach(var ep in _muxer.GetEndPoints())
            {
                var server = _muxer.GetServer(ep);
                var keys = server.Keys(pattern: "*" + pattern + "*");
                foreach (var key in keys)
                    _db.KeyDelete(key);
            }
        }

        public void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            var entryBytes = Serialize(data);
            var expiresIn = TimeSpan.FromMinutes(cacheTime);
            _db.StringSet(key, entryBytes, expiresIn);
        }
    }
}
