// |-----------------------------------------------------------------------------------------------------|
// <copyright file="UsersService.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Users
{
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using MyKitchen.Common.Constants;
    using MyKitchen.Common.ProblemDetails;
    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Common.Dtos.User;
    using MyKitchen.Microservices.Identity.Services.Common.ServiceResult;
    using MyKitchen.Microservices.Identity.Services.Tokens.Contracts;
    using MyKitchen.Microservices.Identity.Services.Users.Contracts;
    using MyKitchen.Microservices.Identity.Services.Users.Dtos.User;

     /// <summary>
    /// This class contains the business logic around the identity user.
    /// </summary>
    /// <inheritdoc cref="IUsersService{TUser, TRole}"/>
    public class UsersService<TUser, TRole> : IUsersService<TUser, TRole>
        where TUser : ApplicationUser, new()
        where TRole : ApplicationRole, new()
    {
        private readonly IMapper mapper;
        private readonly UserManager<TUser> userManager;
        private readonly SignInManager<TUser> signInManager;
        private readonly IUserRolesService<TUser, TRole> userRolesService;
        private readonly ITokensService tokensService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersService{TUser, TRole}"/> class.
        /// </summary>
        /// <param name="mapper">The implementation of <see cref="IMapper"/>.</param>
        /// <param name="userManager">The identity user manager.</param>
        /// <param name="signInManager">The identity sign in manager.</param>
        /// <param name="userRolesService">The implementation of <see cref="IUserRolesService{TUser, TRole}"/>.</param>
        /// <param name="tokensService">The implementation of <see cref="ITokensService"/>.</param>
        public UsersService(
            IMapper mapper,
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            IUserRolesService<TUser, TRole> userRolesService,
            ITokensService tokensService)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.userRolesService = userRolesService;
            this.tokensService = tokensService;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            return await this.userManager.Users
                .ProjectTo<UserDto>(this.mapper.ConfigurationProvider)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<UserDto>> RegisterWithEmailAndUsernameAsync(UserRegisterDto userRegisterDto, IEnumerable<string>? roles = null)
        {
            var isUserValid = this.ValidateDto(userRegisterDto);

            if (isUserValid == false)
            {
                return new BadRequestDetails(string.Format(ExceptionMessages.InvalidModelState, nameof(userRegisterDto)));
            }

            TUser user = this.CreateUserWithEmailAndUsername(userRegisterDto.Email, userRegisterDto.Username);
            var registerResult = await this.userManager.CreateAsync(user, userRegisterDto.Password);

            if (!registerResult.Succeeded)
            {
                var errors = registerResult.Errors.Select(ie => ie.Description);
                return new BadRequestDetails(string.Join(Environment.NewLine, errors));
            }

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    var addToRoleResult = await this.userRolesService.AddToRoleAsync(user, role);

                    if (addToRoleResult.IsSuccessful)
                    {
                        user.Roles.Add(role);
                    }
                }
            }

            return this.mapper.Map<UserDto>(user);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult> UpdateUserAsync(UserDto userDto)
        {
            var isUserValid = this.ValidateDto(userDto);

            if (isUserValid == false)
            {
                return new BadRequestDetails(string.Format(ExceptionMessages.InvalidModelState, nameof(userDto)));
            }

            var findResult = await this.FindUserByIdAsync(userDto.Id.ToString());

            if (!findResult.IsSuccessful)
            {
                return new (new NotFoundDetails(string.Format(ExceptionMessages.NoEntityWithPropertyFound, nameof(userDto), nameof(userDto.Id))));
            }

            var user = this.mapper.Map<TUser>(findResult.Data!);

            foreach (var role in userDto.Roles)
            {
                await this.userRolesService.AddToRoleAsync(user, role);
            }

            var userRoles = (await this.userRolesService.GetUserRolesAsync(user)).Data!;

            foreach (var userRole in userRoles)
            {
                if (!userDto.Roles.Contains(userRole))
                {
                    await this.userRolesService.RemoveFromRoleAsync(user, userRole);
                }
            }

            await this.userManager.UpdateAsync(this.mapper.Map<TUser>(userDto));
            return ServiceResult.Successful;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<(string accessToken, string refreshToken)>> LoginWithEmailAsync(string email, string password, bool isPersistent = false, bool isLockout = true)
        {
            var findResult = await this.FindUserByEmailAsync(email);

            if (findResult.IsFailed)
            {
                return findResult.Failure!;
            }

            return await this.LoginUserWithPasswordAsync(findResult.Data!, password, isPersistent, isLockout);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<(string accessToken, string refreshToken)>> LoginWithUsernameAsync(string username, string password, bool isPersistent = false, bool isLockout = true)
        {
            var findResult = await this.FindUserByUsernameAsync(username);

            if (findResult.IsFailed)
            {
                return findResult.Failure!;
            }

            return await this.LoginUserWithPasswordAsync(findResult.Data!, password, isPersistent, isLockout);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult> LogoutAsync(string accessTokenId)
        {
            await this.tokensService.RevokeTokenAsync(accessTokenId);
            await this.signInManager.SignOutAsync();
            return ServiceResult.Successful;
        }

        /// <inheritdoc/>
        public ServiceResult<bool> IsSignedIn(TUser user)
        {
            if (user == null)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(user)));
            }

            var principal = user as ClaimsPrincipal;

            return this.signInManager.IsSignedIn(principal!);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<UserDto>> FindUserByEmailAsync(string email)
        {
            var user = await this.userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(user)));
            }

            return this.mapper.Map<UserDto>(user);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<UserDto>> FindUserByUsernameAsync(string username)
        {
            var user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(user)));
            }

            return this.mapper.Map<UserDto>(user);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<UserDto>> FindUserByIdAsync(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);

            if (user == null)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(user)));
            }

            return this.mapper.Map<UserDto>(user);
        }

        /// <inheritdoc/>
        public async Task<ServiceResult> DeleteUserWithIdAsync(string userId)
        {
            var findResult = await this.FindUserByIdAsync(userId);

            if (findResult.IsFailed)
            {
                return findResult.Failure!;
            }

            // TODO:
            // 1. Mark user as deleted and anonymize
            // 2. Set changed password timestamp to utc now

            return ServiceResult.Successful;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<bool>> ExistsAsync(string id)
        {
            var findResult = await this.FindUserByIdAsync(id);
            return findResult.Match(s => true, f => false);
        }

        /// <summary>
        /// This method creates an instance of the <typeparamref name="TUser"/> class with the specified email and username.
        /// </summary>
        /// <param name="email">The email of the new user.</param>
        /// <param name="username">The username of the new user.</param>
        /// <returns>Returns the newly created user.</returns>
        private TUser CreateUserWithEmailAndUsername(string email, string username)
        {
            var user = Activator.CreateInstance<TUser>();
            user.Email = email;
            user.UserName = username;
            return user;
        }

        /// <summary>
        /// This method validates any class that has validation attributes.
        /// </summary>
        /// <typeparam name="TDto">The type of the dto to be validated.</typeparam>
        /// <param name="dto">The dto to be validated.</param>
        /// <returns>Returns <see langword="true"/> if the dto state is valid. Otherwise <see langword="false"/>.</returns>
        private bool ValidateDto<TDto>(TDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var context = new ValidationContext(dto, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, context, validationResults, true);

            return isValid;
        }

        private async Task<ServiceResult<(string accessToken, string refreshToken)>> LoginUserWithPasswordAsync(UserDto userDto, string password, bool isPersistent, bool isLockout)
        {
            var user = this.mapper.Map<TUser>(userDto);
            var signInResult = await this.signInManager.PasswordSignInAsync(user, password, isPersistent, isLockout);

            if (!signInResult.Succeeded)
            {
                return new UnauthorizedDetails(string.Format(ExceptionMessages.Unauthorized));
            }

            var accessTokenResult = this.tokensService.GenerateAccessToken(userDto);
            var refreshTokenResult = this.tokensService.GenerateRefreshToken(userDto);

            if (!accessTokenResult.IsSuccessful || !refreshTokenResult.IsSuccessful)
            {
                return new InternalServerErrorDetails(ExceptionMessages.InternalServerError);
            }

            return (accessTokenResult.Data!, refreshTokenResult.Data!);
        }
    }
}
