namespace MyKitchen.Microservices.Identity.Api.Rest.Common.Constants
{
    public static class RouteConstants
    {
        public const string ErrorHandlerRoute = "/error";

        private const string BaseRoute = "api/v1";

        public static class Users
        {
            public const string BaseRoute = $"{RouteConstants.BaseRoute}/users";

            public const string RegisterEndpoint = $"register";

            public const string LoginEndpoint = $"login";

            public const string LogoutEndpoint = $"logout";
        }
    }
}
