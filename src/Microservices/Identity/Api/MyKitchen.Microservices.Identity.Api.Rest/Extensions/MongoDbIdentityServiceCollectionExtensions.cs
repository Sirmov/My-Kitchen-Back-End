namespace MyKitchen.Microservices.Identity.Api.Rest.Extensions
{
    using System.Runtime.CompilerServices;

    using AspNetCore.Identity.Mongo;

    using AutoMapper;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;

    using MongoDB.Bson;

    using MyKitchen.Microservices.Identity.Data.Models;

    public static class MongoDbIdentityServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbIdentity(this IServiceCollection services,
            Action<IdentityOptions> setupIdentityAction,
            Action<MongoIdentityOptions> setupDatabaseAction)
        {
            services
                .AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole, ObjectId>(setupIdentityAction, setupDatabaseAction);

            return services;
        }

        public static IServiceCollection AddMongoDbIdentity(this IServiceCollection services,
            IdentityOptions identityOptions,
            MongoIdentityOptions mongoIdentityOptions)
        {
            services
                .AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole, ObjectId>(
                    identity => identity = identityOptions,
                    mongoIdentity => mongoIdentity = mongoIdentityOptions);

            return services;
        }

        public static IServiceCollection AddMongoDbIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationRoot configurationRoot = (IConfigurationRoot)configuration;
            Console.WriteLine(configurationRoot.GetDebugView());

            IdentityOptions identityOptions = configuration.GetSection(nameof(IdentityOptions)).Get<IdentityOptions>() ??
                throw new ArgumentNullException(nameof(identityOptions));

            MongoIdentityOptions mongoIdentityOptions = configuration.GetSection(nameof(MongoIdentityOptions)).Get<MongoIdentityOptions>() ??
                throw new ArgumentNullException(nameof(mongoIdentityOptions));

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IdentityOptions, IdentityOptions>();
                cfg.CreateMap<MongoIdentityOptions, MongoIdentityOptions>();
            });
            IMapper mapper = new Mapper(mapperConfiguration);

            services
                .AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole, ObjectId>(
                    identity => mapper.Map(identityOptions, identity),
                    mongoIdentity => mapper.Map(mongoIdentityOptions, mongoIdentity));

            return services;
        }

        public static IServiceCollection AddMongoDbIdentity(this IServiceCollection services,
            IConfigureOptions<IdentityOptions> identityConfigureOptions,
            IConfigureOptions<MongoIdentityOptions> mongoIdentityConfigureOptions)
        {
            services
                .AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole, ObjectId>(
                    identityConfigureOptions.Configure,
                    mongoIdentityConfigureOptions.Configure);

            return services;
        }
    }
}
