// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RouteConstants.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Api.Rest.Constants
{
    /// <summary>
    /// This static class contains all routes and endpoints in the identity service.
    /// </summary>
    public static class RouteConstants
    {
        /// <summary>
        /// The uncaught error handler route.
        /// </summary>
        public const string ErrorHandlerRoute = $"{BaseRoute}/error";

        private const string BaseRoute = "/api/v1/recipes";

        /// <summary>
        /// This static class contains all routes and endpoints for the recipes controller.
        /// </summary>
        public static class Recipes
        {
            /// <summary>
            /// The endpoint for the recipes resource.
            /// </summary>
            public const string RecipesEndpoint = BaseRoute;

            /// <summary>
            /// The endpoint for a single recipe.
            /// </summary>
            public const string RecipeEndpoint = $"{BaseRoute}/{{id}}";
        }
    }
}
