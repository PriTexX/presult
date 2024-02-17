namespace PResult;

/// <summary>
/// This struct represents either success or error.
/// </summary>
/// <remarks>
/// Return it from methods when they are expected to have
/// recoverable errors instead of throwing them.
/// </remarks>
/// <typeparam name="T">Any type</typeparam>
public readonly struct Result<T> : IEquatable<Result<T>>
{
    private readonly ResultState _state;
    private readonly T _value;
    private readonly Exception _error;

    /// <summary>
    /// <b>!!! Never user this parameterless constructor as it throws an error !!!</b>
    /// </summary>
    /// <exception cref="EmptyCtorInstantiationException">Always thrown</exception>
    public Result()
    {
        throw new EmptyCtorInstantiationException();
    }

    /// <summary>
    /// Success variant of <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="value">Any value</param>
    public Result(T value)
    {
        _state = ResultState.Ok;
        _value = value;
        _error = default!;
    }

    /// <summary>
    /// Error variant of <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="err">Any error that occured in method.</param>
    /// <remarks><b><paramref name="err"/></b> must be <see cref="Exception"/></remarks>
    /// <exception cref="ArgumentNullException">Passed <b><paramref name="err"/></b> is null</exception>
    public Result(Exception err)
    {
        _state = ResultState.Err;
        _value = default!;
        _error = err ?? throw new ArgumentNullException(nameof(err));
    }

    /// <summary>
    /// Returns true if <see cref="Result{T}"/> is in `Ok` state.
    /// </summary>
    public bool IsOk => _state == ResultState.Ok;

    /// <summary>
    /// Returns true if <see cref="Result{T}"/> is in `Err` state.
    /// </summary>
    public bool IsErr => _state == ResultState.Err;

    /// <summary>
    /// Calls <b><paramref name="ok"/></b> if <see cref="Result{T}"/> is in `Ok` state, otherwise calls <b><paramref name="err"/></b>.
    /// </summary>
    /// <param name="ok">Func that will be called on `Ok` state</param>
    /// <param name="err">Func that will be called on `Err` state</param>
    /// <typeparam name="TRes">Return type</typeparam>
    /// <returns>Result of one of provided functions</returns>
    /// <remarks><b><paramref name="ok"/></b> and <b><paramref name="err"/></b> must have the same return type</remarks>
    public TRes Match<TRes>(Func<T, TRes> ok, Func<Exception, TRes> err) =>
        IsOk ? ok(_value) : err(_error);

    /// <summary>
    /// Async version of <see cref="Match{TRes}">Match</see>
    /// </summary>
    /// <inheritdoc cref="Match{TRes}"/>
    public Task<TRes> MatchAsync<TRes>(Func<T, Task<TRes>> ok, Func<Exception, Task<TRes>> err) =>
        IsOk ? ok(_value) : err(_error);

    /// <summary>
    /// Async version of <see cref="Match{TRes}">Match</see>
    /// </summary>
    /// <inheritdoc cref="Match{TRes}"/>
    public Task<TRes> MatchAsync<TRes>(Func<T, Task<TRes>> ok, Func<Exception, TRes> err) =>
        IsOk ? ok(_value) : Task.FromResult(err(_error));

    /// <summary>
    /// Async version of <see cref="Match{TRes}">Match</see>
    /// </summary>
    /// <inheritdoc cref="Match{TRes}"/>
    public Task<TRes> MatchAsync<TRes>(Func<T, TRes> ok, Func<Exception, Task<TRes>> err) =>
        IsOk ? Task.FromResult(ok(_value)) : err(_error);

    /// <summary>
    /// Calls <b><paramref name="next"/></b> if result is in `Ok` state, otherwise returns error value.
    /// </summary>
    /// <param name="next">Func that will be called with result value</param>
    /// <typeparam name="K">New type that can be returned from next function</typeparam>
    /// <remarks>You can use this method to control flow based on result values</remarks>
    /// <returns>New result that is produced from <b><paramref name="next"/></b> function</returns>
    public Result<K> Then<K>(Func<T, Result<K>> next)
    {
        return Match(next, err => err);
    }

    /// <summary>
    /// Async version of <see cref="Then{K}">Then</see>.
    /// </summary>
    /// <returns>
    /// <para><see cref="AsyncResult{T}"/></para>
    /// <inheritdoc cref="Then{K}"/>
    /// </returns>
    /// <inheritdoc cref="Then{K}"/>
    public AsyncResult<K> ThenAsync<K>(Func<T, Task<Result<K>>> next)
    {
        return Match(next, err => Task.FromResult<Result<K>>(err));
    }

    /// <summary>
    /// Calls <b><paramref name="errNext"/></b> if result is in `Err` state, otherwise returns success value.
    /// </summary>
    /// <param name="errNext">Func that will be called with result error</param>
    /// <remarks>You can use this method to control flow based on result values</remarks>
    /// <returns>New result that is produced from <b><paramref name="errNext"/></b> function</returns>
    public Result<T> ThenErr(Func<Exception, Result<T>> errNext)
    {
        return Match(val => val, errNext);
    }

    /// <summary>
    /// Async version of <see cref="ThenErr">ThenErr</see>.
    /// </summary>
    /// <returns>
    /// <para><see cref="AsyncResult{T}"/></para>
    /// <inheritdoc cref="ThenErr"/>
    /// </returns>
    /// <inheritdoc cref="ThenErr"/>
    public AsyncResult<T> ThenErrAsync(Func<Exception, Task<Result<T>>> errNext)
    {
        return Match(val => Task.FromResult(ResultExt.Ok(val)), errNext);
    }

    /// <summary>
    /// Maps <see cref="Result{T}"/> to <b>Result&lt;K&gt;</b> by applying a <b><paramref name="mapper"></paramref></b> to a success value, leaving error value.
    /// </summary>
    /// <param name="mapper">Func that maps result value to another</param>
    /// <typeparam name="K">Any type</typeparam>
    /// <returns><see cref="Result{T}"/> with new result value returned from <b><paramref name="mapper"/></b></returns>
    public Result<K> Map<K>(Func<T, K> mapper)
    {
        return Match<Result<K>>(val => mapper(val), err => err);
    }

    /// <summary>
    /// Async version of <see cref="Map{K}">Map</see>
    /// </summary>
    /// <inheritdoc cref="Map{K}"/>
    public AsyncResult<K> MapAsync<K>(Func<T, Task<K>> asyncMapper)
    {
        return Match(
            val => asyncMapper(val).ContinueWith(res => ResultExt.Ok<K>(res.Result)),
            err => Task.FromResult(ResultExt.Err<K>(err))
        );
    }

    /// <summary>
    /// Maps error contained in result to a new one, leaving value untouched.
    /// </summary>
    /// <param name="errMapper">Func that maps error to another</param>
    /// <returns><see cref="Result{T}"/> with new error returned from <b><paramref name="errMapper"/></b></returns>
    public Result<T> MapErr(Func<Exception, Exception> errMapper)
    {
        return Match(val => val, err => ResultExt.Err<T>(errMapper(err)));
    }

    /// <summary>
    /// Async version of <see cref="MapErr">MapErr</see>
    /// </summary>
    /// <returns><see cref="AsyncResult{T}"/> with new error returned from <b><paramref name="asyncErrMapper"/></b></returns>
    /// <inheritdoc cref="MapErr"/>
    public AsyncResult<T> MapErrAsync(Func<Exception, Task<Exception>> asyncErrMapper)
    {
        return Match(
            val => Task.FromResult<Result<T>>(val),
            err => asyncErrMapper(err).ContinueWith(res => ResultExt.Err<T>(res.Result))
        );
    }

    /// <summary>
    /// You can access result value by this property, try to use <see cref="Match{TRes}">Match</see> whenever it's possible. But if you need to access value directly make sure it is safe by checking properties <see cref="IsOk"/> or <see cref="IsErr"/>.
    /// </summary>
    /// <exception cref="InvalidResultStateException">If result you are trying to access is in `Err` state</exception>
    public T UnsafeValue => IsErr ? throw new InvalidResultStateException(_state) : _value;

    /// <summary>
    /// You can access result error by this property, try to use <see cref="Match{TRes}">Match</see> whenever it's possible. But if you need to access error directly make sure it is safe by checking properties <see cref="IsOk"/> or <see cref="IsErr"/>.
    /// </summary>
    /// <exception cref="InvalidResultStateException">If result you are trying to access is in `Ok` state</exception>
    public Exception UnsafeError => IsOk ? throw new InvalidResultStateException(_state) : _error;

    /// <summary>
    /// Implicitly converts value to <see cref="Result{T}"/>
    /// </summary>
    public static implicit operator Result<T>(T value) => new(value);

    /// <summary>
    /// Implicitly converts <see cref="Exception"/> to <see cref="Result{T}"/>
    /// </summary>
    public static implicit operator Result<T>(Exception err) => new(err);

    /// <summary>
    /// Checks whether two results are equal
    /// </summary>
    public bool Equals(Result<T> other)
    {
        return _state == other._state
            && EqualityComparer<T>.Default.Equals(_value, other._value)
            && _error.Equals(other._error);
    }

    /// <inheritdoc cref="Equals(PResult.Result{T})"/>
    public override bool Equals(object? obj)
    {
        return obj is Result<T> other && Equals(other);
    }

    /// <summary>
    /// Gets hash code for result
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine((int)_state, _value, _error);
    }

    /// <inheritdoc cref="Equals(PResult.Result{T})"/>
    public static bool operator ==(Result<T> left, Result<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Checks whether two results are not equal
    /// </summary>
    public static bool operator !=(Result<T> left, Result<T> right)
    {
        return !(left == right);
    }
}
