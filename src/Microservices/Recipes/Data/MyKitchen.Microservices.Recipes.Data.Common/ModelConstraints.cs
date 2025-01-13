// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ModelConstraints.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Data.Common
{
    /// <summary>
    /// This static class contains all of the constraints of different models.
    /// </summary>
    public static class ModelConstraints
    {
        /// <summary>
        /// This static class contains all of the constraints for the recipe model.
        /// </summary>
        public static class Recipe
        {
            /// <summary>
            /// An integer defining the title minimum length.
            /// </summary>
            public const int TitleMinLength = 3;

            /// <summary>
            /// An integer defining the title maximum length.
            /// </summary>
            public const int TitleMaxLength = 25;

            /// <summary>
            /// An integer defining the description minimum length.
            /// </summary>
            public const int DescriptionMinLength = 10;

            /// <summary>
            /// An integer defining the description maximum length.
            /// </summary>
            public const int DescriptionMaxLength = 300;

            /// <summary>
            /// An integer defining the ingredients text minimum length.
            /// </summary>
            public const int IngredientsMinLength = 5;

            /// <summary>
            /// An integer defining the ingredients text maximum length.
            /// </summary>
            public const int IngredientsMaxLength = 200;

            /// <summary>
            /// An integer defining the directions text minimum length.
            /// </summary>
            public const int DirectionsMinLength = 10;

            /// <summary>
            /// An integer defining the directions text maximum length.
            /// </summary>
            public const int DirectionsMaxLength = 500;
        }
    }
}
