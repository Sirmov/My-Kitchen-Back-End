// |-----------------------------------------------------------------------------------------------------|
// <copyright file="OrderOption.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Common.QueryOptions
{
    using System.Linq.Expressions;

    using MyKitchen.Common.Constants;

    /// <summary>
    /// This class encapsulates one order option of a query.
    /// It is used in <see cref="QueryOptions{TEntity}"/> as a list of order options.
    /// </summary>
    /// <typeparam name="TClass">The class that should be ordered in a collection.</typeparam>
    public class OrderOption<TClass>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderOption{TClass}"/> class.
        /// </summary>
        public OrderOption()
        {
            this.PropertyName = string.Empty;
            this.PropertyLambda = x => this.GetProperty(x!, this.PropertyName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderOption{TClass}"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property used for sorting.</param>
        /// <param name="order">A parameter of type <see cref="OrderByOrder"/>.</param>
        /// <exception cref="ArgumentNullException">Throws when any of the parameters is null.</exception>
        public OrderOption(string propertyName, OrderByOrder order)
        {
            this.PropertyName = propertyName;
            this.PropertyLambda = x => this.GetProperty(x!, propertyName);
            this.Order = order;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderOption{TClass}"/> class.
        /// </summary>
        /// <param name="propertyLambda">The lambda expression used to capture the object's property.</param>
        /// <param name="order">A parameter of type <see cref="OrderByOrder"/>.</param>
        /// <exception cref="ArgumentNullException">Throws when any of the parameters is null.</exception>
        public OrderOption(Expression<Func<TClass, object>> propertyLambda, OrderByOrder order)
        {
            this.PropertyName = string.Empty;
            this.PropertyLambda = propertyLambda;
            this.Order = order;
        }

        /// <summary>
        /// Gets or sets the name of the property used for sorting.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the lambda expression used to capture the object's property.
        /// </summary>
        public Expression<Func<TClass, object>> PropertyLambda { get; set; }

        /// <summary>
        /// Gets or sets an enum of type <see cref="OrderByOrder"/>.
        /// </summary>
        public OrderByOrder Order { get; set; }

        private object GetProperty(object obj, string propertyName)
        {
            Type type = obj.GetType();
            var property = type.GetProperty(propertyName);

            if (property != null)
            {
                return property;
            }
            else
            {
                throw new ArgumentException(string.Format(ExceptionMessages.PropertyNotFound, propertyName, type.FullName));
            }
        }
    }
}
