using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;
using WebApi.Application.DTOs.Errors;

namespace WebApi.Web.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled error: {Message}", exception.Message);

            var errorResponse = CreateErrorResponse(httpContext, exception);

            httpContext.Response.StatusCode = errorResponse.StatusCode;
            httpContext.Response.ContentType = "application/json";

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await httpContext.Response.WriteAsync(jsonResponse, cancellationToken);

            return true;
        }

        private static ErrorResponse CreateErrorResponse(HttpContext context, Exception exception)
        {
            return exception switch
            {
                KeyNotFoundException => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Reouse not found",
                    Details = exception.Message,
                    Path = context.Request.Path
                },

                ArgumentException => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Ivalid parameters",
                    Details = exception.Message,
                    Path = context.Request.Path
                },

                UnauthorizedAccessException => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "Access denied",
                    Details = exception.Message,
                    Path = context.Request.Path
                },

                InvalidOperationException => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Incorrect operation",
                    Details = exception.Message,
                    Path = context.Request.Path
                },

                NotSupportedException => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Operation not supported",
                    Details = exception.Message,
                    Path = context.Request.Path
                },

                TimeoutException => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.RequestTimeout,
                    Message = "The waiting time has expired.",
                    Details = exception.Message,
                    Path = context.Request.Path
                },

                _ => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Internal server error",
                    Details = "An unexpected error occurred.",
                    Path = context.Request.Path
                }
            };
        }
    }
}
