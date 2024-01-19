namespace PResult;

/// <summary>
/// Thrown when you access `Unsafe` methods of <see cref="Result{T}"/>.
/// </summary>
internal sealed class InvalidResultStateException : Exception
{
    public InvalidResultStateException(ResultState state)
        : base($"Cannot access result {GetValueWord(state)} in {GetStateName(state)} state") { }

    private static string GetValueWord(ResultState state)
    {
        return state == ResultState.Success ? "value" : "error";
    }

    private static string GetStateName(ResultState state)
    {
        return state == ResultState.Success ? "`Success`" : "`Error`";
    }
}
