using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens.Experimental;
using Shared.ErrorModels;

namespace TravelProgram.Factories
{
    public class ApiResponseFactory
    {
        // context 
        public static IActionResult CustomValidationErrorResponse(ActionContext context)
        {
            // get all errors from model state
            var errors = context.ModelState.Where(error => error.Value.Errors.Any()).Select(error => new ValidationErrors
            {
              Field = error.Key ,
              Errors = error.Value.Errors.Select(e => e.ErrorMessage)
            });
            var response = new ValidationErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                ErrorMessage = "Validation Error",
                Errors = errors
            };
          return new BadRequestObjectResult(response);
        }

    }
}
