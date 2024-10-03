namespace MyKitchen.Microservices.Identity.Data.Common
{
    using MyKitchen.Common.Result;

    public class QueryResult<TData> : Result<TData, Exception>
    {
        public QueryResult(TData data)
            : base(data)
        {
        }

        public QueryResult(Exception exception)
            : base(exception)
        {
        }

        public static implicit operator QueryResult<TData>(Exception failure) => new(failure);

        public static implicit operator QueryResult<TData>(TData data) => new(data);
    }
}
