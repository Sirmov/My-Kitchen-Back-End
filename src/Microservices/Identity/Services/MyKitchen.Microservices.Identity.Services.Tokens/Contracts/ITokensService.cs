namespace MyKitchen.Microservices.Identity.Services.Tokens.Contracts
{
    using MyKitchen.Microservices.Identity.Data.Models;

    public interface ITokensService
    {
        public string GenerateAccessTokenAsync(ApplicationUser user);
    }
}
