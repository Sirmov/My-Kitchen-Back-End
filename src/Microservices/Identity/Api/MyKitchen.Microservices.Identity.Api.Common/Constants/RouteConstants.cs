// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RouteConstants.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Api.Common.Constants
{
    /// <summary>
    /// This static class contains all routes and endpoints in the identity service.
    /// </summary>
    public static class RouteConstants
    {
        /// <summary>
        /// The uncaught error handler route.
        /// </summary>
        public const string ErrorHandlerRoute = "/error";

        private const string BaseRoute = "api/v1";

        /// <summary>
        /// This static class contains all routes and endpoints for the users controller.
        /// </summary>
        public static class Users
        {
            /// <summary>
            /// The users controller base route.
            /// </summary>
            public const string BaseRoute = $"{RouteConstants.BaseRoute}/users";

            /// <summary>
            /// The user register endpoint.
            /// </summary>
            public const string RegisterEndpoint = $"register";

            /// <summary>
            /// The user login endpoint.
            /// </summary>
            public const string LoginEndpoint = $"login";

            /// <summary>
            /// The user logout endpoint.
            /// </summary>
            public const string LogoutEndpoint = $"logout";
        }
    }
}
