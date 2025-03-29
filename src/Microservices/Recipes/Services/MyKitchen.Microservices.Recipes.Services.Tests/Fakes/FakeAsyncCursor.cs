// |-----------------------------------------------------------------------------------------------------|
// <copyright file="FakeAsyncCursor.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Tests.Fakes
{
    using MongoDB.Driver;

    /// <summary>
    /// This class is a fake of <see cref="IAsyncCursor{TDocument}"/>.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    public class FakeAsyncCursor<TDocument> : IAsyncCursor<TDocument>
    {
        private bool flag = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeAsyncCursor{TDocument}"/> class.
        /// </summary>
        /// <param name="items">The collection of documents.</param>
        public FakeAsyncCursor(IEnumerable<TDocument> items)
        {
            this.Current = items;
        }

        /// <inheritdoc/>
        public IEnumerable<TDocument> Current { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        /// <inheritdoc/>
        public bool MoveNext(CancellationToken cancellationToken = default)
        {
            if (this.flag)
            {
                this.flag = false;
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public Task<bool> MoveNextAsync(CancellationToken cancellationToken = default)
        {
            if (this.flag)
            {
                this.flag = false;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
