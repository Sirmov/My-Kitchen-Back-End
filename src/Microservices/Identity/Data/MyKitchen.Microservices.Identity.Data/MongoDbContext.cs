// |-----------------------------------------------------------------------------------------------------|
// <copyright file="MongoDbContext.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Data
{
    using MongoDB.Driver;

    using MyKitchen.Microservices.Identity.Data.Contracts;

    /// <summary>
    /// This class represents a MongoDB context. It implements the <see cref="IMongoDbContext"/> interface.
    /// </summary>
    public class MongoDbContext : IMongoDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbContext"/> class.
        /// </summary>
        /// <param name="mongoClient">The mongoDB client.</param>
        /// <param name="mongoDatabase">The mongoDB database.</param>
        public MongoDbContext(IMongoClient mongoClient, IMongoDatabase mongoDatabase)
        {
            this.MongoClient = mongoClient;
            this.MongoDatabase = mongoDatabase;
        }

        /// <inheritdoc/>
        public IMongoClient MongoClient { get; }

        /// <inheritdoc/>
        public IMongoDatabase MongoDatabase { get; }

        /// <inheritdoc/>
        public IMongoCollection<TDocument> Collection<TDocument>(string? collectionName = null)
        {
            string name = collectionName ?? typeof(TDocument).Name;

            return this.MongoDatabase.GetCollection<TDocument>(name);
        }
    }
}
