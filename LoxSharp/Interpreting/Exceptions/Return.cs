namespace LoxSharp.Interpreting.Exceptions;

public class Return : Exception
{
    public object? Value { get; }

    public Return(object? value)
    {
        Value = value;
    }
}