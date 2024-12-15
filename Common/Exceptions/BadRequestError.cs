using System;

namespace ChatApi.Common.Exceptions;

public class BadRequestError(string message="Bad Request") : Exception(message)
{
    public int StatusCode { get; } = 400;
    public string ErrorName { get; } = "BadRequestError";
}
