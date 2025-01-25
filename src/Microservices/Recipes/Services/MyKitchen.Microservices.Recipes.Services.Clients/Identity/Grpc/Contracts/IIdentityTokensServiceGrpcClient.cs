// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IIdentityTokensServiceGrpcClient.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Clients.Identity.Grpc.Contracts
{
    /// <summary>
    /// This interface defines the functionality of the identity service gRPC API client.
    /// </summary>
    public interface IIdentityTokensServiceGrpcClient
    {
        /// <summary>
        /// This method asynchronously checks whether the access token with <paramref name="accessTokeId"/> is revoked.
        /// </summary>
        /// <param name="accessTokeId">The id of the access token to be validated.</param>
        /// <returns>Returns a <see cref="Task"/> of <see langword="bool"/> indicating whether the access token was revoked.</returns>
        public Task<bool> IsAccessTokenRevokeAsync(string accessTokeId);
    }
}
