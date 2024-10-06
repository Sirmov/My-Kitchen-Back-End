namespace MyKitchen.Common.ProblemDetails
{
    using System.Net;

    using Microsoft.AspNetCore.Mvc;

    using MyKitchen.Common.Constants;

    public class InternalServerErrorDetails : ProblemDetails
    {
        public InternalServerErrorDetails(string? details = null)
        {
            this.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
            this.Status = (int)HttpStatusCode.InternalServerError;
            this.Title = HttpStatusCode.InternalServerError.ToString();
            this.Detail = details ?? ExceptionMessages.InternalServerError;
        }
    }
}
