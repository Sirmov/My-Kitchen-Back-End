// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RecipeInputDto.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Recipes.Dtos
{
    using System.ComponentModel.DataAnnotations;

    using AutoMapper.Configuration.Annotations;

    using Microsoft.AspNetCore.Http;

    using MyKitchen.Microservices.Recipes.Data.Models;
    using MyKitchen.Microservices.Recipes.Services.Mapping;

    using static MyKitchen.Microservices.Recipes.Data.Common.ModelConstraints.Recipe;

    /// <summary>
    /// This class is a data transfer object for the <see cref="Recipe"/> model.
    /// </summary>
    public class RecipeInputDto : IMapFrom<Recipe>, IMapTo<Recipe>
    {
        /// <summary>
        /// Gets or sets the image of the recipe.
        /// </summary>
        [Required]
        [Ignore]
        public IFormFile Image { get; set; } = default!;

        /// <summary>
        /// Gets or sets the id of the user to whom the recipe belongs.
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

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

        /// <summary>
        /// Gets or sets the ingredients text for the recipe.
        /// </summary>
        [Required]
        [StringLength(IngredientsMaxLength, MinimumLength = IngredientsMinLength)]
        public string Ingredients { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the directions text for the recipe.
        /// </summary>
        [Required]
        [StringLength(DirectionsMaxLength, MinimumLength = DirectionsMinLength)]
        public string Directions { get; set; } = string.Empty;
    }
}
