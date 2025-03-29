// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RecipesController.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Api.Rest.Controllers
{
    using System.Net.Mime;

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
        private readonly IRecipeImagesService recipeImagesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipesController"/> class.
        /// </summary>
        /// <param name="recipesService">The implementation of the <see cref="IRecipesService"/>.</param>
        /// <param name="recipeImagesService">The implementation of the <see cref="IRecipeImagesService"/>.</param>
        public RecipesController(IRecipesService recipesService, IRecipeImagesService recipeImagesService)
        {
            this.recipeService = recipesService;
            this.recipeImagesService = recipeImagesService;
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

        /// <summary>
        /// This action is responsible for handling the request for creating a recipe.
        /// </summary>
        /// <param name="recipeInputDto">The recipe input dto.</param>
        /// <returns>Returns a <see cref="RecipeDto"/> of the created recipe.</returns>
        [HttpPost]
        [Route(RouteConstants.Recipes.RecipesEndpoint)]
        [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status201Created, MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(BadRequestDetails), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> CreateRecipe([FromForm] RecipeInputDto recipeInputDto)
        {
            var createResult = await this.recipeService.CreateRecipeAsync(this.UserId, recipeInputDto);

            return createResult.ToActionResult(recipe => this.Created($"{RouteConstants.Recipes.RecipesEndpoint}/{recipe.Id}", recipe));
        }

        /// <summary>
        /// This action is responsible for handling the request for updating a existing recipe.
        /// </summary>
        /// <param name="recipeId">The id of the recipe to be updated.</param>
        /// <param name="recipeInputDto">The recipe input dto.</param>
        /// <returns>Returns a <see cref="RecipeDto"/> of the updated recipe.</returns>
        [HttpPut]
        [Route(RouteConstants.Recipes.RecipeEndpoint)]
        [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status201Created, MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(NotFoundDetails), StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(BadRequestDetails), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateRecipeAsync([FromRoute] string recipeId, [FromForm] RecipeInputDto recipeInputDto)
        {
            var updateResult = await this.recipeService.UpdateRecipeAsync(this.UserId, recipeId, recipeInputDto);

            return updateResult.ToActionResult(recipe => this.Ok(recipe));
        }

        /// <summary>
        /// This action is responsible for handling the request for deleting a existing recipe.
        /// </summary>
        /// <param name="recipeId">The id of the recipe to be deleted.</param>
        /// <returns>Returns a empty response.</returns>
        [HttpDelete]
        [Route(RouteConstants.Recipes.RecipeEndpoint)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundDetails), StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> DeleteRecipeAsync([FromRoute] string recipeId)
        {
            var deleteResult = await this.recipeService.DeleteRecipeAsync(this.UserId, recipeId);

            return deleteResult.ToActionResult(_ => this.NoContent());
        }

        /// <summary>
        /// This action is responsible for handling the request for retrieving the recipe image.
        /// </summary>
        /// <param name="imageId">The id of the image.</param>
        /// <returns>Returns a <see cref="FileStream"/>.</returns>
        [HttpGet]
        [Route(RouteConstants.Recipes.RecipeImageEndpoint)]
        [ProducesResponseType(typeof(FileStream), StatusCodes.Status200OK, MediaTypeNames.Application.Octet)]
        [ProducesResponseType(typeof(NotFoundDetails), StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetRecipeImageAsync([FromRoute] string imageId)
        {
            var getResult = await this.recipeImagesService.GetRecipeImageAsync(imageId);

            return getResult.ToActionResult(image => this.File(image.FileStream, image.ContentType, image.FileName));
        }
    }
}
