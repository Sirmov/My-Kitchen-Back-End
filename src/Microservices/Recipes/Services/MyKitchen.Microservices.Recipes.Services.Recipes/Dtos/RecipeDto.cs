// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RecipeDto.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Recipes.Dtos
{
    using MyKitchen.Microservices.Recipes.Data.Models;
    using MyKitchen.Microservices.Recipes.Services.Mapping;

    /// <summary>
    /// This class is a data transfer object for the <see cref="Recipe"/> model.
    /// </summary>
    public class RecipeDto : RecipeInputDto, IMapFrom<Recipe>, IMapTo<Recipe>
    {
        /// <summary>
        /// Gets or sets the id of the recipe.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the recipe image.
        /// </summary>
        public string ImageId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date time when the recipe was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date time when the recipe was last modified.
        /// </summary>
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the recipe is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the date time when the recipe was deleted.
        /// </summary>
        public DateTime? DeletedOn { get; set; }
    }
}
