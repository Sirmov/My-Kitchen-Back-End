// |-----------------------------------------------------------------------------------------------------|
// <copyright file="SwaggerGenOptionsConfigurator.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Api.Rest.Options.Configurator
{
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;

    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// This class implements the <see cref="IConfigureOptions{TOptions}"/>.
    /// It is used to configure the <see cref="SwaggerGenOptions"/>.
    /// </summary>
    public class SwaggerGenOptionsConfigurator : IConfigureOptions<SwaggerGenOptions>
    {
        /// <inheritdoc/>
        public void Configure(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "MyKitchen",
                Description = "Open source recipes manager.",
                TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Nikola Sirmov",
                    Url = new Uri("https://www.linkedin.com/in/sirmov/"),
                },
                License = new OpenApiLicense
                {
                    Name = "GNU General Public License v3.0",
                    Url = new Uri("https://github.com/Sirmov/My-Kitchen-Back-End/blob/main/LICENSE"),
                },
            });

            var currentDirectory = Environment.CurrentDirectory;
            var documentationsDirectory = Path.GetFullPath(Path.Combine(currentDirectory, @"..\..\..\..\Documentation"));

            foreach (var file in Directory.EnumerateFiles(documentationsDirectory, "*.xml", SearchOption.AllDirectories))
            {
                options.IncludeXmlComments(file);
            }

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Description = "Please provide a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    Array.Empty<string>()
                },
            });
        }
    }
}
