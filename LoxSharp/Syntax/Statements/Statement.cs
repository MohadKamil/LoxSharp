using LoxSharp.Expressions;

namespace LoxSharp.Syntax.Statements;

public abstract record Statement
{
    public abstract void Accept(IStatementVisitor visitor);
}

public record IfStatement(Expression Condition, Statement ThenBranch,Statement? ElseBranch) : Statement
{
    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitIfStatement(this);
    }
}

public record BlockStatement(IEnumerable<Statement> Statements) : Statement
{
    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitBlockStatement(this);
    }
}

public record VarStatement(Token Identifier, Expression? Initializer) : Statement
{
    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitVarStatement(this);
    }
}