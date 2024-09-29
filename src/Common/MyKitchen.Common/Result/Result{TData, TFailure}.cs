namespace MyKitchen.Common.Result
{
    public class Result<TData, TFailure> : Result<TFailure>
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

        public static implicit operator Result<TData, TFailure>(TFailure failure) => new(failure);

        public static implicit operator Result<TData, TFailure>(TData data) => new(data);

        public bool DependOn(Result<object, TFailure> result)
        {
            if (result.Failed)
            {
                this.Failure = result.Failure;
            }

            return result.Succeed;
        }

        public Result<TOutData, TFailure> Bind<TOutData>(Func<TData, Result<TOutData, TFailure>> function)
        {
            if (this.Succeed && this.Data != null)
            {
                return function(this.Data);
            }

            return this.Failure!;
        }

        public async Task<Result<TOutData, TFailure>> BindAsync<TOutData>(Func<TData, Task<Result<TOutData, TFailure>>> function)
        {
            if (this.Succeed && this.Data != null)
            {
                return await function(this.Data);
            }

            return this.Failure!;
        }

        public TMatch Match<TMatch>(Func<Result<TData, TFailure>, TMatch> onSuccess, Func<Result<TData, TFailure>, TMatch> onFailure)
        {
            if (this.Succeed)
            {
                return onSuccess(this);
            }

            return onFailure(this);
        }
    }
}
