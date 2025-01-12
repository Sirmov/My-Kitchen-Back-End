// |-----------------------------------------------------------------------------------------------------|
// <copyright file="Program.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Api.Rest
{
    using AutoMapper;

    using MyKitchen.Common.Guard;
    using MyKitchen.Microservices.Recipes.Api.Rest.Constants;
    using MyKitchen.Microservices.Recipes.Api.Rest.Extensions;
    using MyKitchen.Microservices.Recipes.Services.Mapping;

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
            services.AddApplicationOptions();
            services.AddApplicationMiddlewares();
            services.AddApplicationServices();

            services.AddMongoDbClient(configuration);
            services.AddMongoRepository();

            services.AddCors(options =>
            {
                options.AddPolicy("DevelopmentCors", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyOrigin()
                        .AllowAnyMethod();
                });
            });

            services.AddControllers();
            services.AddSingleton<IGuard>(new Guard());

            AutoMapperConfig.RegisterMappings(AppDomain.CurrentDomain.GetAssemblies());
            IMapper mapper = AutoMapperConfig.MapperInstance;
            services.AddSingleton<IMapper>(mapper);

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        private static async Task ConfigurePipelineAsync(WebApplication app)
        {
            app.UseExceptionHandler(RouteConstants.ErrorHandlerRoute);

            if (app.Environment.IsDevelopment())
            {
                IConfigurationRoot configurationRoot = (IConfigurationRoot)app.Configuration;
                Console.WriteLine(configurationRoot.GetDebugView());
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseCors("DevelopmentCors");
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
