namespace LoxSharp.Interpreting;

public class LoxClass
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
}