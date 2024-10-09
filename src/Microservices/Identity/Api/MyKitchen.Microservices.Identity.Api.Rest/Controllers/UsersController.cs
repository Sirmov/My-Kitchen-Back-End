namespace MyKitchen.Microservices.Identity.Api.Rest.Controllers
{
    using System.Net.Mime;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using MyKitchen.Common.ProblemDetails;
    using MyKitchen.Microservices.Identity.Api.Rest.Common.Constants;
    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Tokens.Contracts;
    using MyKitchen.Microservices.Identity.Services.Users.Contracts;
    using MyKitchen.Microservices.Identity.Services.Users.Dtos.User;

    [Route(RouteConstants.Users.BaseRoute)]
    public class UsersController : BaseController
    {
        private readonly IUsersService<ApplicationUser, ApplicationRole> usersService;
        private readonly IUserRolesService<ApplicationUser, ApplicationRole> userRolesService;
        private readonly ITokensService<ApplicationUser, ApplicationRole> tokensService;

        public UsersController(
            IUsersService<ApplicationUser, ApplicationRole> usersService,
            IUserRolesService<ApplicationUser, ApplicationRole> userRolesService,
            ITokensService<ApplicationUser, ApplicationRole> tokensService)
        {
            this.usersService = usersService;
            this.userRolesService = userRolesService;
            this.tokensService = tokensService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route(RouteConstants.Users.RegisterEndpoint)]
        [ProducesResponseType(typeof(ApplicationUser), StatusCodes.Status201Created, MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(BadRequestDetails), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto user)
        {
            var result = await this.usersService.RegisterWithEmailAndUsernameAsync(user);

            return result.ToActionResult(r => StatusCode(201, user));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route(RouteConstants.Users.LoginEndpoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(BadRequestDetails), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UnauthorizedDetails), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            var authenticationResult = await this.usersService.LoginWithEmailAsync(loginDto);

            return await authenticationResult.ToActionResult(async user =>
            {
                var token = await this.tokensService.GenerateAccessTokenAsync(user);

                return Ok(new { Token = token });
            });
        }

        [HttpGet]
        [Route(RouteConstants.Users.LogoutEndpoint)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout()
        {
            var logoutResult = await this.usersService.LogoutAsync();

            return logoutResult.ToActionResult(_ =>
            {
                return NoContent();
            });
        }
    }
}
