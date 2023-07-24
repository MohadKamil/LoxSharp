using LoxSharp.Expressions;

namespace LoxSharp.Syntax.Statements;

public abstract record Statement
{
    public abstract void Accept(IStatementVisitor visitor);
}

public record VarStatement(Token Identifier, Expression? Initializer) : Statement
{
    public override void Accept(IStatementVisitor visitor)
    {
        throw new NotImplementedException();
    }
}