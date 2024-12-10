// |-----------------------------------------------------------------------------------------------------|
// <copyright file="StackExchangeRedisCacheExtensions.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Api.Rest.Extensions
{
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Caching.StackExchangeRedis;
    using Microsoft.Extensions.Options;

    using MyKitchen.Common.Constants;

    using StackExchange.Redis;

    /// <summary>
    /// This static class contains <see cref="IServiceCollection"/> extension methods used for
    /// registering the stack exchange redis cache.
    /// </summary>
    public static class StackExchangeRedisCacheExtensions
    {
        /// <summary>
        /// This method registers the redis cache and configures it using a delegate action <paramref name="options"/>.
        /// </summary>
        /// <param name="services">The service collection where the redis cache should be registered.</param>
        /// <param name="options">The delegate used to configure the <see cref="RedisCacheOptions"/>.</param>
        /// <returns>Returns the service collection with the redis cache configured and registered.</returns>
        public static async Task<IConnectionMultiplexer> AddRedisCache(this IServiceCollection services, Action<RedisCacheOptions> options)
        {
            services.AddSingleton<IDistributedCache, RedisCache>();

            RedisCacheOptions redisCacheOptions = new ();
            options.Invoke(redisCacheOptions);

            IConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(redisCacheOptions.Configuration!);
            services.AddSingleton<IConnectionMultiplexer>(redis);

            return redis;
        }

        /// <summary>
        /// This method registers the redis cache and configures it using the provided <paramref name="redisCacheOptions"/>.
        /// </summary>
        /// <param name="services">The service collection where the redis cache should be registered.</param>
        /// <param name="redisCacheOptions">The <see cref="RedisCacheOptions"/> to be used.</param>
        /// <returns>Returns the service collection with the redis cache configured and registered.</returns>
        public static async Task<IConnectionMultiplexer> AddRedisCacheAsync(this IServiceCollection services, RedisCacheOptions redisCacheOptions)
        {
            services.AddSingleton<IDistributedCache, RedisCache>();

            IConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(redisCacheOptions.Configuration!);
            services.AddSingleton<IConnectionMultiplexer>(redis);

            return redis;
        }

        /// <summary>
        /// This method registers the mongoDB client and configures it using the provided
        /// <paramref name="configuration"/>, where it searches for <see cref="RedisCacheOptions" /> section.
        /// </summary>
        /// <param name="services">The service collection where the redis cache should be registered.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> containing the <see cref="RedisCacheOptions"/>.</param>
        /// <returns>Returns the service collection with the redis cache configured and registered.</returns>
        public static async Task<IConnectionMultiplexer> AddRedisCacheAsync(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IDistributedCache, RedisCache>();

            RedisCacheOptions redisCacheOptions = configuration.GetSection(nameof(RedisCacheOptions)).Get<RedisCacheOptions>() ??
                throw new NullReferenceException(string.Format(ExceptionMessages.VariableIsNull, nameof(RedisCacheOptions)));

            IConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(redisCacheOptions.Configuration!);
            services.AddSingleton<IConnectionMultiplexer>(redis);

            return redis;
        }

        /// <summary>
        /// This method registers the mongoDB client and configures it using the provided <paramref name="configureOptions"/>.
        /// </summary>
        /// <param name="services">The service collection where the redis cache should be registered.</param>
        /// <param name="configureOptions">
        /// <see cref="RedisCacheOptions"/> configurator implementing <see cref="IConfigureOptions{TOptions}"/>.
        /// </param>
        /// <returns>Returns the service collection with the redis cache configured and registered.</returns>
        public static async Task<IConnectionMultiplexer> AddRedisCacheAsync(this IServiceCollection services, IConfigureOptions<RedisCacheOptions> configureOptions)
        {
            services.AddSingleton<IDistributedCache, RedisCache>();

            RedisCacheOptions redisCacheOptions = new ();
            configureOptions.Configure(redisCacheOptions);

            IConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(redisCacheOptions.Configuration!);
            services.AddSingleton<IConnectionMultiplexer>(redis);

            return redis;
        }
    }
}
