namespace MyKitchen.Common.Result.Contracts
{
    public interface IResult<TFailure>
        where TFailure : class
    {
        public TFailure? Failure { get; set; }

        public bool Succeed { get; }

        public bool Failed { get; }

        public bool DependOn(IResult<TFailure> result);

        public TMatch Match<TMatch>(Func<IResult<TFailure>, TMatch> onSuccess, Func<IResult<TFailure>, TMatch> onFailure);
    }
}
