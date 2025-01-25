// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IdentityTokensServiceGrpcClient.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Clients.Identity.Grpc
{
    using System.Threading.Tasks;

    using MyKitchen.Microservices.Identity.Api.Grpc.Protos;
    using MyKitchen.Microservices.Recipes.Services.Clients.Identity.Grpc.Contracts;

    /// <summary>
    /// This class is a gRPC client of the identity service.
    /// </summary>
    public class IdentityTokensServiceGrpcClient : IIdentityTokensServiceGrpcClient
    {
        private readonly Tokens.TokensClient identityTokensServiceGrpcClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityTokensServiceGrpcClient"/> class.
        /// </summary>
        /// <param name="identityTokensServiceGrpcClient">The identity project tokens service grpc client.</param>
        public IdentityTokensServiceGrpcClient(Tokens.TokensClient identityTokensServiceGrpcClient)
        {
            this.identityTokensServiceGrpcClient = identityTokensServiceGrpcClient;
        }

        /// <inheritdoc/>
        public async Task<bool> IsAccessTokenRevokeAsync(string accessTokeId)
        {
            var request = new IsAccessTokenRevokedRequest()
            {
                AccessTokenId = accessTokeId,
            };

            var reply = await this.identityTokensServiceGrpcClient.IsAccessTokenRevokedAsync(request, new ());

            return reply.IsRevoked;
        }
    }
}
