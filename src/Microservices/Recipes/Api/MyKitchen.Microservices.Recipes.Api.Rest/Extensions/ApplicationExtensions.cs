// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ApplicationExtensions.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Api.Rest.Extensions
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;

    using MyKitchen.Microservices.Identity.Api.Grpc.Protos;
    using MyKitchen.Microservices.Recipes.Api.Rest.Middlewares;
    using MyKitchen.Microservices.Recipes.Api.Rest.Options;
    using MyKitchen.Microservices.Recipes.Services.Clients.Identity.Grpc;
    using MyKitchen.Microservices.Recipes.Services.Clients.Identity.Grpc.Contracts;
    using MyKitchen.Microservices.Recipes.Services.Recipes;
    using MyKitchen.Microservices.Recipes.Services.Recipes.Contracts;

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
                .AddOptionsWithValidateOnStart<MongoDbOptions>()
                .BindConfiguration(nameof(MongoDbOptions))
                .ValidateDataAnnotations();

            services
                .AddOptionsWithValidateOnStart<JwtBearerOptions>()
                .BindConfiguration(nameof(JwtBearerOptions))
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
            services.AddScoped(typeof(IRecipesService), typeof(RecipesService));
            services.AddScoped(typeof(IRecipeImagesService), typeof(RecipeImagesService));

            return services;
        }

        /// <summary>
        /// This method registers all application middlewares.
        /// </summary>
        /// <param name="services">The service collection where the application middlewares should be registered.</param>
        /// <returns>Returns the service collection with all application middlewares registered.</returns>
        public static IServiceCollection AddApplicationMiddlewares(this IServiceCollection services)
        {
            services.AddSingleton<AccessTokenInvalidationMiddleware>();

            return services;
        }

        /// <summary>
        /// This method registers all application clients.
        /// </summary>
        /// <param name="services">The service collection where the application clients should be registered.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> containing the addre.</param>
        /// <returns>Returns the service collection with all application clients registered.</returns>
        public static IServiceCollection AddApplicationClients(this IServiceCollection services, IConfiguration configuration)
        {
            var identityMicroServiceOptions = configuration
                .GetSection(nameof(IdentityMicroserviceOptions))
                .Get<IdentityMicroserviceOptions>() ?? throw new ArgumentNullException(nameof(IdentityMicroserviceOptions));

            services
                .AddGrpcClient<Tokens.TokensClient>(nameof(Tokens.TokensClient), options =>
                {
                    options.Address = new Uri(identityMicroServiceOptions.GrpcApiAddress);
                })
                .ConfigureChannel(options =>
                {
                    options.HttpHandler = new SocketsHttpHandler()
                    {
                        EnableMultipleHttp2Connections = true,
                    };
                });

            services.AddScoped(typeof(IIdentityTokensServiceGrpcClient), typeof(IdentityTokensServiceGrpcClient));

            return services;
        }
    }
}
