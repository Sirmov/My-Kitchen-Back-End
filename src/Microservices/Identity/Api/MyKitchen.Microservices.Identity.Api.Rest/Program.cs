
namespace MyKitchen.Microservices.Identity.Api.Rest
{
    using MyKitchen.Common.Guard;
    using MyKitchen.Microservices.Identity.Api.Rest.Extensions;

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
            services.AddApplicationOptions();

            services.AddSingleton<IGuard>(new Guard());

            services.AddMongoDbClient(configuration);
            services.AddMongoDbIdentity(configuration);

            services.AddControllers();
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
