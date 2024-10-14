// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IHaveCustomMappings.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Mapping
{
    using AutoMapper;

    /// <summary>
    /// This interface should be implemented when a class has custom mapping logic.
    /// </summary>
    public interface IHaveCustomMappings
    {
        /// <summary>
        /// This method registers the custom mapping logic in a automapper profile.
        /// </summary>
        /// <param name="configuration">The automapper profile.</param>
        void CreateMappings(IProfileExpression configuration);
    }
}
