namespace MyKitchen.Microservices.Identity.Services.Users.Dtos.User
{
    using System.ComponentModel.DataAnnotations;

    using MongoDB.Bson;

    using MyKitchen.Microservices.Identity.Data.Models;
    using MyKitchen.Microservices.Identity.Services.Mapping;

    using static MyKitchen.Microservices.Identity.Data.Common.Constants.EntityConstraints.ApplicationUser;

    public class UserDto : IMapFrom<ApplicationUser>, IMapTo<ApplicationUser>
    {
        public ObjectId Id { get; set; }

        [EmailAddress]
        [Required(AllowEmptyStrings = false)]
        [StringLength(EmailMaxLength, MinimumLength = EmailMinLength)]
        public string Email { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        [StringLength(UsernameMaxLength, MinimumLength = UsernameMinLength)]
        public string UserName { get; set; } = null!;

        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
    }
}
