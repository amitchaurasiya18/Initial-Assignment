using CoreServices.CustomExceptions;
using CoreServices.DTO;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace CoreServices.ExceptionHandler
{
    public class CustomExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            switch (exception)
            {
                case EmailAlreadyRegistered emailAlreadyRegistered:
                    return await HandleExceptionAsync(httpContext, emailAlreadyRegistered);

                case InvalidPageNumber invalidPageNumber:
                    return await HandleExceptionAsync(httpContext, invalidPageNumber);

                case InvalidPageSize invalidPageSize:
                    return await HandleExceptionAsync(httpContext, invalidPageSize);

                case InvalidCredentials invalidCredentials:
                    return await HandleExceptionAsync(httpContext, invalidCredentials);

                case Unauthorized unauthorized:
                    return await HandleExceptionAsync(httpContext, unauthorized);

                case InvalidSignature invalidSignature:
                    return await HandleExceptionAsync(httpContext, invalidSignature);

                case InvalidFormat invalidFormat:
                    return await HandleExceptionAsync(httpContext, invalidFormat);

                case AccessForbidden accessForbidden:
                    return await HandleExceptionAsync(httpContext, accessForbidden);

                default:
                    return await HandleExceptionAsync(httpContext, exception);
            }
        }

        private async Task<bool> HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            exception.Data["HandledByCustomHandler"] = true;
            var response = new ErrorResponse()
            {
                StatusCode = exception is ICustomException customException ? customException.StatusCode : StatusCodes.Status500InternalServerError,
                ErrorMessage = exception.Message
            };

            httpContext.Response.StatusCode = response.StatusCode;
            await httpContext.Response.WriteAsJsonAsync(response);
            return true;
        }
    }
}
