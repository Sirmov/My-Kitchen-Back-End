// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IUserRolesService.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Users.Contracts
{
    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Common.ServiceResult;

    /// <summary>
    /// This interface defines the functionality of the user roles service.
    /// </summary>
    /// <typeparam name="TUser">The type of the identity user.</typeparam>
    /// <typeparam name="TRole">The type of the identity role.</typeparam>
    public interface IUserRolesService<TUser, TRole>
        where TUser : ApplicationUser, new()
        where TRole : ApplicationRole, new()
    {
        /// <summary>
        /// This method asynchronously gets all roles as <see langword="string"/>.
        /// </summary>
        /// <returns>Returns a <see cref="IEnumerable{T}"/> of <see langword="string"/> with all roles.</returns>
        public Task<ServiceResult<IEnumerable<string>>> GetAllRolesAsync();

        /// <summary>
        /// This method asynchronously gets all user roles <see langword="string"/>.
        /// </summary>
        /// <param name="user">The user which roles to be returned.</param>
        /// <returns>Returns a <see cref="IEnumerable{T}"/> of <see langword="string"/> with all of the roles the user is in.</returns>
        public Task<ServiceResult<IEnumerable<string>>> GetUserRolesAsync(TUser user);

        /// <summary>
        /// This method asynchronously adds a <paramref name="user"/> to a <paramref name="role"/>.
        /// </summary>
        /// <param name="user">The user which needs to be added to a role.</param>
        /// <param name="role">The role in which the user needs to be added.</param>
        /// <returns>Returns an empty <see cref="ServiceResult"/>.</returns>
        public Task<ServiceResult> AddToRoleAsync(TUser user, string role);

        /// <summary>
        /// This method asynchronously removes a <paramref name="user"/> from a <paramref name="role"/>.
        /// </summary>
        /// <param name="user">The user which needs to be removed from a role.</param>
        /// <param name="role">The role which the user needs to be removed from.</param>
        /// <returns>Returns an empty <see cref="ServiceResult"/>.</returns>
        public Task<ServiceResult> RemoveFromRoleAsync(TUser user, string role);

        /// <summary>
        /// This method checks whether a specified <paramref name="role"/> exists.
        /// </summary>
        /// <param name="role">The role to be checked.</param>
        /// <returns>Returns a <see langword="bool"/> indicating whether the <paramref name="role"/> exists.</returns>
        public Task<ServiceResult<bool>> DoesRoleExistAsync(string role);
    }
}
