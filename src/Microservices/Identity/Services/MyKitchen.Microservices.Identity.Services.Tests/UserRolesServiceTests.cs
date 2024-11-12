// |-----------------------------------------------------------------------------------------------------|
// <copyright file="UserRolesServiceTests.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Tests
{
    using MyKitchen.Common.ProblemDetails;
    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Tests.Fakes;
    using MyKitchen.Microservices.Identity.Services.Users;

    /// <summary>
    /// This test fixture contains unit tests for the <see cref="UserRolesService{TUser, TRole}"/> class.
    /// </summary>
    [TestFixture]
    public class UserRolesServiceTests
    {
        private const string RolesCountIsNotCorrectMessage = "The count of roles is no correct.";
        private const string RolesDoNotMatchMessage = "Roles don't match.";
        private const string ObjectNotOfTypeMessage = "{0} type is not correct. Expected: {1}.";
        private const string UserNotInRoleMessage = "User should be in role: {0}.";
        private const string ResultShouldBeSuccessful = "Result should be successful.";
        private const string ResultShouldNotBeSuccessful = "Result should not be successful.";
        private const string ResultShouldBeTrue = "Result data should be true.";
        private const string ResultShouldBeFalse = "Result data should be false.";

        /// <summary>
        /// This test checks whether <see cref="UserRolesService{TUser, TRole}.GetAllRolesAsync"/>
        /// returns only the non null or whitespace roles.
        /// </summary>
        /// <param name="roleNames">The names of the roles to be used for testing.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous method.</returns>
        [TestCase("", "     ", "Admin", null)]
        public async Task GetAllRolesAsync_WhitespaceRoles_ReturnsAllNonWhiteSpaceRoles(params string?[] roleNames)
        {
            // Arrange
            var userManagerFake = new UserManagerFake<ApplicationUser>();

            var roles = new List<ApplicationRole>();

            foreach (var role in roleNames)
            {
                roles.Add(new ApplicationRole() { Name = role });
            }

            var roleManagerFake = new RoleManagerFake<ApplicationRole>(roles);

            var userRolesService = new UserRolesService<ApplicationUser, ApplicationRole>(
                userManagerFake.Instance, roleManagerFake.Instance);

            // Act
            var getResult = await userRolesService.GetAllRolesAsync();

            // Assert
            int nonWhiteSpaceRoles = roles.Where(r => !string.IsNullOrWhiteSpace(r.Name)).Count();

            Assert.That(getResult.Data!.ToList(), Has.Count.EqualTo(nonWhiteSpaceRoles), RolesCountIsNotCorrectMessage);
        }

        /// <summary>
        /// This test checks whether <see cref="UserRolesService{TUser, TRole}.GetUserRolesAsync(TUser)"/>
        /// returns the roles of the specified user.
        /// </summary>
        /// <param name="userRoleNames">The names of the roles of the user to be used for testing.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous method.</returns>
        [TestCase("User", "Admin")]
        public async Task GetUserRolesAsync_UserRoles_ReturnsAllUserRoles(params string[] userRoleNames)
        {
            // Arrange
            var user = new ApplicationUser() { UserName = "test", Roles = userRoleNames.ToList() };
            var users = new List<ApplicationUser>() { user, new ApplicationUser() { UserName = "fake" } };

            var userManagerFake = new UserManagerFake<ApplicationUser>(users);

            var roleManagerFake = new RoleManagerFake<ApplicationRole>();

            var userRolesService = new UserRolesService<ApplicationUser, ApplicationRole>(
                userManagerFake.Instance, roleManagerFake.Instance);

            // Act
            var getResult = await userRolesService.GetUserRolesAsync(user);

            // Assert
            Assert.That(getResult.Data!, Is.EquivalentTo(userRoleNames), RolesDoNotMatchMessage);
        }

        /// <summary>
        /// This test checks whether <see cref="UserRolesService{TUser, TRole}.AddToRoleAsync(TUser, string)"/>
        /// returns a <see cref="NotFoundDetails"/> when the user is <see langword="null"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous method.</returns>
        [Test]
        public async Task AddToRoleAsync_UserIsNull_ReturnsNotFoundDetails()
        {
            // Arrange
            var userManagerFake = new UserManagerFake<ApplicationUser>();
            var roleManagerFake = new RoleManagerFake<ApplicationRole>();

            var userRolesService = new UserRolesService<ApplicationUser, ApplicationRole>(
                userManagerFake.Instance, roleManagerFake.Instance);

            // Act
            var addResult = await userRolesService.AddToRoleAsync(null, "role");

            // Assert
            Assert.That(
                addResult.Failure,
                Is.InstanceOf<NotFoundDetails>(),
                string.Format(ObjectNotOfTypeMessage, "Result failure", nameof(NotFoundDetails)));
        }

        /// <summary>
        /// This test checks whether <see cref="UserRolesService{TUser, TRole}.AddToRoleAsync(TUser, string)"/>
        /// returns a <see cref="NotFoundDetails"/> when the role does not exist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous method.</returns>
        [Test]
        public async Task AddToRoleAsync_RoleDoesNotExist_ReturnsNotFoundDetails()
        {
            // Arrange
            var userManagerFake = new UserManagerFake<ApplicationUser>();
            var roleManagerFake = new RoleManagerFake<ApplicationRole>();

            var userRolesService = new UserRolesService<ApplicationUser, ApplicationRole>(
                userManagerFake.Instance, roleManagerFake.Instance);

            // Act
            var addResult = await userRolesService.AddToRoleAsync(new ApplicationUser(), "role");

            // Assert
            Assert.That(
                addResult.Failure,
                Is.InstanceOf<NotFoundDetails>(),
                string.Format(ObjectNotOfTypeMessage, "Result failure", nameof(NotFoundDetails)));
        }

        /// <summary>
        /// This test checks whether <see cref="UserRolesService{TUser, TRole}.AddToRoleAsync(TUser, string)"/>
        /// returns a <see cref="BadRequestDetails"/> when the user is already in a role.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous method.</returns>
        [Test]
        public async Task AddToRoleAsync_UserAlreadyInRole_ReturnsBadRequestDetails()
        {
            // Arrange
            const string userUsername = "test", userRole = "role";
            var role = new ApplicationRole() { Name = userRole };
            var user = new ApplicationUser() { UserName = userUsername, Roles = { userRole } };

            var userManagerFake = new UserManagerFake<ApplicationUser>([user]);
            var roleManagerFake = new RoleManagerFake<ApplicationRole>([role]);

            var userRolesService = new UserRolesService<ApplicationUser, ApplicationRole>(
                userManagerFake.Instance, roleManagerFake.Instance);

            // Act
            var addResult = await userRolesService.AddToRoleAsync(user, userRole);

            // Assert
            Assert.That(
                addResult.Failure,
                Is.InstanceOf<BadRequestDetails>(),
                string.Format(ObjectNotOfTypeMessage, "Result failure", nameof(BadRequestDetails)));
        }

        /// <summary>
        /// This test checks whether <see cref="UserRolesService{TUser, TRole}.AddToRoleAsync(TUser, string)"/>
        /// adds a user to a role.
        /// </summary>
        /// <param name="userRole">The role the user to be added to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous method.</returns>
        [TestCase("Admin")]
        public async Task AddToRoleAsync_AddsUserToRole_ReturnsSuccessfulResult(string userRole)
        {
            // Arrange
            const string userUsername = "test";
            var role = new ApplicationRole() { Name = userRole };
            var user = new ApplicationUser() { UserName = userUsername };

            var userManagerFake = new UserManagerFake<ApplicationUser>([user]);
            var roleManagerFake = new RoleManagerFake<ApplicationRole>([role]);

            var userRolesService = new UserRolesService<ApplicationUser, ApplicationRole>(
                userManagerFake.Instance, roleManagerFake.Instance);

            // Act
            var addResult = await userRolesService.AddToRoleAsync(user, userRole);

            // Assert
            Assert.That(addResult.IsSuccessful, Is.True, ResultShouldBeSuccessful);

            user = await userManagerFake.Instance.FindByNameAsync(userUsername);
            Assert.That(user!.Roles, Does.Contain(userRole), string.Format(UserNotInRoleMessage, userRole));
        }

        /// <summary>
        /// This test checks whether <see cref="UserRolesService{TUser, TRole}.RemoveFromRoleAsync(TUser, string)"/>
        /// returns a <see cref="NotFoundDetails"/> when the user is null.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous method.</returns>
        [Test]
        public async Task RemoveFromRole_UserIsNull_ReturnsNotFoundDetails()
        {
            // Arrange
            var userManagerFake = new UserManagerFake<ApplicationUser>();
            var roleManagerFake = new RoleManagerFake<ApplicationRole>();

            var userRolesService = new UserRolesService<ApplicationUser, ApplicationRole>(
                userManagerFake.Instance, roleManagerFake.Instance);

            // Act
            var removeResult = await userRolesService.RemoveFromRoleAsync(null, "role");

            // Assert
            Assert.That(
                removeResult.Failure,
                Is.InstanceOf<NotFoundDetails>(),
                string.Format(ObjectNotOfTypeMessage, "Result failure", nameof(NotFoundDetails)));
        }

        /// <summary>
        /// This test checks whether <see cref="UserRolesService{TUser, TRole}.RemoveFromRoleAsync(TUser, string)"/>
        /// returns a <see cref="NotFoundDetails"/> when the role does not exist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous method.</returns>
        [Test]
        public async Task RemoveFromRole_RoleDoesNotExist_ReturnsNotFoundDetails()
        {
            // Arrange
            var userManagerFake = new UserManagerFake<ApplicationUser>();
            var roleManagerFake = new RoleManagerFake<ApplicationRole>();

            var userRolesService = new UserRolesService<ApplicationUser, ApplicationRole>(
                userManagerFake.Instance, roleManagerFake.Instance);

            // Act
            var removeResult = await userRolesService.RemoveFromRoleAsync(new ApplicationUser(), "role");

            // Assert
            Assert.That(
                removeResult.Failure,
                Is.InstanceOf<NotFoundDetails>(),
                string.Format(ObjectNotOfTypeMessage, "Result failure", nameof(NotFoundDetails)));
        }

        /// <summary>
        /// This test checks whether <see cref="UserRolesService{TUser, TRole}.RemoveFromRoleAsync(TUser, string)"/>
        /// returns a <see cref="BadRequestDetails"/> when the user is not in a role.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous method.</returns>
        [Test]
        public async Task RemoveFromRoleAsync_UserNotInRole_ReturnsBadRequestDetails()
        {
            // Arrange
            const string userUsername = "test", userRole = "role";
            var role = new ApplicationRole() { Name = userRole };
            var user = new ApplicationUser() { UserName = userUsername };

            var userManagerFake = new UserManagerFake<ApplicationUser>([user]);
            var roleManagerFake = new RoleManagerFake<ApplicationRole>([role]);

            var userRolesService = new UserRolesService<ApplicationUser, ApplicationRole>(
                userManagerFake.Instance, roleManagerFake.Instance);

            // Act
            var removeResult = await userRolesService.RemoveFromRoleAsync(user, "role");

            // Assert
            Assert.That(
                removeResult.Failure,
                Is.InstanceOf<BadRequestDetails>(),
                string.Format(ObjectNotOfTypeMessage, "Result failure", nameof(BadRequestDetails)));
        }

        /// <summary>
        /// This test checks whether <see cref="UserRolesService{TUser, TRole}.RemoveFromRoleAsync(TUser, string)"/>
        /// removes a user from a role.
        /// </summary>
        /// <param name="userRole">The role the user to be removed from.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous method.</returns>
        [TestCase("Admin")]
        public async Task RemoveFromRoleAsync_RemovesUserFromRole_ReturnsSuccessfulResult(string userRole)
        {
            // Arrange
            const string userUsername = "test";
            var role = new ApplicationRole() { Name = userRole };
            var user = new ApplicationUser() { UserName = userUsername, Roles = { userRole } };

            var userManagerFake = new UserManagerFake<ApplicationUser>([user]);
            var roleManagerFake = new RoleManagerFake<ApplicationRole>([role]);

            var userRolesService = new UserRolesService<ApplicationUser, ApplicationRole>(
                userManagerFake.Instance, roleManagerFake.Instance);

            // Act
            var removeResult = await userRolesService.RemoveFromRoleAsync(user, userRole);

            // Assert
            Assert.That(removeResult.IsSuccessful, Is.True, ResultShouldBeSuccessful);

            user = await userManagerFake.Instance.FindByNameAsync(userUsername);
            Assert.That(user!.Roles, Does.Not.Contain(userRole), string.Format(UserNotInRoleMessage, userRole));
        }

        /// <summary>
        /// This test checks whether <see cref="UserRolesService{TUser, TRole}.DoesRoleExistAsync(string)"/>
        /// returns <see langword="true"/> when the role exists.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous method.</returns>
        [Test]
        public async Task DoesRoleExistAsync_RoleExists_ReturnsTrue()
        {
            // Arrange
            string roleName = "role";
            ApplicationRole role = new ApplicationRole() { Name = roleName };

            var userManagerFake = new UserManagerFake<ApplicationUser>();
            var roleManagerFake = new RoleManagerFake<ApplicationRole>([role]);

            var userRolesService = new UserRolesService<ApplicationUser, ApplicationRole>(
                userManagerFake.Instance, roleManagerFake.Instance);

            // Act
            var existsResult = await userRolesService.DoesRoleExistAsync(roleName);

            // Assert
            Assert.That(existsResult.Data, Is.True, ResultShouldBeTrue);
        }

        /// <summary>
        /// This test checks whether <see cref="UserRolesService{TUser, TRole}.DoesRoleExistAsync(string)"/>
        /// returns <see langword="false"/> when the role does not exist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous method.</returns>
        [Test]
        public async Task DoesRoleExistAsync_RoleDoesNotExist_ReturnsTrue()
        {
            // Arrange
            var userManagerFake = new UserManagerFake<ApplicationUser>();
            var roleManagerFake = new RoleManagerFake<ApplicationRole>();

            var userRolesService = new UserRolesService<ApplicationUser, ApplicationRole>(
                userManagerFake.Instance, roleManagerFake.Instance);

            // Act
            var existsResult = await userRolesService.DoesRoleExistAsync("role");

            // Assert
            Assert.That(existsResult.Data, Is.False, ResultShouldBeFalse);
        }
    }
}
