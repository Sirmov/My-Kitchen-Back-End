// |-----------------------------------------------------------------------------------------------------|
// <copyright file="UsersController.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Api.Rest.Controllers
{
    using System.Net.Mime;

    using AutoMapper;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using MyKitchen.Common.ProblemDetails;
    using MyKitchen.Microservices.Identity.Api.Common.Constants;
    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Common.Dtos.User;
    using MyKitchen.Microservices.Identity.Services.Tokens.Contracts;
    using MyKitchen.Microservices.Identity.Services.Users.Contracts;
    using MyKitchen.Microservices.Identity.Services.Users.Dtos.User;

    /// <summary>
    /// This controller is responsible for the operations regarding the users and their identity.
    /// </summary>
    [Route(RouteConstants.Users.BaseRoute)]
    public class UsersController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IUsersService<ApplicationUser, ApplicationRole> usersService;
        private readonly IUserRolesService<ApplicationUser, ApplicationRole> userRolesService;
        private readonly ITokensService tokensService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="mapper">The implementation of <see cref="IMapper"/>.</param>
        /// <param name="usersService">The implementation of <see cref="IUsersService{TUser, TRole}"/>.</param>
        /// <param name="userRolesService">The implementation of <see cref="IUserRolesService{TUser, TRole}"/>.</param>
        /// <param name="tokensService">The implementation of <see cref="ITokensService"/>.</param>
        public UsersController(
            IMapper mapper,
            IUsersService<ApplicationUser, ApplicationRole> usersService,
            IUserRolesService<ApplicationUser, ApplicationRole> userRolesService,
            ITokensService tokensService)
        {
            this.mapper = mapper;
            this.usersService = usersService;
            this.userRolesService = userRolesService;
            this.tokensService = tokensService;
        }

        /// <summary>
        /// This action is responsible for handling the user registration.
        /// </summary>
        /// <param name="user">The <see cref="UserRegisterDto"/> to be registered.</param>
        /// <returns>Returns a <see cref="UserDto"/> when the registration is successful.</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route(RouteConstants.Users.RegisterEndpoint)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created, MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(BadRequestDetails), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto user)
        {
            var registerResult = await this.usersService.RegisterWithEmailAndUsernameAsync(user);

            return registerResult.ToActionResult(userDto => this.StatusCode(201, registerResult.Data));
        }

        /// <summary>
        /// This action is responsible for handling the login of a user.
        /// </summary>
        /// <param name="loginDto">The user login credentials.</param>
        /// <returns>Returns a <see langword="string"/> representing a JWT access token.</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route(RouteConstants.Users.LoginEndpoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(BadRequestDetails), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UnauthorizedDetails), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            var authenticationResult = await this.usersService.LoginWithEmailAsync(loginDto.Email, loginDto.Password);

            return authenticationResult.ToActionResult((tokensTuple) =>
                this.Ok(new { AccessToken = tokensTuple.accessToken, RefreshToken = tokensTuple.refreshToken }));
        }

        /// <summary>
        /// This action is responsible for handling the logout of a user.
        /// </summary>
        /// <returns>Returns a empty response.</returns>
        [HttpGet]
        [Route(RouteConstants.Users.LogoutEndpoint)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout()
        {
            var accessToken = this.HttpContext.Items ["AccessToken"].ToString();
            var logoutResult = await this.usersService.LogoutAsync(accessToken!);

            return logoutResult.ToActionResult(_ => this.NoContent());
        }
    }
}
