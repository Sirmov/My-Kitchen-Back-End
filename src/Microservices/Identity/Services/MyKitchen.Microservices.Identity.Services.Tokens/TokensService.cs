namespace MyKitchen.Microservices.Identity.Services.Tokens
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;

    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    using MyKitchen.Microservices.Identity.Common;
    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Tokens.Contracts;

    public class TokensService : ITokensService
    {
        private readonly TokenOptions tokenOptions;

        public TokensService(IOptions<TokenOptions> tokenOptions)
        {
            this.tokenOptions = tokenOptions.Value;
        }

        public string GenerateAccessTokenAsync(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // var userRoles = await this.usersService.GetUserRolesAsync(user);

            // foreach (var role in userRoles.Data ?? Enumerable.Empty<string>())
            // {
            //     authClaims.Add(new Claim(ClaimTypes.Role, role));
            // }

            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtBearerOptions = this.tokenOptions.JwtBearerOptions;
            var tokenValidationParameters = jwtBearerOptions.TokenValidationParameters;
            SigningCredentials signingCredentials = new(this.tokenOptions.IssuerSigningKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: tokenValidationParameters.ValidIssuer,
                audience: tokenValidationParameters.ValidAudience,
                expires: DateTime.Now.Add(this.tokenOptions.AccessTokenLifetime),
                claims: authClaims,
                signingCredentials: signingCredentials);

            // user.AccessToken = tokenHandler.WriteToken(token);

            // return user.AccessToken;

            string accessToken = tokenHandler.WriteToken(token);
            return accessToken;
        }
    }
}
