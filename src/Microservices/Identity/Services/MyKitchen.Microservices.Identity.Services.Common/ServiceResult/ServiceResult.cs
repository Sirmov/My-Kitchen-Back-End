namespace MyKitchen.Microservices.Identity.Services.Common.ServiceResult
{
    using Microsoft.AspNetCore.Mvc;

    using MyKitchen.Common.Result;

    public class ServiceResult : Result<ProblemDetails>
    {
        public ServiceResult()
        {
        }

        public ServiceResult(ProblemDetails problemDetails)
            : base(problemDetails)
        {
        }

        public static new ServiceResult Successful => new();

        public static implicit operator ServiceResult(ProblemDetails problemDetails) => new(problemDetails);

        public IActionResult ToActionResult(Func<ServiceResult, IActionResult> onSuccess)
        {
            if (this.Succeed)
            {
                return onSuccess(this);
            }

            return this.ToErrorResponse();
        }

        public async Task<IActionResult> ToActionResult(Func<ServiceResult, Task<IActionResult>> onSuccess)
        {
            if (this.Succeed)
            {
                return await onSuccess(this);
            }

            return this.ToErrorResponse();
        }

        private IActionResult ToErrorResponse()
        {
            var problemDetails = this.Failure;

            return new ObjectResult(problemDetails);
        }
    }
}
