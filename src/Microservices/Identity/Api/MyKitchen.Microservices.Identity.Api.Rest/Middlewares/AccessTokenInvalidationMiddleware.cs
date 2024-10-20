// |-----------------------------------------------------------------------------------------------------|
// <copyright file="AccessTokenInvalidationMiddleware.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Api.Rest.Middlewares
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Net;
    using System.Net.Mime;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Caching.Distributed;

    using MyKitchen.Common.Constants;
    using MyKitchen.Common.ProblemDetails;

    /// <summary>
    /// This middleware is used to invalidate revoked access tokens.
    /// It implements the <see cref="IMiddleware"/> interface.
    /// </summary>
    public class AccessTokenInvalidationMiddleware : IMiddleware
    {
        private readonly IDistributedCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenInvalidationMiddleware"/> class.
        /// </summary>
        /// <param name="cache">The implementation of <see cref="IDistributedCache"/>.</param>
        public AccessTokenInvalidationMiddleware(IDistributedCache cache)
        {
            this.cache = cache;
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

                var entry = await this.cache.GetAsync(tokenId);

                if (entry is not null)
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
