// |-----------------------------------------------------------------------------------------------------|
// <copyright file="TokensServiceTests.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Tests
{
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    using MongoDB.Bson;

    using Moq;

    using MyKitchen.Common.ProblemDetails;
    using MyKitchen.Common.Result.Contracts;
    using MyKitchen.Microservices.Identity.Common.Options;
    using MyKitchen.Microservices.Identity.Services.Common.Dtos.User;
    using MyKitchen.Microservices.Identity.Services.Tokens;

    /// <summary>
    /// This test fixture contains unit tests for the <see cref="TokensService"/> class.
    /// </summary>
    [TestFixture]
    public class TokensServiceTests
    {
        private const string SecurityKey = "This is a test security key. It should be used wisely!";
        private const string ValidIssuer = "Government";
        private const string ValidAudience = "Nation";
        private const string AccessTokenLifetime = "0.00:15:00";
        private const string RefreshTokenLifetime = "7.00:00:00";

        private readonly JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        private Mock<IOptions<TokenOptions>> tokenOptionsMock = null!;
        private Mock<IDataProtectionProvider> dataProtectionProviderMock = null!;
        private Mock<IDataProtector> dataProtectorMock = null!;
        private Mock<IDistributedCache> distributedCacheMock = null!;

        private TokenOptions tokenOptions = null!;
        private TokensService tokensService = null!;

        /// <summary>
        /// This method is called before running every test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.distributedCacheMock = new Mock<IDistributedCache>();

            this.dataProtectorMock = new Mock<IDataProtector>();
            this.dataProtectorMock
                .Setup(protector => protector.Protect(It.IsAny<byte[]>()))
                .Returns((byte[] input) => Encoding.UTF8.GetBytes($"protected_{Encoding.UTF8.GetString(input)}"));
            this.dataProtectorMock
                .Setup(protector => protector.Unprotect(It.IsAny<byte[]>()))
                .Returns((byte[] output) => Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(output).Remove(0, 10)));
            this.dataProtectorMock
                .Setup(protector => protector.CreateProtector(It.IsAny<string>()))
                .Returns(this.dataProtectorMock.Object);

            this.dataProtectionProviderMock = new Mock<IDataProtectionProvider>();
            this.dataProtectionProviderMock
                .Setup(s => s.CreateProtector(It.IsAny<string>()))
                .Returns(this.dataProtectorMock.Object);

            this.tokenOptions = new TokenOptions()
            {
                SecurityKey = SecurityKey,
                JwtBearerOptions = new JwtBearerOptions()
                {
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = ValidIssuer,
                        ValidAudience = ValidAudience,
                    },
                },
                AccessTokenLifetime = TimeSpan.Parse(AccessTokenLifetime, CultureInfo.InvariantCulture),
                RefreshTokenLifetime = TimeSpan.Parse(RefreshTokenLifetime, CultureInfo.InvariantCulture),
            };

            this.tokenOptionsMock = new Mock<IOptions<TokenOptions>>();
            this.tokenOptionsMock.Setup(options => options.Value).Returns(this.tokenOptions);

            this.tokensService = new TokensService(
                this.tokenOptionsMock.Object,
                this.dataProtectionProviderMock.Object,
                this.distributedCacheMock.Object);
        }

        /// <summary>
        /// This test checks whether <see cref="TokensService.GenerateAccessToken(UserDto)"/>
        /// generates a valid access token and sets the correct claims.
        /// </summary>
        [Test]
        public void GenerateAccessToken_ValidUserDto_GeneratesValidAccessTokenWithCorrectPayload()
        {
            // Arrange
            var userDto = new UserDto()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Username = "test",
                Email = "test@mail.com",
                Roles = ["Admin", "Owner", "User"],
            };

            // Act
            var accessTokenResult = this.tokensService.GenerateAccessToken(userDto);
            var timeCreated = DateTime.UtcNow;

            // Assert
            this.AssertResultIsSuccessful(accessTokenResult);

            var jwtToken = this.tokenHandler.ReadJwtToken(accessTokenResult.Data);

            Assert.That(
                jwtToken.Issuer,
                Is.EqualTo(this.tokenOptions.JwtBearerOptions.TokenValidationParameters.ValidIssuer),
                "Access token issuer is not correct.");
            Assert.That(
                jwtToken.Audiences.First(),
                Is.EqualTo(this.tokenOptions.JwtBearerOptions.TokenValidationParameters.ValidAudience),
                "Access token audience is not correct.");

            Assert.That(
                this.dataProtectorMock.Object.Unprotect(jwtToken.Subject),
                Is.EqualTo(userDto.Id.ToString()),
                "Access token user id is not correct.");
            Assert.That(
                jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value,
                Is.EqualTo(userDto.Username),
                "Access token username claim is not correct.");
            Assert.That(
                jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value,
                Is.EqualTo(userDto.Email),
                "Access token email claim is not correct.");
            Assert.That(
                jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value),
                Is.EquivalentTo(userDto.Roles),
                "Access token role claims are not correct.");

            var validUntil = timeCreated.Add(this.tokenOptions.AccessTokenLifetime);
            Assert.That(
                jwtToken.ValidTo.ToString("yyyy/MM/dd/HH/mm", CultureInfo.InvariantCulture),
                Is.EqualTo(validUntil.ToString("yyyy/MM/dd/HH/mm", CultureInfo.InvariantCulture)),
                "Access token lifetime is not correct.");
        }

        /// <summary>
        /// This test checks whether <see cref="TokensService.GenerateRefreshToken(UserDto)"/>
        /// generates a valid refresh token with correct claims.
        /// </summary>
        [Test]
        public void GenerateRefreshToken_ValidUserDto_GeneratesValidRefreshTokenWithCorrectPayload()
        {
            // Arrange
            var userDto = new UserDto()
            {
                Id = ObjectId.GenerateNewId().ToString(),
            };

            // Act
            var refreshTokenResult = this.tokensService.GenerateRefreshToken(userDto);
            var timeCreated = DateTime.UtcNow;

            // Assert
            this.AssertResultIsSuccessful(refreshTokenResult);

            var refreshToken = this.tokenHandler.ReadJwtToken(refreshTokenResult.Data);

            Assert.That(this.dataProtectorMock.Object.Unprotect(refreshToken.Subject), Is.EqualTo(userDto.Id.ToString()), "Refresh token user id is not correct.");

            var validUntil = timeCreated.Add(this.tokenOptions.RefreshTokenLifetime);
            Assert.That(
                refreshToken.ValidTo.ToString("yyyy/MM/dd/HH/mm", CultureInfo.InvariantCulture),
                Is.EqualTo(validUntil.ToString("yyyy/MM/dd/HH/mm", CultureInfo.InvariantCulture)),
                "Refresh token lifetime is not correct.");
        }

        /// <summary>
        /// This test checks whether the <see cref="TokensService.RefreshAccessTokenAsync(string, string)"/>
        /// returns a failed result with <see cref="UnauthorizedDetails"/> when the refresh token is not valid.
        /// </summary>
        /// <param name="issuer">The issuer of the refresh token.</param>
        /// <param name="audience">The audience of the refresh token.</param>
        /// <param name="lifetime">The lifetime of the refresh token.</param>
        /// <param name="securityKey">The security key used to sign the refresh token.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        [TestCase("Invalid issuer", ValidAudience, RefreshTokenLifetime, SecurityKey)]
        [TestCase(ValidIssuer, "Invalid audience", RefreshTokenLifetime, SecurityKey)]
        [TestCase(ValidIssuer, ValidAudience, "0.00:00:00.0000", SecurityKey)]
        [TestCase(ValidIssuer, ValidAudience, RefreshTokenLifetime, "This security key is spare. Be sure to no use it!")]
        public async Task RefreshAccessTokenAsync_RefreshTokenNotValid_ReturnsUnauthorizedDetails(string issuer, string audience, string lifetime, string securityKey)
        {
            // Arrange
            Claim[] claims = [ new Claim(JwtRegisteredClaimNames.Sub, "User id") ];

            string refreshToken = this.GenerateJwtToken(
                issuer, audience, TimeSpan.Parse(lifetime, CultureInfo.InvariantCulture), securityKey, claims);

            string accessToken = this.GenerateJwtToken(
                ValidIssuer, ValidAudience, TimeSpan.Parse(AccessTokenLifetime, CultureInfo.InvariantCulture), SecurityKey, claims);

            this.distributedCacheMock
                .Setup(cache => cache.GetAsync(It.IsAny<string>(), default))
                .ReturnsAsync((byte[]?)null);

            // Act
            var refreshResult = await this.tokensService.RefreshAccessTokenAsync(refreshToken, accessToken);

            // Assert
            this.AssertResultIsFailed(refreshResult);
            Assert.That(refreshResult.Failure, Is.TypeOf<UnauthorizedDetails>());
            Assert.That(refreshResult.Data, Is.Null);
        }

        /// <summary>
        /// This test checks whether the <see cref="TokensService.RefreshAccessTokenAsync(string, string)"/>
        /// returns a failed result with <see cref="UnauthorizedDetails"/> when the access token is not valid.
        /// </summary>
        /// <param name="issuer">The issuer of the access token.</param>
        /// <param name="audience">The audience of the access token.</param>
        /// <param name="securityKey">The security key used to sign the access token.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        [TestCase("Invalid issuer", ValidAudience, SecurityKey)]
        [TestCase(ValidIssuer, "Invalid audience", SecurityKey)]
        [TestCase(ValidIssuer, ValidAudience, "This security key is spare. Be sure to no use it!")]
        public async Task RefreshAccessTokenAsync_AccessTokenNotValid_ReturnsUnauthorizedDetails(string issuer, string audience, string securityKey)
        {
            // Arrange
            Claim[] claims = [new Claim(JwtRegisteredClaimNames.Sub, "User id")];

            string refreshToken = this.GenerateJwtToken(
                ValidIssuer, ValidAudience, TimeSpan.Parse(RefreshTokenLifetime, CultureInfo.InvariantCulture), SecurityKey, claims);

            string accessToken = this.GenerateJwtToken(
                issuer, audience, TimeSpan.Zero, securityKey, claims);

            this.distributedCacheMock
                .Setup(cache => cache.GetAsync(It.IsAny<string>(), default))
                .ReturnsAsync((byte[]?)null);

            // Act
            var refreshResult = await this.tokensService.RefreshAccessTokenAsync(accessToken, accessToken);

            // Assert
            this.AssertResultIsFailed(refreshResult);
            Assert.That(refreshResult.Failure, Is.TypeOf<UnauthorizedDetails>());
            Assert.That(refreshResult.Data, Is.Null);
        }

        /// <summary>
        /// This test checks whether the <see cref="TokensService.RefreshAccessTokenAsync(string, string)"/>
        /// returns a failed result with <see cref="UnauthorizedDetails"/> when the value of the subject claims
        /// of the access token and refresh token don't match.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task RefreshAccessTokenAsync_UserIdsDoNotMatch_ReturnsUnauthorizedDetails()
        {
            // Arrange
            Claim[] refreshTokenClaims = [ new Claim(JwtRegisteredClaimNames.Sub, "User1 id") ];
            Claim[] accessTokenClaims = [ new Claim(JwtRegisteredClaimNames.Sub, "User2 id") ];

            string refreshToken = this.GenerateJwtToken(
                ValidIssuer, ValidAudience, TimeSpan.Parse(RefreshTokenLifetime, CultureInfo.InvariantCulture), SecurityKey, refreshTokenClaims);

            string accessToken = this.GenerateJwtToken(
                ValidIssuer, ValidAudience, TimeSpan.Parse(AccessTokenLifetime, CultureInfo.InvariantCulture), SecurityKey, accessTokenClaims);

            this.distributedCacheMock
                .Setup(cache => cache.GetAsync(It.IsAny<string>(), default))
                .ReturnsAsync((byte[]?)null);

            // Act
            var refreshResult = await this.tokensService.RefreshAccessTokenAsync(refreshToken, accessToken);

            // Assert
            this.AssertResultIsFailed(refreshResult);
            Assert.That(refreshResult.Failure, Is.TypeOf<UnauthorizedDetails>());
            Assert.That(refreshResult.Data, Is.Null);
        }

        /// <summary>
        /// This test checks whether the <see cref="TokensService.RefreshAccessTokenAsync(string, string)"/>
        /// returns a failed result with <see cref="UnauthorizedDetails"/> when the either of the tokens is revoked.
        /// </summary>
        /// <param name="isRefreshTokenRevoked">A boolean indicating whether the refresh token is revoked.</param>
        /// <param name="isAccessTokenRevoked">A boolean indicating whether the access token is revoked.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public async Task RefreshAccessTokenAsync_TokenIsRevoked_ReturnsUnauthorizedDetails(bool isRefreshTokenRevoked, bool isAccessTokenRevoked)
        {
            // Arrange
            string refreshTokenId = Guid.NewGuid().ToString();
            string accessTokenId = Guid.NewGuid().ToString();

            Claim[] refreshTokenClaims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, "User1 id"),
                new Claim(JwtRegisteredClaimNames.Jti, refreshTokenId),
            ];
            Claim[] accessTokenClaims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, "User1 id"),
                new Claim(JwtRegisteredClaimNames.Jti, accessTokenId),
            ];

            string refreshToken = this.GenerateJwtToken(
                ValidIssuer, ValidAudience, TimeSpan.Parse(RefreshTokenLifetime, CultureInfo.InvariantCulture), SecurityKey, refreshTokenClaims);

            string accessToken = this.GenerateJwtToken(
                ValidIssuer, ValidAudience, TimeSpan.Parse(AccessTokenLifetime, CultureInfo.InvariantCulture), SecurityKey, accessTokenClaims);

            this.distributedCacheMock
                .Setup(cache => cache.GetAsync(It.IsAny<string>(), default))
                .ReturnsAsync((byte[]?)null);

            string revokedTokenId = isRefreshTokenRevoked ? refreshTokenId : isAccessTokenRevoked ? accessTokenId : It.IsAny<string>();

            this.distributedCacheMock
                .Setup(cache => cache.GetAsync(revokedTokenId, default))
                .ReturnsAsync([]);

            // Act
            var refreshResult = await this.tokensService.RefreshAccessTokenAsync(refreshToken, accessToken);

            // Assert
            this.AssertResultIsFailed(refreshResult);
            Assert.That(refreshResult.Failure, Is.TypeOf<UnauthorizedDetails>());
            Assert.That(refreshResult.Data, Is.Null);
        }

        /// <summary>
        /// This test checks whether <see cref="TokensService.RefreshAccessTokenAsync(string, string)"/>
        /// returns a new access token with correct payload when both of the token are valid.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task RefreshAccessTokenAsync_BothTokensAreValid_ReturnsNewAccessTokenWithCorrectPayload()
        {
            // Arrange
            var userDto = new UserDto()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Username = "test",
                Email = "test@mail.com",
                Roles = ["Admin", "Owner", "User"],
            };

            Claim[] refreshTokenClaims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, userDto.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            ];

            Claim[] accessTokenClaims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, userDto.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, userDto.Username),
                new Claim(JwtRegisteredClaimNames.Email, userDto.Email),
                ..userDto.Roles.Select(r => new Claim(ClaimTypes.Role, r)),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            ];

            string refreshToken = this.GenerateJwtToken(
                ValidIssuer, ValidAudience, TimeSpan.Parse(RefreshTokenLifetime, CultureInfo.InvariantCulture), SecurityKey, refreshTokenClaims);

            string accessToken = this.GenerateJwtToken(
                ValidIssuer, ValidAudience, TimeSpan.Parse(AccessTokenLifetime, CultureInfo.InvariantCulture), SecurityKey, accessTokenClaims);

            this.distributedCacheMock
                .Setup(cache => cache.GetAsync(It.IsAny<string>(), default))
                .ReturnsAsync((byte[]?)null);

            // Act
            var refreshResult = await this.tokensService.RefreshAccessTokenAsync(accessToken, accessToken);
            var timeCreated = DateTime.UtcNow;

            // Assert
            this.AssertResultIsSuccessful(refreshResult);

            var jwtToken = this.tokenHandler.ReadJwtToken(refreshResult.Data);

            Assert.That(
                jwtToken.Issuer,
                Is.EqualTo(this.tokenOptions.JwtBearerOptions.TokenValidationParameters.ValidIssuer),
                "Access token issuer is not correct.");
            Assert.That(
                jwtToken.Audiences.First(),
                Is.EqualTo(this.tokenOptions.JwtBearerOptions.TokenValidationParameters.ValidAudience),
                "Access token audience is not correct.");

            Assert.That(
                jwtToken.Subject,
                Is.EqualTo(userDto.Id.ToString()),
                "Access token user id is not correct.");
            Assert.That(
                jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value,
                Is.EqualTo(userDto.Username),
                "Access token username claim is not correct.");
            Assert.That(
                jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value,
                Is.EqualTo(userDto.Email),
                "Access token email claim is not correct.");
            Assert.That(
                jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value),
                Is.EquivalentTo(userDto.Roles),
                "Access token role claims are not correct.");

            var validUntil = timeCreated.Add(this.tokenOptions.AccessTokenLifetime);
            Assert.That(
                jwtToken.ValidTo.ToString("yyyy/MM/dd/HH/mm", CultureInfo.InvariantCulture),
                Is.EqualTo(validUntil.ToString("yyyy/MM/dd/HH/mm", CultureInfo.InvariantCulture)),
                "Access token lifetime is not correct.");
        }

        /// <summary>
        /// This test checks whether <see cref="TokensService.RevokeTokenAsync(string)"/>
        /// returns a successful result and doesn't cache the token id.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous task.</returns>
        [Test]
        public async Task RevokeTokenAsync_TokenIsNotValid_TokenIdIsNotCached()
        {
            // Arrange
            var token = this.GenerateJwtToken(string.Empty, string.Empty, TimeSpan.Zero, "This security key is spare. Be sure to no use it!");

            // Act
            var revokeResult = await this.tokensService.RevokeTokenAsync(token);

            // Assert
            this.AssertResultIsSuccessful(revokeResult);

            this.distributedCacheMock
                .Verify(cache => cache.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Never);
        }

        /// <summary>
        /// This test checks whether <see cref="TokensService.RevokeTokenAsync(string)"/>
        /// returns a successful result and caches the token id with the correct lifetime.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous task.</returns>
        [Test]
        public async Task RevokeTokenAsync_TokenIsValid_TokenIdIsCached()
        {
            // Arrange
            var tokenId = Guid.NewGuid().ToString();
            Claim[] tokenClaims = [ new Claim(JwtRegisteredClaimNames.Jti, tokenId) ];
            var token = this.GenerateJwtToken(ValidIssuer, ValidAudience, TimeSpan.FromDays(1), SecurityKey, tokenClaims);

            // Act
            var revokeResult = await this.tokensService.RevokeTokenAsync(token);

            // Assert
            var jwtToken = this.tokenHandler.ReadToken(token);

            this.AssertResultIsSuccessful(revokeResult);

            var cacheEntryOptions = It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpiration == jwtToken.ValidTo);
            this.distributedCacheMock
                .Verify(cache => cache.SetAsync(tokenId, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Once);
        }

        private void AssertResultIsSuccessful<TFailure>(IResult<TFailure> result)
            where TFailure : class
        {
            Assert.That(result.IsSuccessful, Is.True, "Result should be successful.");
            Assert.That(result.IsFailed, Is.False, "Result should not be failed.");
        }

        private void AssertResultIsFailed<TFailure>(IResult<TFailure> result)
            where TFailure : class
        {
            Assert.That(result.IsFailed, Is.True, "Result should be failed.");
            Assert.That(result.IsSuccessful, Is.False, "Result should not be successful.");
        }

        private string GenerateJwtToken(string issuer, string audience, TimeSpan lifetime, string secret, IEnumerable<Claim>? claims = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.UtcNow.Add(lifetime),
                claims: claims ?? Enumerable.Empty<Claim>(),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    SecurityAlgorithms.HmacSha256));

            return tokenHandler.WriteToken(token);
        }
    }
}
