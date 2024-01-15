namespace PResult;

public static class ResultExtensions
{
    public static AsyncResult<T> ToAsyncResult<T>(this Task<Result<T>> task)
    {
        return task;
    }
}
