namespace MyKitchen.Common.ProblemDetails
{
    using System.Net;

    using Microsoft.AspNetCore.Mvc;

    public class NotFoundDetails : ProblemDetails
    {
        public NotFoundDetails(string details)
        {
            this.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";
            this.Status = (int)HttpStatusCode.NotFound;
            this.Title = HttpStatusCode.NotFound.ToString();
            this.Detail = details;
        }
    }
}
