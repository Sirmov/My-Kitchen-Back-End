// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RecipesService.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Recipes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using AutoMapper;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using MyKitchen.Common.Constants;
    using MyKitchen.Common.ProblemDetails;
    using MyKitchen.Microservices.Recipes.Data.Contracts;
    using MyKitchen.Microservices.Recipes.Data.Models;
    using MyKitchen.Microservices.Recipes.Services.Common.QueryOptions;
    using MyKitchen.Microservices.Recipes.Services.Common.ServiceResult;
    using MyKitchen.Microservices.Recipes.Services.Recipes.Contracts;
    using MyKitchen.Microservices.Recipes.Services.Recipes.Dtos;

    /// <summary>
    /// This class encapsulates the business logic related to the <see cref="Recipe"/> model.
    /// </summary>
    public class RecipesService : BaseService<Recipe, string>, IRecipesService
    {
        private readonly IRepository<Recipe, string> recipesRepository;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipesService"/> class.
        /// </summary>
        /// <param name="recipesRepository">The <see cref="Recipe"/> repository.</param>
        /// <param name="mapper">The global automapper instance.</param>
        public RecipesService(IRepository<Recipe, string> recipesRepository, IMapper mapper)
        {
            this.recipesRepository = recipesRepository;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<IEnumerable<RecipeDto>>> GetAllRecipesAsync(QueryOptions<Recipe>? queryOptions = null)
        {
            queryOptions ??= new QueryOptions<Recipe>();

            var query = this.recipesRepository.All(queryOptions.WithDeleted);

            query = this.ModifyQuery(query, queryOptions);

            return (await query.ToListAsync()).Select(r => this.mapper.Map<RecipeDto>(r)).ToArray();
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<RecipeDto>> GetRecipeAsync(string recipeId, QueryOptions<Recipe>? queryOptions = null)
        {
            queryOptions ??= new QueryOptions<Recipe>();

            if (!ObjectId.TryParse(recipeId, out _))
            {
                return new BadRequestDetails(string.Format(ExceptionMessages.InvalidFormat, nameof(recipeId)));
            }

            var findResult = await this.recipesRepository.FindAsync(recipeId, queryOptions.WithDeleted);

            if (findResult.IsFailed)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, "recipe"));
            }

            return this.mapper.Map<RecipeDto>(findResult.Data);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<RecipeDto>> CreateRecipeAsync(string userId, RecipeInputDto recipeInputDto)
        {
            if (recipeInputDto.UserId.ToString() != userId)
            {
                return new UnauthorizedDetails(ExceptionMessages.NotOwner);
            }

            var validationResults = this.ValidateDto(recipeInputDto);

            if (validationResults.Count > 0)
            {
                var errorMessages = string.Join(' ', validationResults.Select(vr => vr.ErrorMessage));
                return new BadRequestDetails(errorMessages);
            }

            var addResult = await this.recipesRepository.AddAsync(this.mapper.Map<Recipe>(recipeInputDto));

            if (addResult.IsFailed)
            {
                return new InternalServerErrorDetails(ExceptionMessages.InternalServerError);
            }

            return this.mapper.Map<RecipeDto>(addResult.Data);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<RecipeDto>> UpdateRecipeAsync(string userId, string recipeId, RecipeInputDto recipeInputDto)
        {
            if (!ObjectId.TryParse(recipeId, out _))
            {
                return new BadRequestDetails(string.Format(ExceptionMessages.InvalidFormat, nameof(recipeId)));
            }

            var findResult = await this.recipesRepository.FindAsync(recipeId, false);

            if (findResult.IsFailed)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, "recipe"));
            }

            var recipe = findResult.Data ?? throw new NullReferenceException(nameof(findResult.Data));

            if (!(recipe.UserId.ToString() == userId && userId == recipeInputDto.UserId.ToString()))
            {
                return new UnauthorizedDetails(ExceptionMessages.NotOwner);
            }

            var validationResults = this.ValidateDto(recipeInputDto);

            if (validationResults.Count > 0)
            {
                var errorMessages = string.Join(' ', validationResults.Select(vr => vr.ErrorMessage));
                return new BadRequestDetails(errorMessages);
            }

            this.CopyProperties(recipeInputDto, recipe);
            var updateResult = await this.recipesRepository.UpdateAsync(recipeId, recipe);

            if (updateResult.IsFailed)
            {
                return new InternalServerErrorDetails(ExceptionMessages.InternalServerError);
            }

            return this.mapper.Map<RecipeDto>(recipe);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult> DeleteRecipeAsync(string userId, string recipeId)
        {
            if (!ObjectId.TryParse(recipeId, out _))
            {
                return new BadRequestDetails(string.Format(ExceptionMessages.InvalidFormat, nameof(recipeId)));
            }

            var findResult = await this.recipesRepository.FindAsync(recipeId, false);

            if (findResult.IsFailed)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, "recipe"));
            }

            var recipe = findResult.Data ?? throw new NullReferenceException(nameof(findResult.Data));

            if (recipe.UserId.ToString() != userId)
            {
                return new UnauthorizedDetails(ExceptionMessages.NotOwner);
            }

            var deleteResult = await this.recipesRepository.DeleteAsync(recipeId);

            if (deleteResult.IsFailed)
            {
                return new InternalServerErrorDetails(ExceptionMessages.InternalServerError);
            }

            return ServiceResult.Success;
        }
    }
}
