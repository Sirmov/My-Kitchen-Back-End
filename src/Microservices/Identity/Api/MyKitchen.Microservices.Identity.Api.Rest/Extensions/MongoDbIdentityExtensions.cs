// |-----------------------------------------------------------------------------------------------------|
// <copyright file="MongoDbIdentityExtensions.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

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

    /// <summary>
    /// This static class contains <see cref="IServiceCollection"/> extension methods used for
    /// registering and configuring mongoDB identity.
    /// </summary>
    public static class MongoDbIdentityExtensions
    {
        /// <summary>
        /// This method registers the mongoDB identity provider and configures it using 2 delegate actions:
        /// <paramref name="setupIdentityAction"/> and <paramref name="setupDatabaseAction"/>.
        /// </summary>
        /// <param name="services">The service collection where the mongoDB identity provider should be registered.</param>
        /// <param name="setupIdentityAction">The delegate used to configure the <see cref="IdentityOptions"/>.</param>
        /// <param name="setupDatabaseAction">The delegate used to configure the <see cref="MongoIdentityOptions"/>.</param>
        /// <returns>Returns the service collection with the mongoDB identity provider configured and registered.</returns>
        public static IServiceCollection AddMongoDbIdentity(
            this IServiceCollection services,
            Action<IdentityOptions> setupIdentityAction,
            Action<MongoIdentityOptions> setupDatabaseAction)
        {
            services
                .AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole, ObjectId>(setupIdentityAction, setupDatabaseAction);

            return services;
        }

        /// <summary>
        /// This method registers the mongoDB identity provider and configures it using the provided
        /// <paramref name="identityOptions"/> and <paramref name="mongoIdentityOptions"/>.
        /// </summary>
        /// <param name="services">The service collection where the mongoDB identity provider should be registered.</param>
        /// <param name="identityOptions">The <see cref="IdentityOptions"/> to be used.</param>
        /// <param name="mongoIdentityOptions">The <see cref="MongoIdentityOptions"/> to be used.</param>
        /// <returns>Returns the service collection with the mongoDB identity provider configured and registered.</returns>
        public static IServiceCollection AddMongoDbIdentity(
            this IServiceCollection services,
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

        /// <summary>
        /// This method registers the mongoDB identity provider and configures it using the provided
        /// <paramref name="configuration"/>, where it searches for <see cref="IdentityOptions"/>
        /// and <see cref="MongoIdentityOptions"/> sections.
        /// </summary>
        /// <param name="services">The service collection where the mongoDB identity provider should be registered.</param>
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/> containing the <see cref="IdentityOptions"/> and <see cref="MongoIdentityOptions"/>.
        /// </param>
        /// <returns>Returns the service collection with the mongoDB identity provider configured and registered.</returns>
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

        /// <summary>
        /// This method registers the mongoDB identity provider and configures it using <see cref="IConfigureOptions{TOptions}"/>
        /// <paramref name="identityConfigureOptions"/> and <paramref name="mongoIdentityConfigureOptions"/>.
        /// </summary>
        /// <param name="services">The service collection where the mongoDB identity provider should be registered.</param>
        /// <param name="identityConfigureOptions">
        /// <see cref="IdentityOptions"/> configurator implementing <see cref="IConfigureOptions{TOptions}"/>.
        /// </param>
        /// <param name="mongoIdentityConfigureOptions">
        /// <see cref="MongoIdentityOptions"/> configurator implementing <see cref="IConfigureOptions{TOptions}"/>.
        /// </param>
        /// <returns>Returns the service collection with the mongoDB identity provider configured and registered.</returns>
        public static IServiceCollection AddMongoDbIdentity(
            this IServiceCollection services,
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
