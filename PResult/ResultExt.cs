namespace PResult;

/// <summary>
/// Extensions of <see cref="Result{T}"/>
/// </summary>
public static class ResultExt
{
    /// <summary>
    /// Makes <see cref="AsyncResult{T}"/> from <b>Task&lt;Result&lt;T&gt;&gt;</b>.
    /// </summary>
    /// <param name="task">Task with result</param>
    /// <typeparam name="T">Any type</typeparam>
    /// <returns><see cref="AsyncResult{T}"/></returns>
    /// <remarks>Use this if method returns <b>Task&lt;Result&lt;T&gt;&gt;</b> but you want additionally modify result before awaiting it.</remarks>
    public static AsyncResult<T> ToAsyncResult<T>(this Task<Result<T>> task)
    {
        return new AsyncResult<T>(task);
    }

    /// <summary>
    /// Creates <see cref="Result{T}"/> from value.
    /// </summary>
    /// <param name="val">Any value</param>
    /// <typeparam name="T">Any type</typeparam>
    /// <returns><see cref="Result{T}"/></returns>
    public static Result<T> Ok<T>(T val)
    {
        return new Result<T>(val);
    }

    /// <summary>
    /// Creates <see cref="Result{T}"/> from error.
    /// </summary>
    /// <param name="err">Any error</param>
    /// <returns><see cref="Result{T}"/></returns>
    public static Result<T> Err<T>(Exception err)
    {
        return new Result<T>(err);
    }

    /// <summary>
    /// Accepts <see cref="Task{T}"/> and returns <see cref="AsyncResult{T}"/> with resolved value or thrown error.
    /// </summary>
    /// <returns><see cref="AsyncResult{T}">AsyncResult&lt;Unit&gt;</see></returns>
    /// <remarks>You can use this function for things like db or api calls to catch any unexpected error and map them, without using try/catch.</remarks>
    public static AsyncResult<Unit> FromTask(Task task)
    {
        return task.ContinueWith<Result<Unit>>(c =>
        {
            if (c is { IsFaulted: true, Exception: not null })
            {
                return c.Exception;
            }

            return Unit.Default;
        });   
    } 
    
    /// <param name="task">Any task</param>
    /// <returns><see cref="AsyncResult{T}"/></returns>
    /// <inheritdoc cref="FromTask(System.Threading.Tasks.Task)"/>
    public static AsyncResult<T> FromTask<T>(Task<T> task)
    {
        return task.ContinueWith<Result<T>>(c =>
        {
            if (c is { IsFaulted: true, Exception: not null })
            {
                return c.Exception;
            }

            return c.Result;
        });
    }

    /// <summary>
    /// Accepts <see cref="Action">Action</see> and returns <see cref="Result{T}"/> with returned value or thrown error.
    /// </summary>
    /// <param name="func">Delegate that may throw <see cref="Exception"/></param>
    /// <returns><see cref="Result{T}">Result&lt;Unit&gt;</see></returns>
    /// <remarks>You can use this function for delegates that may throw an error, to avoid try/catch.</remarks>
    public static Result<Unit> FromThrowable(Action func)
    {
        try
        {
            func.Invoke();
            return Unit.Default;
        }
        catch (Exception err)
        {
            return err;
        }
    }
    
    /// <summary>
    /// Accepts <see cref="Func{T}">Func</see> and returns <see cref="Result{T}"/> with returned value or thrown error.
    /// </summary>
    /// <typeparam name="T">Any type</typeparam>
    /// <returns><see cref="Result{T}"/></returns>
    public static Result<T> FromThrowable<T>(Func<T> func)
    {
        try
        {
            return func.Invoke();
        }
        catch (Exception err)
        {
            return err;
        }
    }
}
