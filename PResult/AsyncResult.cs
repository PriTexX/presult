using System.Runtime.CompilerServices;

namespace PResult;

public readonly struct AsyncResult<T>
{
    private readonly Task<Result<T>> _asyncResult;

    public AsyncResult()
    {
        throw new EmptyCtorInstantiationException();
    }
    
    public AsyncResult(Task<Result<T>> asyncResult)
    {
        _asyncResult = asyncResult;
    }

    public Task<Result<T>> AsTask() => _asyncResult;

    public static implicit operator AsyncResult<T>(Task<Result<T>> asyncResult)
    {
        return new AsyncResult<T>(asyncResult);
    }

    public Task<TRes> Match<TRes>(Func<T, TRes> success, Func<Exception, TRes> fail) =>
        _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;
            return res.Match(success, fail);
        });

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

    public Task<TRes> MatchAsync<TRes>(
        Func<T, Task<TRes>> success,
        Func<Exception, TRes> fail
    ) =>
        _asyncResult
            .ContinueWith(finishedTask =>
            {
                var res = finishedTask.Result;
                return res.MatchAsync(success, fail);
            })
            .Unwrap();

    public Task<TRes> MatchAsync<TRes>(
        Func<T, TRes> success,
        Func<Exception, Task<TRes>> fail
    ) =>
        _asyncResult
            .ContinueWith(finishedTask =>
            {
                var res = finishedTask.Result;
                return res.MatchAsync(success, fail);
            })
            .Unwrap();

    public AsyncResult<K> Then<K>(Func<T, Result<K>> next)
    {
        return _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;
            return res.Then(next);
        });
    }

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

    public AsyncResult<T> ThenErr(Func<Exception, Result<T>> errNext)
    {
        return _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;
            return res.ThenErr(errNext);
        });
    }
    
    public AsyncResult<T> ThenErrAsync(Func<Exception, Task<Result<T>>> errNext)
    {
        return _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;
            return res.ThenErrAsync(errNext).AsTask();
        }).Unwrap();
    }

    public AsyncResult<K> Map<K>(Func<T, K> mapper)
    {
        return _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;
            return res.Map(mapper);
        });
    }

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

    public AsyncResult<T> MapErr<E>(Func<Exception, E> errMap)
        where E : Exception
    {
        return _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;
            return res.MapErr(errMap);
        });
    }

    public AsyncResult<T> MapErrAsync<E>(Func<Exception, Task<E>> mapErrAsync)
        where E : Exception
    {
        return _asyncResult
            .ContinueWith(finishedTask =>
            {
                var res = finishedTask.Result;

                return res.MapErrAsync(mapErrAsync).AsTask();
            })
            .Unwrap();
    }

    public TaskAwaiter<Result<T>> GetAwaiter()
    {
        return _asyncResult.GetAwaiter();
    }
}
