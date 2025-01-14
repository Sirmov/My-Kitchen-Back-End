// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IRepository.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Data.Contracts
{
    using MongoDB.Driver;

    using MyKitchen.Common.Result;

    /// <summary>
    /// This interface defines the functionality of the MongoDB repository.
    /// </summary>
    /// <typeparam name="TDocument">The type of the documents in the collection.</typeparam>
    /// <typeparam name="TKey">The type of the primary key of the document.</typeparam>
    public interface IRepository<TDocument, TKey>
    {
        /// <summary>
        /// This method generates a non materialized query
        /// returning all documents in a collection with or
        /// without the deleted ones. To add additional filters
        /// please refer to the example below.
        /// <para>
        /// <code>
        /// var query = repository.All(false);
        /// query.Filter &amp;= Builders&lt;TDocument&gt;.Filter.Gt(d => d.Age, 14);
        /// query.Filter |= Builders&lt;TDocument&gt;.Filter.Lt(d => d.Salary, 450);
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="withDeleted">A flag indicating whether deleted elements should be returned.</param>
        /// <returns>Returns a <see cref="IFindFluent{TDocument, TProjection}"/> non materialized query.</returns>
        public IFindFluent<TDocument, TDocument> All(bool withDeleted);

        /// <summary>
        /// This method finds the document with a given id.
        /// </summary>
        /// <param name="id">The id of the searched document.</param>
        /// <param name="withDeleted">A flag indicating whether deleted documents should be searched as well.</param>
        /// <returns>Returns a <see cref="Result{TData, Exception}"/> containing the document with the given id.</returns>
        public Result<TDocument, Exception> Find(TKey id, bool withDeleted);

        /// <summary>
        /// This method asynchronously finds the document with a given id.
        /// </summary>
        /// <param name="id">The id of the searched document.</param>
        /// <param name="withDeleted">A flag indicating whether deleted documents should be searched as well.</param>
        /// <returns>Returns a <see cref="Result{TData, Exception}"/> containing the document with the given id.</returns>
        public Task<Result<TDocument, Exception>> FindAsync(TKey id, bool withDeleted);

        /// <summary>
        /// This method adds a document to the collection.
        /// </summary>
        /// <param name="document">The document to be added.</param>
        /// <returns>Returns the created document.</returns>
        public Result<TDocument, Exception> Add(TDocument document);

        /// <summary>
        /// This method asynchronously adds a document to the collection.
        /// </summary>
        /// <param name="document">The document to be added.</param>
        /// <returns>Returns the created document.</returns>
        public Task<Result<TDocument, Exception>> AddAsync(TDocument document);

        /// <summary>
        /// This method add a range of documents to the collection.
        /// </summary>
        /// <param name="documents">The documents to be added.</param>
        /// <returns>Returns an empty <see cref="Result{Exception}"/>.</returns>
        public Result<Exception> AddRange(IEnumerable<TDocument> documents);

        /// <summary>
        /// This method asynchronously add a range of documents to the collection.
        /// </summary>
        /// <param name="documents">The documents to be added.</param>
        /// <returns>Returns an empty <see cref="Result{Exception}"/>.</returns>
        public Task<Result<Exception>> AddRangeAsync(IEnumerable<TDocument> documents);

        /// <summary>
        /// This method updates a document with a given id.
        /// The update works by replacing the whole document with the new one.
        /// </summary>
        /// <param name="id">The id of the document to be updated.</param>
        /// <param name="document">The document with the updated changes.</param>
        /// <returns>Returns a <see cref="Result{TData, Exception}"/> containing a <see cref="ReplaceOneResult"/>.</returns>
        public Result<ReplaceOneResult, Exception> Update(TKey id, TDocument document);

        /// <summary>
        /// This method updates a document with a given id.
        /// The update works by replacing the whole document with the new one.
        /// </summary>
        /// <param name="id">The id of the document to be updated.</param>
        /// <param name="document">The document with the updated changes.</param>
        /// <returns>Returns a <see cref="Result{TData, Exception}"/> containing a <see cref="ReplaceOneResult"/>.</returns>
        public Task<Result<ReplaceOneResult, Exception>> UpdateAsync(TKey id, TDocument document);

        /// <summary>
        /// This method flags a document with a given id as deleted.
        /// </summary>
        /// <param name="id">The id of the document to be flagged as deleted.</param>
        /// <returns>Returns a <see cref="Result{TData, Exception}"/> containing a <see cref="UpdateResult"/>.</returns>
        public Result<UpdateResult, Exception> Delete(TKey id);

        /// <summary>
        /// This method asynchronously flags a document with a given id as deleted.
        /// </summary>
        /// <param name="id">The id of the document to be flagged as deleted.</param>
        /// <returns>Returns a <see cref="Result{TData, Exception}"/> containing a <see cref="UpdateResult"/>.</returns>
        public Task<Result<UpdateResult, Exception>> DeleteAsync(TKey id);

        /// <summary>
        /// This method marks a document previously flagged as deleted to active.
        /// </summary>
        /// <param name="id">The id of the document to be undeleted.</param>
        /// <returns>Returns a <see cref="Result{TData, Exception}"/> containing the undeleted document.</returns>
        public Result<TDocument, Exception> Undelete(TKey id);

        /// <summary>
        /// This method asynchronously marks a document previously flagged as deleted to active.
        /// </summary>
        /// <param name="id">The id of the document to be undeleted.</param>
        /// <returns>Returns a <see cref="Result{TData, Exception}"/> containing the undeleted document.</returns>
        public Task<Result<TDocument, Exception>> UndeleteAsync(TKey id);

        /// <summary>
        /// This method deletes the document with a given id.
        /// This process cannot be undone.
        /// </summary>
        /// <param name="id">The id of the document to be deleted.</param>
        /// <returns>Returns a <see cref="Result{TData, Exception}"/> containing a <see cref="DeleteResult"/>.</returns>
        public Result<DeleteResult, Exception> HardDelete(TKey id);

        /// <summary>
        /// This method asynchronously deletes the document with a given id.
        /// This process cannot be undone.
        /// </summary>
        /// <param name="id">The id of the document to be deleted.</param>
        /// <returns>Returns a <see cref="Result{TData, Exception}"/> containing a <see cref="DeleteResult"/>.</returns>
        public Task<Result<DeleteResult, Exception>> HardDeleteAsync(TKey id);
    }
}
