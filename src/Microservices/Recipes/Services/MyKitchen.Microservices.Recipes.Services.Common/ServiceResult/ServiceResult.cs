// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ServiceResult.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Common.ServiceResult
{
    using Microsoft.AspNetCore.Mvc;

    using MyKitchen.Common.Result;
    using MyKitchen.Common.Result.Contracts;

    /// <summary>
    /// This class is a wrapper around the <see cref="Result{TFailure}"/> class. It uses <see cref="ProblemDetails"/>
    /// for the type parameter TFailure. It implements the <see cref="IResult{TFailure}"/>.
    /// </summary>
    public sealed class ServiceResult : ServiceResultBase<object?>, IResult<ProblemDetails>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResult"/> class.
        /// </summary>
        public ServiceResult()
        {
            this.Data = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResult"/> class.
        /// </summary>
        /// <param name="problemDetails"><inheritdoc cref="Result{TFailure}.Result(TFailure)"/></param>
        public ServiceResult(ProblemDetails problemDetails)
            : base(problemDetails)
        {
            this.Data = null;
        }

        private new object? Data;

        /// <summary>
        /// Gets a new successful <see cref="ServiceResult"/>.
        /// </summary>
        public static ServiceResult Successful => new ();

        /// <summary>
        /// Implicit conversion from a <see cref="ProblemDetails"/> to a failed <see cref="ServiceResult"/>.
        /// </summary>
        /// <param name="problemDetails">The failure details.</param>
        public static implicit operator ServiceResult(ProblemDetails problemDetails) => new (problemDetails);
    }
}
