namespace Result;

public class Unit
{
    private Unit(){}
    
    public static Unit Default { get; } = new();
}