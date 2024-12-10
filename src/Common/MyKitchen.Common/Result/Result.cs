// |-----------------------------------------------------------------------------------------------------|
// <copyright file="Result.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Common.Result
{
    using MyKitchen.Common.Result.Contracts;

    /// <summary>
    /// This class implements the <see cref="IResult{TFailure}"/> interface.
    /// The result is successful while no <see cref="Result{TFailure}.Failure"/> is registered.
    /// </summary>
    /// <typeparam name="TFailure"><inheritdoc cref="IResult{TFailure}"/></typeparam>
    public class Result<TFailure> : IResult<TFailure>
        where TFailure : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TFailure}"/> class.
        /// </summary>
        public Result()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TFailure}"/> class.
        /// The result automatically fails.
        /// </summary>
        /// <param name="failure">The failure details.</param>
        public Result(TFailure failure)
        {
            this.Failure = failure;
        }

        /// <inheritdoc/>
        public TFailure? Failure { get; set; }

        /// <inheritdoc/>
        public bool IsSuccessful => this.Failure == null;

        /// <inheritdoc/>
        public bool IsFailed => this.Failure != null;

        /// <summary>
        /// Implicit conversion from a <typeparamref name="TFailure"/> to a failed <see cref="Result{TFailure}"/>.
        /// </summary>
        /// <param name="failure">The failure details.</param>
        public static implicit operator Result<TFailure>(TFailure failure) => new (failure);

        /// <inheritdoc/>
        public bool DependOn(IResult<TFailure> result)
        {
            if (result.IsFailed)
            {
                this.Failure = result.Failure;
            }

            return result.IsSuccessful;
        }

        /// <inheritdoc/>
        public TMatch Match<TMatch>(Func<IResult<TFailure>, TMatch> onSuccess, Func<IResult<TFailure>, TMatch> onFailure)
        {
            if (this.IsSuccessful)
            {
                return onSuccess(this);
            }

            return onFailure(this);
        }
    }
}
