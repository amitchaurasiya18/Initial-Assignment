using Microsoft.AspNetCore.Diagnostics;
using SchoolAPI.DTO;

namespace SchoolAPI.ExceptionHandler
{
    public class AppKeyNotFoundExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if(exception is KeyNotFoundException)
            {
                var response = new ErrorResponse()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    ErrorTitle = "Not Found",
                    ErrorMessage = exception.Message
                };

                await httpContext.Response.WriteAsJsonAsync(response);
                return true;
            }
            return false;
        }
    }
}
