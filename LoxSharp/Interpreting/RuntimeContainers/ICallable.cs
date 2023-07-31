namespace LoxSharp.Interpreting.RuntimeContainers;

public interface ICallable
{
    object Call(Interpreter interpreter, IEnumerable<object> arguments);
    int Arity();
}