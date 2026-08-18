namespace QueryLib.Clauses;

public sealed class Condition
{
    public required string Column { get; init; }
    public  object? Value { get; init; }
}

