using NewLife.Caching;
using NewLife.Caching.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shiny.Redis
{
    public interface IRedisCacheManager
    {


        #region 普通

        /// <summary>
        /// 获取值，并序列化
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        TEntity Get<TEntity>(string key);

        /// <summary>
        /// 插入数据到redis
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheTime"></param>
        void Set(string key, object value, TimeSpan cacheTime);

        /// <summary>
        /// 插入数据到redis
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheTime"></param>
        void Set<TEntity>(string key, TEntity value, TimeSpan cacheTime);

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey(string key);

        /// <summary>
        /// 移除某一个key
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// 全部清除
        /// </summary>
        void Clear();

        /// <summary>
        /// 设置超时
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeSpan"></param>
        void SetExpire(string key, TimeSpan timeSpan);

        /// <summary>
        /// 根据关键字移除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RemoveByKey(string key, int count);
        /// <summary>
        /// 根据关键字移除所有
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task RemoveAllByKey(string key, int count = 999);

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        List<string> Search(SearchModel model);



        #endregion

        #region HashMap
        /// <summary>
        /// 插入hashmap
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        bool HashSet<T>(string key, Dictionary<string, T> dic, int expire = -1);

        /// <summary>
        /// /插入hashmap带返回结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dic"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        bool HashSetWithResult<T>(string key, Dictionary<string, T> dic, int expire = -1);

        /// <summary>
        /// 获取hashmap
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        List<T> HashGet<T>(string key, params string[] fields);

        /// <summary>
        /// 获取hashmap
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        string[] HashGet(string key, params string[] fields);

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        IDictionary<string, T> HashGetAll<T>(string key);

        /// <summary>
        /// 删除hash值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        int HashDel<T>(string key, params string[] fields);
        #endregion

        #region 队列
        /// <summary>
        /// 添加到队列
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        int AddQueue<T>(string key, T[] value);


        /// <summary>
        /// 添加到队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        int AddQueue<T>(string key, T value);

        /// <summary>
        /// 获取一批队列消息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        List<T> GetQueue<T>(string key, int Count);

        /// <summary>
        /// 获取一个队列消息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetQueueOneAsync<T>(string key);
        #endregion

        #region 可靠队列
        /// <summary>
        /// 获取可靠队列实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        RedisReliableQueue<T> GetReliableQueue<T>(string key);

        /*
        /// <summary>
        /// 批量获取可靠队列消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<List<T>> GetReliableQueueMsgAsync<T>(RedisReliableQueue<T> queue, int count);

        /// <summary>
        /// 获取一条可靠队列消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <returns></returns>
        Task<T> GetReliableQueueMsgAsync<T>(RedisReliableQueue<T> queue);

        /// <summary>
        /// 回滚所有未确认消息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="retryInterval">重新处理确认队列中死信的间隔。默认60s</param>
        /// <returns></returns>
        Task<int> RollbackAllAck(string key, int retryInterval = 60);

        /// <summary>
        /// 确认消息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<int> Acknowledge<T>(RedisReliableQueue<T> queue, T value);
        */

        /// <summary>
        /// 批量添加消息到可靠队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        int AddReliableQueueList<T>(string key, List<T> value);
        /// <summary>
        /// 添加消息到可靠队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        int AddReliableQueue<T>(string key, T value);
        /// <summary>
        /// 回滚所有未确认消息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="retryInterval">重新处理确认队列中死信的间隔。默认60s</param>
        /// <returns></returns>
        int RollbackAllAck(string key, int retryInterval = 60);
        #endregion

        #region 列表
        /// <summary>
        /// 插入列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        int AddList(string key, IEnumerable<string> values, TimeSpan timeSpan);

        /// <summary>
        /// 列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        int AddList(string key, IEnumerable<string> values);

        /// <summary>
        /// 获取list里元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        string GetList(string key, string value);

        /// <summary>
        /// 删除指定元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool DelList(string key, string value);

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool ExistInList(string key, string value);



        #endregion

        #region SortSet
        /// <summary>
        /// 获取SortSet实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        RedisSortedSet<T> GetRedisSortedSet<T>(string key);
        /// <summary>
        /// 批量添加Sorted
        /// 将所有指定成员添加到键为key有序集合（sorted set）里面。 添加时可以指定多个分数/成员（score/member）对。
        /// 如果指定添加的成员已经是有序集合里面的成员，则会更新改成员的分数（scrore）并更新到正确的排序位置。
        /// ZADD 命令在key后面分数/成员（score/member）对前面支持一些参数，他们是：
        /// XX: 仅仅更新存在的成员，不添加新成员。
        /// NX: 不更新存在的成员。只添加新成员。
        /// CH: 修改返回值为发生变化的成员总数，原始是返回新添加成员的总数(CH 是 changed 的意思)。
        /// 更改的元素是新添加的成员，已经存在的成员更新分数。 所以在命令中指定的成员有相同的分数将不被计算在内。
        /// 注：在通常情况下，ZADD返回值只计算新添加成员的数量。
        /// INCR: 当ZADD指定这个选项时，成员的操作就等同ZINCRBY命令，对成员的分数进行递增操作。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        double SortedSetAdd(string key, Dictionary<String, Double> value, string options = null);

        /// <summary>为有序集key的成员member的score值加上增量increment</summary>
        /// <param name="member"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        double SortedSetIncrement<T>(string key, T member, double score);

        /// <summary>删除并返回有序集合key中的最多count个具有最高得分的成员</summary>
        /// <param name="count"></param>
        /// <returns></returns>
        IDictionary<T, double> SortedSetPopMax<T>(string key, int count = 1);


        /// <summary>删除并返回有序集合key中的最多count个具有最低得分的成员</summary>
        /// <param name="count"></param>
        /// <returns></returns>
        IDictionary<T, double> SortedSetPopMin<T>(string key, int count = 1);
        /// <summary>
        /// 返回有序集key中，score值在min和max之间(默认包括score值等于min或max)的成员个数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        int SortedSetFindCount<T>(string key, double min, double max);

        /// <summary>返回指定范围的列表</summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        T[] SortedSetRange<T>(string key, int start, int stop);

        /// <summary>返回指定范围的成员分数对</summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        IDictionary<T, double> SortedSetRangeWithScores<T>(string key, int start, int stop);

        /// <summary>返回指定分数区间的成员列表，低分到高分排序</summary>
        /// <param name="min">低分，包含</param>
        /// <param name="max">高分，包含</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">个数</param>
        /// <returns></returns>
        T[] SortedSetRangeByScore<T>(string key, double min, double max, int offset, int count);

        /// <summary>返回指定分数区间的成员列表，低分到高分排序</summary>
        /// <param name="min">低分，包含</param>
        /// <param name="max">高分，包含</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">个数</param>
        /// <returns></returns>
        Task<T[]> SortedSetRangeByScoreAsync<T>(string key, double min, double max, int offset, int count);
        /// <summary>返回指定分数区间的成员分数对，低分到高分排序</summary>
        /// <param name="min">低分，包含</param>
        /// <param name="max">高分，包含</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">个数</param>
        /// <returns></returns>
        IDictionary<T, double> SortedSetRangeByScoreWithScores<T>(string key, double min, double max, int offset, int count);
        /// <summary>返回有序集key中成员member的排名。其中有序集成员按score值递增(从小到大)顺序排列</summary>
        /// <param name="member"></param>
        /// <returns></returns>
        int SortedSetRank<T>(string key, T member);
        /// <summary>模糊搜索，支持?和*</summary>
        /// <param name="pattern"></param>
        /// <param name="count"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<T, double>> SortedSetSearch<T>(string key, string pattern, int count, int position = 0);
        #endregion

        #region Set
        /// <summary>
        /// 添加Set
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        void SetAdd<T>(string key, T value);
        /// <summary>
        /// 批量添加添加Set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        void SetAddList<T>(string key, T[] value);
        /// <summary>
        /// 移除Set某个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool SetRemove<T>(string key, T value);
        /// <summary>
        /// 判断值是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool SetContains<T>(string key, T value);
        /// <summary>
        /// 清空Set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        void SetClear<T>(string key);
        /// <summary>
        /// 复制Set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <returns></returns>
        void SetCopyTo<T>(string key, T[] array, int arrayIndex);
        /// <summary>
        /// 获取Set实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        RedisSet<T> SetGet<T>(string key);




        #endregion

    }
}
