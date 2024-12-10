// |-----------------------------------------------------------------------------------------------------|
// <copyright file="UserRolesService.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Users
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using MyKitchen.Common.Constants;
    using MyKitchen.Common.ProblemDetails;
    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Common.ServiceResult;
    using MyKitchen.Microservices.Identity.Services.Users.Contracts;

    /// <summary>
    /// This class contains the business logic around the identity user roles.
    /// </summary>
    /// <inheritdoc cref="IUserRolesService{TUser, TRole}"/>
    public class UserRolesService<TUser, TRole> : IUserRolesService<TUser, TRole>
        where TUser : ApplicationUser, new()
        where TRole : ApplicationRole, new()
    {
        private readonly UserManager<TUser> userManager;
        private readonly RoleManager<TRole> roleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRolesService{TUser, TRole}"/> class.
        /// </summary>
        /// <param name="userManager">The identity user manager.</param>
        /// <param name="roleManager">The identity role manager.</param>
        public UserRolesService(
            UserManager<TUser> userManager,
            RoleManager<TRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<IEnumerable<string>>> GetAllRolesAsync()
        {
            var roles = await this.roleManager.Roles
                .Select(r => r.Name ?? string.Empty)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .ToListAsync();

            return roles;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<IEnumerable<string>>> GetUserRolesAsync(TUser user)
        {
            var roles = await this.userManager.GetRolesAsync(user);

            return roles.ToList();
        }

        /// <inheritdoc/>
        public async Task<ServiceResult> AddToRoleAsync(TUser user, string role)
        {
            if (user == null)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(user)));
            }

            var roleExist = (await this.DoesRoleExistAsync(role)).Data;

            if (roleExist == false)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(role)));
            }

            var isInRole = await this.userManager.IsInRoleAsync(user, role);

            if (isInRole)
            {
                return new BadRequestDetails(ExceptionMessages.UserAlreadyInRole);
            }

            await this.userManager.AddToRoleAsync(user, role);
            return ServiceResult.Successful;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult> RemoveFromRoleAsync(TUser user, string role)
        {
            if (user == null)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(user)));
            }

            var roleExist = (await this.DoesRoleExistAsync(role)).Data;

            if (roleExist == false)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(role)));
            }

            var isInRole = await this.userManager.IsInRoleAsync(user, role);

            if (isInRole == false)
            {
                return new BadRequestDetails(ExceptionMessages.UserNotInRole);
            }

            await this.userManager.RemoveFromRoleAsync(user, role);
            return ServiceResult.Successful;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<bool>> DoesRoleExistAsync(string role)
        {
            return await this.roleManager.RoleExistsAsync(role);
        }
    }
}
