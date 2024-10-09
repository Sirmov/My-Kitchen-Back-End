namespace MyKitchen.Microservices.Identity.Services.Tokens.Contracts
{
    using MyKitchen.Microservices.Identity.Data.Models;

    public interface ITokensService<TUser, TRole>
        where TUser : ApplicationUser, new()
        where TRole : ApplicationRole, new()
    {
        public Task<string> GenerateAccessTokenAsync(TUser user);
    }
}
