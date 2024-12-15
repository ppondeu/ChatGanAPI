using System;

namespace ChatApi.Common.Exceptions;

public class ForbiddenError(string message = "Forbidden") : Exception(message)
{
    public int StatusCode { get; } = 403;
    public string ErrorName { get; } = "ForbiddenError";
}
