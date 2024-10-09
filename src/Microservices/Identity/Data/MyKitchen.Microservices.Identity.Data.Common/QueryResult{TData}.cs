namespace MyKitchen.Microservices.Identity.Data.Common
{
    using MyKitchen.Common.Result;
    using MyKitchen.Common.Result.Contracts;

    public class QueryResult<TData> : Result<TData, Exception>, IDataResult<TData, Exception>
    {
        public QueryResult(TData data)
            : base(data)
        {
        }

        public QueryResult(Exception exception)
            : base(exception)
        {
        }

        public static new QueryResult<TData> Successful(TData data) => new(data);

        public static implicit operator QueryResult<TData>(Exception failure) => new(failure);

        public static implicit operator QueryResult<TData>(TData data) => new(data);

        public static implicit operator QueryResult(QueryResult<TData> queryResult) => new(queryResult.Failure!);
    }
}
