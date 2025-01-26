// |-----------------------------------------------------------------------------------------------------|
// <copyright file="Program.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Api.Rest
{
    using AutoMapper;

    using Microsoft.AspNetCore.Authentication.JwtBearer;

    using MyKitchen.Common.Guard;
    using MyKitchen.Microservices.Recipes.Api.Rest.Constants;
    using MyKitchen.Microservices.Recipes.Api.Rest.Extensions;
    using MyKitchen.Microservices.Recipes.Api.Rest.Options.Configurators;
    using MyKitchen.Microservices.Recipes.Services.Mapping;

    internal static class Program
    {
        public static void Main(string [] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();
            ConfigurePipeline(app);
            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationOptions();
            services.AddApplicationMiddlewares();
            services.AddApplicationServices();
            services.AddApplicationClients(configuration);

            services.AddMongoDbClient(configuration);
            services.AddMongoRepository();

            services.AddSingleton<IGuard>(new Guard());

            AutoMapperConfig.RegisterMappings(AppDomain.CurrentDomain.GetAssemblies());
            IMapper mapper = AutoMapperConfig.MapperInstance;
            services.AddSingleton<IMapper>(mapper);

            services.AddCors(options =>
            {
                options.AddPolicy("DevelopmentCors", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyOrigin()
                        .AllowAnyMethod();
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();
            services.ConfigureOptions<JwtBearerOptionsConfigurator>();

            services.AddControllers();
            services.ConfigureOptions<ApiBehaviorOptionsConfigurator>();

            services.ConfigureOptions<SwaggerGenOptionsConfigurator>();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        private static void ConfigurePipeline(WebApplication app)
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

            if (app.Environment.IsDevelopment())
            {
                app.MapControllers().AllowAnonymous();
            }

            app.MapControllers();
        }
    }
}
