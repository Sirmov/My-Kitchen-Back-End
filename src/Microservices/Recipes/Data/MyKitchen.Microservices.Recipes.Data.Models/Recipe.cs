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
    public class Recipe : BaseDocument<ObjectId>
    {
        /// <summary>
        /// Gets or sets the id of the user to whom the recipe belongs.
        /// </summary>
        public ObjectId UserId { get; set; } = ObjectId.Empty;

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
    }
}
