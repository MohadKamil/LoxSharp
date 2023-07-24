using LoxSharp.Expressions;

namespace LoxSharp.Syntax.Statements;

public record ExpressionStatement(Expression Expression) : Statement
{
    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitExpressionStatement(this);
    }
}