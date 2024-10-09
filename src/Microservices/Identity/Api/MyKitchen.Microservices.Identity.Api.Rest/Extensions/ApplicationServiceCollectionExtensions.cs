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

    using TokenOptions = Identity.Common.TokenOptions;

    public static class ApplicationServiceCollectionExtensions
    {
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

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(ITokensService<,>), typeof(TokensService<,>));
            services.AddScoped(typeof(IUsersService<,>), typeof(UsersService<,>));
            services.AddScoped(typeof(IUserRolesService<,>), typeof(UserRolesService<,>));

            return services;
        }
    }
}
