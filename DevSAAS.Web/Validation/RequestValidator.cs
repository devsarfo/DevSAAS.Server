using DevSAAS.Web.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DevSAAS.Web.Validation;

public static class RequestValidator
{
    public static IActionResult MakeValidationResponse(ActionContext context)
    {
        var data = new List<Error>();
        foreach (var keyModelStatePair in context.ModelState)
        {
            var errors = keyModelStatePair.Value.Errors;
            if (errors is not { Count: > 0 }) continue;

            if (errors.Count == 1)
            {
                var errorMessage = GetErrorMessage(errors[0]);
                data.Add(new Error
                {
                    Field = keyModelStatePair.Key,
                    Message = errorMessage
                });
            }
            else
            {
                var errorMessages = new string[errors.Count];
                for (var i = 0; i < errors.Count; i++)
                {
                    errorMessages[i] = GetErrorMessage(errors[i]);
                    data.Add(new Error
                    {
                        Field = keyModelStatePair.Key,
                        Message = errorMessages[i],
                    });
                }
            }
        }

        var apiResponse = new ApiResponse<List<Error>>("error", "Validation Error", data);
        var result = new BadRequestObjectResult(apiResponse);

        result.ContentTypes.Add("application/problem+json");
        return result;
    }

    private static string GetErrorMessage(ModelError error)
    {
        return string.IsNullOrEmpty(error.ErrorMessage) ? "The input was not valid." : error.ErrorMessage;
    }
}

public class Error
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}