// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RecipeDto.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Recipes.Dtos
{
    using MongoDB.Bson;

    using MyKitchen.Microservices.Recipes.Data.Models;

    /// <summary>
    /// This class is a data transfer object for the <see cref="Recipe"/> model.
    /// </summary>
    public class RecipeDto
    {
        /// <summary>
        /// Gets or sets the id of the recipe.
        /// </summary>
        public ObjectId Id { get; set; } = ObjectId.Empty;

        /// <summary>
        /// Gets or sets the id of the user to whom the recipe belongs.
        /// </summary>
        public ObjectId UserId { get; set; } = ObjectId.Empty;

        /// <summary>
        /// Gets or sets the title of the recipe.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the recipe.
        /// </summary>
        public string Description { get; set; } = string.Empty;

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
