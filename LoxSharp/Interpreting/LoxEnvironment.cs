namespace LoxSharp.Interpreting;

public class LoxEnvironment
{
    private readonly IDictionary<string, object> values = new Dictionary<string, object>();

    public void Define(string name, object value)
    {
        values[name] = value;
    }

    public object Get(Token name)
    {
        if (!values.ContainsKey(name.Lexeme))
        {
            throw new RuntimeException(name, "Undefined variable '" + name.Lexeme + "'.");
        }
        return values[name.Lexeme];
    }
}