using Fusion.Microservice.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fusion.Microservice.Filters;

public class ApiResultFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        switch (context.Result)
        {
            case ObjectResult
            {
                StatusCode: >= StatusCodes.Status200OK and < StatusCodes.Status300MultipleChoices
            } objectResult:
            {
                var response = new ApiResult<object?>(objectResult.Value);

                context.Result = new ObjectResult(response)
                {
                    StatusCode = objectResult.StatusCode
                };
                break;
            }
            case ObjectResult objectResult:
            {
                if (objectResult.StatusCode is >= StatusCodes.Status400BadRequest)
                {
                    var response = new ApiResult<object?>(null);

                    context.Result = new ObjectResult(response)
                    {
                        StatusCode = objectResult.StatusCode
                    };
                }

                break;
            }
            case EmptyResult:
            {
                var response = new ApiResult<object?>(default(object))
                {
                    IsSuccess = true,
                    Message = "Request processed successfully."
                };

                context.Result = new ObjectResult(response)
                {
                    StatusCode = StatusCodes.Status204NoContent
                };
                break;
            }
        }
    }
}