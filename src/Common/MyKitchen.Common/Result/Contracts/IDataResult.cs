// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IDataResult.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Common.Result.Contracts
{
    /// <summary>
    /// This interfaces defines the functionality of a generic result class containing data.
    /// It inherits the <see cref="IResult{TFailure}"/> interface.
    /// </summary>
    /// <typeparam name="TData">The type of the data carried by the result.</typeparam>
    /// <typeparam name="TFailure"><inheritdoc cref="IResult{TFailure}"/></typeparam>
    public interface IDataResult<TData, TFailure> : IResult<TFailure>
        where TFailure : class
    {
        /// <summary>
        /// Gets or sets the result data.
        /// </summary>
        public TData? Data { get; set; }

        /// <summary>
        /// Gets a empty successful result.
        /// </summary>
        public static new IDataResult<TData, TFailure> Successful => new Result<TData, TFailure>();

        /// <inheritdoc cref="IResult{TFailure}.DependOn(IResult{TFailure})"/>
        public bool DependOn(IDataResult<object, TFailure> result);

        /// <summary>
        /// This method allows using the data stored in the current result via the <paramref name="function"/> delegate
        /// without explicitly checking whether it is successful or not.
        /// If the current result is successful the delegate is invoked providing the carried data. The delegate
        /// produces <typeparamref name="TOutData"/> which is carried in a new <typeparamref name="TResult"/> and returned.
        /// If the result is not successful a new <typeparamref name="TResult"/> with the same failure property is returned.
        /// </summary>
        /// <typeparam name="TResult">The type of the result carrying the new data.</typeparam>
        /// <typeparam name="TOutData">The type of the data carried in <typeparamref name="TResult"/>.</typeparam>
        /// <param name="function">The delegate transforming the data of the current result.</param>
        /// <returns>
        /// Returns a new result of type <typeparamref name="TResult"/> containing data
        /// of type <typeparamref name="TData"/> or the current failure.
        /// </returns>
        public TResult Bind<TResult, TOutData>(Func<TData, TResult> function)
            where TResult : IDataResult<TOutData, TFailure>, IResult<TFailure>;

        /// <summary>
        /// This method allows using the data stored in the current result asynchronously via the <paramref name="function"/>
        /// asynchronous delegate without explicitly checking whether it is successful or not.
        /// If the current result is successful the delegate is invoked providing the carried data. The delegate
        /// produces <typeparamref name="TOutData"/> which is carried in a new <typeparamref name="TResult"/> and returned.
        /// If the result is not successful a new <typeparamref name="TResult"/> with the same failure property is returned.
        /// </summary>
        /// <param name="function">The async delegate transforming the data of the current result.</param>
        /// <inheritdoc cref="IDataResult{TData, TFailure}.Bind{TResult, TOutData}(Func{TData, TResult})"/>
        public Task<TResult> BindAsync<TResult, TOutData>(Func<TData, Task<TResult>> function)
            where TResult : IDataResult<TOutData, TFailure>, IResult<TFailure>;

        /// <inheritdoc cref="IResult{TFailure}.Match{TMatch}(Func{IResult{TFailure}, TMatch}, Func{IResult{TFailure}, TMatch})"/>
        public TMatch Match<TMatch>(Func<IDataResult<TData, TFailure>, TMatch> onSuccess, Func<IDataResult<TData, TFailure>, TMatch> onFailure);
    }
}
