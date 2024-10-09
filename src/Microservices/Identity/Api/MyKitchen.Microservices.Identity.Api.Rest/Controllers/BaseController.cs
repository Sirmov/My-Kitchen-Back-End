namespace MyKitchen.Microservices.Identity.Api.Rest.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Authorize]
    public class BaseController : Controller
    {
    }
}
