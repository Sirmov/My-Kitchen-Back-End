// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RecipeImageDto.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Recipes.Dtos
{
    /// <summary>
    /// This class is a data transfer object for the recipe image.
    /// </summary>
    public class RecipeImageDto
    {
        /// <summary>
        /// Gets or sets the file stream of the recipe image.
        /// </summary>
        public Stream FileStream { get; set; } = default!;

        /// <summary>
        /// Gets or sets the content type of the recipe image.
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file name of the recipe image.
        /// </summary>
        public string FileName { get; set; } = string.Empty;
    }
}
