using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ShipJobPortal.API.Middlewares
{
    /// <summary>
    /// Middleware to handle all unhandled exceptions globally.
    /// Logs the exception and returns a standardized error response.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Called by the ASP.NET Core pipeline for each HTTP request.
        /// </summary>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Proceed to next middleware or controller
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Unhandled exception occurred");

                // Set HTTP 500 response
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                // Generate response body depending on environment
                var response = _env.IsDevelopment()
                    ? new
                    {
                        message = ex.Message,
                        //stackTrace = ex.StackTrace
                    }
                    : new
                    {
                        message = "An unexpected error occurred. Please contact support."
                    };

                // Serialize response
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);

                // Write to response body
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
