// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ModelConstraints.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Data.Common.Constants
{
    /// <summary>
    /// This static class contains all of the constraints of different models.
    /// </summary>
    public static class ModelConstraints
    {
        /// <summary>
        /// This static class contains all of the constraints for the application user model.
        /// </summary>
        public static class ApplicationUser
        {
            /// <summary>
            /// An integer defining the username minimum length.
            /// </summary>
            public const int UsernameMinLength = 3;

            /// <summary>
            /// An integer defining the username maximum length.
            /// </summary>
            public const int UsernameMaxLength = 32;

            /// <summary>
            /// An integer defining the password minimum length.
            /// </summary>
            public const int PasswordMinLength = 8;

            /// <summary>
            /// An integer defining the password maximum length.
            /// </summary>
            public const int PasswordMaxLength = 32;

            /// <summary>
            /// An integer defining the email minimum length.
            /// </summary>
            public const int EmailMinLength = 6;

            /// <summary>
            /// An integer defining the email maximum length.
            /// </summary>
            public const int EmailMaxLength = 32;
        }
    }
}
