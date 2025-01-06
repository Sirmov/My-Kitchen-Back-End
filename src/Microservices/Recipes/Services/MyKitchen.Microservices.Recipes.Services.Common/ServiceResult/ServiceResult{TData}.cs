// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ServiceResult{TData}.cs" company="MyKitchen">
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
    /// This class is a wrapper around the <see cref="Result{TData, TFailure}"/> class. It uses <see cref="ProblemDetails"/>
    /// for the type parameter TFailure. It implements the <see cref="IDataResult{TData, TFailure}"/>.
    /// </summary>
    /// <typeparam name="TData"><inheritdoc cref="Result{TData, TFailure}"/>.</typeparam>
    public sealed class ServiceResult<TData> : ServiceResultBase<TData>, IDataResult<TData, ProblemDetails>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResult{TData}"/> class.
        /// </summary>
        /// <param name="data"><inheritdoc cref="Result{TData, TFailure}.Result(TData)"/></param>
        public ServiceResult(TData data)
            : base(data)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResult{TData}"/> class.
        /// </summary>
        /// <param name="problemDetails"><inheritdoc cref="Result{TData, TFailure}.Result(TFailure)"/></param>
        public ServiceResult(ProblemDetails problemDetails)
            : base(problemDetails)
        {
        }

        /// <summary>
        /// Implicit conversion from a <see cref="ProblemDetails"/> to a failed <see cref="ServiceResult{TData}"/>.
        /// </summary>
        /// <param name="problemDetails">The failure details.</param>
        public static implicit operator ServiceResult<TData>(ProblemDetails problemDetails) => new (problemDetails);

        /// <summary>
        /// Implicit conversion from a <typeparamref name="TData"/> to a <see cref="ServiceResult{TData}"/>.
        /// </summary>
        /// <param name="data">The data to be carried.</param>
        public static implicit operator ServiceResult<TData>(TData data) => new (data);

        /// <summary>
        /// Implicit conversion from a <see cref="ServiceResult{TData}"/> to a <see cref="ServiceResult"/>.
        /// </summary>
        /// <param name="serviceResult">The <see cref="ServiceResult{TData}"/> to be converted to a <see cref="ServiceResult"/>.</param>
        public static implicit operator ServiceResult(ServiceResult<TData> serviceResult) => new (serviceResult.Failure!);

        /// <summary>
        /// This static method returns a new successful <see cref="ServiceResult{TData}"/>.
        /// </summary>
        /// <param name="data"><inheritdoc cref="ServiceResult{TData}.ServiceResult(TData)"/></param>
        /// <returns>Returns a new successful <see cref="ServiceResult{TData}"/> containing <paramref name="data"/>.</returns>
        public static ServiceResult<TData> Successful(TData data) => new (data);
    }
}
