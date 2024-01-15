﻿using System.Runtime.CompilerServices;

namespace PResult;

public readonly struct AsyncResult<TValue>
{
    private readonly Task<Result<TValue>> _asyncResult;

    private AsyncResult(Task<Result<TValue>> asyncResult)
    {
        _asyncResult = asyncResult;
    }

    public Task<Result<TValue>> AsTask() => _asyncResult;

    public static implicit operator AsyncResult<TValue>(Task<Result<TValue>> asyncResult)
    {
        return new AsyncResult<TValue>(asyncResult);
    }

    public Task<TRes> Match<TRes>(Func<TValue, TRes> success, Func<Exception, TRes> fail) =>
        _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;
            return res.Match(success, fail);
        });

    public Task<TRes> MatchAsync<TRes>(
        Func<TValue, Task<TRes>> success,
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
        Func<TValue, Task<TRes>> success,
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
        Func<TValue, TRes> success,
        Func<Exception, Task<TRes>> fail
    ) =>
        _asyncResult
            .ContinueWith(finishedTask =>
            {
                var res = finishedTask.Result;
                return res.MatchAsync(success, fail);
            })
            .Unwrap();

    public AsyncResult<K> Then<K>(Func<TValue, Result<K>> next)
    {
        return _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;
            return res.Then(next);
        });
    }

    public AsyncResult<K> ThenAsync<K>(Func<TValue, Task<Result<K>>> next)
    {
        return _asyncResult
            .ContinueWith(finishedTask =>
            {
                var res = finishedTask.Result;

                return res.IsError
                    ? Task.FromResult(new Result<K>(res.UnsafeError))
                    : next(res.UnsafeValue);
            })
            .Unwrap();
    }

    public AsyncResult<K> Map<K>(Func<TValue, K> mapper)
    {
        return _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;
            return res.Map(mapper);
        });
    }

    public AsyncResult<K> MapAsync<K>(Func<TValue, Task<K>> asyncMapper)
    {
        return _asyncResult
            .ContinueWith(finishedTask =>
            {
                var res = finishedTask.Result;
                return res.MapAsync(asyncMapper).AsTask();
            })
            .Unwrap();
    }

    public AsyncResult<TValue> MapErr<E>(Func<Exception, E> errMap)
        where E : Exception
    {
        return _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;
            return res.MapErr(errMap);
        });
    }

    public AsyncResult<TValue> MapErrAsync<E>(Func<Exception, Task<E>> mapErrAsync)
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

    public TaskAwaiter<Result<TValue>> GetAwaiter()
    {
        return _asyncResult.GetAwaiter();
    }
}
