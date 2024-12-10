// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IQueryableExtensions.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Common.QueryOptions
{
    using System.Linq.Expressions;

    using MyKitchen.Common.Constants;

    /// <summary>
    /// This static class contains <see cref="IQueryable{T}"/> extension methods.
    /// </summary>
    public static class IQueryableExtensions
    {
        /// <summary>
        /// This <see cref="IQueryable{T}"/> extension method orders collections based on a property name, ascending.
        /// </summary>
        /// <typeparam name="T">The type of objects in the collection.</typeparam>
        /// <param name="query">The <see cref="IQueryable{T}"/>.</param>
        /// <param name="propertyName">The name of the property to be sorted by.</param>
        /// <returns>Returns the <see cref="IQueryable{T}"/>.</returns>
        /// <exception cref="ArgumentException">Throws when property was not found.</exception>
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

        /// <summary>
        /// This <see cref="IQueryable{T}"/> extension method orders collections based on a property name, descending.
        /// </summary>
        /// <typeparam name="T">The type of objects in the collection.</typeparam>
        /// <param name="query">The <see cref="IQueryable{T}"/>.</param>
        /// <param name="propertyName">The name of the property to be sorted by.</param>
        /// <returns>Returns the <see cref="IQueryable{T}"/>.</returns>
        /// <exception cref="ArgumentException">Throws when property was not found.</exception>
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
