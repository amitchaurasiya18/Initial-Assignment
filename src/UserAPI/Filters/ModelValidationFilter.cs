using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserAPI.DTO;

namespace UserAPI.Filters
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
