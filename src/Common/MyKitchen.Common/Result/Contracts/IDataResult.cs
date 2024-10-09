namespace MyKitchen.Common.Result.Contracts
{
    public interface IDataResult<TData, TFailure> : IResult<TFailure>
        where TFailure : class
    {
        public TData? Data { get; set; }

        public bool DependOn(IDataResult<object, TFailure> result);

        public TResult Bind<TResult, TOutData>(Func<TData, TResult> function)
            where TResult : IDataResult<TOutData, TFailure>;

        public Task<TResult> BindAsync<TResult, TOutData>(Func<TData, Task<TResult>> function)
        where TResult : IDataResult<TOutData, TFailure>;

        public TMatch Match<TMatch>(Func<IDataResult<TData, TFailure>, TMatch> onSuccess, Func<IDataResult<TData, TFailure>, TMatch> onFailure);
    }
}
