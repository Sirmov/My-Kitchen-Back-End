namespace MyKitchen.Microservices.Identity.Api.Rest.Common.Constants
{
    public static class RouteConstants
    {
        public const string ErrorHandlerRoute = "/error";

        private const string BaseRoute = "api/v1";

        public static class Users
        {
            private const string BaseRoute = $"{RouteConstants.BaseRoute}/users";

            public const string RegisterEndpoint = $"{BaseRoute}/register";

            public const string LoginEndpoint = $"{BaseRoute}/login";

            public const string LogoutEndpoint = $"{BaseRoute}/logout";
        }
    }
}
