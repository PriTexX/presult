namespace PResult;

public static class AsyncResultExt
{
    public static AsyncResult<T, TError> ToAsyncResult<T, TError>(
        this Task<Result<T, TError>> res
    ) => res;
}
