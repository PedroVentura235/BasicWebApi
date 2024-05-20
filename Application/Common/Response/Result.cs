using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Net;

namespace BasicWebApi.Application.Common.Response;

public class Result<TValue, TError>
{
    public readonly TValue? Value;
    public readonly TError? Error;

    private bool _isSuccess;

    private Result(TValue value)
    {
        _isSuccess = true;
        Value = value;
        Error = default;
    }

    private Result(TError error)
    {
        _isSuccess = false;
        Value = default;
        Error = error;
    }

    //happy path
    public static implicit operator Result<TValue, TError>(TValue value) => new Result<TValue, TError>(value);

    //error path
    public static implicit operator Result<TValue, TError>(TError error) => new Result<TValue, TError>(error);

    public IResult Match(Func<TValue, IResult> success, Func<TError, IResult> failure)
    {
        if (_isSuccess)
        {
            return success(Value!);
        }
        return failure(Error!);
    }
}

public sealed record Error(HttpStatusCode StatusCode, string Code, string? Message = null);

public static class CommonErrors
{
    public static readonly Error DivisionByZero = new(HttpStatusCode.NotFound, "Dived.DivisionByZero",
       "Division by zero is not allowed.");

    public static readonly Error DivisionByOne = new(HttpStatusCode.NotFound, "Dived.DivisionByOne",
      "Division by 1 always results in the dividend.");

    public static Error NotFound(string entityName)
    {
        string message = $"{entityName} not found.";
        return new Error(HttpStatusCode.NotFound, "NotFound", message);
    }

    //then DivedErrors.DivisionByzero
}