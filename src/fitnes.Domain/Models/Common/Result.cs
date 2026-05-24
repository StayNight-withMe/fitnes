using fitnes.Domain.Enums;

namespace fitnes.Domain.Models.Common;

public class Result<T> : Result
{
    public T? Value { get; init; }

    public static Result<T> Success(T value) => new()
    {
        Value = value,
        IsSuccess = true,
        ErrorCode = Errors.None
    };
}
