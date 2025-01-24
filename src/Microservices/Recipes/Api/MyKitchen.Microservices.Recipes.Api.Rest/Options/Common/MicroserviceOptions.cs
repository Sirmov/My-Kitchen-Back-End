// |-----------------------------------------------------------------------------------------------------|
// <copyright file="MicroserviceOptions.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Api.Rest.Options.Common
{
    /// <summary>
    /// This abstract class defines the configuration needed for connecting to another microservice.
    /// </summary>
    public abstract class MicroserviceOptions
    {
        /// <summary>
        /// Gets or sets the name of the micro service.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the base url of the REST API of the micro service.
        /// </summary>
        public string RestApiBaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address of the gRPC API of the micro service.
        /// </summary>
        public string GrpcApiAddress { get; set; } = string.Empty;
    }
}
