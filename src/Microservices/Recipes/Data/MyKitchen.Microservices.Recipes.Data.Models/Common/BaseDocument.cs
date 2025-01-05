// |-----------------------------------------------------------------------------------------------------|
// <copyright file="BaseDocument.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Data.Models.Common
{
    using MongoDB.Bson.Serialization.Attributes;

    using MyKitchen.Microservices.Recipes.Data.Models.Contracts;

    /// <summary>
    /// This abstract class is a base class for all documents.
    /// </summary>
    /// <typeparam name="TKey">The type of the id of the document.</typeparam>
    public abstract class BaseDocument<TKey> : IAuditable, IDeletable
    {
        /// <summary>
        /// Gets or sets the id of the document.
        /// </summary>
        [BsonId]
        public TKey Id { get; set; } = default!;

        /// <inheritdoc/>
        public DateTime CreatedOn { get; set; }

        /// <inheritdoc/>
        public DateTime? ModifiedOn { get; set; }

        /// <inheritdoc/>
        public bool IsDeleted { get; set; }

        /// <inheritdoc/>
        public DateTime? DeletedOn { get; set; }
    }
}
