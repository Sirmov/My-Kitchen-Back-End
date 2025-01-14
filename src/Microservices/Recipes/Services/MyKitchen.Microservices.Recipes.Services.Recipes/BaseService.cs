// |-----------------------------------------------------------------------------------------------------|
// <copyright file="BaseService.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Recipes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq.Expressions;
    using System.Reflection;

    using MongoDB.Driver;

    using MyKitchen.Common.Constants;
    using MyKitchen.Microservices.Recipes.Data.Models.Common;
    using MyKitchen.Microservices.Recipes.Services.Common.QueryOptions;

    /// <summary>
    /// This is a abstract base class for all services.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the id of the entity.</typeparam>
    public abstract class BaseService<TEntity, TKey>
        where TEntity : BaseDocument<TKey>
        where TKey : notnull
    {
        /// <summary>
        /// This method modifies the query based on the <see cref="QueryOptions{TDocument}"/> passed.
        /// </summary>
        /// <typeparam name="TDocument">The type of the entities returned by the query.</typeparam>
        /// <param name="query">The original query.</param>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>Returns the modified query.</returns>
        protected IFindFluent<TDocument, TDocument> ModifyQuery<TDocument>(IFindFluent<TDocument, TDocument> query, QueryOptions<TDocument> queryOptions)
            where TDocument : BaseDocument<TKey>
        {
            if (queryOptions == null)
            {
                return query;
            }

            if (!queryOptions.WithDeleted)
            {
                query.Filter &= Builders<TDocument>.Filter.Eq(x => x.IsDeleted, false);
            }

            foreach (var orderOption in queryOptions.OrderOptions)
            {
                if (orderOption.Order == OrderByOrder.Ascending)
                {
                    query = query.SortBy(orderOption.PropertyLambda);
                }
                else
                {
                    query = query.SortByDescending(orderOption.PropertyLambda);
                }
            }

            queryOptions.Skip = queryOptions.Skip < 0 ? 0 : queryOptions.Skip;

            queryOptions.Take = queryOptions.Take < 0 ? 0 : queryOptions.Take;
            queryOptions.Take = queryOptions.Take > 100 ? 100 : queryOptions.Take;

            query = query.Skip(queryOptions.Skip);
            query = query.Limit(queryOptions.Take);

            return query;
        }

        /// <summary>
        /// This method initializes a new <see cref="ValidationContext"/> and by using <see cref="Validator"/>
        /// determines whether a dto is valid by looking at his validation attributes.
        /// </summary>
        /// <typeparam name="TDto">The type of the dto.</typeparam>
        /// <param name="dto">The dto to be validated.</param>
        /// <returns>Returns a collection of <see cref="ValidationResult"/>>.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="dto"/> is null.</exception>
        protected List<ValidationResult> ValidateDto<TDto>(TDto dto)
        {
            var validationResults = new List<ValidationResult>();

            if (dto is null)
            {
                validationResults.Add(new ValidationResult(string.Format(ExceptionMessages.NullReference, nameof(dto))));
                return validationResults;
            }

            var context = new ValidationContext(dto, serviceProvider: null, items: null);
            Validator.TryValidateObject(dto, context, validationResults, true);

            return validationResults;
        }

        /// <summary>
        /// This method copies all of the values of the properties of the <paramref name="source"/> object
        /// and copies them to the <paramref name="destination"/> object if their names are equal
        /// and the set method of the destination property is not private or static.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="destination">The destination object.</param>
        /// <exception cref="ArgumentNullException">Throws <see cref="ArgumentNullException"/> if any of the arguments are null.</exception>
        protected void CopyProperties(object source, object destination)
        {
            if (source == null || destination == null)
            {
                throw new ArgumentNullException(ExceptionMessages.SourceOrDestinationNull);
            }

            Type destinationType = destination.GetType();
            Type sourceType = source.GetType();

            var mappableProperties = from sourceProperty in sourceType.GetProperties()
                          let targetProperty = destinationType.GetProperty(sourceProperty.Name)
                          where sourceProperty.CanRead
                          && targetProperty != null
                          && targetProperty.GetSetMethod(true) != null && !targetProperty.GetSetMethod(true)!.IsPrivate
                          && (targetProperty.GetSetMethod()!.Attributes & MethodAttributes.Static) == 0
                          && targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType)
                          select new { sourceProperty, targetProperty };

            foreach (var property in mappableProperties)
            {
                property.targetProperty.SetValue(destination, property.sourceProperty.GetValue(source, null), null);
            }
        }

        /// <summary>
        /// This method gets the <see cref="PropertyInfo"/> of a property by a given <paramref name="propertyLambda"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TProperty">The type of the property of the source.</typeparam>
        /// <param name="source">The source object.</param>
        /// <param name="propertyLambda">The property lambda function.</param>
        /// <returns>Returns the <see cref="PropertyInfo"/> of the property which the <paramref name="propertyLambda"/> returns.</returns>
        /// <exception cref="ArgumentException">Throws <see cref="ArgumentException"/> if the <paramref name="propertyLambda"/> does not return a property.</exception>
        protected PropertyInfo GetPropertyInfo<TSource, TProperty>(TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression? member = propertyLambda.Body as MemberExpression ??
            throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo? propInfo = member.Member as PropertyInfo ??
            throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType!))
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));
            }

            return propInfo;
        }
    }
}
