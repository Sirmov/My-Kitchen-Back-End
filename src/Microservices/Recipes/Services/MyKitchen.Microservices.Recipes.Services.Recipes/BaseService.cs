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
    using System.Threading.Tasks;

    using AutoMapper;

    using MongoDB.Driver;

    using MyKitchen.Common.Constants;
    using MyKitchen.Common.ProblemDetails;
    using MyKitchen.Microservices.Recipes.Data;
    using MyKitchen.Microservices.Recipes.Data.Contracts;
    using MyKitchen.Microservices.Recipes.Data.Models.Common;
    using MyKitchen.Microservices.Recipes.Services.Common.QueryOptions;
    using MyKitchen.Microservices.Recipes.Services.Common.ServiceResult;

    /// <summary>
    /// This is a abstract base class for all services. It contains usefull
    /// functions that apply for any service.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the id of the entity.</typeparam>
    public abstract class BaseService<TEntity, TKey>
        where TEntity : BaseDocument<TKey>
        where TKey : notnull
    {
        private readonly IRepository<TEntity, TKey> entityRepository;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{TEntity, TKey}"/> class.
        /// Uses constructor injection to resolve dependencies.
        /// </summary>
        /// <param name="entityRepository">The <typeparamref name="TEntity"/> database repository.</param>
        /// <param name="mapper">The global automapper instance.</param>
        public BaseService(
            IRepository<TEntity, TKey> entityRepository,
            IMapper mapper)
        {
            this.entityRepository = entityRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// This method asynchronously returns the entity with the corresponding id.
        /// </summary>
        /// <param name="id">The id of the entity.</param>
        /// <param name="queryOptions">The query options.</param>
        /// <typeparam name="TDto">The type of dto to be returned.</typeparam>
        /// <returns>Return a service result with the dto of the entity with the give id.</returns>
        protected virtual async Task<ServiceResult<TDto>> GetAsync<TDto>(TKey id, QueryOptions<TEntity>? queryOptions = null)
        {
            queryOptions ??= new QueryOptions<TEntity>();

            var findResult = await this.entityRepository.FindAsync(id, queryOptions.WithDeleted);

            if (!findResult.IsSuccessful)
            {
                if (findResult.Failure is NullReferenceException)
                {
                    return new NotFoundDetails(findResult.Failure.Message);
                }

                return new InternalServerErrorDetails(ExceptionMessages.InternalServerError);
            }

            var entity = findResult.Data!;

            if (entity.IsDeleted && !queryOptions.WithDeleted)
            {
                return new NotFoundDetails(
                    string.Format(ExceptionMessages.NoEntityWithPropertyFound, nameof(entity), nameof(id)));
            }

            var dto = this.mapper.Map<TDto>(entity);

            return dto;
        }

        /// <summary>
        /// This method asynchronously returns all entities.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <typeparam name="TDto">The type of dto to be returned.</typeparam>
        /// <returns>Returns a service result with the collection of all dtos of the entities.</returns>
        protected virtual async Task<ServiceResult<IEnumerable<TDto>>> GetAllAsync<TDto>(QueryOptions<TEntity>? queryOptions = null)
        {
            queryOptions ??= new QueryOptions<TEntity>();

            var query = this.entityRepository.All(queryOptions.WithDeleted);

            query = this.ModifyQuery(query, queryOptions);

            var models = await query.ToListAsync();

            var dtos = models.Select(this.mapper.Map<TDto>).ToList();

            return dtos;
        }

        /// <summary>
        /// This method asynchronously creates a new entity in the database.
        /// </summary>
        /// <param name="inputDto">The entity to be created.</param>
        /// <typeparam name="TDto">The type of returned dto.</typeparam>
        /// <typeparam name="TInputDto">The type of the input dto.</typeparam>
        /// <returns>Returns a service result with the dto of the newly created entity.</returns>
        protected virtual async Task<ServiceResult<TDto>> CreateAsync<TDto, TInputDto>(TInputDto inputDto)
        {
            var isValid = this.ValidateDto(inputDto);

            if (!isValid)
            {
                return new BadRequestDetails(
                    string.Format(ExceptionMessages.InvalidModelState, nameof(inputDto)));
            }

            TEntity entity = this.mapper.Map<TEntity>(inputDto);
            var addResult = await this.entityRepository.AddAsync(entity);

            if (!addResult.IsSuccessful)
            {
                return new InternalServerErrorDetails();
            }

            var dto = this.mapper.Map<TDto>(entity);

            return dto;
        }

        /// <summary>
        /// This method asynchronously updates an existing entity. If the <paramref name="inputDto"/>
        /// contains id, make sure that it matches with the provided <paramref name="id"/>.
        /// Otherwise the id of the entity with <paramref name="id"/> would be changed
        /// to the id of the <paramref name="inputDto"/>.
        /// <para>
        /// It uses property name matching to copy properties from the <paramref name="inputDto"/>
        /// to the entity with <paramref name="id"/>. Every entity's matching property value will be
        /// replaced with the value of the corresponding <paramref name="inputDto"/> value.
        /// </para>
        /// </summary>
        /// <param name="id">The id of the entity to be updated.</param>
        /// <param name="inputDto">The dto containing the new information.</param>
        /// <typeparam name="TDto">The type of returned dto.</typeparam>
        /// <typeparam name="TInputDto">The type of input dto.</typeparam>
        /// <returns>Returns an empty service result.</returns>
        protected virtual async Task<ServiceResult<TDto>> UpdateAsync<TDto, TInputDto>(TKey id, TInputDto inputDto)
        {
            var existsResult = await this.ExistsAsync(id);

            if (!existsResult.Data)
            {
                return new NotFoundDetails(
                    string.Format(ExceptionMessages.EntityNotFound, nameof(TEntity)));
            }

            var isValid = this.ValidateDto(inputDto);

            if (!isValid)
            {
                return new BadRequestDetails(
                    string.Format(ExceptionMessages.InvalidModelState, nameof(inputDto)));
            }

            TEntity oldEntity = (await this.entityRepository.FindAsync(id, false)).Data!;
            this.CopyProperties(inputDto!, oldEntity);
            oldEntity.ModifiedOn = DateTime.UtcNow;

            var updateResult = await this.entityRepository.UpdateAsync(id, oldEntity);

            if (!updateResult.IsSuccessful)
            {
                return new InternalServerErrorDetails();
            }

            return this.mapper.Map<TDto>(oldEntity);
        }

        /// <summary>
        /// This method asynchronously deletes the entity with the provided id.
        /// </summary>
        /// <param name="id">The id of the entity to be deleted.</param>
        /// <returns>Returns an empty service result.</returns>
        protected virtual async Task<ServiceResult> DeleteAsync(TKey id)
        {
            var deleteResult = await this.entityRepository.DeleteAsync(id);

            if (!deleteResult.IsSuccessful)
            {
                if (deleteResult.Failure is InvalidOperationException)
                {
                    return new NotFoundDetails(deleteResult.Failure.Message);
                }

                return new InternalServerErrorDetails(ExceptionMessages.InternalServerError);
            }

            return new ServiceResult();
        }

        /// <summary>
        /// This method asynchronously checks if a entity with a given id exists.
        /// </summary>
        /// <param name="id">The id of the entity.</param>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>Returns a service result with a boolean indication where the category exists.</returns>
        protected virtual async Task<ServiceResult<bool>> ExistsAsync(TKey id, QueryOptions<TEntity>? queryOptions = null)
        {
            var withDeleted = queryOptions?.WithDeleted ?? false;

            var findResult = await this.entityRepository.FindAsync(id, true);

            if (!findResult.IsSuccessful || (!withDeleted && findResult.Data!.IsDeleted))
            {
                return false;
            }

            return true;
        }

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
        /// <returns>Returns a <see cref="bool"/> indicating whether the dto is valid.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="dto"/> is null.</exception>
        protected bool ValidateDto<TDto>(TDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var context = new ValidationContext(dto, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, context, validationResults, true);

            return isValid;
        }

        private void CopyProperties(object source, object destination)
        {
            // If any this null throw an exception
            if (source == null || destination == null)
            {
                throw new Exception(ExceptionMessages.SourceOrDestinationNull);
            }

            // Getting the Types of the objects
            Type typeDest = destination.GetType();
            Type typeSrc = source.GetType();

            // Collect all the valid properties to map
            var results = from srcProp in typeSrc.GetProperties()
                          let targetProperty = typeDest.GetProperty(srcProp.Name)
                          where srcProp.CanRead
                          && targetProperty != null
                          && targetProperty.GetSetMethod(true) != null && !targetProperty.GetSetMethod(true)!.IsPrivate
                          && (targetProperty.GetSetMethod()!.Attributes & MethodAttributes.Static) == 0
                          && targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType)
                          select new { sourceProperty = srcProp, targetProperty };

            // Map the properties
            foreach (var props in results)
            {
                props.targetProperty.SetValue(destination, props.sourceProperty.GetValue(source, null), null);
            }
        }

#pragma warning disable IDE0060 // Remove unused parameter / The parameter is used for generic intellisense
        private PropertyInfo GetPropertyInfo<TSource, TProperty>(TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            Type type = typeof(TSource);
            MemberExpression? member = propertyLambda.Body as MemberExpression;

            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));
            }

            PropertyInfo? propInfo = member.Member as PropertyInfo;

            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));
            }

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
