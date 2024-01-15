namespace PResult;

public readonly struct Result<T> : IEquatable<Result<T>>
{
    private readonly ResultState _state;
    private readonly T _value;
    private readonly Exception _error;

    public Result()
    {
        throw new EmptyCtorInstantiationException();
    }

    public Result(T value)
    {
        _state = ResultState.Success;
        _value = value;
        _error = default!;
    }

    public Result(Exception error)
    {
        _state = ResultState.Failure;
        _value = default!;
        _error = error ?? throw new ArgumentNullException(nameof(error));
    }

    public bool IsSuccess => _state == ResultState.Success;
    public bool IsError => _state == ResultState.Failure;

    public TRes Match<TRes>(Func<T, TRes> success, Func<Exception, TRes> fail) =>
        IsSuccess ? success(_value) : fail(_error);

    public Task<TRes> MatchAsync<TRes>(
        Func<T, Task<TRes>> success,
        Func<Exception, Task<TRes>> fail
    ) => IsSuccess ? success(_value) : fail(_error);

    public Task<TRes> MatchAsync<TRes>(
        Func<T, Task<TRes>> success,
        Func<Exception, TRes> fail
    ) => IsSuccess ? success(_value) : Task.FromResult(fail(_error));

    public Task<TRes> MatchAsync<TRes>(
        Func<T, TRes> success,
        Func<Exception, Task<TRes>> fail
    ) => IsSuccess ? Task.FromResult(success(_value)) : fail(_error);

    public Result<K> Then<K>(Func<T, Result<K>> next)
    {
        return Match(next, err => err);
    }

    public AsyncResult<K> ThenAsync<K>(Func<T, Task<Result<K>>> next)
    {
        return Match(next, err => Task.FromResult<Result<K>>(err));
    }

    public Result<T> ThenErr(Func<Exception, Result<T>> errNext)
    {
        return Match(val => val, errNext);
    }

    public AsyncResult<T> ThenErrAsync(Func<Exception, Task<Result<T>>> errNext)
    {
        return Match(val => Task.FromResult(Ok(val)), errNext);
    }

    public Result<K> Map<K>(Func<T, K> mapper)
    {
        return Match<Result<K>>(val => mapper(val), err => err);
    }

    public AsyncResult<K> MapAsync<K>(Func<T, Task<K>> asyncMapper)
    {
        return Match(
            val => asyncMapper(val).ContinueWith(res => Result<K>.Ok(res.Result)),
            err => Task.FromResult(Result<K>.Err(err))
        );
    }

    public Result<T> MapErr<E>(Func<Exception, E> errMap)
        where E : Exception
    {
        return Match(val => val, err => Err(errMap(err)));
    }

    public AsyncResult<T> MapErrAsync<E>(Func<Exception, Task<E>> errMapAsync)
        where E : Exception
    {
        return Match(
            val => Task.FromResult<Result<T>>(val),
            err => errMapAsync(err).ContinueWith(res => Err(res.Result))
        );
    }

    public T UnsafeValue =>
        IsError ? throw new ArgumentException("Trying to access value in Failure state") : _value;

    public Exception UnsafeError =>
        IsSuccess ? throw new ArgumentException("Trying to access error in Success state") : _error;

    public static Result<T> Ok(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Err(Exception err)
    {
        return new Result<T>(err);
    }

    public static AsyncResult<T> FromTask(Task<T> task)
    {
        return task.ContinueWith<Result<T>>(c =>
        {
            if (c is { IsFaulted: true, Exception: not null })
            {
                return c.Exception;
            }

            return c.Result;
        });
        ;
    }

    public static AsyncResult<Unit> FromTask(Task task)
    {
        return task.ContinueWith<Result<Unit>>(c =>
        {
            if (c is { IsFaulted: true, Exception: not null })
            {
                return c.Exception;
            }

            return Unit.Default;
        });
        ;
    }

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Exception error) => new(error);

    public bool Equals(Result<T> other)
    {
        return _state == other._state
            && EqualityComparer<T>.Default.Equals(_value, other._value)
            && _error.Equals(other._error);
    }

    public override bool Equals(object? obj)
    {
        return obj is Result<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)_state, _value, _error);
    }

    public static bool operator ==(Result<T> left, Result<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Result<T> left, Result<T> right)
    {
        return !(left == right);
    }
}
