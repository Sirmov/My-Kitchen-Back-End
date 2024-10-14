// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ApplicationServiceCollectionExtensions.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Api.Rest.Extensions
{
    using AspNetCore.Identity.Mongo;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;

    using MyKitchen.Microservices.Identity.Api.Rest.Options;
    using MyKitchen.Microservices.Identity.Services.Tokens;
    using MyKitchen.Microservices.Identity.Services.Tokens.Contracts;
    using MyKitchen.Microservices.Identity.Services.Users;
    using MyKitchen.Microservices.Identity.Services.Users.Contracts;

    using TokenOptions = MyKitchen.Microservices.Identity.Common.Options.TokenOptions;

    /// <summary>
    /// This static class contains <see cref="IServiceCollection"/> extension methods used for
    /// registering application dependencies.
    /// </summary>
    public static class ApplicationServiceCollectionExtensions
    {
        /// <summary>
        /// This method registers all application options configuration.
        /// </summary>
        /// <param name="services">The service collection where the application options should be registered.</param>
        /// <returns>Returns the service collection with all application options registered.</returns>
        public static IServiceCollection AddApplicationOptions(this IServiceCollection services)
        {
            services
                .AddOptions<IdentityOptions>()
                .BindConfiguration(nameof(IdentityOptions));

            services
                .AddOptionsWithValidateOnStart<TokenOptions>()
                .BindConfiguration(nameof(TokenOptions))
                .ValidateDataAnnotations();

            services
                .AddOptionsWithValidateOnStart<JwtBearerOptions>()
                .BindConfiguration(string.Join(':', nameof(TokenOptions), nameof(JwtBearerOptions)))
                .ValidateDataAnnotations();

            services
                .AddOptionsWithValidateOnStart<MongoDbOptions>()
                .BindConfiguration(nameof(MongoDbOptions))
                .ValidateDataAnnotations();

            services
                .AddOptionsWithValidateOnStart<MongoIdentityOptions>()
                .BindConfiguration(nameof(MongoIdentityOptions))
                .ValidateDataAnnotations();

            return services;
        }

        /// <summary>
        /// This method registers all application services.
        /// </summary>
        /// <param name="services">The service collection where the application services should be registered.</param>
        /// <returns>Returns the service collection with all application services registered.</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(ITokensService<,>), typeof(TokensService<,>));
            services.AddScoped(typeof(IUsersService<,>), typeof(UsersService<,>));
            services.AddScoped(typeof(IUserRolesService<,>), typeof(UserRolesService<,>));

            return services;
        }
    }
}
