// |-----------------------------------------------------------------------------------------------------|
// <copyright file="MongoDbRepository.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Data
{
    using MongoDB.Driver;

    using MyKitchen.Common.Constants;
    using MyKitchen.Common.Guard;
    using MyKitchen.Common.Result;
    using MyKitchen.Microservices.Recipes.Data.Contracts;
    using MyKitchen.Microservices.Recipes.Data.Models.Common;

    /// <summary>
    /// This class represents a MongoDB repository. It implements the <see cref="IRepository{TDocument, TKey}"/>
    /// interface.
    /// </summary>
    /// <typeparam name="TDocument">The type of documents that the collection holds.</typeparam>
    /// <typeparam name="TKey">The type of the id of the <typeparamref name="TDocument"/>.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:Braces should not be omitted", Justification = "Cleaner visibility")]
    public class MongoDbRepository<TDocument, TKey> : IRepository<TDocument, TKey>
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
        public Result<TDocument, Exception> Find(TKey id, bool withDeleted)
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
        public async Task<Result<TDocument, Exception>> FindAsync(TKey id, bool withDeleted)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);

            if (!withDeleted)
            {
                filter &= Builders<TDocument>.Filter.Eq(d => d.IsDeleted, false);
            }

            var document = await this.mongoCollection.Find(filter).FirstOrDefaultAsync();

            exception = this.guard.AgainstNull<NullReferenceException>(
               document,
               ExceptionMessages.NoEntityWithPropertyFound,
               nameof(document),
               nameof(id));
            if (exception != null) return exception;

            return document;
        }

        /// <inheritdoc/>
        public Result<TDocument, Exception> Add(TDocument document)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(document, ExceptionMessages.ArgumentIsNull, nameof(document));
            if (exception != null) return exception;

            document.CreatedOn = DateTime.UtcNow;
            this.mongoCollection.InsertOne(document);

            return document;
        }

        /// <inheritdoc/>
        public async Task<Result<TDocument, Exception>> AddAsync(TDocument document)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(document, ExceptionMessages.ArgumentIsNull, nameof(document));
            if (exception != null) return exception;

            document.CreatedOn = DateTime.UtcNow;
            await this.mongoCollection.InsertOneAsync(document);

            return document;
        }

        /// <inheritdoc/>
        public Result<Exception> AddRange(IEnumerable<TDocument> documents)
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

            return Result<Exception>.Success;
        }

        /// <inheritdoc/>
        public async Task<Result<Exception>> AddRangeAsync(IEnumerable<TDocument> documents)
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

            return Result<Exception>.Success;
        }

        /// <inheritdoc/>
        public Result<Exception> Update(TKey id, TDocument document)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            exception = this.guard
                .AgainstNull<ArgumentNullException>(document, ExceptionMessages.ArgumentIsNull, nameof(document));
            if (exception != null) return exception;

            if (!EqualityComparer<TKey>.Default.Equals(id, document.Id))
            {
                return Result<Exception>.Success;
            }

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            document.ModifiedOn = DateTime.UtcNow;

            this.mongoCollection.ReplaceOne(filter, document);

            return Result<Exception>.Success;
        }

        /// <inheritdoc/>
        public async Task<Result<Exception>> UpdateAsync(TKey id, TDocument document)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            exception = this.guard
                .AgainstNull<ArgumentNullException>(document, ExceptionMessages.ArgumentIsNull, nameof(document));
            if (exception != null) return exception;

            if (!EqualityComparer<TKey>.Default.Equals(id, document.Id))
            {
                return Result<Exception>.Success;
            }

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            document.ModifiedOn = DateTime.UtcNow;

            await this.mongoCollection.ReplaceOneAsync(filter, document);

            return Result<Exception>.Success;
        }

        /// <inheritdoc/>
        public Result<Exception> Delete(TKey id)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            var update = Builders<TDocument>.Update
                .Set(d => d.IsDeleted, true)
                .Set(d => d.DeletedOn, DateTime.UtcNow);

            this.mongoCollection.UpdateOne(filter, update);

            return Result<Exception>.Success;
        }

        /// <inheritdoc/>
        public async Task<Result<Exception>> DeleteAsync(TKey id)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);
            var update = Builders<TDocument>.Update
                .Set(d => d.IsDeleted, true)
                .Set(d => d.DeletedOn, DateTime.UtcNow);

            await this.mongoCollection.UpdateOneAsync(filter, update);

            return Result<Exception>.Success;
        }

        /// <inheritdoc/>
        public Result<TDocument, Exception> Undelete(TKey id)
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
        public async Task<Result<TDocument, Exception>> UndeleteAsync(TKey id)
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
        public Result<Exception> HardDelete(TKey id)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);

            this.mongoCollection.DeleteOne(filter);

            return Result<Exception>.Success;
        }

        /// <inheritdoc/>
        public async Task<Result<Exception>> HardDeleteAsync(TKey id)
        {
            Exception? exception = this.guard
                .AgainstNull<ArgumentNullException>(id, ExceptionMessages.ArgumentIsNull, nameof(id));
            if (exception != null) return exception;

            var filter = Builders<TDocument>.Filter.Eq(d => d.Id, id);

            await this.mongoCollection.DeleteOneAsync(filter);

            return Result<Exception>.Success;
        }
    }
}
