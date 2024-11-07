using Microsoft.AspNetCore.Diagnostics;
using SchoolAPI.DTO;

namespace SchoolAPI.ExceptionHandler
{
    public class AppInternalServerErrorExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if(exception is BadHttpRequestException)
            {
                var response = new ErrorResponse()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorTitle = "Internal Server Error",
                    ErrorMessage = exception.Message
                };

                await httpContext.Response.WriteAsJsonAsync(response);
                return true;
            }
            return false;
        }
    }
}
