// |-----------------------------------------------------------------------------------------------------|
// <copyright file="QueryResult{TData}.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Data.Common.QueryResult
{
    using MyKitchen.Common.Result;
    using MyKitchen.Common.Result.Contracts;

    /// <summary>
    /// This class is a wrapper around the <see cref="Result{TData, TFailure}"/> class. It uses <see cref="Exception"/>
    /// for the type parameter TFailure. It implements the <see cref="IDataResult{TData, TFailure}"/>.
    /// </summary>
    /// <typeparam name="TData"><inheritdoc cref="Result{TData, TFailure}"/>.</typeparam>
    public class QueryResult<TData> : Result<TData, Exception>, IDataResult<TData, Exception>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{TData}"/> class.
        /// </summary>
        /// <param name="data"><inheritdoc cref="Result{TData, TFailure}.Result(TData)"/></param>
        public QueryResult(TData data)
            : base(data)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{TData}"/> class.
        /// </summary>
        /// <param name="exception"><inheritdoc cref="Result{TData, TFailure}.Result(TFailure)"/></param>
        public QueryResult(Exception exception)
            : base(exception)
        {
        }

        /// <summary>
        /// Implicit conversion from a <see cref="Exception"/> to a failed <see cref="QueryResult{TData}"/>.
        /// </summary>
        /// <param name="failure">The failure details.</param>
        public static implicit operator QueryResult<TData>(Exception failure) => new (failure);

        /// <summary>
        /// Implicit conversion from a <typeparamref name="TData"/> to a <see cref="QueryResult{TData}"/>.
        /// </summary>
        /// <param name="data">The data to be carried.</param>
        public static implicit operator QueryResult<TData>(TData data) => new (data);

        /// <summary>
        /// Implicit conversion from a <see cref="QueryResult{TData}"/> to a <see cref="QueryResult"/>.
        /// </summary>
        /// <param name="queryResult">The <see cref="QueryResult{TData}"/> to be converted to a <see cref="QueryResult"/>.</param>
        public static implicit operator QueryResult(QueryResult<TData> queryResult) => new (queryResult.Failure!);

        /// <summary>
        /// This static method returns a new successful <see cref="QueryResult{TData}"/>.
        /// </summary>
        /// <param name="data"><inheritdoc cref="QueryResult{TData}.QueryResult(TData)"/></param>
        /// <returns>Returns a new successful <see cref="QueryResult{TData}"/> containing <paramref name="data"/>.</returns>
        public static QueryResult<TData> Successful(TData data) => new (data);
    }
}
