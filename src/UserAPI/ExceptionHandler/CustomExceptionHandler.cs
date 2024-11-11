using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using UserAPI.CustomExceptions;
using UserAPI.DTO;

namespace UserAPI.ExceptionHandler
{
    public class CustomExceptionHandler: IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is UserNotFound userNotFound)
            {
                var response = new ErrorResponse()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    ErrorMessage = userNotFound.Message
                };
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                await httpContext.Response.WriteAsJsonAsync(response);
                return true;
            }

            if (exception is EmailAlreadyRegistered emailAlreadyRegistered)
            {
                var response = new ErrorResponse()
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    ErrorMessage = emailAlreadyRegistered.Message
                };
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                await httpContext.Response.WriteAsJsonAsync(response);
                return true;
            }

            if (exception is NoUsersFound noUsersFound)
            {
                var response = new ErrorResponse()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    ErrorMessage = noUsersFound.Message
                };
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                await httpContext.Response.WriteAsJsonAsync(response);
                return true;
            }

            if (exception is InvalidCredentials invalidCredentials)
            {
                var response = new ErrorResponse()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorMessage = invalidCredentials.Message
                };
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(response);
                return true;
            }

            return false;
        }
    }
}
