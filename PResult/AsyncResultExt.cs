namespace PResult;

/// <summary>
/// Extensions of <see cref="AsyncResult{T}"/>
/// </summary>
public static class AsyncResultExt
{
    /// <summary>
    /// Calls <b><paramref name="ok"/></b> if <see cref="AsyncResult{T}"/> is in `Ok` state, otherwise calls <b><paramref name="err"/></b>.
    /// </summary>
    /// <param name="asyncRes">Result task</param>
    /// <param name="ok">Func that will be called on `Ok` state</param>
    /// <param name="err">Func that will be called on `Err` state</param>
    /// <typeparam name="T">Result type</typeparam>
    /// <typeparam name="TRes">Return type</typeparam>
    /// <returns>Task with result of one of provided functions</returns>
    /// <remarks><b><paramref name="ok"/></b> and <b><paramref name="err"/></b> must have the same return type</remarks>
    public static Task<TRes> Match<T, TRes>(
        this Task<Result<T>> asyncRes,
        Func<T, TRes> ok,
        Func<Exception, TRes> err
    ) => asyncRes.ToAsyncResult().Match(ok, err);

    /// <summary>
    /// Async version of <see cref="Match{T,TRes}">Match</see>
    /// </summary>
    /// <inheritdoc cref="Match{T,TRes}"/>
    public static Task<TRes> MatchAsync<T, TRes>(
        this Task<Result<T>> asyncRes,
        Func<T, Task<TRes>> ok,
        Func<Exception, Task<TRes>> err
    ) => asyncRes.ToAsyncResult().MatchAsync(ok, err);

    /// <summary>
    /// Async version of <see cref="Match{T,TRes}">Match</see>
    /// </summary>
    /// <inheritdoc cref="Match{T,TRes}"/>
    public static Task<TRes> MatchAsync<T, TRes>(
        this Task<Result<T>> asyncRes,
        Func<T, Task<TRes>> ok,
        Func<Exception, TRes> err
    ) => asyncRes.ToAsyncResult().MatchAsync(ok, err);

    /// <summary>
    /// Async version of <see cref="Match{T,TRes}">Match</see>
    /// </summary>
    /// <inheritdoc cref="Match{T,TRes}"/>
    public static Task<TRes> MatchAsync<T, TRes>(
        this Task<Result<T>> asyncRes,
        Func<T, TRes> ok,
        Func<Exception, Task<TRes>> err
    ) => asyncRes.ToAsyncResult().MatchAsync(ok, err);

    /// <summary>
    /// Calls <b><paramref name="next"/></b> if result is in `Ok` state, otherwise returns error value.
    /// </summary>
    /// <param name="asyncRes">Result task</param>
    /// <param name="next">Func that will be called with result value</param>
    /// <typeparam name="T">Result type</typeparam>
    /// <typeparam name="K">New type that can be returned from next function</typeparam>
    /// <remarks>You can use this method to control flow based on result values</remarks>
    /// <returns>New result that is produced from <b><paramref name="next"/></b> function</returns>
    public static AsyncResult<K> Then<T, K>(
        this Task<Result<T>> asyncRes,
        Func<T, Result<K>> next
    ) => asyncRes.ToAsyncResult().Then(next);

    /// <summary>
    /// Async version of <see cref="Then{T,K}">Then</see>.
    /// </summary>
    /// <returns>
    /// <para><see cref="AsyncResult{T}"/></para>
    /// <inheritdoc cref="Then{T,K}"/>
    /// </returns>
    /// <inheritdoc cref="Then{T,K}"/>
    public static AsyncResult<K> ThenAsync<T, K>(
        this Task<Result<T>> asyncRes,
        Func<T, Task<Result<K>>> next
    ) => asyncRes.ToAsyncResult().ThenAsync(next);

    /// <summary>
    /// Calls <b><paramref name="errNext"/></b> if result is in `Err` state, otherwise returns success value.
    /// </summary>
    /// <param name="asyncRes">Result task</param>
    /// <param name="errNext">Func that will be called with result error</param>
    /// <remarks>You can use this method to control flow based on result values</remarks>
    /// <returns>New result that is produced from <b><paramref name="errNext"/></b> function</returns>
    public static AsyncResult<T> ThenErr<T>(
        this Task<Result<T>> asyncRes,
        Func<Exception, Result<T>> errNext
    ) => asyncRes.ToAsyncResult().ThenErr(errNext);

    /// <summary>
    /// Async version of <see cref="ThenErr{T}">ThenErr</see>.
    /// </summary>
    /// <returns>
    /// <para><see cref="AsyncResult{T}"/></para>
    /// <inheritdoc cref="ThenErr{T}"/>
    /// </returns>
    /// <inheritdoc cref="ThenErr{T}"/>
    public static AsyncResult<T> ThenErrAsync<T>(
        this Task<Result<T>> asyncRes,
        Func<Exception, Task<Result<T>>> errNext
    ) => asyncRes.ToAsyncResult().ThenErrAsync(errNext);

    /// <summary>
    /// Maps <see cref="AsyncResult{T}"/> to <b>AsyncResult&lt;K&gt;</b> by applying a <b><paramref name="mapper"></paramref></b> to a success value, leaving error value.
    /// </summary>
    /// <param name="asyncRes">Result task</param>
    /// <param name="mapper">Func that maps result value to another</param>
    /// <typeparam name="T">Result type</typeparam>
    /// <typeparam name="K">Any type</typeparam>
    /// <returns><see cref="AsyncResult{T}"/> with new result value returned from <b><paramref name="mapper"/></b></returns>
    public static AsyncResult<K> Map<T, K>(this Task<Result<T>> asyncRes, Func<T, K> mapper) =>
        asyncRes.ToAsyncResult().Map(mapper);

    /// <summary>
    /// Async version of <see cref="Map{T,K}">Map</see>
    /// </summary>
    /// <inheritdoc cref="Map{T,K}"/>
    public static AsyncResult<K> MapAsync<T, K>(
        this Task<Result<T>> asyncRes,
        Func<T, Task<K>> asyncMapper
    ) => asyncRes.ToAsyncResult().MapAsync(asyncMapper);

    /// <summary>
    /// Maps error contained in result to a new one, leaving value untouched.
    /// </summary>
    /// <param name="asyncRes">Result task</param>
    /// <param name="errMapper">Func that maps error to another</param>
    /// <returns><see cref="Result{T}"/> with new error returned from <b><paramref name="errMapper"/></b></returns>
    public static AsyncResult<T> MapErr<T>(
        this Task<Result<T>> asyncRes,
        Func<Exception, Exception> errMapper
    ) => asyncRes.ToAsyncResult().MapErr(errMapper);

    /// <summary>
    /// Async version of <see cref="MapErr{T}">MapErr</see>
    /// </summary>
    /// <returns><see cref="AsyncResult{T}"/> with new error returned from <b><paramref name="asyncErrMapper"/></b></returns>
    /// <inheritdoc cref="MapErr{T}"/>
    public static AsyncResult<T> MapErrAsync<T>(
        this Task<Result<T>> asyncRes,
        Func<Exception, Task<Exception>> asyncErrMapper
    ) => asyncRes.ToAsyncResult().MapErrAsync(asyncErrMapper);
}
