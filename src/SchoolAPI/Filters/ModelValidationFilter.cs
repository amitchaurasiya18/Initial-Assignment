using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;
using SchoolAPI.DTO;
using SchoolAPI.Validators;
using FluentValidation;

public class ModelValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {

        if(!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(err => err.ErrorMessage).ToList()
                );

            var errorDetails = new ErrorDetails
            {
                Message = "One or more validation errors occurred from Amit",
                StatusCode = (int)HttpStatusCode.BadRequest,
                ExceptionMessage = "Validation failed.",
                Errors = errors
            };

            context.Result = new BadRequestObjectResult(errorDetails);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {}
}
