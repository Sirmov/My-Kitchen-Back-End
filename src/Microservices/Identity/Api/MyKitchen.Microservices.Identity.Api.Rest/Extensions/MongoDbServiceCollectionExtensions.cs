namespace MyKitchen.Microservices.Identity.Api.Rest.Extensions
{
    using Microsoft.Extensions.Options;

    using MongoDB.Bson.Serialization.Conventions;
    using MongoDB.Driver;

    using MyKitchen.Microservices.Identity.Api.Rest.Options;

    public static class MongoDbServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbClient(this IServiceCollection services, Action<MongoDbOptions> options)
        {
            MongoDbOptions configuration = new MongoDbOptions();
            options.Invoke(configuration);

            SetFieldNamingConvention();
            MongoClient client = new(configuration.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(configuration.DatabaseName);

            services.AddSingleton<MongoClient>(client);
            services.AddSingleton<IMongoDatabase>(database);

            return services;
        }

        public static IServiceCollection AddMongoDbClient(this IServiceCollection services, MongoDbOptions options)
        {
            SetFieldNamingConvention();
            MongoClient client = new(options.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(options.DatabaseName);

            services.AddSingleton<MongoClient>(client);
            services.AddSingleton<IMongoDatabase>(database);

            return services;
        }

        public static IServiceCollection AddMongoDbClient(this IServiceCollection services, IConfiguration configuration)
        {
            MongoDbOptions options = configuration.GetSection(nameof(MongoDbOptions)).Get<MongoDbOptions>() ??
                throw new ArgumentNullException(nameof(options));

            SetFieldNamingConvention();
            MongoClient client = new(options.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(options.DatabaseName);

            services.AddSingleton<IMongoClient>(client);
            services.AddSingleton<IMongoDatabase>(database);

            return services;
        }

        public static IServiceCollection AddMongoDbClient(this IServiceCollection services, IConfigureOptions<MongoDbOptions> configureOptions)
        {
            MongoDbOptions options = new();
            configureOptions.Configure(options);

            SetFieldNamingConvention();
            MongoClient client = new(options.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(options.DatabaseName);

            services.AddSingleton<MongoClient>(client);
            services.AddSingleton<IMongoDatabase>(database);

            return services;
        }

        private static void SetFieldNamingConvention()
        {
            ConventionPack camelCaseConvention = new() { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register(nameof(camelCaseConvention), camelCaseConvention, type => true);
        }
    }
}
