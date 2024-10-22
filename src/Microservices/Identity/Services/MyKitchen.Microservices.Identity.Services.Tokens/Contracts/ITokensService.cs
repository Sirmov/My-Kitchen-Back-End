// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ITokensService.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Tokens.Contracts
{
    using MyKitchen.Microservices.Identity.Services.Common.Dtos.User;
    using MyKitchen.Microservices.Identity.Services.Common.ServiceResult;

    /// <summary>
    /// This interface defines the functionality of the tokens service.
    /// </summary>
    public interface ITokensService
    {
        /// <summary>
        /// This method generates a JWT bearer access token based on a identity <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The <see cref="UserDto"/> containing the user claims.</param>
        /// <returns>Returns a <see langword="string"/> representation of the access token.</returns>
        public ServiceResult<string> GenerateAccessToken(UserDto user);

        /// <summary>
        /// This method generates a JWT refresh token based on a identity <paramref name="user"/>
        /// used for generating new access tokens.
        /// </summary>
        /// <param name="user">The <see cref="UserDto"/> containing the user claims.</param>
        /// <returns>Returns a <see langword="string"/> representation of the refresh token. </returns>
        public ServiceResult<string> GenerateRefreshToken(UserDto user);

        /// <summary>
        /// This method asynchronously generates a new access token using a refresh token and the old access token.
        /// </summary>
        /// <param name="refreshToken">The <see langword="string"/> representation of the refresh token.</param>
        /// <param name="accessToken">The <see langword="string"/> representation of the access token.</param>
        /// <returns>Returns a <see langword="string"/> representation of the new refreshed access token.</returns>
        public Task<ServiceResult<string>> RefreshAccessTokenAsync(string refreshToken, string accessToken);

        /// <summary>
        /// This method asynchronously revokes a token.
        /// </summary>
        /// <param name="token">The token to be revoked.</param>
        /// <returns>Returns an empty <see cref="ServiceResult"/>.</returns>
        public Task<ServiceResult> RevokeTokenAsync(string token);
    }
}
