using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;
using FluentValidation.Results;
using MVCAPI.Extensions;

namespace MVCAPI.Validations
{
    public class ValidationFilter<T> : IAsyncActionFilter where T : class
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
            if (validator != null)
            {
                var param = context.ActionArguments.Values.OfType<T>().FirstOrDefault();
                if (param != null)
                {
                    var validationResult = await validator.ValidateAsync(param);
                    if (!validationResult.IsValid)
                    {
                        context.Result = new UnprocessableEntityObjectResult(validationResult.ToDictionary());
                        return;
                    }
                }
            }
            await next();
        }
    }
}
