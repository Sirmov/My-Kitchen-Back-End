// |-----------------------------------------------------------------------------------------------------|
// <copyright file="QueryResult.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Data.Common.QueryResult
{
    using MyKitchen.Common.Result;
    using MyKitchen.Common.Result.Contracts;

    /// <summary>
    /// This class is a wrapper around the <see cref="Result{TFailure}"/> class. It uses <see cref="Exception"/>
    /// for the type parameter TFailure. It implements the <see cref="IResult{TFailure}"/>.
    /// </summary>
    public class QueryResult : Result<Exception>, IResult<Exception>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult"/> class.
        /// </summary>
        public QueryResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult"/> class.
        /// </summary>
        /// <param name="exception"><inheritdoc cref="Result{TFailure}.Result(TFailure)"/></param>
        public QueryResult(Exception exception)
            : base(exception)
        {
        }

        /// <summary>
        /// Gets a new successful <see cref="QueryResult"/>.
        /// </summary>
        public static new QueryResult Successful => new ();

        /// <summary>
        /// Implicit conversion from a <see cref="Exception"/> to a failed <see cref="QueryResult"/>.
        /// </summary>
        /// <param name="failure">The failure details.</param>
        public static implicit operator QueryResult(Exception failure) => new (failure);
    }
}
