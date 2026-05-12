using QuickBite.Auth.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace QuickBite.Auth.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedException ex)
            {
                await WriteResponseAsync(context, HttpStatusCode.Unauthorized, "Unauthorized", ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteResponseAsync(context, HttpStatusCode.Unauthorized, "Unauthorized", ex.Message);
            }
            catch (ConflictException ex)
            {
                await WriteResponseAsync(context, HttpStatusCode.Conflict, "Conflict", ex.Message);
            }
            catch (ArgumentException ex)
            {
                await WriteResponseAsync(context, HttpStatusCode.BadRequest, "Bad request", ex.Message);
            }
            catch (Exception ex)
            {
                await WriteResponseAsync(context, HttpStatusCode.InternalServerError, "Something went wrong", ex.Message);
            }
        }

        private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string message, string error)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                message,
                error
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
