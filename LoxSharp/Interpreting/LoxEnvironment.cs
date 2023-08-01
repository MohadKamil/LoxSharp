using LoxSharp.Interpreting.Exceptions;

namespace LoxSharp.Interpreting;

public class LoxEnvironment
{
    private readonly IDictionary<string, object> values = new Dictionary<string, object>();

    public readonly LoxEnvironment? ParentLoxEnvironment;

    public LoxEnvironment(): this(null)
    {
        
    }
    
    public LoxEnvironment(LoxEnvironment? parentLoxEnvironment)
    {
        ParentLoxEnvironment = parentLoxEnvironment;
    }

    public void Define(string name, object value)
    {
        values[name] = value;
    }

    public object Get(Token name)
    {
        if (values.TryGetValue(name.Lexeme, out var value))
        {
            return value;
        }
        if (ParentLoxEnvironment is not null)
        {
            return ParentLoxEnvironment.Get(name);
        }
        throw new RuntimeException(name, "Undefined variable '" + name.Lexeme + "'.");
    }

    public void Assign(Token name, object value)
    {
        if (values.ContainsKey(name.Lexeme))
        {
            values[name.Lexeme] = value;   
            return;
        }

        if (ParentLoxEnvironment is not null)
        {
            ParentLoxEnvironment.Assign(name,value);
            return;
        }
        throw new RuntimeException(name, "Undefined variable '" + name.Lexeme + "'.");
    }

    public object? GetAt(int depth, string lexeme)
    {
        return Ancestor(depth)?.values[lexeme];
    }

    private LoxEnvironment? Ancestor(int distance) {
        var environment = this;
        for (var i = 0; i < distance; i++) {
            environment = environment?.ParentLoxEnvironment; 
        }

        return environment;
    }

    public void AssignAt(int depth, Token name, object value)
    {
        Ancestor(depth).values[name.Lexeme] = value;
    }
}