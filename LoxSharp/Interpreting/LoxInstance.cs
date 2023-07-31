namespace LoxSharp.Interpreting;

public class LoxInstance
{
    private readonly LoxClass loxClass;

    public LoxInstance(LoxClass loxClass)
    {
        this.loxClass = loxClass;
    }

    public override string ToString()
    {
        return $"{loxClass} Instance";
    }
}