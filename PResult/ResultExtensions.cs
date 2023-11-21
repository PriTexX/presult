namespace PResult;

public static class ResultExtensions
{
    public static AsyncResult<T> ToAsyncResult<T>(this Task<Result<T>> task)
    {
        return task;
    }

    public static AsyncResult<T> ToAsyncResult<T>(this Task<T> task)
    {
        return task.ContinueWith<Result<T>>(
            finishedTask =>
                finishedTask.Exception is not null ? finishedTask.Exception : finishedTask.Result
        );
    }

    public static AsyncResult<Unit> ToAsyncResult(this Task task)
    {
        return task.ContinueWith<Result<Unit>>(
            finishedTask =>
                finishedTask.Exception is not null ? finishedTask.Exception : Unit.Default
        );
    }
}
