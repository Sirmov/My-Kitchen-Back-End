namespace MyKitchen.Microservices.Identity.Data.Common
{
    using MyKitchen.Common.Result;

    public class QueryResult : Result<Exception>
    {
        public QueryResult()
        {
        }

        public QueryResult(Exception exception)
            : base(exception)
        {
        }

        public static QueryResult Successful => new();

        public static implicit operator QueryResult(Exception failure) => new(failure);
    }
}
