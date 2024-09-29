namespace MyKitchen.Microservices.Identity.Api.Rest.Extensions
{
    using AspNetCore.Identity.Mongo;

    using Microsoft.AspNetCore.Identity;

    using MyKitchen.Microservices.Identity.Api.Rest.Options;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationOptions(this IServiceCollection services)
        {
            services
                .AddOptions<IdentityOptions>()
                .BindConfiguration(nameof(IdentityOptions));

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
    }
}
