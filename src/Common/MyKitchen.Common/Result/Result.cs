namespace MyKitchen.Common.Result
{
    public class Result<TFailure>
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

        public static Result<TFailure> Successful => new();

        public static implicit operator Result<TFailure>(TFailure failure) => new(failure);

        public bool DependOn(Result<TFailure> result)
        {
            if (result.Failed)
            {
                this.Failure = result.Failure;
            }

            return result.Succeed;
        }
    }
}
