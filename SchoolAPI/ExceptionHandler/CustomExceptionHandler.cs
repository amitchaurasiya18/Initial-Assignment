using System.Net;
using SchoolAPI.DTO;

namespace SchoolAPI.ExceptionHandler
{
    public class CustomExceptionHandler : IMiddleware
    {
        private readonly ILogger<CustomExceptionHandler> _logger;

        public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = Guid.NewGuid();
            _logger.LogError($"TraceId: {traceId}, Path: {context.Request.Path}, Method: {context.Request.Method}, " +
                             $"Exception: {exception.Message}, StackTrace: {exception.StackTrace}");

            context.Response.ContentType = "application/json";

            var (statusCode, errorMessage) = exception switch
            {
                KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found."),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access."),
                _ => (HttpStatusCode.InternalServerError, "Internal Server Error from the custom middleware.")
            };

            context.Response.StatusCode = (int)statusCode;

            var errorDetails = new ErrorDetails
            {
                TraceId = traceId,
                Message = errorMessage,
                StatusCode = context.Response.StatusCode,
                Instance = context.Request.Path,
                ExceptionMessage = exception.Message
            };

            return context.Response.WriteAsJsonAsync(errorDetails);
        }
    }
}