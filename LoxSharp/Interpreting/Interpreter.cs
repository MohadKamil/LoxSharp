using System.Globalization;
using LoxSharp.Expressions;
using LoxSharp.Syntax.Statements;
using static LoxSharp.TokenType;

namespace LoxSharp.Interpreting;

public class Interpreter : IVisitor<object>, IStatementVisitor
{
    private readonly LoxEnvironment LoxEnvironment = new LoxEnvironment();

    internal void Interpret(IEnumerable<Statement> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeException exception)
        {
            Lox.RuntimeError(exception);
        }
    }

    private void Execute(Statement statement)
    {
        statement.Accept(this);
    }
    
    
    public object VisitBinaryExpression(BinaryExpression expression)
    {
        var right = Evaluate(expression.Right);
        var left = Evaluate(expression.Left);

        switch (expression.Operand.TokenType)
        {
            case PLUS:
                return left switch
                {
                    double leftDouble when right is double rightDouble => leftDouble + rightDouble,
                    string leftString when right is string rightString => leftString + rightString,
                    _ => throw new RuntimeException(expression.Operand, "Operands must be two numbers or two strings.")
                };
            case GREATER:
                CheckNumberOperand(expression.Operand, left,right);
                return (double)left > (double)right;
            case GREATER_EQUAL:
                CheckNumberOperand(expression.Operand, left,right);
                return (double)left >= (double)right;
            case LESS:
                CheckNumberOperand(expression.Operand, left,right);
                return (double)left < (double)right;
            case LESS_EQUAL:
                CheckNumberOperand(expression.Operand, left,right);
                return (double)left <= (double)right;
            case BANG_EQUAL:
                return !IsEqual(left, right);
            case EQUAL_EQUAL: 
                return IsEqual(left, right);
            case MINUS:
                CheckNumberOperand(expression.Operand, left,right);
                return (double)left - (double)right;
            case SLASH:
                CheckNumberOperand(expression.Operand, left,right);
                return (double)left / (double)right;
            case STAR:
                CheckNumberOperand(expression.Operand, left,right);
                return (double)left * (double)right;
            default:
                return null!;
        }
    }

    public object VisitGroupingExpression(Grouping expression)
    {
        return Evaluate(expression.Expression);
    }

    private static bool IsEqual(object? a, object? b)
    {
        return a switch
        {
            null when b == null => true,
            null => false,
            _ => a.Equals(b)
        };
    }

    public object VisitLiteralExpression(Literal expression)
    {
        return expression.Value;
    }

    public object VisitUnaryExpression(Unary expression)
    {
        var right = Evaluate(expression.Right);

        switch (expression.Operator.TokenType)
        {
            case TokenType.MINUS:
                CheckNumberOperand(expression.Operator, right);
                right = -(double)right;
                break;
            case TokenType.BANG:
                return !IsTruthy(right);
        }

        return right;
    }

    public object VisitVarExpression(VarExpression expression)
    {
        return LoxEnvironment.Get(expression.Name);
    }

    public object VisitAssignExpression(AssignExpression expression)
    {
        var value = Evaluate(expression.Value);
        LoxEnvironment.Assign(expression.Name, value);
        return value;
    }

    private static bool IsTruthy(object? @object)
    {
        return @object switch
        {
            null => false,
            bool b => b,
            _ => true
        };
    }
    
    private object Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }
    
    private static void CheckNumberOperand(Token @operator,params object[] operand) {
        if (operand.All(o => o is double)) return;
        throw new RuntimeException(@operator, "Operand must be a number.");
    }
    
    private static string? ToLoxString(object? @object) {
        switch (@object)
        {
            case null:
                return "nil";
            case double d:
            {
                var text = d.ToString(CultureInfo.InvariantCulture);
                if (text.EndsWith(".0")) {
                    text = text[..^2];
                }
                return text;
            }
            default:
                return @object.ToString();
        }
    }

    public void VisitExpressionStatement(ExpressionStatement statement)
    {
        var result = Evaluate(statement.Expression);
    }

    public void VisitPrintStatement(PrintStatement statement)
    {
        var value = Evaluate(statement.Expression);
        Console.WriteLine(ToLoxString(value));
    }

    public void VisitVarStatement(VarStatement statement)
    {
        object? value = null;
        if (statement.Initializer != null) {
            value = Evaluate(statement.Initializer);
        }

        LoxEnvironment.Define(statement.Identifier.Lexeme, value);
    }
}