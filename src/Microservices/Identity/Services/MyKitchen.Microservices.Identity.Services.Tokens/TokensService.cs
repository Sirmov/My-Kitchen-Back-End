// |-----------------------------------------------------------------------------------------------------|
// <copyright file="TokensService.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Tokens
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;

    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    using MyKitchen.Microservices.Identity.Common.Options;
    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Tokens.Contracts;
    using MyKitchen.Microservices.Identity.Services.Users.Contracts;

    /// <summary>
    /// This class contains the business logic around different application tokens.
    /// It implements <see cref="ITokensService{TUser, TRole}"/>.
    /// </summary>
    /// <inheritdoc cref="ITokensService{TUser, TRole}"/>
    public class TokensService<TUser, TRole> : ITokensService<TUser, TRole>
        where TUser : ApplicationUser, new()
        where TRole : ApplicationRole, new()
    {
        private readonly JwtSecurityTokenHandler tokenHandler;
        private readonly TokenOptions tokenOptions;
        private readonly IDataProtector userIdProtector;
        private readonly IUserRolesService<TUser, TRole> userRolesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokensService{TUser, TRole}"/> class.
        /// </summary>
        /// <param name="tokenOptions">The <see cref="TokenOptions"/>.</param>
        /// <param name="dataProtectionProvider">The implementation of <see cref="IDataProtectionProvider"/>.</param>
        /// <param name="userRolesService">The implementation of <see cref="IUserRolesService{TUser, TRole}"/>.</param>
        public TokensService(
            IOptions<TokenOptions> tokenOptions,
            IDataProtectionProvider dataProtectionProvider,
            IUserRolesService<TUser, TRole> userRolesService)
        {
            this.tokenHandler = new JwtSecurityTokenHandler();

            this.tokenOptions = tokenOptions.Value;
            this.userIdProtector = dataProtectionProvider.CreateProtector(this.GetType().Namespace!, nameof(this.userIdProtector), "v1");
            this.userRolesService = userRolesService;
        }

        /// <inheritdoc/>
        public async Task<string> GenerateAccessTokenAsync(TUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, this.userIdProtector.Protect(user.Id.ToString())),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var userRoles = await this.userRolesService.GetUserRolesAsync(user);

            foreach (var role in userRoles.Data ?? Enumerable.Empty<string>())
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            return this.GenerateToken(authClaims, this.tokenOptions.AccessTokenLifetime);
        }

        /// <inheritdoc/>
        public string GenerateRefreshToken(TUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, this.userIdProtector.Protect(user.Id.ToString())),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            return this.GenerateToken(claims, this.tokenOptions.RefreshTokenLifetime);
        }

        private string GenerateToken(IEnumerable<Claim> claims, TimeSpan expiresAfter)
        {
            var jwtBearerOptions = this.tokenOptions.JwtBearerOptions;
            var tokenValidationParameters = jwtBearerOptions.TokenValidationParameters;
            SigningCredentials signingCredentials = new (this.tokenOptions.IssuerSigningKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: tokenValidationParameters.ValidIssuer,
                audience: tokenValidationParameters.ValidAudience,
                expires: DateTime.Now.Add(expiresAfter),
                claims: claims,
                signingCredentials: signingCredentials);

            return this.tokenHandler.WriteToken(token);
        }
    }
}
