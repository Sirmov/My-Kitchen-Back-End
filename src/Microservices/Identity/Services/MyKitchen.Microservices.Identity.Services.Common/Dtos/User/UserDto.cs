// |-----------------------------------------------------------------------------------------------------|
// <copyright file="UserDto.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Common.Dtos.User
{
    using System.ComponentModel.DataAnnotations;

    using MongoDB.Bson;

    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Mapping;

    using static MyKitchen.Microservices.Identity.Data.Common.Constants.ModelConstraints.ApplicationUser;

    /// <summary>
    /// This class is a data transfer object for the <see cref="ApplicationUser"/> model.
    /// </summary>
    public class UserDto : IMapFrom<ApplicationUser>, IMapTo<ApplicationUser>
    {
        /// <summary>
        /// Gets or sets the id of the user.
        /// </summary>
        public ObjectId Id { get; set; }

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        [EmailAddress]
        [Required(AllowEmptyStrings = false)]
        [StringLength(EmailMaxLength, MinimumLength = EmailMinLength)]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(UsernameMaxLength, MinimumLength = UsernameMinLength)]
        public string Username { get; set; } = null!;

        /// <summary>
        /// Gets or sets the roles of the user.
        /// </summary>
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
    }
}
