namespace MyKitchen.Microservices.Identity.Services.Common.QueryOptions
{
    using System.Linq.Expressions;

    using MyKitchen.Common.Constants;

    public static class IQueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName)
        {
            var entityType = typeof(T);
            var propertyInfo = entityType.GetProperty(propertyName) ??
             throw new ArgumentException(string.Format(ExceptionMessages.PropertyNotFound, propertyName, entityType.Name));
            var parameter = Expression.Parameter(entityType, "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);

            MethodCallExpression orderByCallExpression = Expression.Call(
                typeof(Queryable),
                "OrderBy",
                new Type[] { entityType, propertyInfo.PropertyType },
                query.Expression,
                Expression.Quote(orderByExp));

            return query.Provider.CreateQuery<T>(orderByCallExpression);
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName)
        {
            var entityType = typeof(T);
            var propertyInfo = entityType.GetProperty(propertyName) ??
             throw new ArgumentException(string.Format(ExceptionMessages.PropertyNotFound, propertyName, entityType.Name));
            var parameter = Expression.Parameter(entityType, "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);

            MethodCallExpression orderByCallExpression = Expression.Call(
                typeof(Queryable),
                "OrderByDescending",
                new Type[] { entityType, propertyInfo.PropertyType },
                query.Expression,
                Expression.Quote(orderByExp));

            return query.Provider.CreateQuery<T>(orderByCallExpression);
        }
    }
}
