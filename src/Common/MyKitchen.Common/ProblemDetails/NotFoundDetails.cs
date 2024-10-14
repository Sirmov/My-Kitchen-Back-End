// |-----------------------------------------------------------------------------------------------------|
// <copyright file="NotFoundDetails.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Common.ProblemDetails
{
    using System.Net;

    using Microsoft.AspNetCore.Mvc;

        /// <summary>
    /// This class is a machine-readable format for specifying not found errors in HTTP API responses based
    /// on https://datatracker.ietf.org/doc/html/rfc7807/. It inherits the <see cref="ProblemDetails"/> class.
    /// </summary>
    public class NotFoundDetails : ProblemDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundDetails"/> class.
        /// </summary>
        /// <param name="details">The details of the not found error.</param>
        public NotFoundDetails(string details)
        {
            this.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";
            this.Status = (int)HttpStatusCode.NotFound;
            this.Title = HttpStatusCode.NotFound.ToString();
            this.Detail = details;
        }
    }
}
