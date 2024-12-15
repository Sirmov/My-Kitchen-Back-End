// |-----------------------------------------------------------------------------------------------------|
// <copyright file="Program.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Api.Rest
{
    internal static class Program
    {
        public static async Task Main(string [] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            await ConfigureServicesAsync(builder.Services, builder.Configuration);

            var app = builder.Build();
            await ConfigurePipelineAsync(app);
            app.Run();
        }

        private static async Task ConfigureServicesAsync(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddOpenApi();
        }

        private static async Task ConfigurePipelineAsync(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
