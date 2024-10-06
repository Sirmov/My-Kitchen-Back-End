namespace MyKitchen.Microservices.Identity.Services.Common.ServiceResult
{
    using Microsoft.AspNetCore.Mvc;

    using MyKitchen.Common.Result;

    public class ServiceResult<TData> : Result<TData, ProblemDetails>
    {
        public ServiceResult(TData data)
            : base(data)
        {
        }

        public ServiceResult(ProblemDetails problemDetails)
            : base(problemDetails)
        {
        }

        public static new ServiceResult Successful => new();

        public static implicit operator ServiceResult<TData>(ProblemDetails problemDetails) => new(problemDetails);

        public static implicit operator ServiceResult<TData>(TData data) => new(data);

        public static implicit operator ServiceResult(ServiceResult<TData> serviceResult) => new(serviceResult.Failure!);

        public IActionResult ToActionResult(Func<ServiceResult<TData>, IActionResult> onSuccess)
        {
            if (this.Succeed)
            {
                return onSuccess(this.Data!);
            }

            return this.ToErrorResponse();
        }

        public async Task<IActionResult> ToActionResult(Func<ServiceResult<TData>, Task<IActionResult>> onSuccess)
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
