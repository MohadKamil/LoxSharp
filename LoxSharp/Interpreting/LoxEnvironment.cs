using LoxSharp.Interpreting.Exceptions;

namespace LoxSharp.Interpreting;

public class LoxEnvironment
{
    private readonly IDictionary<string, object> values = new Dictionary<string, object>();

    private readonly LoxEnvironment? parentLoxEnvironment;

    public LoxEnvironment(): this(null)
    {
        
    }
    
    public LoxEnvironment(LoxEnvironment? parentLoxEnvironment)
    {
        this.parentLoxEnvironment = parentLoxEnvironment;
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
        if (parentLoxEnvironment is not null)
        {
            return parentLoxEnvironment.Get(name);
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

        if (parentLoxEnvironment is not null)
        {
            parentLoxEnvironment.Assign(name,value);
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
            environment = environment?.parentLoxEnvironment; 
        }

        return environment;
    }

    public void AssignAt(int depth, Token name, object value)
    {
        Ancestor(depth).values[name.Lexeme] = value;
    }
}