namespace MyKitchen.Microservices.Identity.Api.Rest.Extensions
{
    using AspNetCore.Identity.Mongo;

    using AutoMapper;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;

    using MongoDB.Bson;

    using MyKitchen.Common.Constants;
    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Mapping;

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
            IMapper mapper = AutoMapperConfig.CreateDuplicateTypeMapper(typeof(IdentityOptions), typeof(MongoIdentityOptions));

            services
                .AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole, ObjectId>(
                    identity => mapper.Map(identityOptions, identity),
                    mongoIdentity => mapper.Map(mongoIdentityOptions, mongoIdentity));

            return services;
        }

        public static IServiceCollection AddMongoDbIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            IdentityOptions identityOptions = configuration.GetSection(nameof(IdentityOptions)).Get<IdentityOptions>() ??
                throw new NullReferenceException(string.Format(ExceptionMessages.VariableIsNull, nameof(IdentityOptions)));

            MongoIdentityOptions mongoIdentityOptions = configuration.GetSection(nameof(MongoIdentityOptions)).Get<MongoIdentityOptions>() ??
                throw new NullReferenceException(string.Format(ExceptionMessages.VariableIsNull, nameof(MongoIdentityOptions)));

            IMapper mapper = AutoMapperConfig.CreateDuplicateTypeMapper(typeof(IdentityOptions), typeof(MongoIdentityOptions));

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
