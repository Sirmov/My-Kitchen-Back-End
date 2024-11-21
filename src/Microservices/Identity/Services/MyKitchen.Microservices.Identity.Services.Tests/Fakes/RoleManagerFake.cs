// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RoleManagerFake.cs" company="MyKitchen">
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
    /// This class is a fake of <see cref="RoleManager{TRole}"/>.
    /// </summary>
    /// <typeparam name="TRole">The type used for identity role.</typeparam>
    public class RoleManagerFake<TRole> : IFake<RoleManager<TRole>>
        where TRole : ApplicationRole
    {
        private readonly List<TRole> roles;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleManagerFake{TRole}"/> class.
        /// </summary>
        public RoleManagerFake()
            : this(new List<TRole>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleManagerFake{TRole}"/> class.
        /// </summary>
        /// <param name="roles">A list of roles used to populate the <see cref="RoleManagerFake{TUser}"/>.<</param>
        public RoleManagerFake(List<TRole> roles)
        {
            this.roles = roles;

            var roleStoreMock = new Mock<IRoleStore<TRole>>();
            this.Mock = new Mock<RoleManager<TRole>>(roleStoreMock.Object, null!, null!, null!, null!);
            this.Mock.Object.RoleValidators.Add(new RoleValidator<TRole>());

            this.SetupBehavior(this.Mock);

            this.Instance = this.Mock.Object;
        }

        /// <inheritdoc/>
        public RoleManager<TRole> Instance { get; }

        /// <inheritdoc/>
        public Mock<RoleManager<TRole>> Mock { get; }

        /// <inheritdoc/>
        public void SetupBehavior(Mock<RoleManager<TRole>> mock)
        {
            var rolesMock = this.roles.BuildMock();
            mock.Setup(x => x.Roles).Returns(rolesMock);

            mock.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync((string roleName) => this.roles.Exists(r => r.Name == roleName));
        }
    }
}
