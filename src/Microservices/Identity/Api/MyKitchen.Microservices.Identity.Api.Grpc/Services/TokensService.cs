// |-----------------------------------------------------------------------------------------------------|
// <copyright file="TokensService.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Api.Grpc.Services
{
    using System.Threading.Tasks;

    using Microsoft.Extensions.Caching.Distributed;

    using MyKitchen.Microservices.Identity.Api.Grpc.Protos;

    /// <summary>
    /// This gRPC service is responsible for calls regarding JWT tokens.
    /// </summary>
    public class TokensService : Tokens.TokensBase
    {
        private readonly IDistributedCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokensService"/> class.
        /// </summary>
        /// <param name="cache">The implementation of <see cref="IDistributedCache"/>.</param>
        public TokensService(IDistributedCache cache)
        {
            this.cache = cache;
        }

        /// <summary>
        /// This remote procedure call invalidates a access token given it's id.
        /// </summary>
        /// <param name="request">The <see cref="IsAccessTokenRevokedRequest"/> message containing the access token id.</param>
        /// <param name="context">The server-side call context.</param>
        /// <returns>Returns a <see cref="IsAccessTokenRevokedReply"/> containing a <see langword="bool"/> indicating whether the access token is valid.</returns>
        public override async Task<IsAccessTokenRevokedReply> IsAccessTokenRevoked(IsAccessTokenRevokedRequest request, ServerCallContext context)
        {
            var entry = await this.cache.GetAsync(request.AccessTokenId);

            return new IsAccessTokenRevokedReply
            {
                IsRevoked = entry is not null,
            };
        }
    }
}
