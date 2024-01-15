namespace PResult;

public readonly struct Result<TValue> : IEquatable<Result<TValue>>
{
    private readonly ResultState _state;
    private readonly TValue _value;
    private readonly Exception _error;

    public Result()
    {
        throw new EmptyCtorInstantiationException();
    }

    public Result(TValue value)
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

    public TRes Match<TRes>(Func<TValue, TRes> success, Func<Exception, TRes> fail) =>
        IsSuccess ? success(_value) : fail(_error);

    public Task<TRes> MatchAsync<TRes>(
        Func<TValue, Task<TRes>> success,
        Func<Exception, Task<TRes>> fail
    ) => IsSuccess ? success(_value) : fail(_error);

    public Task<TRes> MatchAsync<TRes>(
        Func<TValue, Task<TRes>> success,
        Func<Exception, TRes> fail
    ) => IsSuccess ? success(_value) : Task.FromResult(fail(_error));

    public Task<TRes> MatchAsync<TRes>(
        Func<TValue, TRes> success,
        Func<Exception, Task<TRes>> fail
    ) => IsSuccess ? Task.FromResult(success(_value)) : fail(_error);

    public Result<K> Then<K>(Func<TValue, Result<K>> next)
    {
        return Match(next, err => err);
    }

    public AsyncResult<K> ThenAsync<K>(Func<TValue, Task<Result<K>>> next)
    {
        return Match(next, err => Task.FromResult<Result<K>>(err));
    }

    public Result<K> Map<K>(Func<TValue, K> mapper)
    {
        return Match<Result<K>>(val => mapper(val), err => err);
    }

    public AsyncResult<K> MapAsync<K>(Func<TValue, Task<K>> asyncMapper)
    {
        return Match(
            val => asyncMapper(val).ContinueWith(res => Result<K>.Ok(res.Result)),
            err => Task.FromResult(Result<K>.Err(err))
        );
    }

    public Result<TValue> MapErr<E>(Func<Exception, E> errMap)
        where E : Exception
    {
        return Match(val => val, err => Err(errMap(err)));
    }

    public AsyncResult<TValue> MapErrAsync<E>(Func<Exception, Task<E>> errMapAsync)
        where E : Exception
    {
        return Match(
            val => Task.FromResult<Result<TValue>>(val),
            err => errMapAsync(err).ContinueWith(res => Err(res.Result))
        );
    }

    public TValue UnsafeValue =>
        IsError ? throw new ArgumentException("Trying to access value in Failure state") : _value;

    public Exception UnsafeError =>
        IsSuccess ? throw new ArgumentException("Trying to access error in Success state") : _error;

    public static Result<TValue> Ok(TValue value)
    {
        return new Result<TValue>(value);
    }

    public static Result<TValue> Err(Exception err)
    {
        return new Result<TValue>(err);
    }

    public static AsyncResult<TValue> FromTask(Task<TValue> task)
    {
        return task.ContinueWith<Result<TValue>>(c =>
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

    public static implicit operator Result<TValue>(TValue value) => new(value);

    public static implicit operator Result<TValue>(Exception error) => new(error);

    public bool Equals(Result<TValue> other)
    {
        return _state == other._state
            && EqualityComparer<TValue>.Default.Equals(_value, other._value)
            && _error.Equals(other._error);
    }

    public override bool Equals(object? obj)
    {
        return obj is Result<TValue> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)_state, _value, _error);
    }

    public static bool operator ==(Result<TValue> left, Result<TValue> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Result<TValue> left, Result<TValue> right)
    {
        return !(left == right);
    }
}
