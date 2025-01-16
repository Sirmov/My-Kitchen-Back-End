// |-----------------------------------------------------------------------------------------------------|
// <copyright file="Recipe.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Data.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    using MyKitchen.Microservices.Recipes.Data.Models.Common;

    /// <summary>
    /// This class represents a recipe document model.
    /// </summary>
    public class Recipe : BaseDocument<string>
    {
        /// <summary>
        /// Gets or sets the URL of the recipe image.
        /// </summary>
        [BsonRequired]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ImageId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the id of the user to whom the recipe belongs.
        /// </summary>
        [BsonRequired]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the title of the recipe.
        /// </summary>
        [BsonRequired]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the recipe.
        /// </summary>
        [BsonRequired]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ingredients text for the recipe.
        /// </summary>
        [BsonRequired]
        public string Ingredients { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the directions text for the recipe.
        /// </summary>
        [BsonRequired]
        public string Directions { get; set; } = string.Empty;
    }
}
