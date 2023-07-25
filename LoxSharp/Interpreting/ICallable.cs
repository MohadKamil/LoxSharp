namespace LoxSharp.Interpreting;

public interface ICallable
{
    object Call(Interpreter interpreter, IEnumerable<object> arguments);
    int Arity();
}