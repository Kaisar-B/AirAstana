namespace Web.API.Models;

using System;
using System.Diagnostics.CodeAnalysis;

public sealed class ApiResult<T>
{
    private ApiResult(bool isSuccess, T? data, string? message)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
        Timestamp = DateTimeOffset.UtcNow;
    }

    [MemberNotNullWhen(true, nameof(Data))]
    [MemberNotNullWhen(false, nameof(Message))]
    public bool IsSuccess { get; }

    [MemberNotNullWhen(true, nameof(Message))]
    [MemberNotNullWhen(false, nameof(Data))]
    public bool IsFailure => !IsSuccess;

    public T? Data { get; }
    public string? Message { get; }
    public DateTimeOffset Timestamp { get; }

    public static ApiResult<T> Ok(T data, string? message = null) => new(true, data, message);
    public static ApiResult<T> Fail(T data, string message) => new(false, data, message);
}
