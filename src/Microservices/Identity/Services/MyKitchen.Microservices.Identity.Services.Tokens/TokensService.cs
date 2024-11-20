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
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    using MyKitchen.Common.Constants;
    using MyKitchen.Common.ProblemDetails;
    using MyKitchen.Microservices.Identity.Common.Options;
    using MyKitchen.Microservices.Identity.Services.Common.Dtos.User;
    using MyKitchen.Microservices.Identity.Services.Common.ServiceResult;
    using MyKitchen.Microservices.Identity.Services.Tokens.Contracts;

    /// <summary>
    /// This class contains the business logic around different application tokens.
    /// It implements <see cref="ITokensService"/>.
    /// </summary>
    /// <inheritdoc cref="ITokensService"/>
    public class TokensService : ITokensService
    {
        private readonly JwtSecurityTokenHandler tokenHandler;
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly TokenOptions tokenOptions;
        private readonly IDataProtector userIdProtector;
        private readonly IDistributedCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokensService"/> class.
        /// </summary>
        /// <param name="tokenOptions">The <see cref="TokenOptions"/>.</param>
        /// <param name="dataProtectionProvider">The implementation of <see cref="IDataProtectionProvider"/>.</param>
        /// <param name="cache">The implementation of <see cref="IDistributedCache"/>.</param>
        public TokensService(
            IOptions<TokenOptions> tokenOptions,
            IDataProtectionProvider dataProtectionProvider,
            IDistributedCache cache)
        {
            this.tokenHandler = new JwtSecurityTokenHandler();
            this.tokenOptions = tokenOptions.Value;
            this.tokenValidationParameters = this.tokenOptions.JwtBearerOptions.TokenValidationParameters;
            this.userIdProtector = dataProtectionProvider.CreateProtector(this.GetType().Namespace!, nameof(this.userIdProtector), "v1");
            this.cache = cache;
        }

        /// <inheritdoc/>
        public ServiceResult<string> GenerateAccessToken(UserDto user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, this.userIdProtector.Protect(user.Id.ToString())),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in user.Roles ?? Enumerable.Empty<string>())
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            return this.GenerateToken(authClaims, this.tokenOptions.AccessTokenLifetime);
        }

        /// <inheritdoc/>
        public ServiceResult<string> GenerateRefreshToken(UserDto user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, this.userIdProtector.Protect(user.Id.ToString())),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            return this.GenerateToken(claims, this.tokenOptions.RefreshTokenLifetime);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<string>> RefreshAccessTokenAsync(string refreshToken, string accessToken)
        {
            var refreshTokenJwt = this.tokenHandler.ReadJwtToken(refreshToken);
            var accessTokenJwt = this.tokenHandler.ReadJwtToken(accessToken);

            bool isRefreshTokenValid = await this.ValidateTokenAsync(refreshToken);
            bool isAccessTokenValid = await this.ValidateTokenAsync(accessToken, false);
            bool isAccessTokenRevoked = (await this.cache.GetAsync(accessTokenJwt.Id)) is not null;
            bool isRefreshTokenRevoked = (await this.cache.GetAsync(refreshTokenJwt.Id)) is not null;
            bool areUserIdsEqual = refreshTokenJwt.Subject == accessTokenJwt.Subject;

            if (!isRefreshTokenValid || !isAccessTokenValid || !areUserIdsEqual || isAccessTokenRevoked || isRefreshTokenRevoked)
            {
                new UnauthorizedDetails(ExceptionMessages.Unauthorized);
            }

            var oldAccessTokenJwt = this.tokenHandler.ReadJwtToken(accessToken);

            return this.GenerateToken(oldAccessTokenJwt.Claims, this.tokenOptions.AccessTokenLifetime);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult> RevokeTokenAsync(string token)
        {
            var tokenJwt = this.tokenHandler.ReadJwtToken(token);
            var isTokenValid = await this.ValidateTokenAsync(token);

            if (isTokenValid)
            {
                DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = tokenJwt.ValidTo,
                };

                await this.cache.SetStringAsync(tokenJwt.Id, string.Empty, cacheOptions);
            }

            return ServiceResult.Successful;
        }

        private string GenerateToken(IEnumerable<Claim> claims, TimeSpan expiresAfter)
        {
            SigningCredentials signingCredentials = new (this.tokenOptions.IssuerSigningKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: this.tokenValidationParameters.ValidIssuer,
                audience: this.tokenValidationParameters.ValidAudience,
                expires: DateTime.UtcNow.Add(expiresAfter),
                claims: claims,
                signingCredentials: signingCredentials);

            return this.tokenHandler.WriteToken(token);
        }

        private async Task<bool> ValidateTokenAsync(string token, bool validateLifetime = true)
        {
            var jwtToken = this.tokenHandler.ReadJwtToken(token);
            return await this.ValidateToken(jwtToken, validateLifetime);
        }

        private async Task<bool> ValidateToken(JwtSecurityToken jwtToken, bool validateLifetime = true)
        {
            var validateParameters = this.tokenValidationParameters.Clone();
            validateParameters.ValidateLifetime = validateLifetime;

            var validationResult = await this.tokenHandler.ValidateTokenAsync(jwtToken, validateParameters);
            return validationResult.IsValid;
        }
    }
}
