namespace PResult;

/// <summary>
/// Special class that represents successful result.
/// </summary>
/// <remarks>Use it instead of `void` when function returns no result.</remarks>
public sealed class Unit
{
    private Unit() { }

    /// <summary>
    /// Returns instance of <see cref="Unit"/>
    /// </summary>
    public static Unit Default { get; } = new();
}
