// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ServiceResult.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Common.ServiceResult
{
    using Microsoft.AspNetCore.Mvc;

    using MyKitchen.Common.Result;
    using MyKitchen.Common.Result.Contracts;

    /// <summary>
    /// This class is a wrapper around the <see cref="Result{TFailure}"/> class. It uses <see cref="ProblemDetails"/>
    /// for the type parameter TFailure. It implements the <see cref="IResult{TFailure}"/>.
    /// </summary>
    public sealed class ServiceResult : Result<ProblemDetails>, IResult<ProblemDetails>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResult"/> class.
        /// </summary>
        public ServiceResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResult"/> class.
        /// </summary>
        /// <param name="problemDetails"><inheritdoc cref="Result{TFailure}.Result(TFailure)"/></param>
        public ServiceResult(ProblemDetails problemDetails)
            : base(problemDetails)
        {
        }

        /// <summary>
        /// Gets a new successful <see cref="ServiceResult"/>.
        /// </summary>
        public static ServiceResult Successful => new ();

        /// <summary>
        /// Implicit conversion from a <see cref="ProblemDetails"/> to a failed <see cref="ServiceResult"/>.
        /// </summary>
        /// <param name="problemDetails">The failure details.</param>
        public static implicit operator ServiceResult(ProblemDetails problemDetails) => new (problemDetails);

        /// <summary>
        /// This method transforms the <see cref="ServiceResult"/> to a <see cref="IActionResult"/> using
        /// the <paramref name="onSuccess"/> delegate. If the result is not successful
        /// a <see cref="ProblemDetails"/> is returned.
        /// </summary>
        /// <param name="onSuccess">Transformation delegate.</param>
        /// <returns>Return a <see cref="IActionResult"/>.</returns>
        public IActionResult ToActionResult(Func<ServiceResult, IActionResult> onSuccess)
        {
            if (this.IsSuccessful)
            {
                return onSuccess(this);
            }

            return this.ToErrorResponse();
        }

        /// <summary>
        /// This method transforms the <see cref="ServiceResult"/> to a <see cref="IActionResult"/> using
        /// the asynchronous <paramref name="onSuccess"/> delegate. If the result is not successful
        /// a <see cref="ProblemDetails"/> is returned.
        /// </summary>
        /// <param name="onSuccess">Asynchronous transformation delegate.</param>
        /// <returns>Return a <see cref="Task{TResult}"/> of <see cref="IActionResult"/>.</returns>
        public async Task<IActionResult> ToActionResult(Func<ServiceResult, Task<IActionResult>> onSuccess)
        {
            if (this.IsSuccessful)
            {
                return await onSuccess(this);
            }

            return this.ToErrorResponse();
        }

        private IActionResult ToErrorResponse()
        {
            var problemDetails = this.Failure;

            return new ObjectResult(problemDetails);
        }
    }
}
