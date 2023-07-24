namespace LoxSharp.Syntax.Statements;

public abstract record Statement
{
    public abstract void Accept(IStatementVisitor visitor);
}