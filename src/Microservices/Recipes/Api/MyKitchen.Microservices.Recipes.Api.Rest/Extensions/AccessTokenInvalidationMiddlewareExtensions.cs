// |-----------------------------------------------------------------------------------------------------|
// <copyright file="AccessTokenInvalidationMiddlewareExtensions.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Api.Rest.Extensions
{
    using MyKitchen.Microservices.Recipes.Api.Rest.Middlewares;

    /// <summary>
    /// This static class contains extension method related to the <see cref="AccessTokenInvalidationMiddleware"/>.
    /// </summary>
    public static class AccessTokenInvalidationMiddlewareExtensions
    {
        /// <summary>
        /// This <see cref="IApplicationBuilder"/> extension method adds the <see cref="AccessTokenInvalidationMiddleware"/>
        /// for invalidating revoked access tokens.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="IApplicationBuilder"/> where the <see cref="AccessTokenInvalidationMiddleware"/> should be added.
        /// </param>
        /// <returns>Returns the application <paramref name="builder"/> after adding the <see cref="AccessTokenInvalidationMiddleware"/>.</returns>
        public static IApplicationBuilder UseAccessTokenInvalidation(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AccessTokenInvalidationMiddleware>();
        }
    }
}
