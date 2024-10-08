namespace MyKitchen.Microservices.Identity.Services.Users.Dtos.User
{
    using System.ComponentModel.DataAnnotations;

    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Mapping;

    using static MyKitchen.Microservices.Identity.Data.Common.Constants.EntityConstraints.ApplicationUser;

    public class UserRegisterDto : UserLoginDto, IMapFrom<ApplicationUser>, IMapTo<ApplicationUser>
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(UsernameMaxLength, MinimumLength = UsernameMinLength)]
        public string Username { get; set; } = null!;
    }
}
