// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RoleManagerFake.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Tests.Fakes
{
    using Microsoft.AspNetCore.Identity;

    using Moq;

    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Tests.Fakes.Contracts;

    /// <summary>
    /// This class is a fake of <see cref="RoleManager{TRole}"/>.
    /// </summary>
    /// <typeparam name="TRole">The type used for identity role.</typeparam>
    public class RoleManagerFake<TRole> : IFake<RoleManager<TRole>>
        where TRole : ApplicationRole
    {
        private readonly List<TRole> roles = new List<TRole>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleManagerFake{TRole}"/> class.
        /// </summary>
        public RoleManagerFake()
        {
            var roleStoreMock = new Mock<IRoleStore<TRole>>();
            this.Mock = new Mock<RoleManager<TRole>>(roleStoreMock.Object);
            this.Mock.Object.RoleValidators.Add(new RoleValidator<TRole>());

            this.SetupBehavior(this.Mock);

            this.Instance = this.Mock.Object;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleManagerFake{TRole}"/> class.
        /// </summary>
        /// <param name="roles">A list of roles used to populate the <see cref="RoleManagerFake{TUser}"/>.<</param>
        public RoleManagerFake(List<TRole> roles)
            : this()
        {
            this.roles = roles;
        }

        /// <inheritdoc/>
        public RoleManager<TRole> Instance { get; }

        /// <inheritdoc/>
        public Mock<RoleManager<TRole>> Mock { get; }

        /// <inheritdoc/>
        public void SetupBehavior(Mock<RoleManager<TRole>> mock)
        {
            mock.Setup(x => x.Roles).Returns(this.roles.AsQueryable());

            mock.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync((string roleName) => this.roles.Exists(r => r.Name == roleName));
        }
    }
}
