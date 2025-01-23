// |-----------------------------------------------------------------------------------------------------|
// <copyright file="Program.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Api.Grpc
{
    using System.Threading.Tasks;

    using MyKitchen.Microservices.Identity.Api.Grpc.Extensions;
    using MyKitchen.Microservices.Identity.Api.Grpc.Services;

    internal static class Program
    {
        public static async Task Main(string [] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            await ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();
            ConfigurePipeline(app);
            app.Run();
        }

        public static async Task ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationOptions();
            await services.AddRedisCacheAsync(configuration);
            services.AddGrpc();
            services.AddGrpcReflection();
        }

        public static void ConfigurePipeline(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapGrpcReflectionService().AllowAnonymous();
            }

            app.MapGrpcService<TokensService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");
        }
    }
}
