// |-----------------------------------------------------------------------------------------------------|
// <copyright file="Program.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Api.Rest
{
    using System.Reflection;

    using AutoMapper;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;

    using MyKitchen.Common.Constants;
    using MyKitchen.Common.Guard;
    using MyKitchen.Microservices.Identity.Api.Common.Constants;
    using MyKitchen.Microservices.Identity.Api.Rest.Extensions;
    using MyKitchen.Microservices.Identity.Api.Rest.Options.Configurator;
    using MyKitchen.Microservices.Identity.Services.Mapping;
    using MyKitchen.Microservices.Identity.Services.Users.Dtos.User;

    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            await ConfigureServicesAsync(builder.Services, builder.Configuration);

            var app = builder.Build();
            ConfigurePipelineAsync(app);
            app.Run();
        }

        private static async Task ConfigureServicesAsync(IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationOptions();

            var connectionMultiplexer = await services.AddRedisCacheAsync(configuration);

            services.AddDataProtection()
                .PersistKeysToStackExchangeRedis(connectionMultiplexer, "DataProtection-Keys")
                .SetApplicationName(ApplicationConstants.Title);

            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("DevelopmentCors", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyOrigin()
                        .AllowAnyMethod();
                });
            });

            services.AddApplicationServices();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();
            services.ConfigureOptions<JwtBearerOptionsConfigurator>();

            services.AddSingleton<IGuard>(new Guard());
            services.AddMongoDbClient(configuration);
            services.AddMongoDbIdentity(configuration);

            // Automapper configuration
            var assemblies = new Assembly[]
            {
                typeof(UserDto).GetTypeInfo().Assembly,
            };
            AutoMapperConfig.RegisterMappings(assemblies);
            IMapper mapper = AutoMapperConfig.MapperInstance;
            services.AddSingleton<IMapper>(mapper);

            services.ConfigureOptions<SwaggerGenOptionsConfigurator>();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        private static void ConfigurePipelineAsync(WebApplication app)
        {
            app.UseExceptionHandler(RouteConstants.ErrorHandlerRoute);

            if (app.Environment.IsDevelopment())
            {
                // IConfigurationRoot configurationRoot = (IConfigurationRoot)app.Configuration;
                // Console.WriteLine(configurationRoot.GetDebugView());
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseCors("DevelopmentCors");
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
