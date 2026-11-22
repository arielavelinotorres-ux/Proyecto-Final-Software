using Microsoft.Data.SqlClient;
using SistemaEducativoDeRiesgo.Application.DTOs;
using SistemaEducativoDeRiesgo.Application.Serialization;


namespace SistemaEducativoDeRiesgo.Web.Api.Middlewares
{
    public class SqlExceptionMiddleware
    {

        private readonly RequestDelegate _next;

        public SqlExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (SqlException ex)
            {
                var statusCode = ex.Number switch
                {
                    50001 => StatusCodes.Status400BadRequest,
                    50003 => StatusCodes.Status404NotFound,
                    _ => StatusCodes.Status500InternalServerError
                };

                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsJsonAsync(
                    new MensajeResponse(ex.Message),
                    AppJsonSerializerContext.Default.MensajeResponse);
            }
            catch (ArgumentException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(
                    new MensajeResponse(ex.Message),
                    AppJsonSerializerContext.Default.MensajeResponse);
            }
        }

    }
}
