using System.Net;
using CoreServices.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreServices.Filters
{
    public class ModelValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {

            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(err => err.ErrorMessage).ToList()
                    );

                var errorDetails = new ValidationErrorDetails
                {
                    Message = "One or more validation errors occurred",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    ExceptionMessage = "Validation failed.",
                    Errors = errors
                };

                context.Result = new BadRequestObjectResult(errorDetails);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        { }
    }
}
