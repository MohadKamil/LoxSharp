namespace LoxSharp.Expressions;

public abstract record Expression;

public record BinaryExpression(Expression Left, Token Operand, Expression Right) : Expression;

public record Grouping(Expression Expression) : Expression;

public record Literal(object Value) : Expression;

public record Unary(Token Operator, Expression Right);