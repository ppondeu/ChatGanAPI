using System;

namespace ChatApi.Common.Exceptions;

public class NotFoundError(string message = "Not Found") : Exception(message)
{
    public int StatusCode { get; } = 404;
    public string ErrorName { get; } = "NotFoundError";
}
