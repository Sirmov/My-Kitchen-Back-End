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
        /// Gets the connection uri.
        /// </summary>
        public string ConnectionURI
#pragma warning disable SA1101 // Prefix local calls with this
            => $"mongodb://{Username}:{Password}@{Host}:{Port}/{DatabaseName}?authMechanism={AuthMechanism}&authSource={AuthenticationDatabase}";
#pragma warning restore SA1101 // Prefix local calls with this

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        [Required]
        public string Host { get; set; } = "localhost";

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        [Required]
        public string Port { get; set; } = "27017";

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; } = null!;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        public string DatabaseName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the name of the authentication database.
        /// </summary>
        public string AuthenticationDatabase { get; set; } = null!;

        /// <summary>
        /// Gets or sets the name of the authentication mechanism.
        /// </summary>
        public string AuthMechanism { get; set; } = null!;

        /// <summary>
        /// Gets or sets the name of the collection holding the users.
        /// </summary>
        public string UsersCollectionName { get; set; } = "users";

        /// <summary>
        /// Gets or sets the name of the collection holding the roles.
        /// </summary>
        public string RolesCollectionName { get; set; } = "roles";

        /// <summary>
        /// Gets or sets the name of the collection holding the migrations.
        /// </summary>
        public string MigrationsCollectionName { get; set; } = "_migrations";
    }
}
