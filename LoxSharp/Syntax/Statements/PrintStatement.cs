namespace LoxSharp.Syntax.Statements;

public record PrintStatement(object Value) : Statement
{
    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitPrintStatement(this);
    }
}