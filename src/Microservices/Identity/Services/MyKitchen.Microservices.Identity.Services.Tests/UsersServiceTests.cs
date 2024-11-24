// |-----------------------------------------------------------------------------------------------------|
// <copyright file="UsersServiceTests.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Tests
{
    using AutoMapper;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;

    using Moq;

    using MyKitchen.Common.ProblemDetails;
    using MyKitchen.Common.Result.Contracts;
    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Common.Dtos.User;
    using MyKitchen.Microservices.Identity.Services.Common.ServiceResult;
    using MyKitchen.Microservices.Identity.Services.Mapping;
    using MyKitchen.Microservices.Identity.Services.Tests.Fakes;
    using MyKitchen.Microservices.Identity.Services.Tests.Fakes.Contracts;
    using MyKitchen.Microservices.Identity.Services.Tokens.Contracts;
    using MyKitchen.Microservices.Identity.Services.Users;
    using MyKitchen.Microservices.Identity.Services.Users.Contracts;
    using MyKitchen.Microservices.Identity.Services.Users.Dtos.User;

    /// <summary>
    /// This test fixture contains unit tests for the <see cref="UsersService"/> class.
    /// </summary>
    public class UsersServiceTests
    {
        private List<ApplicationUser> users = new List<ApplicationUser>()
        {
            new ApplicationUser()
            {
                UserName = "andrey",
                Email = "andrey@mail.com",
            },
            new ApplicationUser()
            {
                UserName = "helen",
                Email = "helen@mail.com",
            },
        };

        private IMapper mapper = null!;
        private IFake<UserManager<ApplicationUser>> userManagerFake = null!;
        private Mock<SignInManager<ApplicationUser>> signInManagerMock = null!;
        private Mock<IUserRolesService<ApplicationUser, ApplicationRole>> userRolesServiceMock = null!;
        private Mock<ITokensService> tokensServiceMock = null!;

        private UsersService<ApplicationUser, ApplicationRole> usersService = null!;

        /// <summary>
        /// This method is called before running every test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            AutoMapperConfig.RegisterMappings(typeof(UserDto).Assembly);
            this.mapper = AutoMapperConfig.MapperInstance;

            this.userManagerFake = new UserManagerFake<ApplicationUser>(this.users);

            this.signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                this.userManagerFake.Instance,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null!,
                null!,
                null!,
                null!);

            this.userRolesServiceMock = new Mock<IUserRolesService<ApplicationUser, ApplicationRole>>();

            this.userRolesServiceMock
                .Setup(userRolesService => userRolesService.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(ServiceResult.Successful);

            this.tokensServiceMock = new Mock<ITokensService>();

            this.usersService = new UsersService<ApplicationUser, ApplicationRole>(
                this.mapper,
                this.userManagerFake.Instance,
                this.signInManagerMock.Object,
                this.userRolesServiceMock.Object,
                this.tokensServiceMock.Object);
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.GetAllUsersAsync"/>
        /// returns all users as dtos.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task GetAllUsersAsync_UsersCollectionNotEmpty_ReturnsAllUsersAsDtos()
        {
            // Arrange
            // Act
            var usersResult = await this.usersService.GetAllUsersAsync();

            // Assert
            this.AssertResultIsSuccessful(usersResult);

            var userDtos = this.users.Select(u => this.mapper.Map<UserDto>(u)).ToList();

            Assert.That(usersResult.Data, Is.EquivalentTo(userDtos)
                .Using<UserDto>((x, y) => x.Id == y.Id));
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.RegisterWithEmailAndUsernameAsync(UserRegisterDto, IEnumerable{string}?)"/>
        /// returns <see cref="BadRequestDetails"/> whenever the user register dto is not valid.
        /// </summary>
        /// <param name="username">The username used for creating the user register dto.</param>
        /// <param name="email">The email used for creating the user register dto.</param>
        /// <param name="password">The password used for creating the user register dto.</param>
        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        [TestCase("", "tony@mail.com", "Ton1Rules!")] // Username is not valid
        [TestCase("tony", "", "Ton1Rules!")] // Email is not valid
        [TestCase("tony", "tony@mail.com", "")] // Password is not valid
        public async Task RegisterWithEmailAndUsernameAsync_UserRegisterDtoIsNotValid_ReturnsBadRequestDetails(string username, string email, string password)
        {
            // Arrange
            var userRegisterDto = new UserRegisterDto()
            {
                Username = username,
                Email = email,
                Password = password,
            };

            // Act
            var registerResult = await this.usersService.RegisterWithEmailAndUsernameAsync(userRegisterDto);

            // Assert
            this.AssertResultIsFailed(registerResult);
            Assert.That(registerResult.Failure, Is.InstanceOf<BadRequestDetails>());
            Assert.That(registerResult.Data, Is.Null);
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.RegisterWithEmailAndUsernameAsync(UserRegisterDto, IEnumerable{string}?)"/>
        /// returns <see cref="BadRequestDetails"/> whenever the user creation fails.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task RegisterWithEmailAndUsernameAsync_UserCreatingFails_ReturnsBadRequestDetails()
        {
            // Arrange
            var userRegisterDto = new UserRegisterDto()
            {
                Username = "tony",
                Email = "tony@mail.com",
                Password = "Ton1Rules!",
            };

            this.userManagerFake.Mock
                .Setup(userManager => userManager.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            var registerResult = await this.usersService.RegisterWithEmailAndUsernameAsync(userRegisterDto);

            // Assert
            this.AssertResultIsFailed(registerResult);
            Assert.That(registerResult.Failure, Is.InstanceOf<BadRequestDetails>());
            Assert.That(registerResult.Data, Is.Null);
        }

        /// <summary>
        /// This testh checks whether <see cref="UsersService{TUser, TRole}.RegisterWithEmailAndUsernameAsync(UserRegisterDto, IEnumerable{string}?)"/>
        /// calls the <see cref="UserRolesService{TUser, TRole}.AddToRoleAsync(TUser, string)"/> with the correct arguments.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task RegisterWithEmailAndUsernameAsync_UserHasRoles_CallsAddToRoleAsync()
        {
            // Arrange
            var userRegisterDto = new UserRegisterDto()
            {
                Username = "tony",
                Email = "tony@mail.com",
                Password = "Ton1Rules!",
            };

            string[] roles = [ "Admin", "Owner"];

            // Act
            _ = await this.usersService.RegisterWithEmailAndUsernameAsync(userRegisterDto, roles);

            // Assert
            foreach (var role in roles)
            {
                this.userRolesServiceMock
                    .Verify(
                        userRolesService => userRolesService.AddToRoleAsync(It.IsAny<ApplicationUser>(), role),
                        $"User roles service AddToRoleAsync method should be called with {role} role.");
            }
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.RegisterWithEmailAndUsernameAsync(UserRegisterDto, IEnumerable{string}?)"/>
        /// calls the <see cref="UserManager{TUser}.CreateAsync(TUser)"/> once and returns the dto of the registered user.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task RegisterWithEmailAndUsernameAsync_UserRegisterDtoIsValid_ReturnsTheRegisteredUserDto()
        {
            // Arrange
            var userRegisterDto = new UserRegisterDto()
            {
                Username = "tony",
                Email = "tony@mail.com",
                Password = "Ton1Rules!",
            };

            string[] roles = [ "Admnin" ];

            // Act
            var registerResult = await this.usersService.RegisterWithEmailAndUsernameAsync(userRegisterDto, roles);

            // Assert
            this.AssertResultIsSuccessful(registerResult);

            this.userManagerFake.Mock
                .Verify(userManager => userManager.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);

            Assert.That(registerResult.Data!.UserName, Is.EqualTo(userRegisterDto.Username));
            Assert.That(registerResult.Data!.Email, Is.EqualTo(userRegisterDto.Email));
            Assert.That(registerResult.Data!.Roles, Is.EquivalentTo(roles));
        }

        public async Task UpdateUserAsync_UserDtoIsNotValid_ReturnsBadRequestDetails()
        {
            // TODO
            Assert.Pass();
        }

        public async Task UpdateUserAsync_UserDoesNotExist_ReturnsNotFoundDetails()
        {
            // TODO
            Assert.Pass();
        }

        public async Task UpdateUserAsync_UserDtoHasDifferentRoles_UpdatesUserRoles()
        {
            // TODO
            Assert.Pass();
        }

        public async Task UpdateUserAsync_UserDtoIsValid_UpdatesAndReturnsUpdatedUser()
        {
            // TODO
            Assert.Pass();
        }

        public async Task LoginWithEmailAsync_UserDoesNotExist_ReturnsNotFoundDetails()
        {
            // TODO
            Assert.Pass();
        }

        public async Task LoginWithEmailAsync_PasswordIsNotCorrect_ReturnsUnauthorizedDetails()
        {
            // TODO
            Assert.Pass();
        }

        public async Task LoginWithEmailAsync_CredentialAreCorrect_SignsAndReturnsUser()
        {
            // TODO
            Assert.Pass();
        }

        public async Task LoginWithUsernameAsync_UserDoesNotExist_ReturnsNotFoundDetails()
        {
            // TODO
            Assert.Pass();
        }

        public async Task LoginWithUsernameAsync_PasswordIsNotCorrect_ReturnsUnauthorizedDetails()
        {
            // TODO
            Assert.Pass();
        }

        public async Task LoginWithUsernameAsync_CredentialAreCorrect_SignsAndReturnsUser()
        {
            // TODO
            Assert.Pass();
        }

        public async Task LogoutAsync_AccessTokenIsNotNull_RevokesTokenAndSignsOut()
        {
            // TODO
            Assert.Pass();
        }

        public async Task IsSignedIn_UserIsSignedIn_ReturnsTrue()
        {
            // TODO
            Assert.Pass();
        }

        public async Task IsSignedIn_UserIsNotSignedIn_ReturnsFalse()
        {
            // TODO
            Assert.Pass();
        }

        public async Task FindUserByEmailAsync_UserDoesNotExist_ReturnsNotFoundDetails()
        {
            // TODO
            Assert.Pass();
        }

        public async Task FindUserByEmailAsync_UserExist_ReturnsUserAsDto()
        {
            // TODO
            Assert.Pass();
        }

        public async Task FindUserByUsernameAsync_UserDoesNotExist_ReturnsNotFoundDetails()
        {
            // TODO
            Assert.Pass();
        }

        public async Task FindUserByUsernameAsync_UserExist_ReturnsUserAsDto()
        {
            // TODO
            Assert.Pass();
        }

        public async Task FindUserByIdAsync_UserDoesNotExist_ReturnsNotFoundDetails()
        {
            // TODO
            Assert.Pass();
        }

        public async Task FindUserByIdAsync_UserExist_ReturnsUserAsDto()
        {
            // TODO
            Assert.Pass();
        }

        public async Task DeleteUserWithId_UserDoesNotExist_ReturnsNotFoundDetails()
        {
            // TODO
            Assert.Pass();
        }

        public async Task DeleteUserWithId_UserExist_ReturnsSuccessfulResult()
        {
            // TODO
            Assert.Pass();
        }

        public async Task ExistsAsync_UserExists_ReturnsTrue()
        {
            // TODO
            Assert.Pass();
        }

        public async Task ExistsAsync_UserDoesntExists_ReturnsFalse()
        {
            // TODO
            Assert.Pass();
        }

        private void AssertResultIsSuccessful<TFailure>(IResult<TFailure> result)
            where TFailure : class
        {
            Assert.That(result.IsSuccessful, Is.True, "Result should be successful.");
            Assert.That(result.IsFailed, Is.False, "Result should not be failed.");
        }

        private void AssertResultIsFailed<TFailure>(IResult<TFailure> result)
            where TFailure : class
        {
            Assert.That(result.IsFailed, Is.True, "Result should be failed.");
            Assert.That(result.IsSuccessful, Is.False, "Result should not be successful.");
        }
    }
}
