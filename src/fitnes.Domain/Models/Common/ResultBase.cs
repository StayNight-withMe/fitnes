using fitnes.Domain.Enums;

namespace fitnes.Domain.Models.Common;

public class Result
{
    public bool IsSuccess { get; init; }
    public Errors ErrorCode { get; init; }
    public string[] ErrorsList { get; init; } = [];

    //public static Result Failure(Errors errorCode, IEnumerable<string>? errors = null) => new()
    //{
    //    IsSuccess = false,
    //    ErrorCode = errorCode,
    //    ErrorsList = errors?.ToArray() ?? [errorCode.ToString()]
    //};

    public static Result<T> Failure<T>(Errors errorCode, IEnumerable<string>? errors = null) => new()
    {
        IsSuccess = false,
        ErrorCode = errorCode,
        ErrorsList = errors?.ToArray() ?? [errorCode.ToString()]
    };
}
