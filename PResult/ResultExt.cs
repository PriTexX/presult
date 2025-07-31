namespace PResult;

public static class Result
{
    /// <summary>Creates a success result</summary>
    public static Result<T, TError> Ok<T, TError>(T value) => new(value);

    /// <summary>Creates an error result</summary>
    public static Result<T, TError> Err<T, TError>(TError error) => new(error);

    /// <summary>Creates an async success result</summary>
    public static AsyncResult<T, TError> OkAsync<T, TError>(T value) =>
        Task.FromResult(Ok<T, TError>(value));

    /// <summary>Creates an async error result</summary>
    public static AsyncResult<T, TError> ErrAsync<T, TError>(TError error) =>
        Task.FromResult(Err<T, TError>(error));

    /// <summary>Safely run a Task, capturing exceptions as <see cref="Exception"/> errors.</summary>
    public static AsyncResult<Unit, Exception> FromTask(Task task)
    {
        return task.ContinueWith<Result<Unit, Exception>>(r =>
            r is { IsFaulted: true, Exception: not null } ? r.Exception : Unit.Default
        );
    }

    /// <summary>Safely run a Task&lt;T&gt;, capturing exceptions as <see cref="Exception"/> errors.</summary>
    public static AsyncResult<T, Exception> FromTask<T>(Task<T> task)
    {
        return task.ContinueWith<Result<T, Exception>>(r =>
            r is { IsFaulted: true, Exception: not null } ? r.Exception : r.Result
        );
    }

    /// <summary>Execute an action, returning success or capturing any exception as an error.</summary>
    public static Result<Unit, Exception> FromThrowable(Action fn)
    {
        try
        {
            fn();
            return Unit.Default;
        }
        catch (Exception err)
        {
            return err;
        }
    }

    /// <summary>Execute a function, returning its value or capturing any exception as an error.</summary>
    public static Result<T, Exception> FromThrowable<T>(Func<T> fn)
    {
        try
        {
            return fn();
        }
        catch (Exception err)
        {
            return err;
        }
    }
}
