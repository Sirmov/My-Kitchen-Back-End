namespace MyKitchen.Microservices.Identity.Services.Users
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Common.ServiceResult;
    using MyKitchen.Microservices.Identity.Services.Users.Contracts;
    using MyKitchen.Common.Constants;
    using MyKitchen.Common.ProblemDetails;

    public class UserRolesService<TUser, TRole> : IUserRolesService<TUser, TRole>
        where TUser : ApplicationUser, new()
        where TRole : ApplicationRole, new()
    {
        private readonly UserManager<TUser> userManager;
        private readonly RoleManager<TRole> roleManager;

        public UserRolesService(
            UserManager<TUser> userManager,
            RoleManager<TRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<ServiceResult<IEnumerable<string>>> GetAllRolesAsync()
        {
            var roles = await this.roleManager.Roles
                .Select(r => r.Name ?? string.Empty)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .ToListAsync();

            return roles;
        }

        public async Task<ServiceResult<IEnumerable<string>>> GetUserRolesAsync(TUser user)
        {
            var roles = await this.userManager.GetRolesAsync(user);

            return roles.ToList();
        }

        public async Task<ServiceResult> AddToRoleAsync(TUser user, string role)
        {
            if (user == null)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(user)));
            }

            var roleExist = (await this.DoesRoleExistAsync(role)).Data;

            if (roleExist == false)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(role)));
            }

            var isInRole = await this.userManager.IsInRoleAsync(user, role);

            if (isInRole)
            {
                return new BadRequestDetails(ExceptionMessages.UserAlreadyInRole);
            }

            await this.userManager.AddToRoleAsync(user, role);
            return ServiceResult.Successful;
        }

        public async Task<ServiceResult> RemoveFromRoleAsync(TUser user, string role)
        {
            if (user == null)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(user)));
            }

            var roleExist = (await this.DoesRoleExistAsync(role)).Data;

            if (roleExist == false)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(role)));
            }

            var isInRole = await this.userManager.IsInRoleAsync(user, role);

            if (isInRole == false)
            {
                return new BadRequestDetails(ExceptionMessages.UserNotInRole);
            }

            await this.userManager.RemoveFromRoleAsync(user, role);
            return ServiceResult.Successful;
        }

        public async Task<ServiceResult<bool>> DoesRoleExistAsync(string role)
        {
            return await this.roleManager.RoleExistsAsync(role);
        }
    }
}
