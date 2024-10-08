namespace MyKitchen.Microservices.Identity.Data.Common.Constants
{
    public static class EntityConstraints
    {
        public static class ApplicationUser
        {
            public const int UsernameMinLength = 3;

            public const int UsernameMaxLength = 32;

            public const int PasswordMinLength = 8;

            public const int PasswordMaxLength = 32;

            public const int EmailMinLength = 6;

            public const int EmailMaxLength = 32;
        }
    }
}
