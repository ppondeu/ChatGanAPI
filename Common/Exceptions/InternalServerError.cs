using System;

namespace ChatApi.Common.Exceptions;

public class InternalServerError(string message = "Internal Server Error") : Exception(message)
{
    public int StatusCode { get; } = 500;
    public string ErrorName { get; } = "InternalServerError";
}
