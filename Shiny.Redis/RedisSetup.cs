using Microsoft.Extensions.DependencyInjection;
using System;

namespace Shiny.Redis
{
    /// <summary>
    /// redis服务拓展类
    /// </summary>
    public static class RedisSetUp
    {
        /// <summary>
        /// 添加redis服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="redisConfiguration"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddRedisCacheManager(this IServiceCollection services, string redisConfiguration = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (!string.IsNullOrEmpty(redisConfiguration))
            {
                services.AddSingleton<IRedisCacheManager, RedisCacheManager>(x => new RedisCacheManager(redisConfiguration));

            }
            else
            {
                services.AddSingleton<IRedisCacheManager, RedisCacheManager>();
            }

        }
    }
}
