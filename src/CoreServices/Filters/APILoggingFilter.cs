using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using CoreServices.DTO;
using Microsoft.AspNetCore.Http;

namespace SchoolAPI.Filters
{
    public class APILoggingFilter : IAsyncActionFilter
    {
        private readonly Serilog.ILogger _logger;

        public APILoggingFilter()
        {
            _logger = Log.ForContext<APILoggingFilter>();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            request.EnableBuffering();

            string requestBody;
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            _logger.Information("Request Body: {@RequestBody}", requestBody);

            request.Body.Position = 0;

            var resultContext = await next();

            var logEntry = new ApiLog
            {
                Method = request.Method,
                Path = request.Path,
                StatusCode = context.HttpContext.Response.StatusCode
            };

            _logger.Information("API Log Entry: {@LogEntry}", logEntry);

            if (resultContext.Exception != null)
            {
                _logger.Error("Exception occurred: {@Exception}", resultContext.Exception.Message);
                return;
            }

            if (resultContext.Result is ObjectResult { Value: { } } objectResult)
            {
                _logger.Information("Returned Object: {@ReturnedObject}", objectResult.Value);
            }
        }
    }
}
