// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RecipesController.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Api.Rest.Controllers
{
    using System.Net.Mime;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using MyKitchen.Common.ProblemDetails;
    using MyKitchen.Microservices.Recipes.Api.Rest.Constants;
    using MyKitchen.Microservices.Recipes.Data.Models;
    using MyKitchen.Microservices.Recipes.Services.Common.QueryOptions;
    using MyKitchen.Microservices.Recipes.Services.Recipes.Contracts;
    using MyKitchen.Microservices.Recipes.Services.Recipes.Dtos;

    /// <summary>
    /// This api controller is responsible for handling all requests regarding the <see cref="Recipe"/> model.
    /// </summary>
    public class RecipesController : BaseController
    {
        private readonly IRecipesService recipeService;

        public RecipesController(IRecipesService recipesService)
        {
            this.recipeService = recipesService;
        }

        /// <summary>
        /// This action is responsible for handling the request for retrieving multiple recipes.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>Returns a collection of <see cref="RecipeDto"/>.</returns>
        [HttpGet]
        [Route(RouteConstants.Recipes.RecipesEndpoint)]
        [ProducesResponseType(typeof(IEnumerable<RecipeDto>), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetAllRecipes([FromQuery] QueryOptions<Recipe> queryOptions)
        {
            var getResult = await this.recipeService.GetAllRecipesAsync(queryOptions);

            return getResult.ToActionResult(recipes => this.Ok(recipes));
        }

        /// <summary>
        /// This action is responsible for handling the request for retrieving a single recipe.
        /// </summary>
        /// <param name="recipeId">The id of the recipe to be retrieved.</param>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>Returns a <see cref="RecipeDto"/>.</returns>
        [HttpGet]
        [Route(RouteConstants.Recipes.RecipeEndpoint)]
        [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(NotFoundDetails), StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetRecipe([FromRoute] string recipeId, [FromQuery] QueryOptions<Recipe> queryOptions)
        {
            var getResult = await this.recipeService.GetRecipeAsync(recipeId, queryOptions);

            return getResult.ToActionResult(recipes => this.Ok(recipes));
        }
    }
}
