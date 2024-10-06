namespace MyKitchen.Common.ProblemDetails
{
    using System.Net;

    using Microsoft.AspNetCore.Mvc;

    public class UnauthorizedDetails : ProblemDetails
    {
        public UnauthorizedDetails(string details)
        {
            this.Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1";
            this.Status = (int)HttpStatusCode.Unauthorized;
            this.Title = HttpStatusCode.Unauthorized.ToString();
            this.Detail = details;
        }
    }
}
