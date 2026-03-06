using System.Diagnostics.CodeAnalysis;

namespace Application.Common;
public sealed class Result<TResult>
{
    private Result(bool isSuccess, TResult? @value, string[]? errorMessage)
    {
        IsSuccess = isSuccess;
        ResultData = @value;
        Errors = errorMessage;
    }

    [MemberNotNullWhen(true, nameof(ResultData))]
    [MemberNotNullWhen(false, nameof(Errors))]
    public bool IsSuccess { get; }

    [MemberNotNullWhen(true, nameof(Errors))]
    [MemberNotNullWhen(false, nameof(ResultData))]
    public bool IsFailure => !IsSuccess;
    public string[] Errors { get; set; }
    public TResult? ResultData { get; set; }

    public static Result<TResult> Ok(TResult result) => new(true, result, null);
    public static Result<TResult> Fail(string[] errorMessage) => new(false, default, errorMessage);
}
