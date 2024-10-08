namespace MyKitchen.Microservices.Identity.Services.Users.Contracts
{
    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Common.ServiceResult;

    public interface IUserRolesService<TUser, TRole>
        where TUser : ApplicationUser, new()
        where TRole : ApplicationRole, new()
    {
        public Task<ServiceResult<IEnumerable<string>>> GetAllRolesAsync();

        public Task<ServiceResult<IEnumerable<string>>> GetUserRolesAsync(TUser user);

        public Task<ServiceResult> AddToRoleAsync(TUser user, string role);

        public Task<ServiceResult> RemoveFromRoleAsync(TUser user, string role);

        public Task<ServiceResult<bool>> DoesRoleExistAsync(string role);
    }
}
