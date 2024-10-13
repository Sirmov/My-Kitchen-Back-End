// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ErrorController.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Api.Rest.Controllers
{
    using System.Net.Mime;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Mvc;

    using MyKitchen.Microservices.Identity.Api.Common.Constants;

    /// <summary>
    /// This controller is responsible for handling all uncaught errors.
    /// </summary>
    public class ErrorController : BaseController
    {
        /// <summary>
        /// This action handles all uncaught errors.
        /// </summary>
        /// <param name="hostEnvironment">Provides information about the hosting environment the application is running in.</param>
        /// <returns>Return a <see cref="ProblemDetails"/>.</returns>
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
        [Route(RouteConstants.ErrorHandlerRoute)]
        public IActionResult HandleError([FromServices] IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment.IsDevelopment())
            {
                var exceptionHandlerFeature = this.HttpContext.Features.Get<IExceptionHandlerFeature>()!;

                return this.Problem(
                    detail: exceptionHandlerFeature.Error.StackTrace,
                    title: exceptionHandlerFeature.Error.Message);
            }

            return this.Problem(null, null, 500, "Internal Server Error", "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1");
        }
    }
}
