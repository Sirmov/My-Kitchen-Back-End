// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IMongoDbContext.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Data.Contracts
{
    using MongoDB.Driver;

    /// <summary>
    /// This interface defines the mongoDB context.
    /// </summary>
    public interface IMongoDbContext
    {
        /// <summary>
        /// Gets the mongoDB client.
        /// </summary>
        public IMongoClient MongoClient { get; }

        /// <summary>
        /// Gets the mongoDB database.
        /// </summary>
        public IMongoDatabase MongoDatabase { get; }

        /// <summary>
        /// This method returns the collection containing the <typeparamref name="TDocument"/>s.
        /// If the collection name differs from the <typeparamref name="TDocument"/> name,
        /// the <paramref name="collectionName"/> has to be specified.
        /// </summary>
        /// <typeparam name="TDocument">The type of document the collection contains.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>Returns a <see cref="IMongoCollection{TDocument}"/> containing <typeparamref name="TDocument"/>s.</returns>
        public IMongoCollection<TDocument> Collection<TDocument>(string? collectionName = null);
    }
}
