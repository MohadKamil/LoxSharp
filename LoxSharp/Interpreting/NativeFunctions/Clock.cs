namespace LoxSharp.Interpreting.NativeFunctions;

public class Clock : ICallable
{
    public object Call(Interpreter interpreter, IEnumerable<object> arguments)
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    public int Arity() => 0;
}