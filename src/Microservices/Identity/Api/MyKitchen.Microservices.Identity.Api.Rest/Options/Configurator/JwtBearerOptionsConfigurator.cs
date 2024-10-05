namespace MyKitchen.Microservices.Identity.Api.Rest.Options.Configurator
{
    using AutoMapper;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    using MyKitchen.Microservices.Identity.Common;
    using MyKitchen.Microservices.Identity.Services.Mapping;

    public class JwtBearerOptionsConfigurator : IConfigureOptions<JwtBearerOptions>
    {
        private readonly TokenOptions tokenOptions;

        public JwtBearerOptionsConfigurator(IOptions<TokenOptions> tokenOptions)
        {
            this.tokenOptions = tokenOptions.Value;
        }

        public void Configure(JwtBearerOptions options)
        {
            IMapper mapper = AutoMapperConfig.CreateDuplicateTypeMapper<JwtBearerOptions>();

            mapper.Map(this.tokenOptions.JwtBearerOptions, options);

            SymmetricSecurityKey issuerSigningKey = this.tokenOptions.IssuerSigningKey;
            options.TokenValidationParameters.IssuerSigningKey = issuerSigningKey;
        }
    }
}
