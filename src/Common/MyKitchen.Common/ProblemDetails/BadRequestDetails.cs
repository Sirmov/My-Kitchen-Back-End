// |-----------------------------------------------------------------------------------------------------|
// <copyright file="BadRequestDetails.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Common.ProblemDetails
{
    using System.Net;

    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// This class is a machine-readable format for specifying bad request errors in HTTP API responses based
    /// on https://datatracker.ietf.org/doc/html/rfc7807/. It inherits the <see cref="ProblemDetails"/> class.
    /// </summary>
    public class BadRequestDetails : ProblemDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestDetails"/> class.
        /// </summary>
        /// <param name="details">The details of the bad request problem.</param>
        public BadRequestDetails(string details)
        {
            this.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
            this.Status = (int)HttpStatusCode.BadRequest;
            this.Title = HttpStatusCode.BadRequest.ToString();
            this.Detail = details;
        }
    }
}
