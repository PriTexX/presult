using System.Diagnostics.CodeAnalysis;

namespace PResult;

public readonly struct Result<T, TError> : IEquatable<Result<T, TError>>
{
    private readonly State _state;
    private readonly T _value;
    private readonly TError _error;

    private enum State
    {
        Ok,
        Err
    }

    /// <summary>Creates a success result.</summary>
    internal Result(T value)
    {
        _state = State.Ok;
        _value = value;
        _error = default!;
    }

    /// <summary>Creates an error result.</summary>
    internal Result(TError err)
    {
        _state = State.Err;
        _value = default!;
        _error = err;
    }

    /// <summary>Checks if result is in `Ok` state.</summary>
    public bool IsOk() => _state == State.Ok;

    /// <summary>Checks if result is in `Err` state.</summary>
    public bool IsErr() => _state == State.Err;

    /// <summary>Handles success or error branch.</summary>
    public TRes Match<TRes>(Func<T, TRes> ok, Func<TError, TRes> err) =>
        IsOk() ? ok(_value) : err(_error);

    /// <summary>Runs provided computation on success.</summary>
    public Result<K, TError> Then<K>(Func<T, Result<K, TError>> next) => Match(next, err => err);

    /// <summary>Asynchronously runs provided computation on success.</summary>
    public AsyncResult<K, TError> ThenAsync<K>(Func<T, AsyncResult<K, TError>> next) =>
        Match(next, Result.ErrAsync<K, TError>);

    /// <summary>Asynchronously runs provided computation on success.</summary>
    public AsyncResult<K, TError> ThenAsync<K>(Func<T, Task<Result<K, TError>>> next) =>
        Match(v => next(v), Result.ErrAsync<K, TError>);

    /// <summary>Runs provided computation on error.</summary>
    public Result<T, TError> ThenErr(Func<TError, Result<T, TError>> errNext) =>
        Match(val => val, errNext);

    /// <summary>Asynchronously runs provided computation on error.</summary>
    public AsyncResult<T, TError> ThenErrAsync(Func<TError, AsyncResult<T, TError>> errNext) =>
        Match(Result.OkAsync<T, TError>, errNext);

    /// <summary>Asynchronously runs provided computation on error.</summary>
    public AsyncResult<T, TError> ThenErrAsync(Func<TError, Task<Result<T, TError>>> errNext) =>
        Match(Result.OkAsync<T, TError>, err => errNext(err));

    /// <summary>Transforms success value.</summary>
    public Result<K, TError> Map<K>(Func<T, K> mapper) =>
        Match(val => mapper(val), Result.Err<K, TError>);

    /// <summary>Asynchronously transforms success value.</summary>
    public AsyncResult<K, TError> MapAsync<K>(Func<T, Task<K>> mapper) =>
        Match(val => mapper(val).Continue(Result.Ok<K, TError>), Result.ErrAsync<K, TError>);

    /// <summary>Transforms error.</summary>
    public Result<T, KError> MapErr<KError>(Func<TError, KError> errMapper) =>
        Match(Result.Ok<T, KError>, err => errMapper(err));

    /// <summary>Asynchronously transforms error.</summary>
    public AsyncResult<T, KError> MapErrAsync<KError>(Func<TError, Task<KError>> errMapper) =>
        Match(Result.OkAsync<T, KError>, err => errMapper(err).Continue(Result.Err<T, KError>));

    /// <summary>Tries to get a value or error.</summary>
    public bool TryPickValue(
        [NotNullWhen(true)] out T? value,
        [NotNullWhen(false)] out TError? error
    )
    {
        if (IsOk())
        {
            value = _value;
            error = default;

            return true;
        }

        value = default;
        error = _error;

        return false;
    }

    /// <summary>Tries to get an error or value.</summary>
    public bool TryPickError(
        [NotNullWhen(true)] out TError? error,
        [NotNullWhen(false)] out T? value
    )
    {
        if (IsErr())
        {
            value = default;
            error = _error;

            return true;
        }

        value = _value;
        error = default;

        return false;
    }

    /// <summary>Returns success or fallback value.</summary>
    public T ValueOr(T fallback) => IsOk() ? _value : fallback;

    /// <summary>Gets success or throws.</summary>
    public T UnsafeValue =>
        IsOk() ? _value : throw new InvalidResultStateException(_state.ToString());

    /// <summary>Gets error or throws.</summary>
    public TError UnsafeError =>
        IsErr() ? _error : throw new InvalidResultStateException(_state.ToString());

    public static implicit operator Result<T, TError>(T value) => new(value);

    public static implicit operator Result<T, TError>(TError err) => new(err);

    public bool Equals(Result<T, TError> other)
    {
        if (_state != other._state)
        {
            return false;
        }

        return _state == State.Ok
            ? EqualityComparer<T>.Default.Equals(_value, other._value)
            : EqualityComparer<TError>.Default.Equals(_error, other._error);
    }

    public override bool Equals(object? obj) => obj is Result<T, TError> other && Equals(other);

    public override int GetHashCode() =>
        _state == State.Ok ? HashCode.Combine(0, _value) : HashCode.Combine(1, _error);

    public static bool operator ==(Result<T, TError> left, Result<T, TError> right) =>
        left.Equals(right);

    public static bool operator !=(Result<T, TError> left, Result<T, TError> right) =>
        !(left == right);
}
