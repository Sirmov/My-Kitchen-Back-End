// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IAuditable.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Data.Models.Contracts
{
    /// <summary>
    /// This interface defines the properties of a entity that a audit info can be kept track of.
    /// </summary>
    public interface IAuditable
    {
        /// <summary>
        /// Gets or sets the date and time of the creation of the entity.
        /// </summary>
        DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was last modified.
        /// </summary>
        DateTime? ModifiedOn { get; set; }
    }
}
