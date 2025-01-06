// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RecipeCreateDto.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Recipes.Dtos
{
    using System.ComponentModel.DataAnnotations;

    using MongoDB.Bson;

    using MyKitchen.Microservices.Recipes.Data.Models;

    using static MyKitchen.Microservices.Recipes.Data.Common.ModelConstraints.Recipe;

    /// <summary>
    /// This class is a data transfer object for the <see cref="Recipe"/> model.
    /// </summary>
    public class RecipeCreateDto
    {
        /// <summary>
        /// Gets or sets the id of the user to whom the recipe belongs.
        /// </summary>
        [Required]
        public ObjectId UserId { get; set; } = ObjectId.Empty;

        /// <summary>
        /// Gets or sets the title of the recipe.
        /// </summary>
        [Required]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the recipe.
        /// </summary>
        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength)]
        public string Description { get; set; } = string.Empty;
    }
}
