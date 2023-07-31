namespace LoxSharp.Expressions;

public abstract record Expression
{
    public abstract TR Accept<TR>(IVisitor<TR> visitor);
};

public record ThisExpression(Token Keyword) : Expression
{
    public override TR Accept<TR>(IVisitor<TR> visitor)
    {
        return visitor.VisitThisExpression(this);
    }
}

public record SetExpression(Expression Object, Token Name,Expression Value) : Expression
{
    public override TR Accept<TR>(IVisitor<TR> visitor)
    {
        return visitor.VisitSetExpression(this);
    }
}

public record GetExpression(Expression Object,Token Name) : Expression
{
    public override TR Accept<TR>(IVisitor<TR> visitor)
    {
        return visitor.VisitGetExpression(this);
    }
}

public record CallExpression(Expression Callee, Token Paren, IEnumerable<Expression> Arguments) : Expression
{
    public override TR Accept<TR>(IVisitor<TR> visitor)
    {
        return visitor.VisitCallExpression(this);
    }
}

public record LogicalExpression(Expression Left, Token Token, Expression Right) : Expression
{
    public override TR Accept<TR>(IVisitor<TR> visitor)
    {
        return visitor.VisitLogicalExpression(this);
    }
}

public record AssignExpression(Token Name, Expression Value) : Expression
{
    public override TR Accept<TR>(IVisitor<TR> visitor)
    {
        return visitor.VisitAssignExpression(this);
    }
}

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