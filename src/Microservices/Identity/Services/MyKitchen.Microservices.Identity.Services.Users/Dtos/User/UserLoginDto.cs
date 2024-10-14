// |-----------------------------------------------------------------------------------------------------|
// <copyright file="UserLoginDto.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Users.Dtos.User
{
    using System.ComponentModel.DataAnnotations;

    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Mapping;

    using static MyKitchen.Microservices.Identity.Data.Common.Constants.ModelConstraints.ApplicationUser;

    /// <summary>
    /// This class is a data transfer object for the <see cref="ApplicationUser"/> model.
    /// It is used for logging in a user.
    /// </summary>
    public class UserLoginDto : IMapFrom<ApplicationUser>, IMapTo<ApplicationUser>
    {
        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        [EmailAddress]
        [Required(AllowEmptyStrings = false)]
        [StringLength(EmailMaxLength, MinimumLength = EmailMinLength)]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength)]
        public string Password { get; set; } = null!;
    }
}
