using System.Runtime.CompilerServices;

namespace PResult;

/// <summary>
/// Similar to <see cref="Result{T}"/> but asynchronous and awaitable. Also all methods return <see cref="AsyncResult{T}"/> instead of common <see cref="Result{T}"/>
/// </summary>
/// <remarks>You cannot directly return <see cref="AsyncResult{T}"/> from async methods, instead you have to return <b>Task&lt;Result&lt;T&gt;&gt;</b></remarks>
/// <typeparam name="T">Any type</typeparam>
public readonly struct AsyncResult<T>
{
    private readonly Task<Result<T>> _asyncResult;

    /// <summary>
    /// <b>!!! Never user this parameterless constructor as it throws an error !!!</b>
    /// </summary>
    /// <exception cref="EmptyCtorInstantiationException">Always thrown</exception>
    public AsyncResult()
    {
        throw new EmptyCtorInstantiationException();
    }

    /// <summary>
    /// Async variant of <see cref="Result{T}"/>
    /// </summary>
    /// <param name="asyncResult">Task with <see cref="Result{T}"/></param>
    public AsyncResult(Task<Result<T>> asyncResult)
    {
        _asyncResult = asyncResult;
    }

    /// <summary>
    /// Gives you access to internal task
    /// </summary>
    /// <returns>Task with <see cref="Result{T}"/></returns>
    public Task<Result<T>> AsTask() => _asyncResult;

    /// <summary>
    /// Implicitly converts task with result to <see cref="AsyncResult{T}"/>.
    /// </summary>
    /// <param name="asyncResult">Task with <see cref="Result{T}"/></param>
    /// <returns><see cref="AsyncResult{T}"/></returns>
    public static implicit operator AsyncResult<T>(Task<Result<T>> asyncResult)
    {
        return new AsyncResult<T>(asyncResult);
    }

    /// <summary>
    /// Calls <b><paramref name="success"/></b> if <see cref="AsyncResult{T}"/> is in success state, otherwise calls <b><paramref name="fail"/></b>.
    /// </summary>
    /// <param name="success">Func that will be called on success state</param>
    /// <param name="fail">Func that will be called on error state</param>
    /// <typeparam name="TRes">Return type</typeparam>
    /// <returns>Task with result of one of provided functions</returns>
    /// <remarks><b><paramref name="success"/></b> and <b><paramref name="fail"/></b> must have the same return type</remarks>
    public Task<TRes> Match<TRes>(Func<T, TRes> success, Func<Exception, TRes> fail) =>
        _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;
            return res.Match(success, fail);
        });

    /// <summary>
    /// Async version of <see cref="Match{TRes}">Match</see>
    /// </summary>
    /// <inheritdoc cref="Match{TRes}"/>
    public Task<TRes> MatchAsync<TRes>(
        Func<T, Task<TRes>> success,
        Func<Exception, Task<TRes>> fail
    ) =>
        _asyncResult
            .ContinueWith(finishedTask =>
            {
                var res = finishedTask.Result;
                return res.MatchAsync(success, fail);
            })
            .Unwrap();

    /// <summary>
    /// Async version of <see cref="Match{TRes}">Match</see>
    /// </summary>
    /// <inheritdoc cref="Match{TRes}"/>
    public Task<TRes> MatchAsync<TRes>(Func<T, Task<TRes>> success, Func<Exception, TRes> fail) =>
        _asyncResult
            .ContinueWith(finishedTask =>
            {
                var res = finishedTask.Result;
                return res.MatchAsync(success, fail);
            })
            .Unwrap();

    /// <summary>
    /// Async version of <see cref="Match{TRes}">Match</see>
    /// </summary>
    /// <inheritdoc cref="Match{TRes}"/>
    public Task<TRes> MatchAsync<TRes>(Func<T, TRes> success, Func<Exception, Task<TRes>> fail) =>
        _asyncResult
            .ContinueWith(finishedTask =>
            {
                var res = finishedTask.Result;
                return res.MatchAsync(success, fail);
            })
            .Unwrap();

    /// <summary>
    /// Calls <b><paramref name="next"/></b> if result is in success state, otherwise returns error value.
    /// </summary>
    /// <param name="next">Func that will be called with result value</param>
    /// <typeparam name="K">New type that can be returned from next function</typeparam>
    /// <remarks>You can use this method to control flow based on result values</remarks>
    /// <returns>New result that is produced from <b><paramref name="next"/></b> function</returns>
    public AsyncResult<K> Then<K>(Func<T, Result<K>> next)
    {
        return _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;
            return res.Then(next);
        });
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
        return _asyncResult
            .ContinueWith(finishedTask =>
            {
                var res = finishedTask.Result;
                return res.ThenAsync(next).AsTask();
            })
            .Unwrap();
    }

    /// <summary>
    /// Calls <b><paramref name="errNext"/></b> if result is in error state, otherwise returns success value.
    /// </summary>
    /// <param name="errNext">Func that will be called with result error</param>
    /// <remarks>You can use this method to control flow based on result values</remarks>
    /// <returns>New result that is produced from <b><paramref name="errNext"/></b> function</returns>
    public AsyncResult<T> ThenErr(Func<Exception, Result<T>> errNext)
    {
        return _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;
            return res.ThenErr(errNext);
        });
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
        return _asyncResult
            .ContinueWith(finishedTask =>
            {
                var res = finishedTask.Result;
                return res.ThenErrAsync(errNext).AsTask();
            })
            .Unwrap();
    }

    /// <summary>
    /// Maps <see cref="AsyncResult{T}"/> to <b>AsyncResult&lt;K&gt;</b> by applying a <b><paramref name="mapper"></paramref></b> to a success value, leaving error value.
    /// </summary>
    /// <param name="mapper">Func that maps result value to another</param>
    /// <typeparam name="K">Any type</typeparam>
    /// <returns><see cref="AsyncResult{T}"/> with new result value returned from <b><paramref name="mapper"/></b></returns>
    public AsyncResult<K> Map<K>(Func<T, K> mapper)
    {
        return _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;
            return res.Map(mapper);
        });
    }

    /// <summary>
    /// Async version of <see cref="Map{K}">Map</see>
    /// </summary>
    /// <inheritdoc cref="Map{K}"/>
    public AsyncResult<K> MapAsync<K>(Func<T, Task<K>> asyncMapper)
    {
        return _asyncResult
            .ContinueWith(finishedTask =>
            {
                var res = finishedTask.Result;
                return res.MapAsync(asyncMapper).AsTask();
            })
            .Unwrap();
    }

    /// <summary>
    /// Maps error contained in result to a new one, leaving value untouched.
    /// </summary>
    /// <param name="errMapper">Func that maps error to another</param>
    /// <returns><see cref="Result{T}"/> with new error returned from <b><paramref name="errMapper"/></b></returns>
    public AsyncResult<T> MapErr(Func<Exception, Exception> errMapper)
    {
        return _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;
            return res.MapErr(errMapper);
        });
    }

    /// <summary>
    /// Async version of <see cref="MapErr">MapErr</see>
    /// </summary>
    /// <returns><see cref="AsyncResult{T}"/> with new error returned from <b><paramref name="asyncErrMapper"/></b></returns>
    /// <inheritdoc cref="MapErr"/>
    public AsyncResult<T> MapErrAsync(Func<Exception, Task<Exception>> asyncErrMapper)
    {
        return _asyncResult
            .ContinueWith(finishedTask =>
            {
                var res = finishedTask.Result;

                return res.MapErrAsync(asyncErrMapper).AsTask();
            })
            .Unwrap();
    }

    /// <summary>
    /// Magic method that allows `await` AsyncResult.
    /// </summary>
    /// <returns></returns>
    public TaskAwaiter<Result<T>> GetAwaiter()
    {
        return _asyncResult.GetAwaiter();
    }
}
