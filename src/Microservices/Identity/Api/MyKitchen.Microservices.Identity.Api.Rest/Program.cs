
namespace MyKitchen.Microservices.Identity.Api.Rest
{
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

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(null);
        }

        private static void ConfigurePipelineAsync(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(null);
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
        }
    }
}