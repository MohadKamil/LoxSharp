namespace LoxSharp.Interpreting.RuntimeContainers;

public class LoxClass : ICallable
{
    private readonly string name;

    public LoxClass(string name)
    {
        this.name = name;
    }

    public override string ToString()
    {
        return name;
    }

    public object Call(Interpreter interpreter, IEnumerable<object> arguments)
    {
        var instance = new LoxInstance(this);
        return instance;
    }

    public int Arity() => 0;
}