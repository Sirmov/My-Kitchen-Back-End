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

    using MyKitchen.Microservices.Recipes.Data;
    using MyKitchen.Microservices.Recipes.Data.Contracts;
    using MyKitchen.Microservices.Recipes.Data.Models;
    using MyKitchen.Microservices.Recipes.Services.Common.QueryOptions;
    using MyKitchen.Microservices.Recipes.Services.Common.ServiceResult;
    using MyKitchen.Microservices.Recipes.Services.Recipes.Contracts;
    using MyKitchen.Microservices.Recipes.Services.Recipes.Dtos;

    /// <summary>
    /// This class encapsulates the business logic related to the <see cref="Recipe"/> model.
    /// </summary>
    public class RecipesService : BaseService<Recipe, ObjectId>, IRecipesService
    {
        private readonly IRepository<Recipe, ObjectId> recipesRepository;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipesService"/> class.
        /// </summary>
        /// <param name="recipesRepository">The <see cref="Recipe"/> repository.</param>
        /// <param name="mapper">The global automapper instance.</param>
        public RecipesService(IRepository<Recipe, ObjectId> recipesRepository, IMapper mapper)
            : base(recipesRepository, mapper)
        {
            this.recipesRepository = recipesRepository;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<RecipeDto>> CreateRecipeAsync(RecipeInputDto recipeInputDto)
        {
            var result = await base.CreateAsync<RecipeDto, RecipeInputDto>(recipeInputDto);

            return result;
        }

        /// <inheritdoc/>
        public Task<ServiceResult> DeleteRecipeAsync(string recipeId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<ServiceResult<IEnumerable<RecipeDto>>> GetAllRecipesAsync(QueryOptions<Recipe>? queryOptions = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<ServiceResult<RecipeDto>> GetRecipeAsync(string recipeId, QueryOptions<Recipe>? queryOptions = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<ServiceResult<RecipeDto>> UpdateRecipeAsync(string recipeId, RecipeInputDto recipeInputDto)
        {
            throw new NotImplementedException();
        }
    }
}
