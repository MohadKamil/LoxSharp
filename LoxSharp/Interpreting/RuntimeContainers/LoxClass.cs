namespace LoxSharp.Interpreting.RuntimeContainers;

public class LoxClass : ICallable
{
    private readonly string name;
    private readonly LoxClass? superClass;
    private readonly IDictionary<string, LoxFunction> methods;

    public LoxClass(string name, LoxClass? superClass, IDictionary<string, LoxFunction> methods)
    {
        this.name = name;
        this.superClass = superClass;
        this.methods = methods;
    }

    public override string ToString()
    {
        return name;
    }

    public object Call(Interpreter interpreter, IEnumerable<object> arguments)
    {
        var instance = new LoxInstance(this);
        // MK: design calls for returning the current object from init method, it's better if it's called constructor, just like in Kotlin (Secondary Constructors)
        var init = GetMethod("init");

        init?.Bind(instance).Call(interpreter, arguments);
        return instance;
    }

    public int Arity()
    {
        var init = GetMethod("init");
        return init?.Arity() ?? 0;
    }

    public LoxFunction? GetMethod(string methodName)
    {
        return methods.TryGetValue(methodName, out var method) 
            ? method 
            : superClass?.GetMethod(methodName);
    }
    
}