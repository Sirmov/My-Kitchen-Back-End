// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ApplicationExtensions.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Api.Grpc.Extensions
{
    using Microsoft.Extensions.Caching.StackExchangeRedis;

    using StackExchange.Redis;

    /// <summary>
    /// This static class contains <see cref="IServiceCollection"/> extension methods used for
    /// registering application dependencies.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// This method registers all application options configuration.
        /// </summary>
        /// <param name="services">The service collection where the application options should be registered.</param>
        /// <returns>Returns the service collection with all application options registered.</returns>
        public static IServiceCollection AddApplicationOptions(this IServiceCollection services)
        {
            services
                .AddOptions<RedisCacheOptions>()
                .BindConfiguration(nameof(RedisCacheOptions))
                .Configure<IServiceProvider>((options, serviceProvider) =>
                {
                    options.ConnectionMultiplexerFactory = () => Task.FromResult(serviceProvider.GetService<IConnectionMultiplexer>() !);
                });

            return services;
        }
    }
}
