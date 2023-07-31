using LoxSharp.Interpreting.Exceptions;

namespace LoxSharp.Interpreting.RuntimeContainers;

public class LoxInstance
{
    private readonly IDictionary<string, object> fields = new Dictionary<string, object>();
    private readonly LoxClass loxClass;

    public LoxInstance(LoxClass loxClass)
    {
        this.loxClass = loxClass;
    }

    public override string ToString()
    {
        return $"{loxClass} Instance";
    }

    public object Get(Token name)
    {
        if (fields.TryGetValue(name.Lexeme, out var field))
        {
            return field;
        }

        throw new RuntimeException(name, "Undefined property '" + name.Lexeme + "'.");
    }

    public object this[Token key]
    {
        get => Get(key);
        set
        {
            /* set the specified index to value here */
        }
    }
}