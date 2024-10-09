namespace MyKitchen.Common.Result
{
    using MyKitchen.Common.Result.Contracts;

    public class Result<TData, TFailure> : Result<TFailure>, IDataResult<TData, TFailure>
        where TFailure : class
    {
        public Result()
            : base()
        {
        }

        public Result(TData data)
        {
            this.Data = data;
        }

        public Result(TFailure failure)
            : base(failure)
        {
            this.Failure = failure;
        }

        public TData? Data { get; set; } = default;

        public static new IDataResult<TData, TFailure> Successful => new Result<TData, TFailure>();

        public static implicit operator Result<TData, TFailure>(TFailure failure) => new(failure);

        public static implicit operator Result<TData, TFailure>(TData data) => new(data);

        public bool DependOn(IDataResult<object, TFailure> result)
        {
            if (result.Failed)
            {
                this.Failure = result.Failure;
            }

            return result.Succeed;
        }

        public virtual TResult Bind<TResult, TOutData>(Func<TData, TResult> function)
            where TResult : IDataResult<TOutData, TFailure>
        {
            if (this.Succeed && this.Data != null)
            {
                return function(this.Data);
            }

            IDataResult<TOutData, TFailure> result = new Result<TOutData, TFailure>(this.Failure!);
            return (TResult)result;
        }

        public virtual async Task<TResult> BindAsync<TResult, TOutData>(Func<TData, Task<TResult>> function)
            where TResult : IDataResult<TOutData, TFailure>
        {
            if (this.Succeed && this.Data != null)
            {
                return await function(this.Data);
            }

            IDataResult<TOutData, TFailure> result = new Result<TOutData, TFailure>(this.Failure!);
            return (TResult)result;
        }

        public TMatch Match<TMatch>(Func<IDataResult<TData, TFailure>, TMatch> onSuccess, Func<IDataResult<TData, TFailure>, TMatch> onFailure)
        {
            if (this.Succeed)
            {
                return onSuccess(this);
            }

            return onFailure(this);
        }
    }
}
