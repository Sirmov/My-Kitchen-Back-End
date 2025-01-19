// |-----------------------------------------------------------------------------------------------------|
// <copyright file="FakeFindFluent.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Tests.Fakes
{
    using System.Linq;

    using MongoDB.Driver;

    /// <summary>
    /// This class is a fake of <see cref="IFindFluent{TDocument, TProjection}"/>.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TProjection">The type of the projection (same as TDocument if there is no projection).</typeparam>
    public class FakeFindFluent<TDocument, TProjection> : IFindFluent<TDocument, TDocument>
    {
        private readonly IEnumerable<TDocument> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeFindFluent{TDocument, TProjection}"/> class.
        /// </summary>
        /// <param name="items">The collection of documents.</param>
        public FakeFindFluent(IEnumerable<TDocument> items)
        {
            this.items = items ?? Enumerable.Empty<TDocument>();
            this.Filter = FilterDefinition<TDocument>.Empty;
        }

        /// <inheritdoc/>
        public FilterDefinition<TDocument> Filter { get; set; }

        /// <inheritdoc/>
        public FindOptions<TDocument, TDocument> Options => throw new NotImplementedException();

        /// <inheritdoc/>
        public IFindFluent<TDocument, TResult> As<TResult>(MongoDB.Bson.Serialization.IBsonSerializer<TResult> resultSerializer = null!)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public long Count(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<long> CountAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public long CountDocuments(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<long> CountDocumentsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IFindFluent<TDocument, TDocument> Limit(int? limit)
        {
            if (limit is null)
            {
                return this;
            }

            return new FakeFindFluent<TDocument, TDocument>(this.items.Take(limit.Value));
        }

        /// <inheritdoc/>
        public IFindFluent<TDocument, TNewProjection> Project<TNewProjection>(ProjectionDefinition<TDocument, TNewProjection> projection)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IFindFluent<TDocument, TDocument> Skip(int? skip)
        {
            if (skip is null)
            {
                return this;
            }

            return new FakeFindFluent<TDocument, TDocument>(this.items.Skip(skip.Value));
        }

        /// <inheritdoc/>
        public IFindFluent<TDocument, TDocument> Sort(SortDefinition<TDocument> sort)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IAsyncCursor<TDocument> ToCursor(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IAsyncCursor<TDocument>> ToCursorAsync(CancellationToken cancellationToken = default)
        {
            IAsyncCursor<TDocument> cursor = new FakeAsyncCursor<TDocument>(this.items);
            var task = Task.FromResult(cursor);

            return task;
        }
    }
}
