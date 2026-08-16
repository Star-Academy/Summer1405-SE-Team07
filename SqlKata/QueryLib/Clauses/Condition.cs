using System;
using System.Collections.Generic;

namespace QueryLib;

public sealed class Condition
{
    public required string Column { get; init; }
    public  object? Value { get; init; }
}

