namespace PResult;

/// <summary>
/// Thrown when you access `Unsafe` methods of <see cref="Result{T, TError}"/>.
/// </summary>
public sealed class InvalidResultStateException : Exception
{
    private const string OkState = "Ok";

    public InvalidResultStateException(string state)
        : base($"Cannot access result {GetValueWord(state)} in `{state}` state") { }

    private static string GetValueWord(string state)
    {
        return state == OkState ? "value" : "error";
    }
}
