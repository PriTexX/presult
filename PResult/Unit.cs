namespace PResult;

/// <summary>
/// Special type that can be used instead of `void` when function returns no value.
/// </summary>
public readonly struct Unit
{
    /// <summary>
    /// Instance of <see cref="Unit"/>
    /// </summary>
    public static Unit Default { get; } = new();
}
