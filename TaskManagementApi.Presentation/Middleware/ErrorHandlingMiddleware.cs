using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagementApi.Presentation.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {ErrorMessage}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json"; // Use ProblemDetails content type
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7807", // Standard RFC for Problem Details
                Title = "An error occurred while processing your request.",
                Detail = "An unexpected internal server error occurred.",
                Instance = context.Request.Path
            };

            // Only expose sensitive details in Development environment
            if (context.RequestServices.GetService(typeof(Microsoft.AspNetCore.Hosting.IWebHostEnvironment)) is Microsoft.AspNetCore.Hosting.IWebHostEnvironment env && env.IsDevelopment())
            {
                problemDetails.Detail = exception.Message;
                problemDetails.Extensions.Add("stackTrace", exception.StackTrace);
                // Consider adding more details like inner exceptions if needed for debugging
                if (exception.InnerException != null)
                {
                    problemDetails.Extensions.Add("innerExceptionMessage", exception.InnerException.Message);
                }
            }

            var result = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return context.Response.WriteAsync(result);
        }
    }
}
