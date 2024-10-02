namespace MyKitchen.Microservices.Identity.Api.Rest.Extensions
{
    using AspNetCore.Identity.Mongo;

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

            services
                .AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole, ObjectId>(
                    identity =>
                    {
                        identity.SignIn.RequireConfirmedAccount = identityOptions.SignIn.RequireConfirmedAccount;

                        identity.Lockout.MaxFailedAccessAttempts = identityOptions.Lockout.MaxFailedAccessAttempts;
                        identity.Lockout.DefaultLockoutTimeSpan = identityOptions.Lockout.DefaultLockoutTimeSpan;

                        identity.Password.RequireNonAlphanumeric = identityOptions.Password.RequireNonAlphanumeric;
                        identity.Password.RequireDigit = identityOptions.Password.RequireDigit;
                        identity.Password.RequireUppercase = identityOptions.Password.RequireUppercase;
                        identity.Password.RequiredLength = identityOptions.Password.RequiredLength;
                    },
                    mongoIdentity =>
                    {
                        mongoIdentity.ConnectionString = mongoIdentityOptions.ConnectionString;
                        mongoIdentity.UsersCollection = mongoIdentityOptions.UsersCollection;
                        mongoIdentity.RolesCollection = mongoIdentityOptions.RolesCollection;
                        mongoIdentity.MigrationCollection = mongoIdentityOptions.MigrationCollection;
                    });

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