// |-----------------------------------------------------------------------------------------------------|
// <copyright file="UserManagerFake.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Tests.Fakes
{
    using Microsoft.AspNetCore.Identity;

    using MockQueryable;

    using Moq;

    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Tests.Fakes.Contracts;

    /// <summary>
    /// This class is a fake of <see cref="UserManager{TUser}"/>.
    /// </summary>
    /// <typeparam name="TUser">The type used for identity user.</typeparam>
    public class UserManagerFake<TUser> : IFake<UserManager<TUser>>
        where TUser : ApplicationUser
    {
        private readonly List<TUser> users = new List<TUser>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerFake{TUser}"/> class.
        /// </summary>
        public UserManagerFake()
            : this(new List<TUser>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagerFake{TUser}"/> class.
        /// </summary>
        /// <param name="users">A list of users used to populate the <see cref="UserManagerFake{TUser}"/>.<</param>
        public UserManagerFake(List<TUser> users)
        {
            this.users = users;

            var userStoreMock = new Mock<IUserStore<TUser>>();
            this.Mock = new Mock<UserManager<TUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            this.Mock.Object.UserValidators.Add(new UserValidator<TUser>());
            this.Mock.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            this.SetupBehavior(this.Mock);

            this.Instance = this.Mock.Object;
        }

        /// <inheritdoc/>
        public UserManager<TUser> Instance { get; }

        /// <inheritdoc/>
        public Mock<UserManager<TUser>> Mock { get; }

        /// <inheritdoc/>
        public void SetupBehavior(Mock<UserManager<TUser>> mock)
        {
            mock.Setup(x => x.Users).Returns(this.users.BuildMock());

            mock.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .Callback((TUser user, string _) => this.users.Add(user))
                .ReturnsAsync(IdentityResult.Success);

            mock.Setup(x => x.UpdateAsync(It.IsAny<TUser>()))
                .Callback((TUser updatedUser) => this.users[this.users.FindIndex(u => u.Id == updatedUser.Id)] = updatedUser)
                .ReturnsAsync(IdentityResult.Success);

            mock.Setup(x => x.DeleteAsync(It.IsAny<TUser>()))
                .Callback((TUser user) => this.users.RemoveAt(this.users.FindIndex(u => u.Id == user.Id)))
                .ReturnsAsync(IdentityResult.Success);

            mock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string userId) => this.users.Find(u => u.Id.ToString() == userId));

            mock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((string userEmail) => this.users.Find(u => u.Email == userEmail));

            mock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((string userUsername) => this.users.Find(u => u.UserName == userUsername));

            mock.Setup(x => x.GetRolesAsync(It.IsAny<TUser>())).ReturnsAsync((TUser user) => user.Roles);

            mock.Setup(x => x.IsInRoleAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync((TUser user, string role) => user.Roles.Contains(role));

            mock.Setup(x => x.AddToRoleAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .Callback((TUser user, string role) => user.Roles.Add(role))
                .ReturnsAsync(IdentityResult.Success);

            mock.Setup(x => x.RemoveFromRoleAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .Callback((TUser user, string role) => user.Roles.Remove(role))
                .ReturnsAsync(IdentityResult.Success);
        }
    }
}
