using Microsoft.Extensions.Configuration;
using NewLife.Caching;
using NewLife.Caching.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shiny.Redis
{
    public class RedisCacheManager : IRedisCacheManager
    {
        private readonly string redisConnenctionString;
        public volatile FullRedis redisConnection;
        private readonly object redisConnectionLock = new object();
        public RedisCacheManager(IConfiguration configuration)
        {

            string redisConfiguration = configuration["ConnectionStrings:Redis"];//获取连接字符串
            //string Db = Appsettings.App(new string[] { "AppSettings", "RedisCaching", "Db" });
            if (string.IsNullOrWhiteSpace(redisConfiguration))
            {
                throw new ArgumentException("redis config [ConnectionStrings: Redis] is empty", nameof(redisConfiguration));
            }
            //FullRedis.Register();
            this.redisConnenctionString = redisConfiguration;
            this.redisConnection = GetRedisConnection();
        }

        public RedisCacheManager(string redisConfiguration)
        {
            if (string.IsNullOrWhiteSpace(redisConfiguration))
            {
                throw new ArgumentException("redis config is empty", nameof(redisConfiguration));
            }
            //FullRedis.Register();
            this.redisConnenctionString = redisConfiguration;
            this.redisConnection = GetRedisConnection();
        }

        /// <summary>
        /// 核心代码，获取连接实例
        /// 通过双if 夹lock的方式，实现单例模式
        /// </summary>
        /// <returns></returns>
        private FullRedis GetRedisConnection()
        {
            //如果已经连接实例，直接返回
            if (this.redisConnection != null)
            {
                return this.redisConnection;
            }
            //加锁，防止异步编程中，出现单例无效的问题
            lock (redisConnectionLock)
            {
                if (this.redisConnection != null)
                {
                    //释放redis连接
                    this.redisConnection.Dispose();
                }
                try
                {

                    this.redisConnection = FullRedis.Create(redisConnenctionString);
                    this.redisConnection.Timeout = 10000;
                    Console.WriteLine("redis启动成功!");

                }
                catch (Exception ex)
                {

                    throw new Exception("Redis服务未启用，请开启该服务");
                }
            }
            return this.redisConnection;
        }


        #region 普通
        public TEntity Get<TEntity>(string key)
        {
            return redisConnection.Get<TEntity>(key);
        }

        public void Set<TEntity>(string key, TEntity value, TimeSpan cacheTime)
        {
            if (value != null)
            {
                redisConnection.Set(key, value, cacheTime);
            }
        }

        public void Set(string key, object value, TimeSpan cacheTime)
        {
            redisConnection.Set(key, value, cacheTime);
        }

        public bool ContainsKey(string key)
        {

            return redisConnection.ContainsKey(key);
        }

        public void Remove(string key)
        {
            redisConnection.Remove(key);
        }

        public List<string> Search(SearchModel model)
        {

            return redisConnection.Search(model).ToList();
        }

        public void Clear()
        {
            redisConnection.Clear();
        }

        public void SetExpire(string key, TimeSpan timeSpan)
        {
            redisConnection.SetExpire(key, timeSpan);
        }


        public async Task RemoveByKey(string key, int count)
        {
            await Task.Run(() =>
            {
                var keys = redisConnection.Search(key, count).ToList();
                foreach (var k in keys)
                    redisConnection.Remove(k);
            });
        }


        public async Task RemoveAllByKey(string key, int count = 999)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    var keyList = redisConnection.Search(key, count).ToList();
                    if (keyList.Count > 0)
                    {
                        foreach (var k in keyList)
                            redisConnection.Remove(k);
                    }
                    else
                    {
                        break;
                    }
                }
            });
        }

        #endregion

        #region HashMap
        public bool HashSetWithResult<T>(string key, Dictionary<string, T> dic, int expire = -1)
        {
            var hash = redisConnection.GetDictionary<T>(key) as RedisHash<string, T>;
            var result = hash.HMSet(dic);
            return result;

        }

        public bool HashSet<T>(string key, Dictionary<string, T> dic, int expire = -1)
        {
            var hash = redisConnection.GetDictionary<T>(key) as RedisHash<string, T>;
            var result = hash.HMSet(dic);
            return result;
        }

        public string[] HashGet(string key, params string[] fields)
        {
            var hash = redisConnection.GetDictionary<string>(key) as RedisHash<string, string>;
            var result = hash.HMGet(fields);
            return result;
        }

        public List<T> HashGet<T>(string key, params string[] fields)
        {
            var hash = redisConnection.GetDictionary<T>(key) as RedisHash<string, T>;
            var result = hash.HMGet(fields);
            return result.ToList();
        }

        public IDictionary<string, T> HashGetAll<T>(string key)
        {
            var hash = redisConnection.GetDictionary<T>(key) as RedisHash<string, T>;
            var result = hash.GetAll();
            return result;
        }

        public int HashDel<T>(string key, params string[] fields)
        {
            var hash = redisConnection.GetDictionary<T>(key) as RedisHash<string, T>;
            var result = hash.HDel(fields);
            return result;
        }
        #endregion

        #region 队列

        public int AddQueue<T>(string key, T[] value)
        {
            var queue = redisConnection.GetQueue<T>(key);
            return queue.Add(value);
        }

        public int AddQueue<T>(string key, T value)
        {
            var queue = redisConnection.GetQueue<T>(key);
            return queue.Add(value);
        }


        public List<T> GetQueue<T>(string key, int Count = 1)
        {
            var queue = redisConnection.GetQueue<T>(key);
            var result = queue.Take(Count).ToList();
            return result;
        }

        public async Task<T> GetQueueOneAsync<T>(string key)
        {
            var queue = redisConnection.GetQueue<T>(key);
            return await queue.TakeOneAsync(1);
        }
        #endregion

        #region 可靠队列
        public RedisReliableQueue<T> GetReliableQueue<T>(string key)
        {
            var queue = redisConnection.GetReliableQueue<T>(key);
            return queue;
        }
        /* 
        public async Task<List<T>> GetReliableQueueMsgAsync<T>(RedisReliableQueue<T> queue, int count)
        {
            return await Task.Run(() =>
            {
                return queue.Take(count).ToList();
            });
        }

        public async Task<T> GetReliableQueueMsgAsync<T>(RedisReliableQueue<T> queue)
        {
            return await Task.Run(() =>
            {
                return queue.TakeOne();
            });
        }
        public async Task<int> Acknowledge<T>(RedisReliableQueue<T> queue, T value)
        {
            return await Task.Run(() =>
             {
                 var data = value.ToJson();
                 var count = queue.Acknowledge(data);
                 return count;
             });
        }


        */
        public int RollbackAllAck(string key, int retryInterval = 60)
        {
            var queue = redisConnection.GetReliableQueue<string>(key);
            queue.RetryInterval = retryInterval;
            return queue.RollbackAllAck();
        }

        public int AddReliableQueueList<T>(string key, List<T> value)
        {
            var queue = redisConnection.GetReliableQueue<T>(key);
            var count = queue.Count;
            var result = queue.Add(value.ToArray());
            return result - count;
        }

        public int AddReliableQueue<T>(string key, T value)
        {
            var queue = redisConnection.GetReliableQueue<T>(key);
            var count = queue.Count;
            var result = queue.Add(value);
            return result - count;
        }
        #endregion

        #region 列表
        public int AddList(string key, IEnumerable<string> values, TimeSpan timeSpan)
        {
            var list = redisConnection.GetList<string>(key) as RedisList<string>;
            redisConnection.SetExpire(key, timeSpan);
            return list.AddRange(values);
        }

        public int AddList(string key, IEnumerable<string> values)
        {
            var list = redisConnection.GetList<string>(key) as RedisList<string>;
            return list.AddRange(values);
        }

        public string GetList(string key, string value)
        {
            var list = redisConnection.GetList<string>(key) as RedisList<string>;
            return list[list.IndexOf(value)];
        }

        public bool ExistInList(string key, string value)
        {
            var list = redisConnection.GetList<string>(key) as RedisList<string>;
            return list.Contains(value);
        }

        public bool DelList(string key, string value)
        {
            var list = redisConnection.GetList<string>(key) as RedisList<string>;
            return list.Remove(value);
        }

        #endregion

        #region SortSet

        public RedisSortedSet<T> GetRedisSortedSet<T>(string key)
        {
            var zset = new RedisSortedSet<T>(redisConnection, key);
            return zset;
        }


        public double SortedSetAdd(string key, Dictionary<string, double> value, string options = null)
        {
            var zset = new RedisSortedSet<string>(redisConnection, key);
            return zset.Add(options, value);
        }

        public IDictionary<T, double> SortedSetPopMax<T>(string key, int count = 1)
        {
            var zset = new RedisSortedSet<T>(redisConnection, key);
            return zset.PopMax(count);
        }
        public IDictionary<T, double> SortedSetPopMin<T>(string key, int count = 1)
        {
            var zset = new RedisSortedSet<T>(redisConnection, key);
            return zset.PopMin(count);
        }

        public int SortedSetFindCount<T>(string key, double min, double max)
        {
            var zset = new RedisSortedSet<T>(redisConnection, key);
            return zset.FindCount(min, max);
        }

        public T[] SortedSetRange<T>(string key, int start, int stop)
        {
            var zset = new RedisSortedSet<T>(redisConnection, key);
            return zset.Range(start, stop);
        }

        public IDictionary<T, double> SortedSetRangeWithScores<T>(string key, int start, int stop)
        {
            var zset = new RedisSortedSet<T>(redisConnection, key);
            return zset.RangeWithScores(start, stop);
        }
        public T[] SortedSetRangeByScore<T>(string key, double min, double max, int offset, int count)
        {
            var zset = new RedisSortedSet<T>(redisConnection, key);
            return zset.RangeByScore(min, max, offset, count);
        }

        public async Task<T[]> SortedSetRangeByScoreAsync<T>(string key, double min, double max, int offset, int count)
        {
            var zset = new RedisSortedSet<T>(redisConnection, key);
            return await zset.RangeByScoreAsync(min, max, offset, count);
        }
        public IDictionary<T, double> SortedSetRangeByScoreWithScores<T>(string key, double min, double max, int offset, int count)
        {
            var zset = new RedisSortedSet<T>(redisConnection, key);
            return zset.RangeByScoreWithScores(min, max, offset, count);
        }

        public int SortedSetRank<T>(string key, T member)
        {
            var zset = new RedisSortedSet<T>(redisConnection, key);
            return zset.Rank(member);
        }
        public IEnumerable<KeyValuePair<T, double>> SortedSetSearch<T>(string key, string pattern, int count, int position = 0)
        {
            var zset = new RedisSortedSet<T>(redisConnection, key);
            return zset.Search(pattern, count, position);

        }

        public double SortedSetIncrement<T>(string key, T member, double score)
        {
            var zset = new RedisSortedSet<T>(redisConnection, key);
            return zset.Increment(member, score);
        }
        #endregion

        #region Set
        public void SetAdd<T>(string key, T value)
        {
            var set = redisConnection.GetSet<T>(key);
            set.Add(value);
        }


        public RedisSet<T> SetGet<T>(string key)
        {
            var set = redisConnection.GetSet<T>(key);
            var set2 = set as RedisSet<T>;
            return set2;
        }

        public void SetAddList<T>(string key, T[] value)
        {
            var set = redisConnection.GetSet<string>(key);
            var set2 = set as RedisSet<T>;
            set2.SAdd(value);
        }

        public bool SetRemove<T>(string key, T value)
        {
            var set = redisConnection.GetSet<T>(key);
            return set.Remove(value);
        }
        public bool SetContains<T>(string key, T value)
        {
            var set = redisConnection.GetSet<T>(key);
            return set.Contains(value);
        }

        public void SetClear<T>(string key)
        {
            var set = redisConnection.GetSet<T>(key);
            set.Clear();
        }

        public void SetCopyTo<T>(string key, T[] array, int arrayIndex)
        {
            var set = redisConnection.GetSet<T>(key);
            set.CopyTo(array, arrayIndex);
        }
        #endregion




    }
}
