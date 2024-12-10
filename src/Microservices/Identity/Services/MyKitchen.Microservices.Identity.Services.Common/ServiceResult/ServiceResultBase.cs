// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ServiceResultBase.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Common.ServiceResult
{
    using Microsoft.AspNetCore.Mvc;

    using MyKitchen.Common.Result;

    /// <summary>
    /// This abstract is a wrapper around the <see cref="Result{TData, TFailure}"/> class.
    /// It is a base class for all service result classes, containing their common functionality.
    /// It uses <see cref="ProblemDetails"/> for the type parameter TFailure.
    /// </summary>
    /// <typeparam name="TData"><inheritdoc cref="Result{TData, TFailure}"/>.</typeparam>
    public abstract class ServiceResultBase<TData> : Result<TData, ProblemDetails>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResultBase{TData}"/> class.
        /// </summary>
        protected ServiceResultBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResultBase{TData}"/> class.
        /// </summary>
        /// <param name="problemDetails"><inheritdoc cref="Result{TFailure}.Result(TFailure)"/></param>
        protected ServiceResultBase(ProblemDetails problemDetails)
            : base(problemDetails)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResultBase{TData}"/> class.
        /// </summary>
        /// <param name="data"><inheritdoc cref="Result{TData, TFailure}.Result(TData)"/></param>
        protected ServiceResultBase(TData data)
            : base(data)
        {
        }

        /// <summary>
        /// This method transforms the <see cref="ServiceResult"/> to a <see cref="IActionResult"/> using
        /// the <paramref name="onSuccess"/> delegate. If the result is not successful
        /// a <see cref="ProblemDetails"/> is returned.
        /// </summary>
        /// <param name="onSuccess">Transformation delegate.</param>
        /// <returns>Return a <see cref="IActionResult"/>.</returns>
        public IActionResult ToActionResult(Func<IActionResult> onSuccess)
        {
            if (this.IsSuccessful)
            {
                return onSuccess();
            }

            return this.ToErrorResponse();
        }

        /// <summary>
        /// This method transforms the <see cref="ServiceResult{TData}"/> to a <see cref="IActionResult"/> using
        /// the <paramref name="onSuccess"/> delegate. If the result is not successful
        /// a <see cref="ProblemDetails"/> is returned.
        /// </summary>
        /// <param name="onSuccess">Transformation delegate.</param>
        /// <returns>Return a <see cref="IActionResult"/>.</returns>
        public IActionResult ToActionResult(Func<TData, IActionResult> onSuccess)
        {
            if (this.IsSuccessful && this.Data != null)
            {
                return onSuccess(this.Data);
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
        public async Task<IActionResult> ToActionResultAsync(Func<Task<IActionResult>> onSuccess)
        {
            if (this.IsSuccessful)
            {
                return await onSuccess();
            }

            return this.ToErrorResponse();
        }

        /// <summary>
        /// This method transforms the <see cref="ServiceResult{TData}"/> to a <see cref="IActionResult"/> using
        /// the <paramref name="onSuccess"/> delegate. If the result is not successful
        /// a <see cref="ProblemDetails"/> is returned.
        /// </summary>
        /// <param name="onSuccess">Asynchronous transformation delegate.</param>
        /// <returns>Return a <see cref="Task{TResult}"/> of <see cref="IActionResult"/>.</returns>
        public async Task<IActionResult> ToActionResultAsync(Func<TData, Task<IActionResult>> onSuccess)
        {
            if (this.IsSuccessful && this.Data != null)
            {
                return await onSuccess(this.Data);
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
