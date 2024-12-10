// |-----------------------------------------------------------------------------------------------------|
// <copyright file="Result{TData,TFailure}.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Common.Result
{
    using MyKitchen.Common.Result.Contracts;

    /// <summary>
    /// This class implements the <see cref="IDataResult{TData, TFailure}"/> interface.
    /// The result is successful while no <see cref="Result{TFailure}.Failure"/> is registered.
    /// </summary>
    /// <typeparam name="TData"><inheritdoc cref="IDataResult{TData, TFailure}"/></typeparam>
    /// <typeparam name="TFailure"><inheritdoc cref="IResult{TFailure}"/></typeparam>
    public class Result<TData, TFailure> : Result<TFailure>, IDataResult<TData, TFailure>
        where TFailure : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TData, TFailure}"/> class.
        /// </summary>
        public Result()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TData, TFailure}"/> class.
        /// </summary>
        /// <param name="data">The data to be carried.</param>
        public Result(TData data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TData, TFailure}"/> class.
        /// </summary>
        /// <inheritdoc cref="Result{TFailure}.Result(TFailure)"/>
        public Result(TFailure failure)
            : base(failure)
        {
            this.Failure = failure;
        }

        /// <inheritdoc/>
        public TData? Data { get; set; } = default;

        /// <summary>
        /// Implicit conversion from a <typeparamref name="TFailure"/> to a failed <see cref="Result{TData, TFailure}"/>.
        /// </summary>
        /// <param name="failure">The failure details.</param>
        public static implicit operator Result<TData, TFailure>(TFailure failure) => new (failure);

        /// <summary>
        /// Implicit conversion from a <typeparamref name="TData"/> to a <see cref="Result{TData, TFailure}"/>.
        /// </summary>
        /// <param name="data">The data to be carried.</param>
        public static implicit operator Result<TData, TFailure>(TData data) => new (data);

        /// <inheritdoc/>
        public bool DependOn(IDataResult<object, TFailure> result)
        {
            if (result.IsFailed)
            {
                this.Failure = result.Failure;
            }

            return result.IsSuccessful;
        }

        /// <inheritdoc/>
        public virtual TResult Bind<TResult, TOutData>(Func<TData, TResult> function)
            where TResult : IDataResult<TOutData, TFailure>, IResult<TFailure>
        {
            if (this.IsSuccessful && this.Data != null)
            {
                return function(this.Data);
            }

            IDataResult<TOutData, TFailure> result = new Result<TOutData, TFailure>(this.Failure!);
            return (TResult)result;
        }

        /// <inheritdoc/>
        public virtual async Task<TResult> BindAsync<TResult, TOutData>(Func<TData, Task<TResult>> function)
            where TResult : IDataResult<TOutData, TFailure>, IResult<TFailure>
        {
            if (this.IsSuccessful && this.Data != null)
            {
                return await function(this.Data);
            }

            IDataResult<TOutData, TFailure> result = new Result<TOutData, TFailure>(this.Failure!);
            return (TResult)result;
        }

        /// <inheritdoc/>
        public TMatch Match<TMatch>(Func<IDataResult<TData, TFailure>, TMatch> onSuccess, Func<IDataResult<TData, TFailure>, TMatch> onFailure)
        {
            if (this.IsSuccessful)
            {
                return onSuccess(this);
            }

            return onFailure(this);
        }
    }
}
