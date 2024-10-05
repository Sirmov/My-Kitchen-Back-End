
namespace MyKitchen.Microservices.Identity.Api.Rest
{
    using System.Reflection;

    using AutoMapper;

    using Microsoft.AspNetCore.Authentication.JwtBearer;

    using MyKitchen.Common.Guard;
    using MyKitchen.Microservices.Identity.Api.Rest.Extensions;
    using MyKitchen.Microservices.Identity.Api.Rest.Options.Configurator;
    using MyKitchen.Microservices.Identity.Services.Mapping;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();
            ConfigurePipelineAsync(app);
            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.AddApplicationOptions();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();
            services.ConfigureOptions<JwtBearerOptionsConfigurator>();

            services.AddSingleton<IGuard>(new Guard());
            services.AddMongoDbClient(configuration);
            services.AddMongoDbIdentity(configuration);

            // Automapper configuration
            var asseblies = new Assembly[]
            {
                //  typeof(ExampleDto).GetTypeInfo().Assembly,
            };
            AutoMapperConfig.RegisterMappings(asseblies);
            IMapper mapper = AutoMapperConfig.MapperInstance;
            services.AddSingleton<IMapper>(mapper);

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        private static void ConfigurePipelineAsync(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                // IConfigurationRoot configurationRoot = (IConfigurationRoot)app.Configuration;
                // Console.WriteLine(configurationRoot.GetDebugView());
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
        }
    }
}
