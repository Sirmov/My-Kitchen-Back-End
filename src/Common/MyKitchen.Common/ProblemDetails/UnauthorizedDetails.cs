// |-----------------------------------------------------------------------------------------------------|
// <copyright file="UnauthorizedDetails.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Common.ProblemDetails
{
    using System.Net;

    using Microsoft.AspNetCore.Mvc;

        /// <summary>
    /// This class inherits the <see cref="ProblemDetails"/> class. It is a machine-readable format for
    /// specifying unauthorized errors in HTTP API responses based on https://datatracker.ietf.org/doc/html/rfc7807/.
    /// </summary>
    public class UnauthorizedDetails : ProblemDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedDetails"/> class.
        /// </summary>
        /// <param name="details">The details of the unauthorized error.</param>
        public UnauthorizedDetails(string details)
        {
            this.Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1";
            this.Status = (int)HttpStatusCode.Unauthorized;
            this.Title = HttpStatusCode.Unauthorized.ToString();
            this.Detail = details;
        }
    }
}
