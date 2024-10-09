namespace MyKitchen.Common.Result
{
    using MyKitchen.Common.Result.Contracts;

    public class Result<TFailure> : IResult<TFailure>
        where TFailure : class
    {
        public Result()
        {
        }

        public Result(TFailure failure)
        {
            this.Failure = failure;
        }

        public TFailure? Failure { get; set; } = null;

        public bool Succeed => this.Failure == null;

        public bool Failed => this.Failure != null;

        public static IResult<TFailure> Successful => new Result<TFailure>();

        public static implicit operator Result<TFailure>(TFailure failure) => new(failure);

        public bool DependOn(IResult<TFailure> result)
        {
            if (result.Failed)
            {
                this.Failure = result.Failure;
            }

            return result.Succeed;
        }

        public TMatch Match<TMatch>(Func<IResult<TFailure>, TMatch> onSuccess, Func<IResult<TFailure>, TMatch> onFailure)
        {
            if (this.Succeed)
            {
                return onSuccess(this);
            }

            return onFailure(this);
        }
    }
}
