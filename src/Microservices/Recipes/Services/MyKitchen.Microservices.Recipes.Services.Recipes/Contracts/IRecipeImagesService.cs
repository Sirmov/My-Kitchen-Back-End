// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IRecipeImagesService.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Recipes.Contracts
{
    using Microsoft.AspNetCore.Http;

    using MyKitchen.Microservices.Recipes.Services.Common.ServiceResult;
    using MyKitchen.Microservices.Recipes.Services.Recipes.Dtos;

    /// <summary>
    /// This interface defines the functionality of the recipe images service.
    /// </summary>
    public interface IRecipeImagesService
    {
        /// <summary>
        /// This method asynchronously uploads a recipe image.
        /// </summary>
        /// <param name="recipeImage">The image to be uploaded.</param>
        /// <returns>Returns the image id.</returns>
        public Task<ServiceResult<string>> UploadRecipeImageAsync(IFormFile recipeImage);

        /// <summary>
        /// This method asynchronously returns a open stream of the recipe image.
        /// </summary>
        /// <param name="recipeImageId">The id of the image to be returned.</param>
        /// <returns>Returns a <see cref="RecipeImageDto"/>.</returns>
        public Task<ServiceResult<RecipeImageDto>> GetRecipeImageAsync(string recipeImageId);

        /// <summary>
        /// This method asynchronously deletes a recipe image.
        /// </summary>
        /// <param name="recipeImageId">The id of the image to be deleted.</param>
        /// <returns>Returns an empty <see cref="ServiceResult"/>.</returns>
        public Task<ServiceResult> DeleteRecipeImageAsync(string recipeImageId);
    }
}
