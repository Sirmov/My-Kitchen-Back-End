namespace MyKitchen.Microservices.Identity.Api.Rest.Extensions
{
    using MyKitchen.Microservices.Identity.Api.Rest.Options;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationOptions(this IServiceCollection services)
        {
            services
                .AddOptionsWithValidateOnStart<MongoDbOptions>()
                .BindConfiguration(nameof(MongoDbOptions))
                .ValidateDataAnnotations();

            return services;
        }
    }
}
