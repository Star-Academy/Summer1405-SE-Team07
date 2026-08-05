using System;
using System.Collections.Generic;

namespace QueryLib
{
public sealed class Condition
{
    public string Column { get; }
    public object? Value { get; }

    public Condition(string column, object? value)
    {
        Column = column;
        Value = value;
    }
}

}