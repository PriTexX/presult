namespace PResult;

public static class InternalTaskExt
{
    public static Task<K> Continue<T, K>(this Task<T> task, Func<T, K> continueFn)
    {
        return task.ContinueWith(res => continueFn(res.Result));
    }
}
