namespace PResult;

public sealed class EmptyCtorInstantiationException : Exception
{
    public EmptyCtorInstantiationException()
        : base("You cannot use empty constructor of Result.") { }
}
