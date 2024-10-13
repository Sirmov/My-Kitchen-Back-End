// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ITokensService.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Tokens.Contracts
{
    using MyKitchen.Microservices.Identity.Data.Models;

    /// <summary>
    /// This interface defines the functionality of the tokens service.
    /// </summary>
    /// <typeparam name="TUser">The type of the identity user.</typeparam>
    /// <typeparam name="TRole">The type of the identity role.</typeparam>
    public interface ITokensService<TUser, TRole>
        where TUser : ApplicationUser, new()
        where TRole : ApplicationRole, new()
    {
        /// <summary>
        /// This method generates a JWT bearer access token based on a identity <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The <typeparamref name="TUser"/> containing the user claims.</param>
        /// <returns>Returns a <see langword="string"/>.</returns>
        public Task<string> GenerateAccessTokenAsync(TUser user);
    }
}
