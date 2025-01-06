// |-----------------------------------------------------------------------------------------------------|
// <copyright file="OrderByOrder.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Common.QueryOptions
{
    /// <summary>
    /// This enumeration contains the possible orders of order by.
    /// </summary>
    public enum OrderByOrder
    {
        /// <summary>
        /// Indicates that the order is ascending.
        /// </summary>
        Ascending,

        /// <summary>
        /// Indicates that the order is descending.
        /// </summary>
        Descending,
    }
}
