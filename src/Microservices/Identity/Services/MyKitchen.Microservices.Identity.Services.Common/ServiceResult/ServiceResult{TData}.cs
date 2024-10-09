namespace MyKitchen.Microservices.Identity.Services.Common.ServiceResult
{
    using Microsoft.AspNetCore.Mvc;

    using MyKitchen.Common.Result;
    using MyKitchen.Common.Result.Contracts;

    public sealed class ServiceResult<TData> : Result<TData, ProblemDetails>, IDataResult<TData, ProblemDetails>
    {
        public ServiceResult(TData data)
            : base(data)
        {
        }

        public ServiceResult(ProblemDetails problemDetails)
            : base(problemDetails)
        {
        }

        public static new ServiceResult<TData> Successful(TData data) => new(data);

        public static implicit operator ServiceResult<TData>(ProblemDetails problemDetails) => new(problemDetails);

        public static implicit operator ServiceResult<TData>(TData data) => new(data);

        public static implicit operator ServiceResult(ServiceResult<TData> serviceResult) => new(serviceResult.Failure!);

        public IActionResult ToActionResult(Func<TData, IActionResult> onSuccess)
        {
            if (this.Succeed && this.Data != null)
            {
                return onSuccess(this.Data);
            }

            return this.ToErrorResponse();
        }

        public async Task<IActionResult> ToActionResult(Func<TData, Task<IActionResult>> onSuccess)
        {
            if (this.Succeed && this.Data != null)
            {
                return await onSuccess(this.Data);
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
