namespace PResult;

/// <summary>
/// Exception that is thrown if you try to instantiate empty Result ctor.
/// </summary>
/// <seealso cref="Result{T}"/>
internal sealed class EmptyCtorInstantiationException : Exception
{
    public EmptyCtorInstantiationException()
        : base("You cannot use empty constructor of Result.") { }
}
