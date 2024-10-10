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
    using MyKitchen.Microservices.Identity.Services.Common.ServiceResult;
    using MyKitchen.Microservices.Identity.Services.Users.Contracts;
    using MyKitchen.Microservices.Identity.Services.Users.Dtos.User;

    public class UsersService<TUser, TRole> : IUsersService<TUser, TRole>
        where TUser : ApplicationUser, new()
        where TRole : ApplicationRole, new()
    {
        private readonly IMapper mapper;
        private readonly UserManager<TUser> userManager;
        private readonly SignInManager<TUser> signInManager;
        private readonly IUserRolesService<TUser, TRole> userRolesService;

        public UsersService(
            IMapper mapper,
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            IUserRolesService<TUser, TRole> userRolesService)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.userRolesService = userRolesService;
        }

        public async Task<ServiceResult<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            return await this.userManager.Users
                .ProjectTo<UserDto>(this.mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ServiceResult<TUser>> RegisterWithEmailAndUsernameAsync(UserRegisterDto userRegisterDto, IEnumerable<string>? roles = null)
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
                user = (await this.FindUserByEmailAsync(userRegisterDto.Email)).Data!;

                foreach (var role in roles)
                {
                    await this.userRolesService.AddToRoleAsync(user, role);
                }
            }

            return user;
        }

        public async Task<ServiceResult> UpdateUserAsync(UserDto userDto)
        {
            var isUserValid = this.ValidateDto(userDto);

            if (isUserValid == false)
            {
                return new BadRequestDetails(string.Format(ExceptionMessages.InvalidModelState, nameof(userDto)));
            }

            var findResult = await this.FindUserByIdAsync(userDto.Id.ToString());

            if (!findResult.Succeed)
            {
                return new(new NotFoundDetails(string.Format(ExceptionMessages.NoEntityWithPropertyFound, nameof(userDto), nameof(userDto.Id))));
            }

            var user = findResult.Data!;

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

            await this.userManager.UpdateAsync(user);
            return ServiceResult.Successful;
        }

        public async Task<ServiceResult<TUser>> LoginWithEmailAsync(UserLoginDto userLoginDto, bool isPersistant = false, bool isLockout = true)
        {
            var findResult = await this.FindUserByEmailAsync(userLoginDto.Email);

            var signInResult = await findResult.BindAsync<ServiceResult<SignInResult>, SignInResult>(async user =>
            {
                var signInResult = await this.signInManager.PasswordSignInAsync(user, userLoginDto.Password, isPersistant, isLockout);

                return signInResult.Succeeded ? signInResult : new UnauthorizedDetails(string.Format(ExceptionMessages.Unauthorized));
            });

            findResult.DependOn(signInResult);
            return findResult;
        }

        public async Task<ServiceResult<TUser>> LoginWithUsernameAsync(string username, string password, bool isPersistant = false, bool isLockout = true)
        {
            var findResult = await this.FindUserByUsernameAsync(username);

            var signInResult = await findResult.BindAsync<ServiceResult<SignInResult>, SignInResult>(async user =>
            {
                var signInResult = await this.signInManager.PasswordSignInAsync(user, password, isPersistant, isLockout);

                return signInResult.Succeeded ? signInResult : new UnauthorizedDetails(string.Format(ExceptionMessages.Unauthorized));
            });

            findResult.DependOn(signInResult);
            return findResult;
        }

        public async Task<ServiceResult> LogoutAsync()
        {
            await this.signInManager.SignOutAsync();
            return ServiceResult.Successful;
        }

        public ServiceResult<bool> IsSignedIn(TUser user)
        {
            if (user == null)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(user)));
            }

            var principal = user as ClaimsPrincipal;

            return this.signInManager.IsSignedIn(principal!);
        }

        public async Task<ServiceResult<TUser>> FindUserByEmailAsync(string email)
        {
            var user = await this.userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(user)));
            }

            return user;
        }

        public async Task<ServiceResult<TUser>> FindUserByUsernameAsync(string username)
        {
            var user = await this.userManager.FindByNameAsync(username);

            if (user == null)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(user)));
            }

            return user;
        }

        public async Task<ServiceResult<TUser>> FindUserByIdAsync(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);

            if (user == null)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(user)));
            }

            return user;
        }

        public Task<ServiceResult<IdentityResult>> DeleteUserWithId(string id)
        {
            throw new NotImplementedException();
        }

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
    }
}
