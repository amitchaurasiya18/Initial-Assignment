using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SchoolAPI.CustomExceptions;
using SchoolAPI.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace SchoolAPI.ExceptionHandler
{
    public class CustomExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is StudentNotFound studentNotFound)
            {
                var response = new ErrorResponse()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    ErrorMessage = studentNotFound.Message
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

            if (exception is NoStudentsFound noStudentsFound)
            {
                var response = new ErrorResponse()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    ErrorMessage = noStudentsFound.Message
                };
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                await httpContext.Response.WriteAsJsonAsync(response);
                return true;
            }

            if (exception is InvalidPageNumber invalidPageNumber)
            {
                var response = new ErrorResponse()
                {
                    StatusCode = StatusCodes.Status406NotAcceptable,
                    ErrorMessage = invalidPageNumber.Message
                };
                httpContext.Response.StatusCode = StatusCodes.Status406NotAcceptable;
                await httpContext.Response.WriteAsJsonAsync(response);
                return true;
            }

            if (exception is InvalidPageSize invalidPageSize)
            {
                var response = new ErrorResponse()
                {
                    StatusCode = StatusCodes.Status406NotAcceptable,
                    ErrorMessage = invalidPageSize.Message
                };
                httpContext.Response.StatusCode = StatusCodes.Status406NotAcceptable;
                await httpContext.Response.WriteAsJsonAsync(response);
                return true;
            }

            return false;
        }
    }
}
