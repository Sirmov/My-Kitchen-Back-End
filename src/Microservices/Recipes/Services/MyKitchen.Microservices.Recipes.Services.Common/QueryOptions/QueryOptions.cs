// |-----------------------------------------------------------------------------------------------------|
// <copyright file="QueryOptions.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Common.QueryOptions
{
    /// <summary>
    /// This class is used to modify service queries. That way we eliminate the creating of multiple similar methods.
    /// Example:
    /// <code>GetAll();
    /// GetAllOrderedByName();
    /// GetAllOrderedByNameWithDeleted();
    /// </code>
    /// All of this can be achieved by using like this:
    /// <code>GetAll(QuerryOptions options);</code>
    /// </summary>
    /// <typeparam name="TEntity">The type of the quired entities.</typeparam>
    public class QueryOptions<TEntity>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entities should be tracked.
        /// </summary>
        public bool IsReadOnly { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether only not deleted entities should be returned.
        /// </summary>
        public bool WithDeleted { get; set; } = false;

        /// <summary>
        /// Gets or sets a list of <see cref="OrderOption{TClass}"/>.
        /// The direct use of <see cref="List{T}"/> instead of <see cref="ICollection{T}"/> or <see cref="IEnumerable{T}"/> is because
        /// the ease of use and readability of the <c>new()</c> operator.
        /// </summary>
        public List<OrderOption<TEntity>> OrderOptions { get; set; } = new List<OrderOption<TEntity>>();

        /// <summary>
        /// Gets or sets the amount of entities to be skipped.
        /// </summary>
        public int Skip { get; set; } = 0;

        /// <summary>
        /// Gets or sets the amount of entities to be taken.
        /// </summary>
        public int Take { get; set; } = 10;
    }
}
