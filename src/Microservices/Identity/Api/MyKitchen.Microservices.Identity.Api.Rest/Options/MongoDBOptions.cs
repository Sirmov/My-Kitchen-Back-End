// |-----------------------------------------------------------------------------------------------------|
// <copyright file="MongoDbOptions.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Api.Rest.Options
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// This class adapts the options patter in .NET.
    /// It is used to encapsulate the configuration of the MongoDB driver.
    /// </summary>
    public class MongoDbOptions
    {
        /// <summary>
        /// Gets or sets the connection uri.
        /// </summary>
        [Required]
        public string ConnectionURI { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>\
        [Required]
        public string DatabaseName { get; set; } = null!;
    }
}
