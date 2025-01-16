// |-----------------------------------------------------------------------------------------------------|
// <copyright file="MongoDbExtensions.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Api.Rest.Extensions
{
    using Microsoft.Extensions.Options;

    using MongoDB.Bson.Serialization.Conventions;
    using MongoDB.Driver;
    using MongoDB.Driver.GridFS;

    using MyKitchen.Microservices.Recipes.Api.Rest.Options;
    using MyKitchen.Microservices.Recipes.Data;
    using MyKitchen.Microservices.Recipes.Data.Contracts;

    /// <summary>
    /// This static class contains <see cref="IServiceCollection"/> extension methods used for
    /// registering and configuring mongoDB client and connection.
    /// </summary>
    public static class MongoDbExtensions
    {
        /// <summary>
        /// This method registers the mongoDB client and configures it using a delegate action <paramref name="options"/>.
        /// </summary>
        /// <param name="services">The service collection where the mongoDB client should be registered.</param>
        /// <param name="options">The delegate used to configure the <see cref="MongoDbOptions"/>.</param>
        /// <returns>Returns the service collection with the mongoDB identity provider configured and registered.</returns>
        public static IServiceCollection AddMongoDbClient(this IServiceCollection services, Action<MongoDbOptions> options)
        {
            MongoDbOptions configuration = new MongoDbOptions();
            options.Invoke(configuration);

            SetFieldNamingConvention();
            MongoClient client = new (configuration.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(configuration.DatabaseName);

            services.AddSingleton<MongoClient>(client);
            services.AddSingleton<IMongoDatabase>(database);

            return services;
        }

        /// <summary>
        /// This method registers the mongoDB client and configures it using the provided <paramref name="options"/>.
        /// </summary>
        /// <param name="services">The service collection where the mongoDB client should be registered.</param>
        /// <param name="options">The <see cref="MongoDbOptions"/> to be used.</param>
        /// <returns>Returns the service collection with the mongoDB identity provider configured and registered.</returns>
        public static IServiceCollection AddMongoDbClient(this IServiceCollection services, MongoDbOptions options)
        {
            SetFieldNamingConvention();
            MongoClient client = new (options.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(options.DatabaseName);

            services.AddSingleton<MongoClient>(client);
            services.AddSingleton<IMongoDatabase>(database);

            return services;
        }

        /// <summary>
        /// This method registers the mongoDB client and configures it using the provided
        /// <paramref name="configuration"/>, where it searches for <see cref="MongoDbOptions" /> section.
        /// </summary>
        /// <param name="services">The service collection where the mongoDB client should be registered.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> containing the <see cref="MongoDbOptions"/>.</param>
        /// <returns>Returns the service collection with the mongoDB identity provider configured and registered.</returns>
        public static IServiceCollection AddMongoDbClient(this IServiceCollection services, IConfiguration configuration)
        {
            MongoDbOptions options = configuration.GetSection(nameof(MongoDbOptions)).Get<MongoDbOptions>() ??
                throw new ArgumentNullException(nameof(options));

            SetFieldNamingConvention();
            MongoClient client = new (options.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(options.DatabaseName);
            GridFSBucket bucket = new GridFSBucket(database);

            services.AddSingleton<IMongoClient>(client);
            services.AddSingleton<IMongoDatabase>(database);
            services.AddSingleton<IGridFSBucket>(bucket);

            return services;
        }

        /// <summary>
        /// This method registers the mongoDB client and configures it using the provided <paramref name="configureOptions"/>.
        /// </summary>
        /// <param name="services">The service collection where the mongoDB client should be registered.</param>
        /// <param name="configureOptions">
        /// <see cref="MongoDbOptions"/> configurator implementing <see cref="IConfigureOptions{TOptions}"/>.
        /// </param>
        /// <returns>Returns the service collection with the mongoDB identity provider configured and registered.</returns>
        public static IServiceCollection AddMongoDbClient(this IServiceCollection services, IConfigureOptions<MongoDbOptions> configureOptions)
        {
            MongoDbOptions options = new ();
            configureOptions.Configure(options);

            SetFieldNamingConvention();
            MongoClient client = new (options.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(options.DatabaseName);

            services.AddSingleton<MongoClient>(client);
            services.AddSingleton<IMongoDatabase>(database);

            return services;
        }

        /// <summary>
        /// This method registers the <see cref="MongoDbRepository{TDocument, TKey}"/> as the
        /// implementation of <see cref="IRepository{TDocument, TKey}"/>.
        /// </summary>
        /// <param name="services">The service collection where the mongoDB repository should be registered.</param>
        /// <returns>Returns the service collection with the mongoDB repository registered.</returns>
        public static IServiceCollection AddMongoRepository(this IServiceCollection services)
        {
            services.AddSingleton<IMongoDbContext, MongoDbContext>();
            services.AddScoped(typeof(IRepository<,>), typeof(MongoDbRepository<,>));

            return services;
        }

        private static void SetFieldNamingConvention()
        {
            ConventionPack camelCaseConvention = new () { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register(nameof(camelCaseConvention), camelCaseConvention, type => true);
        }
    }
}
