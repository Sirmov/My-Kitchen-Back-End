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
    // using MyKitchen.Microservices.Identity.Services.Users;
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
        private readonly TokenOptions tokenOptions;
        private readonly IDataProtector userIdProtector;
        // private readonly IUsersService<TUser, TRole> usersService;
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
            // UsersService<TUser, TRole> usersService,
            IUserRolesService<TUser, TRole> userRolesService)
        {
            this.tokenOptions = tokenOptions.Value;
            this.userIdProtector = dataProtectionProvider.CreateProtector(this.GetType().Namespace!, nameof(this.userIdProtector), "v1");
            // this.usersService = usersService;
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

            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtBearerOptions = this.tokenOptions.JwtBearerOptions;
            var tokenValidationParameters = jwtBearerOptions.TokenValidationParameters;
            SigningCredentials signingCredentials = new (this.tokenOptions.IssuerSigningKey, SecurityAlgorithms.HmacSha256);

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
