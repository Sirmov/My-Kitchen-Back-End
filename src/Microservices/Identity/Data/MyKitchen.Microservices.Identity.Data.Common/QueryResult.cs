namespace MyKitchen.Microservices.Identity.Data.Common.QueryResult
{
    using MyKitchen.Common.Result;
    using MyKitchen.Common.Result.Contracts;

    public class QueryResult : Result<Exception>, IResult<Exception>
    {
        public QueryResult()
        {
        }

        public QueryResult(Exception exception)
            : base(exception)
        {
        }

        public static new QueryResult Successful => new();

        public static implicit operator QueryResult(Exception failure) => new(failure);
    }
}
