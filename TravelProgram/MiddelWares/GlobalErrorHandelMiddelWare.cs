using System.Net;
using System.Text.Json;
using Azure;
using Domain.Exceptions;
using Shared.ErrorModels;

namespace TravelProgram.MiddelWares
{
    public class GlobalErrorHandelMiddelWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandelMiddelWare> _logger;

        public GlobalErrorHandelMiddelWare(RequestDelegate next,ILogger<GlobalErrorHandelMiddelWare> logger)
        {
           _next = next;
           _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
                if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    await HandelNotFoundEndPointAsync(context);
                }
            }
            catch (Exception ex) 
            {
                // log exception 
                _logger.LogError($"something went wrong {ex.Message}");
                // handel exception
                await HandelExceptionAsync(context,ex);
            }
        }

        private async Task HandelNotFoundEndPointAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            var response = new ErrorDetails
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                ErrorMessage = $"the end point {context.Request.Path} not found"
            };
            await context.Response.WriteAsync(response.ToString());
        }

        private async Task HandelExceptionAsync(HttpContext context, Exception ex)
        {
            // set content type => application\json
            context.Response.ContentType = "application/json";
            //set default status code => 500
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            //return standard response 
            var response = new ErrorDetails
            {
                ErrorMessage = ex.Message
            };

            context.Response.StatusCode = ex switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                UnAuthorizedException => (int)HttpStatusCode.Unauthorized,
                ValidationException validationException => HandelValidationException(validationException, response),
                _ => (int)HttpStatusCode.InternalServerError
            };
            response.StatusCode = context.Response.StatusCode;

          
            await context.Response.WriteAsync(response.ToString()); 
        }

        private int HandelValidationException(ValidationException validationException, ErrorDetails response)
        {
            response.Errors = validationException.Errors;
            return (int)HttpStatusCode.BadRequest;
        }
    }
}
