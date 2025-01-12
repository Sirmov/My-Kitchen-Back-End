// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IRecipesService.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Recipes.Contracts
{
    using MyKitchen.Microservices.Recipes.Data.Models;
    using MyKitchen.Microservices.Recipes.Services.Common.QueryOptions;
    using MyKitchen.Microservices.Recipes.Services.Common.ServiceResult;
    using MyKitchen.Microservices.Recipes.Services.Recipes.Dtos;

    /// <summary>
    /// This interface defines the functionality of the recipes service.
    /// </summary>
    public interface IRecipesService
    {
        /// <summary>
        /// This method asynchronously creates a new recipe.
        /// </summary>
        /// <param name="recipeInputDto">The recipe input dto.</param>
        /// <returns>Returns a dto of the newly created recipe.</returns>
        public Task<ServiceResult<RecipeDto>> CreateRecipeAsync(RecipeInputDto recipeInputDto);

        /// <summary>
        /// This method asynchronously updates a existing recipe.
        /// </summary>
        /// <param name="recipeId">The id of the recipe to be updated.</param>
        /// <param name="recipeInputDto">The recipe input dto.</param>
        /// <returns>Returns a dto of the updated recipe.</returns>
        public Task<ServiceResult<RecipeDto>> UpdateRecipeAsync(string recipeId, RecipeInputDto recipeInputDto);

        /// <summary>
        /// This method asynchronously returns all recipes async.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>Returns a collection of all recipe dtos.</returns>
        public Task<ServiceResult<IEnumerable<RecipeDto>>> GetAllRecipesAsync(QueryOptions<Recipe>? queryOptions = null);

        /// <summary>
        /// This method asynchronously returns a recipe with a matching <paramref name="recipeId"/>.
        /// </summary>
        /// <param name="recipeId">The id of the recipe to be returned.</param>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>Returns a recipe with a matching <paramref name="recipeId"/>.</returns>
        public Task<ServiceResult<RecipeDto>> GetRecipeAsync(string recipeId, QueryOptions<Recipe>? queryOptions = null);

        /// <summary>
        /// This method asynchronously marks a recipe with <paramref name="recipeId"/> as deleted.
        /// </summary>
        /// <param name="recipeId">The id of the recipe to be marked as deleted.</param>
        /// <returns>Returns a empty <see cref="ServiceResult"/>.</returns>
        public Task<ServiceResult> DeleteRecipeAsync(string recipeId);
    }
}
