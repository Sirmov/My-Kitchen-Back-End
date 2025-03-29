// |-----------------------------------------------------------------------------------------------------|
// <copyright file="JwtBearerOptionsConfigurator.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Api.Rest.Options.Configurators
{
    using System.Text;

    using AutoMapper;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    using MyKitchen.Microservices.Recipes.Services.Mapping;

    /// <summary>
    /// This class implements the <see cref="IConfigureOptions{TOptions}"/>.
    /// It is used to configure the <see cref="JwtBearerOptions"/>.
    /// </summary>
    public class JwtBearerOptionsConfigurator : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtBearerOptionsConfigurator"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        public JwtBearerOptionsConfigurator(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public void Configure(JwtBearerOptions options)
        {
            this.Configure(JwtBearerDefaults.AuthenticationScheme, options);
        }

        /// <inheritdoc/>
        public void Configure(string? name, JwtBearerOptions options)
        {
            IMapper mapper = AutoMapperConfig.CreateDuplicateTypeMapper<JwtBearerOptions>();
            var jwtBearerOptions = this.configuration.GetSection(nameof(JwtBearerOptions)).Get<JwtBearerOptions>()!;
            mapper.Map(jwtBearerOptions, options);

            string securityKey = this.configuration.GetValue(nameof(securityKey), string.Empty)!;
            options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.IncludeErrorDetails = true;
            }
        }
    }
}
