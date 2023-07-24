using LoxSharp.Expressions;

namespace LoxSharp.Syntax.Statements;

public record PrintStatement(Expression Expression) : Statement
{
    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitPrintStatement(this);
    }
}