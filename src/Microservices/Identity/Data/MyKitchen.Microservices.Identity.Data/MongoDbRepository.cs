// |-----------------------------------------------------------------------------------------------------|
// <copyright file="MongoDbRepository.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Data
{
    using MongoDB.Driver;

    using MyKitchen.Common.Constants;
    using MyKitchen.Common.Guard;
    using MyKitchen.Microservices.Identity.Data.Common.QueryResult;
    using MyKitchen.Microservices.Identity.Data.Contracts;
    using MyKitchen.Microservices.Identity.Data.Models.Common;

    /// <summary>
    /// This class represents a MongoDB repository. It implements the <see cref="IMongoDbRepository{TDocument, TKey}"/>
    /// interface.
    /// </summary>
    /// <typeparam name="TDocument">The type of documents that the collection holds.</typeparam>
    /// <typeparam name="TKey">The type of the id of the <typeparamref name="TDocument"/>.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:Braces should not be omitted", Justification = "Cleaner visibility")]
    public class MongoDbRepository<TDocument, TKey> : IMongoDbRepository<TDocument, TKey>
            where TDocument : BaseDocument<TKey>
            where TKey : notnull
    {
        private readonly IGuard guard;

        private readonly IMongoDbContext mongoDbContext;

        private readonly IMongoCollection<TDocument> mongoCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbRepository{TDocument, TKey}"/> class.
        /// </summary>
        /// <param name="mongoDbContext">The mongoDB context.</param>
        /// <param name="guard">The implementation of <see cref="IGuard"/>.</param>
        public MongoDbRepository(IMongoDbContext mongoDbContext, IGuard guard)
        {
            this.mongoDbContext = mongoDbContext;
            this.guard = guard;
            this.mongoCollection = this.mongoDbContext.Collection<TDocument>();
        }

        /// <inheritdoc/>
        public IFindFluent<TDocument, TDocument> All(bool withDeleted)
        {
            var filter = withDeleted ?
                Builders<TDocument>.Filter.Empty :
                Builders<TDocument>.Filter.Eq(d => d.IsDeleted, false);

            return this.mongoCollection.Find(filter);
        }

        /// <inheritdoc/>
        public QueryResult<TDocument> Find(TKey id, bool withDeleted)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);

            if (!withDeleted)
            {
                filter &= Builders<TDocument>.Filter.Eq(d => d.IsDeleted, false);
            }

            var document = this.mongoCollection.Find(filter).FirstOrDefault();

            exception = this.guard.AgainstNull<NullReferenceException>(
                document,
                ExceptionMessages.NoEntityWithPropertyFound,
                nameof(document),
                nameof(id));
            if (exception != null) return exception;

            return document;
        }

        /// <inheritdoc/>
        public async Task<QueryResult<TDocument>> FindAsync(TKey id, bool withDeleted)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);

            if (!withDeleted)
            {
                filter &= Builders<TDocument>.Filter.Eq(d => d.IsDeleted, false);
            }

            var document = await this.mongoCollection.Find(filter).FirstAsync();

            exception = this.guard.AgainstNull<NullReferenceException>(
               document,
               ExceptionMessages.NoEntityWithPropertyFound,
               nameof(document),
               nameof(id));
            if (exception != null) return exception;

            return document;
        }

        /// <inheritdoc/>
        public QueryResult Add(TDocument document)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(document, ExceptionMessages.ArgumentIsNull, nameof(document));
            if (exception != null) return exception;

            document.CreatedOn = DateTime.UtcNow;
            this.mongoCollection.InsertOne(document);

            return QueryResult.Successful;
        }

        /// <inheritdoc/>
        public async Task<QueryResult> AddAsync(TDocument document)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(document, ExceptionMessages.ArgumentIsNull, nameof(document));
            if (exception != null) return exception;

            document.CreatedOn = DateTime.UtcNow;
            await this.mongoCollection.InsertOneAsync(document);

            return QueryResult.Successful;
        }

        /// <inheritdoc/>
        public QueryResult AddRange(IEnumerable<TDocument> documents)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(documents, ExceptionMessages.ArgumentIsNull, nameof(documents));
            if (exception != null) return exception;

            foreach (var document in documents)
            {
                exception = this.guard
                    .AgainstNull<NullReferenceException>(document, ExceptionMessages.VariableIsNull, nameof(document));
                if (exception != null) return exception;

                document.CreatedOn = DateTime.UtcNow;
            }

            this.mongoCollection.InsertMany(documents);

            return QueryResult.Successful;
        }

        /// <inheritdoc/>
        public async Task<QueryResult> AddRangeAsync(IEnumerable<TDocument> documents)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(documents, ExceptionMessages.ArgumentIsNull, nameof(documents));
            if (exception != null) return exception;

            foreach (var document in documents)
            {
                exception = this.guard
                   .AgainstNull<NullReferenceException>(document, ExceptionMessages.VariableIsNull, nameof(document));
                if (exception != null) return exception;

                document.CreatedOn = DateTime.UtcNow;
            }

            await this.mongoCollection.InsertManyAsync(documents);

            return QueryResult.Successful;
        }

        /// <inheritdoc/>
        public QueryResult<ReplaceOneResult> Update(TKey id, TDocument document)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            exception = this.guard
                .AgainstNull<ArgumentNullException>(document, ExceptionMessages.ArgumentIsNull, nameof(document));
            if (exception != null) return exception;

            if (!EqualityComparer<TKey>.Default.Equals(id, document.Id))
            {
                return new ReplaceOneResult.Acknowledged(0, 0, null);
            }

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            document.ModifiedOn = DateTime.UtcNow;

            return this.mongoCollection.ReplaceOne(filter, document);
        }

        /// <inheritdoc/>
        public async Task<QueryResult<ReplaceOneResult>> UpdateAsync(TKey id, TDocument document)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            exception = this.guard
                .AgainstNull<ArgumentNullException>(document, ExceptionMessages.ArgumentIsNull, nameof(document));
            if (exception != null) return exception;

            if (!EqualityComparer<TKey>.Default.Equals(id, document.Id))
            {
                return new ReplaceOneResult.Acknowledged(0, 0, null);
            }

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            document.ModifiedOn = DateTime.UtcNow;

            return await this.mongoCollection.ReplaceOneAsync(filter, document);
        }

        /// <inheritdoc/>
        public QueryResult<UpdateResult> Delete(TKey id)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            var update = Builders<TDocument>.Update
                .Set(d => d.IsDeleted, true)
                .Set(d => d.DeletedOn, DateTime.UtcNow);

            return this.mongoCollection.UpdateOne(filter, update);
        }

        /// <inheritdoc/>
        public async Task<QueryResult<UpdateResult>> DeleteAsync(TKey id)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            var update = Builders<TDocument>.Update
                .Set(d => d.IsDeleted, true)
                .Set(d => d.DeletedOn, DateTime.UtcNow);

            return await this.mongoCollection.UpdateOneAsync(filter, update);
        }

        /// <inheritdoc/>
        public QueryResult<TDocument> Undelete(TKey id)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            var update = Builders<TDocument>.Update
                .Set(d => d.IsDeleted, false)
                .Set(d => d.DeletedOn, null);

            return this.mongoCollection.FindOneAndUpdate(filter, update);
        }

        /// <inheritdoc/>
        public async Task<QueryResult<TDocument>> UndeleteAsync(TKey id)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            var update = Builders<TDocument>.Update
                .Set(d => d.IsDeleted, false)
                .Set(d => d.DeletedOn, null);

            return await this.mongoCollection.FindOneAndUpdateAsync(filter, update);
        }

        /// <inheritdoc/>
        public QueryResult<DeleteResult> HardDelete(TKey id)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);

            return this.mongoCollection.DeleteOne(filter);
        }

        /// <inheritdoc/>
        public async Task<QueryResult<DeleteResult>> HardDeleteAsync(TKey id)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);

            return await this.mongoCollection.DeleteOneAsync(filter);
        }
    }
}
