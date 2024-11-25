// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IUsersService.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Users.Contracts
{
    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Common.Dtos.User;
    using MyKitchen.Microservices.Identity.Services.Common.ServiceResult;
    using MyKitchen.Microservices.Identity.Services.Users.Dtos.User;

    /// <summary>
    /// This interface defines the functionality of the users service.
    /// </summary>
    /// <typeparam name="TUser">The type of the identity user.</typeparam>
    /// <typeparam name="TRole">The type of the identity role.</typeparam>
    public interface IUsersService<TUser, TRole>
        where TUser : ApplicationUser, new()
        where TRole : ApplicationRole, new()
    {
        /// <summary>
        /// This method asynchronously gets all users.
        /// </summary>
        /// <returns>Returns a <see cref="IEnumerable{T}"/> of <see cref="UserDto"/>.</returns>
        public Task<ServiceResult<IEnumerable<UserDto>>> GetAllUsersAsync();

        /// <summary>
        /// This method asynchronously registers a new user with an email and username.
        /// </summary>
        /// <param name="userRegisterDto">The <see cref="UserRegisterDto"/> containing the register credentials.</param>
        /// <param name="roles">The roles which the new user has to be in.</param>
        /// <returns>Returns the created user.</returns>
        public Task<ServiceResult<UserDto>> RegisterWithEmailAndUsernameAsync(UserRegisterDto userRegisterDto, IEnumerable<string>? roles = null);

        /// <summary>
        /// This method asynchronously updates a user.
        /// </summary>
        /// <param name="userDto">The <see cref="UserDto"/> containing the updated information.</param>
        /// <returns>Returns an empty <see cref="ServiceResult"/>.</returns>
        public Task<ServiceResult> UpdateUserAsync(UserDto userDto);

        /// <summary>
        /// This method asynchronously signs in a user in with an email and password.
        /// </summary>
        /// <param name="email">The email of the user trying to login.</param>
        /// <param name="password">The password of the user trying to login.</param>
        /// <param name="isPersistent">Flag indicating whether the sign-in cookie should persist after the browser is closed.</param>
        /// <param name="isLockout">Flag indicating if the user account should be locked if the sign in fails.</param>
        /// <returns>
        /// Returns a tuple of string the first one of which containing
        /// the access token and the second one the refresh token.
        /// </returns>
        public Task<ServiceResult<(string accessToken, string refreshToken)>> LoginWithEmailAsync(string email, string password, bool isPersistent = false, bool isLockout = true);

        /// <summary>
        /// This method asynchronously signs in a user in with an username and password.
        /// </summary>
        /// <param name="username">The username of the user trying to login.</param>
        /// <param name="password">The password of the user trying to login.</param>
        /// <param name="isPersistent">Flag indicating whether the sign-in cookie should persist after the browser is closed.</param>
        /// <param name="isLockout">Flag indicating if the user account should be locked if the sign in fails.</param>
        /// <returns>
        /// Returns a tuple of string the first one of which containing
        /// the access token and the second one the refresh token.
        /// </returns>
        public Task<ServiceResult<(string accessToken, string refreshToken)>> LoginWithUsernameAsync(string username, string password, bool isPersistent = false, bool isLockout = true);

        /// <summary>
        /// This method asynchronously signs out a user.
        /// </summary>
        /// <param name="accessTokenId">The id of the access token of the user.</param>
        /// <returns>Returns am empty <see cref="ServiceResult"/>.</returns>
        public Task<ServiceResult> LogoutAsync(string accessTokenId);

        /// <summary>
        /// This method asynchronously checks whether a user is signed in.
        /// </summary>
        /// <param name="user">The user to be checked.</param>
        /// <returns>Returns a <see langword="bool"/> indicating whether the <paramref name="user"/> is signed in.</returns>
        public ServiceResult<bool> IsSignedIn(TUser user);

        /// <summary>
        /// This method asynchronously finds a user with the specified <paramref name="email"/>.
        /// </summary>
        /// <param name="email">The email of the searched user.</param>
        /// <returns>Returns the found user.</returns>
        public Task<ServiceResult<UserDto>> FindUserByEmailAsync(string email);

        /// <summary>
        /// This method asynchronously finds a user with the specified <paramref name="username"/>.
        /// </summary>
        /// <param name="username">The username of the searched user.</param>
        /// <returns>Returns the found user.</returns>
        public Task<ServiceResult<UserDto>> FindUserByUsernameAsync(string username);

        /// <summary>
        /// This method asynchronously finds a user with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the searched user.</param>
        /// <returns>Returns the found user.</returns>
        public Task<ServiceResult<UserDto>> FindUserByIdAsync(string id);

        /// <summary>
        /// This method asynchronously deletes the user with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the user to be deleted.</param>
        /// <returns>Returns am empty <see cref="ServiceResult"/>.</returns>
        public Task<ServiceResult> DeleteUserWithIdAsync(string id);

        /// <summary>
        /// This method asynchronously checks whether a user with the specified <paramref name="id"/> exists.
        /// </summary>
        /// <param name="id">The id of user to checked.</param>
        /// <returns>Returns a <see langword="bool"/> indicating whether the user exists.</returns>
        public Task<ServiceResult<bool>> ExistsAsync(string id);
    }
}
