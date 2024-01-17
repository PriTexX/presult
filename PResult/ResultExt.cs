namespace PResult;

public static class ResultExt
{
    public static AsyncResult<T> ToAsyncResult<T>(this Task<Result<T>> task)
    {
        return new AsyncResult<T>(task);
    }

    public static Result<T> Ok<T>(T val)
    {
        return new Result<T>(val);
    }

    public static Result<T> Err<T>(Exception error)
    {
        return new Result<T>(error);
    }
}
