// |-----------------------------------------------------------------------------------------------------|
// <copyright file="BaseController.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Api.Rest.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// This class is a base api controller for all controllers.
    /// It inherits the <see cref="ControllerBase"/>.
    /// </summary>
    [ApiController]
    [Authorize]
    public class BaseController : ControllerBase
    {
    }
}
