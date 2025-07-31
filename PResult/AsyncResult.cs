using System.Runtime.CompilerServices;

namespace PResult;

public readonly struct AsyncResult<T, TError>
{
    private readonly Task<Result<T, TError>> _task;

    private AsyncResult(Task<Result<T, TError>> task) => _task = task;

    /// <summary>Handles success or error branch.</summary>
    public Task<TRes> Match<TRes>(Func<T, TRes> ok, Func<TError, TRes> err) =>
        _task.Continue(res => res.Match(ok, err));

    /// <summary>Runs provided computation on success.</summary>
    public AsyncResult<K, TError> Then<K>(Func<T, Result<K, TError>> next) =>
        Match(next, err => err);

    /// <summary>Asynchronously runs provided computation on success.</summary>
    public AsyncResult<K, TError> ThenAsync<K>(Func<T, AsyncResult<K, TError>> next) =>
        _task.Continue(res => res.ThenAsync(next).AsTask()).Unwrap();

    /// <summary>Asynchronously runs provided computation on success.</summary>
    public AsyncResult<K, TError> ThenAsync<K>(Func<T, Task<Result<K, TError>>> next) =>
        _task.Continue(res => res.ThenAsync(next).AsTask()).Unwrap();

    /// <summary>Runs provided computation on error.</summary>
    public AsyncResult<T, TError> ThenErr(Func<TError, Result<T, TError>> errNext) =>
        Match(val => val, errNext);

    /// <summary>Asynchronously runs provided computation on error.</summary>
    public AsyncResult<T, TError> ThenErrAsync(Func<TError, AsyncResult<T, TError>> errNext) =>
        _task.Continue(res => res.ThenErrAsync(errNext).AsTask()).Unwrap();

    /// <summary>Asynchronously runs provided computation on error.</summary>
    public AsyncResult<T, TError> ThenErrAsync(Func<TError, Task<Result<T, TError>>> errNext) =>
        _task.Continue(res => res.ThenErrAsync(errNext).AsTask()).Unwrap();

    /// <summary>Transforms success value.</summary>
    public AsyncResult<K, TError> Map<K>(Func<T, K> mapper) =>
        Match(val => mapper(val), Result.Err<K, TError>);

    /// <summary>Asynchronously transforms success value.</summary>
    public AsyncResult<K, TError> MapAsync<K>(Func<T, Task<K>> mapper) =>
        _task.Continue(res => res.MapAsync(mapper).AsTask()).Unwrap();

    /// <summary>Transforms error.</summary>
    public AsyncResult<T, KError> MapErr<KError>(Func<TError, KError> errMapper) =>
        Match(Result.Ok<T, KError>, err => errMapper(err));

    /// <summary>Asynchronously transforms error.</summary>
    public AsyncResult<T, KError> MapErrAsync<KError>(Func<TError, Task<KError>> errMapper) =>
        _task.Continue(res => res.MapErrAsync(errMapper).AsTask()).Unwrap();

    /// <summary>Returns success or fallback value.</summary>
    public Task<T> ValueOr(T fallback) => _task.Continue(res => res.ValueOr(fallback));

    /// <summary>Gets success or throws.</summary>
    public Task<T> UnsafeValue => _task.Continue(res => res.UnsafeValue);

    /// <summary>Gets error or throws.</summary>
    public Task<TError> UnsafeError => _task.Continue(res => res.UnsafeError);

    public Task<Result<T, TError>> AsTask() => _task;

    public static implicit operator AsyncResult<T, TError>(Task<Result<T, TError>> task) =>
        new(task);

    public static implicit operator AsyncResult<T, TError>(Result<T, TError> result) =>
        new(Task.FromResult(result));

    /// <summary>
    /// Magic method that allows to `await` AsyncResult.
    /// </summary>
    public TaskAwaiter<Result<T, TError>> GetAwaiter() => _task.GetAwaiter();
}
