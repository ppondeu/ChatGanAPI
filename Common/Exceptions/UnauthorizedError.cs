using System;

namespace ChatApi.Common.Exceptions;

public class UnauthorizedError(string message = "Unauthorized") : Exception(message)
{
    public int StatusCode { get; } = 401;
    public string ErrorName { get; } = "UnauthorizedError";
}
