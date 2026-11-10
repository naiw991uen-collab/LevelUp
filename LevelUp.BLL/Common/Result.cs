namespace LevelUp.BLL.Common;

public record class Result(bool Success, string? Error = null, ResultKind Kind = ResultKind.OK)
{
    public static Result OK() => new(true);
    public static Result Fail(string error, ResultKind kind = ResultKind.Conflict) => new(false, error, kind);
    public static Result NotFound(string error = "العنصر غير موجود") => new(false, error, ResultKind.NotFound);
    public static Result Validation(string error) => new(false, error, ResultKind.ValidationFailed);
}

public record class Result<T>(bool Success, T? Value, string? Error = null, ResultKind Kind = ResultKind.OK)
{
    public static Result<T> OK(T value) => new(true, value);
    public static Result<T> Fail(string error, ResultKind kind = ResultKind.Conflict) => new(false, default, error, kind);
    public static Result<T> NotFound(string error = "العنصر غير موجود") => new(false, default, error, ResultKind.NotFound);
    public static Result<T> Validation(string error) => new(false, default, error, ResultKind.ValidationFailed);
}
