namespace LoxSharp.Expressions;

public abstract record Expression
{
    public abstract TR Accept<TR>(IVisitor<TR> visitor);
};

public record VarExpression(Token Name) : Expression
{
    public override TR Accept<TR>(IVisitor<TR> visitor)
    {
        return visitor.VisitVarExpression(this);
    }
}

public record BinaryExpression(Expression Left, Token Operand, Expression Right) : Expression
{
    public override TR Accept<TR>(IVisitor<TR> visitor)
    {
        return visitor.VisitBinaryExpression(this);
    }
}

public record Grouping(Expression Expression) : Expression
{
    public override TR Accept<TR>(IVisitor<TR> visitor)
    {
        return visitor.VisitGroupingExpression(this);
    }
}

public record Literal(object? Value) : Expression
{
    public override TR Accept<TR>(IVisitor<TR> visitor)
    {
        return visitor.VisitLiteralExpression(this);
    }
}

public record Unary(Token Operator, Expression Right) : Expression
{
    public override TR Accept<TR>(IVisitor<TR> visitor)
    {
        return visitor.VisitUnaryExpression(this);
    }
}