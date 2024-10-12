// |-----------------------------------------------------------------------------------------------------|
// <copyright file="InternalServerErrorDetails.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Common.ProblemDetails
{
    using System.Net;

    using Microsoft.AspNetCore.Mvc;

    using MyKitchen.Common.Constants;

    /// <summary>
    /// This class inherits the <see cref="ProblemDetails"/> class. It is a machine-readable format for
    /// specifying internal server errors in HTTP API responses based on https://datatracker.ietf.org/doc/html/rfc7807/.
    /// </summary>
    public class InternalServerErrorDetails : ProblemDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalServerErrorDetails"/> class.
        /// </summary>
        /// <param name="details">The details of the internal server error.</param>
        public InternalServerErrorDetails(string? details = null)
        {
            this.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
            this.Status = (int)HttpStatusCode.InternalServerError;
            this.Title = HttpStatusCode.InternalServerError.ToString();
            this.Detail = details ?? ExceptionMessages.InternalServerError;
        }
    }
}
