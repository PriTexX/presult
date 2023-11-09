using System.Runtime.CompilerServices;

namespace Result;

[AsyncMethodBuilder(typeof(AsyncResultMethodBuilder<>))]
public readonly struct AsyncResult<TValue>
{
    private readonly Task<Result<TValue>> _asyncResult;

    private AsyncResult(Task<Result<TValue>> asyncResult)
    {
        _asyncResult = asyncResult;
    }

    public static implicit operator AsyncResult<TValue>(Task<Result<TValue>> asyncResult)
    {
        return new AsyncResult<TValue>(asyncResult);
    }

    public Task<TRes> Match<TRes>(Func<TValue, TRes> success, Func<Exception, TRes> fail) => _asyncResult.ContinueWith(
        finishedTask =>
        {
            var res = finishedTask.Result;
            return res.Match(success, fail);
        });

    public Task<TRes> MatchAsync<TRes>(Func<TValue, Task<TRes>> success, Func<Exception, Task<TRes>> fail) =>
        _asyncResult.ContinueWith(
            finishedTask =>
            {
                var res = finishedTask.Result;
                return res.MatchAsync(success, fail);
            }).Unwrap();
    
    public Task<TRes> MatchAsync<TRes>(Func<TValue, Task<TRes>> success, Func<Exception, TRes> fail) =>
        _asyncResult.ContinueWith(
            finishedTask =>
            {
                var res = finishedTask.Result;
                return res.MatchAsync(success, fail);
            }).Unwrap();

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
        return _asyncResult.ContinueWith(finishedTask =>
        {
            var res = finishedTask.Result;

            return res.IsError ? Task.FromResult(new Result<K>(res.UnsafeError)) : next(res.UnsafeValue);
        }).Unwrap();
    }

    public TaskAwaiter<Result<TValue>> GetAwaiter()
    {
        return _asyncResult.GetAwaiter();
    }
}

public struct AsyncResultMethodBuilder<TResult>
{
    private AsyncResult<TResult> m_task;
    private AsyncTaskMethodBuilder<TResult> _taskBuilder = AsyncTaskMethodBuilder<TResult>.Create();

    public AsyncResultMethodBuilder()
    {
        m_task = default;
    }

    public static AsyncResultMethodBuilder<TResult> Create()
    {
        return default;
    }
    
    public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
    {
        _taskBuilder.Start(ref stateMachine);
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        _taskBuilder.SetStateMachine(stateMachine);
    }

    public void SetResult(TResult result)
    {
        _taskBuilder.SetResult(result);
    }
    public void SetException(Exception exception)
    {
        _taskBuilder.SetException(exception);
    }

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        _taskBuilder.AwaitOnCompleted(ref awaiter, ref stateMachine);
    }

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        _taskBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
    }

    public AsyncResult<TResult> Task => m_task;
}
