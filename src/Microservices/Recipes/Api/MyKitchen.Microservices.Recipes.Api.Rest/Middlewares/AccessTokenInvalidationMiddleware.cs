// |-----------------------------------------------------------------------------------------------------|
// <copyright file="AccessTokenInvalidationMiddleware.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Api.Rest.Middlewares
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Net;
    using System.Net.Mime;
    using System.Text;
    using System.Text.Json;

    using MyKitchen.Common.Constants;
    using MyKitchen.Common.ProblemDetails;
    using MyKitchen.Microservices.Recipes.Services.Clients.Identity.Grpc.Contracts;

    /// <summary>
    /// This middleware is used to invalidate revoked access tokens.
    /// It implements the <see cref="IMiddleware"/> interface.
    /// </summary>
    public class AccessTokenInvalidationMiddleware : IMiddleware
    {
        private readonly IIdentityTokensServiceGrpcClient tokensClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenInvalidationMiddleware"/> class.
        /// </summary>
        /// <param name="tokensClient">The implementation of <see cref="IIdentityTokensServiceGrpcClient"/>.</param>
        public AccessTokenInvalidationMiddleware(IIdentityTokensServiceGrpcClient tokensClient)
        {
            this.tokensClient = tokensClient;
        }

        /// <inheritdoc/>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var authorizationHeader = context.Request.Headers["Authorization"];

            if (context.User.Identity?.IsAuthenticated ?? false && authorizationHeader.ToString().StartsWith("Bearer "))
            {
                string accessToken = authorizationHeader.ToString().Substring("Bearer ".Length).Trim();

                context.Items["AccessToken"] = accessToken;

                var handler = new JwtSecurityTokenHandler();
                var jwtBearerToken = handler.ReadJwtToken(accessToken);
                var tokenId = jwtBearerToken.Id;

                var isRevoked = await this.tokensClient.IsAccessTokenRevokeAsync(tokenId);

                if (isRevoked)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = MediaTypeNames.Application.Json;

                    UnauthorizedDetails problemDetails = new (ExceptionMessages.AccessTokenRevoked);
                    string payload = JsonSerializer.Serialize(problemDetails);
                    await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(payload));

                    return;
                }
            }

            await next(context);
        }
    }
}
