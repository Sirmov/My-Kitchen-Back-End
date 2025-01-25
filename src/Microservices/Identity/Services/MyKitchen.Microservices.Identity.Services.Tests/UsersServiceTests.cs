// |-----------------------------------------------------------------------------------------------------|
// <copyright file="UsersServiceTests.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Tests
{
    using System.Security.Claims;

    using AutoMapper;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;

    using MongoDB.Bson;

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
        private readonly List<ApplicationUser> users = new List<ApplicationUser>()
        {
            new ApplicationUser()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserName = "andrey",
                Email = "andrey@mail.com",
                Roles = [ "Admin "],
            },
            new ApplicationUser()
            {
                Id = ObjectId.GenerateNewId().ToString(),
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

            this.userRolesServiceMock
                .Setup(userRolesService => userRolesService.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(ServiceResult.Successful);

            this.userRolesServiceMock
                .Setup(userRolesService => userRolesService.GetUserRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync((ApplicationUser user) => new List<string>(user.Roles));

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
            Assert.That(registerResult.Failure, Is.TypeOf<BadRequestDetails>());
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
            Assert.That(registerResult.Failure, Is.TypeOf<BadRequestDetails>());
            Assert.That(registerResult.Data, Is.Null);
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.RegisterWithEmailAndUsernameAsync(UserRegisterDto, IEnumerable{string}?)"/>
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

            string [] roles = ["Admin", "Owner"];

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

            string [] roles = ["Admin"];

            // Act
            var registerResult = await this.usersService.RegisterWithEmailAndUsernameAsync(userRegisterDto, roles);

            // Assert
            this.AssertResultIsSuccessful(registerResult);

            this.userManagerFake.Mock
                .Verify(userManager => userManager.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);

            Assert.That(registerResult.Data!.Username, Is.EqualTo(userRegisterDto.Username));
            Assert.That(registerResult.Data!.Email, Is.EqualTo(userRegisterDto.Email));
            Assert.That(registerResult.Data!.Roles, Is.EquivalentTo(roles));
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.UpdateUserAsync(UserDto)"/>
        /// returns <see cref="BadRequestDetails"/> whenever the user dto is not valid.
        /// </summary>
        /// <param name="username">The username of the user dto.</param>
        /// <param name="email">The email of the  user dto.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        [TestCase("", "tony@mail.com")] // Username is not valid
        [TestCase("tony", "")] // Email is not valid
        public async Task UpdateUserAsync_UserDtoIsNotValid_ReturnsBadRequestDetails(string username, string email)
        {
            // Arrange
            var userDto = new UserDto()
            {
                Username = username,
                Email = email,
            };

            // Act
            var updateResult = await this.usersService.UpdateUserAsync(userDto);

            // Assert
            this.AssertResultIsFailed(updateResult);
            Assert.That(updateResult.Failure, Is.TypeOf<BadRequestDetails>());
        }

        /// <summary>
        /// This test checks whether the <see cref="UsersService{TUser, TRole}.UpdateUserAsync(UserDto)"/>
        /// returns a <see cref="NotFoundDetails"/> whenever the user is not found.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task UpdateUserAsync_UserDoesNotExist_ReturnsNotFoundDetails()
        {
            // Arrange
            var userDto = new UserDto()
            {
                Username = "tony",
                Email = "tony@mail.com",
            };

            // Act
            var updateResult = await this.usersService.UpdateUserAsync(userDto);

            // Assert
            this.AssertResultIsFailed(updateResult);
            Assert.That(updateResult.Failure, Is.TypeOf<NotFoundDetails>());
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.UpdateUserAsync(UserDto)"/>
        /// correctly updates the roles of the user. Removing those that are missing from the updated dto
        /// and adding those that are missing from the user.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task UpdateUserAsync_UserDtoHasDifferentRoles_UpdatesUserRoles()
        {
            // Arrange
            var user = this.users [0];
            string [] updatedRoles = ["Administrator"];
            var userDto = this.mapper.Map<UserDto>(user);

            this.userRolesServiceMock
                .Setup(userRolesService => userRolesService.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback((ApplicationUser user, string role) => user.Roles.Add(role))
                .ReturnsAsync(ServiceResult.Successful);

            this.userRolesServiceMock
                .Setup(userRolesService => userRolesService.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback((ApplicationUser user, string role) => user.Roles.Remove(role))
                .ReturnsAsync(ServiceResult.Successful);

            // Act
            userDto.Roles = updatedRoles;
            var updateResult = await this.usersService.UpdateUserAsync(userDto);

            // Assert
            this.AssertResultIsSuccessful(updateResult);

            var updatedUser = await this.userManagerFake.Instance.FindByIdAsync(userDto.Id.ToString());
            Assert.That(updatedUser!.Roles, Is.EquivalentTo(updatedRoles));
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.UpdateUserAsync(UserDto)"/>
        /// updates the user successfully.
        /// </summary>
        /// <param name="username">The updated username.</param>
        /// <param name="email">The updated email.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        [TestCase("george", "george@mail.com")]
        public async Task UpdateUserAsync_UserDtoIsValid_UpdatesUserSuccessfully(string username, string email)
        {
            // Arrange
            var user = this.users [0];
            var userDto = this.mapper.Map<UserDto>(user);

            // Act
            userDto.Username = username;
            userDto.Email = email;
            var updateResult = await this.usersService.UpdateUserAsync(userDto);

            // Assert
            this.AssertResultIsSuccessful(updateResult);

            var updatedUser = await this.userManagerFake.Instance.FindByIdAsync(userDto.Id.ToString());
            Assert.That(updatedUser!.UserName, Is.EquivalentTo(username));
            Assert.That(updatedUser!.Email, Is.EquivalentTo(email));
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.LoginWithEmailAsync(string, string, bool, bool)"/>
        /// and <see cref="UsersService{TUser, TRole}.LoginWithUsernameAsync(string, string, bool, bool)"/> return
        /// <see cref="NotFoundDetails"/> whenever no user with the provided identificator exist.
        /// </summary>
        /// <param name="withEmail">A boolean indicating whether to test login with email.</param>
        /// <param name="withUsername">A boolean indicating whether to test login with username.</param>
        /// <returns>Returns a <see cref="Task"/> indicating the asynchronous operation.</returns>
        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task LoginWithEmailAsync_LoginWithUsernameAsync_UserDoesNotExist_ReturnsNotFoundDetails(bool withEmail, bool withUsername)
        {
            // Arrange
            Func<string, string, bool, bool, Task<ServiceResult<(string, string)>>> loginMethodAsync = null!;

            loginMethodAsync = withEmail ? this.usersService.LoginWithEmailAsync : loginMethodAsync;
            loginMethodAsync = withUsername ? this.usersService.LoginWithUsernameAsync : loginMethodAsync;

            Assert.That(loginMethodAsync, Is.Not.Null, "No login method selected for testing.");

            // Act
            var loginResult = await loginMethodAsync(string.Empty, string.Empty, false, false);

            // Assert
            this.AssertResultIsFailed(loginResult);
            Assert.That(loginResult.Failure, Is.TypeOf<NotFoundDetails>());
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.LoginWithEmailAsync(string, string, bool, bool)"/>
        /// and <see cref="UsersService{TUser, TRole}.LoginWithUsernameAsync(string, string, bool, bool)"/> return
        /// <see cref="UnauthorizedDetails"/> whenever the login credential are not correct.
        /// </summary>
        /// <param name="withEmail">A boolean indicating whether to test login with email.</param>
        /// <param name="withUsername">A boolean indicating whether to test login with username.</param>
        /// <returns>Returns a <see cref="Task"/> indicating the asynchronous operation.</returns>
        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task LoginWithEmailAsync_LoginWithUsernameAsync_PasswordIsNotCorrect_ReturnsUnauthorizedDetails(bool withEmail, bool withUsername)
        {
            // Arrange
            Func<string, string, bool, bool, Task<ServiceResult<(string, string)>>> loginMethodAsync = null!;
            string userIdentificator = string.Empty;

            if (withEmail)
            {
                loginMethodAsync = this.usersService.LoginWithEmailAsync;
                userIdentificator = this.users.First().Email!;
            }
            else if (withUsername)
            {
                loginMethodAsync = this.usersService.LoginWithUsernameAsync;
                userIdentificator = this.users.First().UserName!;
            }

            Assert.That(loginMethodAsync, Is.Not.Null, "No login method selected for testing.");

            this.signInManagerMock
                .Setup(signInManger => signInManger.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var loginResult = await loginMethodAsync(userIdentificator, string.Empty, false, false);

            // Assert
            this.AssertResultIsFailed(loginResult);
            Assert.That(loginResult.Failure, Is.TypeOf<UnauthorizedDetails>());
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.LoginWithEmailAsync(string, string, bool, bool)"/>
        /// and <see cref="UsersService{TUser, TRole}.LoginWithUsernameAsync(string, string, bool, bool)"/> return
        /// a tuple with the access and refresh tokens when the credential are correct..
        /// </summary>
        /// <param name="withEmail">A boolean indicating whether to test login with email.</param>
        /// <param name="withUsername">A boolean indicating whether to test login with username.</param>
        /// <returns>Returns a <see cref="Task"/> indicating the asynchronous operation.</returns>
        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task LoginWithEmailAsync_LoginWithUsernameAsync_CredentialAreCorrect_SignsAndReturnsAccessAndRefreshTokens(bool withEmail, bool withUsername)
        {
            // Arrange
            Func<string, string, bool, bool, Task<ServiceResult<(string, string)>>> loginMethodAsync = null!;
            string userIdentificator = string.Empty;

            if (withEmail)
            {
                loginMethodAsync = this.usersService.LoginWithEmailAsync;
                userIdentificator = this.users.First().Email!;
            }
            else if (withUsername)
            {
                loginMethodAsync = this.usersService.LoginWithUsernameAsync;
                userIdentificator = this.users.First().UserName!;
            }

            Assert.That(loginMethodAsync, Is.Not.Null, "No login method selected for testing.");

            this.signInManagerMock
                .Setup(signInManger => signInManger.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            string accessToken = nameof(accessToken);
            this.tokensServiceMock
                .Setup(tokensService => tokensService.GenerateAccessToken(It.IsAny<UserDto>()))
                .Returns(accessToken);

            string refreshToken = nameof(refreshToken);
            this.tokensServiceMock
                .Setup(tokensService => tokensService.GenerateRefreshToken(It.IsAny<UserDto>()))
                .Returns(refreshToken);

            // Act
            var loginResult = await loginMethodAsync(userIdentificator, string.Empty, false, false);

            // Assert
            this.AssertResultIsSuccessful(loginResult);

            Assert.That(loginResult.Data.Item1, Is.EqualTo(accessToken));
            Assert.That(loginResult.Data.Item2, Is.EqualTo(refreshToken));
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.LogoutAsync(string)"/>
        /// revokes the access token of the user trying to logout and signs him out.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task LogoutAsync_AccessTokenIsNotNull_RevokesTokenAndSignsOut()
        {
            // Arrange
            string accessToken = nameof(accessToken);

            // Act
            var logoutResult = await this.usersService.LogoutAsync(accessToken);

            // Assert
            this.AssertResultIsSuccessful(logoutResult);
            this.tokensServiceMock.Verify(tokensService => tokensService.RevokeTokenAsync(accessToken), Times.Once);
            this.signInManagerMock.Verify(signInManager => signInManager.SignOutAsync(), Times.Once);
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.IsSignedIn(TUser)"/>
        /// returns <see langword="true"/> whenever the provided user is signed in and
        /// <see cref="false"/> whenever he is not.
        /// </summary>
        /// <param name="isUserSignedIn">A boolean indicating whether the user is signed in or not.</param>
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsSignedIn_IsUserSignedInFlag_ReturnsFlag(bool isUserSignedIn)
        {
            // Arrange
            this.signInManagerMock
                .Setup(signInManager => signInManager.IsSignedIn(It.IsAny<ClaimsPrincipal>()))
                .Returns(isUserSignedIn);

            // Act
            var isSignedInResult = this.usersService.IsSignedIn(this.users.First());

            // Assert
            this.AssertResultIsSuccessful(isSignedInResult);
            Assert.That(isSignedInResult.Data, Is.EqualTo(isUserSignedIn));
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.IsSignedIn(TUser)"/>
        /// returns <see cref="NotFoundDetails"/> when the provided user is <see langword="null"/>.
        /// </summary>
        [Test]
        public void IsSignedIn_UserIsNull_ReturnsNotFoundDetails()
        {
            // Arrange
            // Act
            var isSignedInResult = this.usersService.IsSignedIn(null!);

            // Assert
            this.AssertResultIsFailed(isSignedInResult);
            Assert.That(isSignedInResult.Failure, Is.TypeOf<NotFoundDetails>());
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.FindUserByEmailAsync(string)"/>,
        /// <see cref="UsersService{TUser, TRole}.FindUserByUsernameAsync(string)"/> and
        /// <see cref="UsersService{TUser, TRole}.FindUserByIdAsync(string)"/> return <see cref="NotFoundDetails"/>
        /// whenever no user with the provided identifier exists.
        /// </summary>
        /// <param name="byEmail">A boolean indicating whether to test finding by email.</param>
        /// <param name="byUsername">A boolean indicating whether to test finding by username.</param>
        /// <param name="byId">A boolean indicating whether to test finding by id.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        public async Task FindUserByEmailAsync_FindUserByUsernameAsync_FindUserByIdAsync_UserDoesNotExist_ReturnsNotFoundDetails(bool byEmail, bool byUsername, bool byId)
        {
            // Arrange
            Func<string, Task<ServiceResult<UserDto>>> findMethodAsync = null!;

            findMethodAsync = byEmail ? this.usersService.FindUserByEmailAsync : findMethodAsync;
            findMethodAsync = byUsername ? this.usersService.FindUserByUsernameAsync : findMethodAsync;
            findMethodAsync = byId ? this.usersService.FindUserByIdAsync : findMethodAsync;

            Assert.That(findMethodAsync, Is.Not.Null, "No find method selected for testing.");

            // Act
            var findResult = await findMethodAsync(string.Empty);

            // Assert
            this.AssertResultIsFailed(findResult);
            Assert.That(findResult.Failure, Is.TypeOf<NotFoundDetails>());
            Assert.That(findResult.Data, Is.Null);
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.FindUserByEmailAsync(string)"/>,
        /// <see cref="UsersService{TUser, TRole}.FindUserByUsernameAsync(string)"/> and
        /// <see cref="UsersService{TUser, TRole}.FindUserByIdAsync(string)"/> return the user dto
        /// of the existing user.
        /// </summary>
        /// <param name="byEmail">A boolean indicating whether to test finding by email.</param>
        /// <param name="byUsername">A boolean indicating whether to test finding by username.</param>
        /// <param name="byId">A boolean indicating whether to test finding by id.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        public async Task FindUserByEmailAsync_UserExist_ReturnsUserAsDto(bool byEmail, bool byUsername, bool byId)
        {
            // Arrange
            Func<string, Task<ServiceResult<UserDto>>> findMethodAsync = null!;
            string userIdentificator = string.Empty;

            if (byEmail)
            {
                findMethodAsync = this.usersService.FindUserByEmailAsync;
                userIdentificator = this.users.First().Email!;
            }
            else if (byUsername)
            {
                findMethodAsync = this.usersService.FindUserByUsernameAsync;
                userIdentificator = this.users.First().UserName!;
            }
            else if (byId)
            {
                findMethodAsync = this.usersService.FindUserByIdAsync;
                userIdentificator = this.users.First().Id.ToString();
            }

            Assert.That(findMethodAsync, Is.Not.Null, "No find method selected for testing.");

            // Act
            var findResult = await findMethodAsync(userIdentificator);

            // Assert
            this.AssertResultIsSuccessful(findResult);
            Assert.That(findResult.Data!.Id, Is.EqualTo(this.users.First().Id));
            Assert.That(findResult.Data.Email, Is.EqualTo(this.users.First().Email));
            Assert.That(findResult.Data.Username, Is.EqualTo(this.users.First().UserName));
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.DeleteUserWithIdAsync(string)"/>
        /// returns a successful result when a user with the provided identificator exists.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task DeleteUserWithId_UserExist_ReturnsSuccessfulResult()
        {
            // Arrange
            var userIdentificator = this.users.First().Id.ToString();

            // Act
            var deleteResult = await this.usersService.DeleteUserWithIdAsync(userIdentificator);

            // Assert
            this.AssertResultIsSuccessful(deleteResult);
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.DeleteUserWithIdAsync(string)"/>
        /// returns <see cref="NotFoundDetails"/> whenever no user with the provided identificator was found.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task DeleteUserWithId_UserDoesNotExist_ReturnsNotFoundDetails()
        {
            // Arrange
            // Act
            var deleteResult = await this.usersService.DeleteUserWithIdAsync(string.Empty);

            // Assert
            this.AssertResultIsFailed(deleteResult);
            Assert.That(deleteResult.Failure, Is.TypeOf<NotFoundDetails>());
        }

        /// <summary>
        /// This test checks whether <see cref="UsersService{TUser, TRole}.ExistsAsync(string)"/>
        /// returns <see langword="true"/> whenever a user with the provided id exists and
        /// <see langword="false"/> otherwise.
        /// </summary>
        /// <param name="doesUserExist">A boolean indicating whether the user exists.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task ExistsAsync_DoesUserExistFlag_ReturnsFlag(bool doesUserExist)
        {
            // Arrange
            var userIdentificator = doesUserExist ? this.users.First().Id.ToString() : string.Empty;

            // Act
            var existsResult = await this.usersService.ExistsAsync(userIdentificator);

            // Assert
            this.AssertResultIsSuccessful(existsResult);
            Assert.That(existsResult.Data, Is.EqualTo(doesUserExist));
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
