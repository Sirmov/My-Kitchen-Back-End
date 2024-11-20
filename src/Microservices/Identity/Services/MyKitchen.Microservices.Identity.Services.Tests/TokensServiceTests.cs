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

    using MyKitchen.Common.Result.Contracts;
    using MyKitchen.Microservices.Identity.Common.Options;
    using MyKitchen.Microservices.Identity.Services.Common.Dtos.User;
    using MyKitchen.Microservices.Identity.Services.Tokens;

    /// <summary>
    /// This test fixture contains unit tests for the <see cref="TokensService}"/> class.
    /// </summary>
    [TestFixture]
    public class TokensServiceTests
    {
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
                SecurityKey = "This is a test security key. It should be used wisely!",
                JwtBearerOptions = new JwtBearerOptions()
                {
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = "Government",
                        ValidAudience = "Nation",
                    },
                },
                AccessTokenLifetime = TimeSpan.FromMinutes(15),
                RefreshTokenLifetime = TimeSpan.FromDays(7),
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
        public void GenerateAccessToken_ValidUserDto_GeneretesValidAccessTokenWithCorrectPayload()
        {
            // Arrange
            var userDto = new UserDto()
            {
                Id = ObjectId.GenerateNewId(),
                UserName = "test",
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
                Is.EqualTo(userDto.UserName),
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
        /// This test checks wheter <see cref="TokensService.GenerateRefreshToken(UserDto)"/>
        /// generates a valid refresh token with correct claims.
        /// </summary>
        [Test]
        public void GenerateRefreshToken_ValidUserDto_GeneratesValidRefreshTokenWithCorrectPayload()
        {
            // Arrange
            var userDto = new UserDto()
            {
                Id = ObjectId.GenerateNewId(),
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

        [Test]
        public void RefreshAccessTokenAsync_RefreshTokenNotValid_ReturnsUnauthorizedDetails()
        {
            // TODO
            Assert.Pass();
        }

        [Test]
        public void RefreshAccessTokenAsync_AccessTokenNotValid_ReturnsUnauthorizedDetails()
        {
            // TODO
            Assert.Pass();
        }

        [Test]
        public void RefreshAccessTokenAsync_UserIdsDoNotMatch_ReturnsUnauthorizedDetails()
        {
            // TODO
            Assert.Pass();
        }

        [Test]
        public void RefreshAccessTokenAsync_AccessTokenIsRevoked_ReturnsUnauthorizedDetails()
        {
            // TODO
            Assert.Pass();
        }

        [Test]
        public void RefreshAccessTokenAsync_RefreshTokenIsRevoked_ReturnsUnauthorizedDetails()
        {
            // TODO
            Assert.Pass();
        }

        [Test]
        public void RefreshAccessTokenAsync_BothTokensAreValid_ReturnsNewAccessTokenWithCorrectPayload()
        {
            // TODO
            Assert.Pass();
        }

        [Test]
        public void RevokeTokenAsync_TokenIsNotValid_ReturnsSuccess()
        {
            // TODO
            Assert.Pass();
        }

        [Test]
        public void RevokeTokenAsync_TokenIsValid_TokenIdIsCached()
        {
            // TODO
            Assert.Pass();
        }

        private void AssertResultIsSuccessful<TFailure>(IResult<TFailure> result)
            where TFailure : class
        {
            Assert.That(result.IsSuccessful, Is.True, "Result should be successful.");
            Assert.That(result.IsFailed, Is.False, "Result should not be failed.");
        }

    //     private void AssertResultIsFailed<TFailure>(IResult<TFailure> result)
    //         where TFailure : class
    //     {
    //         Assert.That(result.IsFailed, Is.True, "Result should be failed.");
    //         Assert.That(result.IsSuccessful, Is.False, "Result should not be successful.");
    //     }

    //     private string GenerateJwtToken(TimeSpan lifetime)
    //     {
    //         var tokenHandler = new JwtSecurityTokenHandler();
    //         var token = new JwtSecurityToken(
    //             issuer: this.tokenOptions.JwtBearerOptions.TokenValidationParameters.ValidIssuer,
    //             audience: this.tokenOptions.JwtBearerOptions.TokenValidationParameters.ValidAudience,
    //             expires: DateTime.Now.Add(lifetime),
    //             signingCredentials: new SigningCredentials(
    //                 this.tokenOptions.IssuerSigningKey,
    //                 SecurityAlgorithms.HmacSha256));

    //         return tokenHandler.WriteToken(token);
    //     }
    }
}
