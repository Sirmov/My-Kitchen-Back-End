// |-----------------------------------------------------------------------------------------------------|
// <copyright file="JwtBearerOptionsConfigurator.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Api.Rest.Options.Configurator
{
    using AutoMapper;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Options;

    using MyKitchen.Microservices.Identity.Common.Options;
    using MyKitchen.Microservices.Identity.Services.Mapping;

    /// <summary>
    /// This class implements the <see cref="IConfigureOptions{TOptions}"/>.
    /// It is used to configure the <see cref="JwtBearerOptions"/>.
    /// </summary>
    public class JwtBearerOptionsConfigurator : IConfigureOptions<JwtBearerOptions>
    {
        private readonly TokenOptions tokenOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtBearerOptionsConfigurator"/> class.
        /// </summary>
        /// <param name="tokenOptions">The application token options.</param>
        public JwtBearerOptionsConfigurator(IOptions<TokenOptions> tokenOptions)
        {
            this.tokenOptions = tokenOptions.Value;
        }

        /// <inheritdoc/>
        public void Configure(JwtBearerOptions options)
        {
            IMapper mapper = AutoMapperConfig.CreateDuplicateTypeMapper<JwtBearerOptions>();

            mapper.Map(this.tokenOptions.JwtBearerOptions, options);

            options.TokenValidationParameters.IssuerSigningKey = this.tokenOptions.IssuerSigningKey;
        }
    }
}
