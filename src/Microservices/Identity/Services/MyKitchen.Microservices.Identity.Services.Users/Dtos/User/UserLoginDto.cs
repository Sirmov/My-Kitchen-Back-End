namespace MyKitchen.Microservices.Identity.Services.Users.Dtos.User
{
    using System.ComponentModel.DataAnnotations;

    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Mapping;

    using static MyKitchen.Microservices.Identity.Data.Common.Constants.EntityConstraints.ApplicationUser;

    public class UserLoginDto : IMapFrom<ApplicationUser>, IMapTo<ApplicationUser>
    {
        [EmailAddress]
        [Required(AllowEmptyStrings = false)]
        [StringLength(EmailMaxLength, MinimumLength = EmailMinLength)]
        public string Email { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        [StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength)]
        public string Password { get; set; } = null!;
    }
}
