namespace MyKitchen.Microservices.Identity.Api.Rest.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Mvc;

    using MyKitchen.Microservices.Identity.Api.Rest.Common.Constants;

    public class ErrorController : BaseController
    {
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route(RouteConstants.ErrorHandlerRoute)]
        public IActionResult HandleError([FromServices] IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment.IsDevelopment())
            {
                var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

                return Problem(
                    detail: exceptionHandlerFeature.Error.StackTrace,
                    title: exceptionHandlerFeature.Error.Message);
            }

            return Problem(null, null, 500, "Internal Server Error", "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1");
        }
    }
}
