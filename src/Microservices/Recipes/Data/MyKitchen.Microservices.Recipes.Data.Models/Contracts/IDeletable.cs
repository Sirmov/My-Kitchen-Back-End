// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IDeletable.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Data.Models.Contracts
{
    /// <summary>
    /// This interface defines the properties of a deletable document.
    /// </summary>
    public interface IDeletable
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entity is deleted or active.
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was deleted.
        /// </summary>
        DateTime? DeletedOn { get; set; }
    }
}
