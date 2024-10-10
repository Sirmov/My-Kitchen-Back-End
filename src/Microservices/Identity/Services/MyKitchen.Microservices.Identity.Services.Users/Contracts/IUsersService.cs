namespace MyKitchen.Microservices.Identity.Services.Users.Contracts
{
    using Microsoft.AspNetCore.Identity;

    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Common.ServiceResult;
    using MyKitchen.Microservices.Identity.Services.Users.Dtos.User;

    public interface IUsersService<TUser, TRole>
        where TUser : ApplicationUser, new()
        where TRole : ApplicationRole, new()
    {
        public Task<ServiceResult<IEnumerable<UserDto>>> GetAllUsersAsync();

        public Task<ServiceResult<TUser>> RegisterWithEmailAndUsernameAsync(UserRegisterDto userRegisterDto, IEnumerable<string>? roles = null);

        public Task<ServiceResult> UpdateUserAsync(UserDto userDto);

        public Task<ServiceResult<TUser>> LoginWithEmailAsync(UserLoginDto userLoginDto, bool isPersistant = false, bool isLockout = true);

        public Task<ServiceResult<TUser>> LoginWithUsernameAsync(string username, string password, bool isPersistant = false, bool isLockout = true);

        public Task<ServiceResult> LogoutAsync();

        public ServiceResult<bool> IsSignedIn(TUser user);

        public Task<ServiceResult<TUser>> FindUserByEmailAsync(string email);

        public Task<ServiceResult<TUser>> FindUserByUsernameAsync(string username);

        public Task<ServiceResult<TUser>> FindUserByIdAsync(string id);

        public Task<ServiceResult<IdentityResult>> DeleteUserWithId(string id);

        public Task<ServiceResult<bool>> ExistsAsync(string id);
    }
}
