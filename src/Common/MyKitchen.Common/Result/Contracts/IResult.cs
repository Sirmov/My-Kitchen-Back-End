// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IResult.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Common.Result.Contracts
{
    /// <summary>
    /// This interface defines the functionality of a generic result class.
    /// </summary>
    /// <typeparam name="TFailure">The type used for describing the <see cref="IResult{TFailure}.Failure"/>.</typeparam>
    public interface IResult<TFailure>
        where TFailure : class
    {
        /// <summary>
        /// Gets or sets the class containing the failure details.
        /// </summary>
        public TFailure? Failure { get; set; }

        /// <summary>
        /// Gets a value indicating whether the result succeeded.
        /// </summary>
        public bool Succeed { get; }

        /// <summary>
        /// Gets a value indicating whether the result failed.
        /// </summary>
        public bool Failed { get; }

        /// <summary>
        /// Gets a empty successful result.
        /// </summary>
        public static IResult<TFailure> Successful => new Result<TFailure>();

        /// <summary>
        /// This method updates the state of the current result based on a depending one.
        /// If the depending result in not successful this result also becomes not successful.
        /// </summary>
        /// <param name="result">The dependency result.</param>
        /// <returns>Returns a <see cref="bool"/> indicating whether the results are successful.</returns>
        public bool DependOn(IResult<TFailure> result);

        /// <summary>
        /// This method transforms a result based on whether it is successful or not.
        /// If it is successful the <paramref name="onSuccess"/> transformation delegate is invoked.
        /// If not the <paramref name="onFailure"/> transformation delegate is invoked.
        /// </summary>
        /// <typeparam name="TMatch">The type to be transformed to.</typeparam>
        /// <param name="onSuccess">The transformation delegate in case of a successful result.</param>
        /// <param name="onFailure">The transformation delegate in case of a failed result.</param>
        /// <returns>Returns a <typeparamref name="TMatch"/> based on the result state.</returns>
        public TMatch Match<TMatch>(Func<IResult<TFailure>, TMatch> onSuccess, Func<IResult<TFailure>, TMatch> onFailure);
    }
}
