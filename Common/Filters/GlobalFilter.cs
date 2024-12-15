using ChatApi.Common.Exceptions;
using ChatApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChatApi.Common.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        Console.WriteLine($"{new string('-', 10)} An exception occurred {new string('-', 10)}");
        Console.WriteLine($"An exception occurred: {context.Exception.Message}");
        Console.WriteLine($"Stack trace: {context.Exception.StackTrace}");
        Console.WriteLine($"Exception type: {context.Exception.GetType()}");
        Console.WriteLine($"{new string('-', 10)} End of exception {new string('-', 10)}");
        if (context.Exception is NotFoundError notFoundError)
        {
            context.Result = new ObjectResult(new ApiResponse<object>
            {
                StatusCode = notFoundError.StatusCode,
                Message = notFoundError.ErrorName,
                Data = null,
                Errors = [notFoundError.Message]
            })
            {
                StatusCode = notFoundError.StatusCode
            };
            context.ExceptionHandled = true;
        }
        // Handle ForbiddenError
        else if (context.Exception is ForbiddenError forbiddenError)
        {
            context.Result = new ObjectResult(new ApiResponse<object>
            {
                StatusCode = forbiddenError.StatusCode,
                Message = forbiddenError.ErrorName,
                Data = null,
                Errors = [forbiddenError.Message]
            })
            {
                StatusCode = forbiddenError.StatusCode
            };
            context.ExceptionHandled = true;
        }
        // Handle BadRequestError
        else if (context.Exception is BadRequestError badRequestError)
        {
            context.Result = new ObjectResult(new ApiResponse<object>
            {
                StatusCode = badRequestError.StatusCode,
                Message = badRequestError.ErrorName,
                Data = null,
                Errors = [badRequestError.Message]
            })
            {
                StatusCode = badRequestError.StatusCode
            };
            context.ExceptionHandled = true;
        }
        // Handle UnauthorizedError
        else if (context.Exception is UnauthorizedError unauthorizedError)
        {
            context.Result = new ObjectResult(new ApiResponse<object>
            {
                StatusCode = unauthorizedError.StatusCode,
                Message = unauthorizedError.ErrorName,
                Data = null,
                Errors = [unauthorizedError.Message]
            })
            {
                StatusCode = unauthorizedError.StatusCode
            };
            context.ExceptionHandled = true;
        }
        else if (context.Exception is InternalServerError internalServerError)
        {
            context.Result = new ObjectResult(new ApiResponse<object>
            {
                StatusCode = 500,
                Message = "Internal Server Error",
                Errors = [internalServerError.Message]
            })
            {
                StatusCode = 500
            };
            context.ExceptionHandled = true;
        }
        else
        {
            context.Result = new ObjectResult(new ApiResponse<object>
            {
                StatusCode = 500,
                Message = "Internal Server Error",
                Errors = ["An unexpected error occurred."]
            })
            {
                StatusCode = 500
            };
            context.ExceptionHandled = true;
        }
    }
}
