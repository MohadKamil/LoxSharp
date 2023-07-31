namespace LoxSharp.Interpreting.RuntimeContainers;

public class LoxClass : ICallable
{
    private readonly string name;
    private readonly IDictionary<string, LoxFunction> methods;

    public LoxClass(string name, IDictionary<string, LoxFunction> methods)
    {
        this.name = name;
        this.methods = methods;
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

    public LoxFunction? GetMethod(Token token)
    {
        return methods.TryGetValue(token.Lexeme, out var method) ? method : null;
    }
    
}