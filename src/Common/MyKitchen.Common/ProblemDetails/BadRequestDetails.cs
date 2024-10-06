namespace MyKitchen.Common.ProblemDetails
{
    using System.Net;

    using Microsoft.AspNetCore.Mvc;

    public class BadRequestDetails : ProblemDetails
    {
        public BadRequestDetails(string details)
        {
            this.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
            this.Status = (int)HttpStatusCode.BadRequest;
            this.Title = HttpStatusCode.BadRequest.ToString();
            this.Detail = details;
        }
    }
}
