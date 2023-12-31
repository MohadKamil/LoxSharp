﻿using System.Text;

namespace LoxSharp.Expressions;

public class ASTPrinter: IVisitor<string>
{

    public string Print(Expression expression)
    {
        return expression.Accept(this);
    }
    public string VisitBinaryExpression(BinaryExpression expression)
    {
        return Parenthesize(expression.Operand.Lexeme,
            expression.Left, expression.Right);
    }

    public string VisitGroupingExpression(Grouping expression)
    {
        return Parenthesize("group", expression.Expression);
    }

    public string VisitLiteralExpression(Literal expression)
    {
        return expression.Value == null ? "nil" : expression.Value.ToString();
    }

    public string VisitUnaryExpression(Unary expression)
    {
        return Parenthesize(expression.Operator.Lexeme, expression.Right);
    }

    public string VisitVarExpression(VarExpression expression)
    {
        return expression.Name.Lexeme;
    }

    public string VisitAssignExpression(AssignExpression expression)
    {
        throw new NotImplementedException();
    }

    public string VisitLogicalExpression(LogicalExpression logicalExpression)
    {
        throw new NotImplementedException();
    }

    public string VisitCallExpression(CallExpression callExpression)
    {
        throw new NotImplementedException();
    }

    public string VisitGetExpression(GetExpression getExpression)
    {
        throw new NotImplementedException();
    }

    public string VisitSetExpression(SetExpression setExpression)
    {
        throw new NotImplementedException();
    }

    public string VisitThisExpression(ThisExpression thisExpression)
    {
        throw new NotImplementedException();
    }

    public string VisitSuperExpression(SuperExpression superExpression)
    {
        throw new NotImplementedException();
    }

    private string Parenthesize(string name, params Expression[] expressions) {
        var builder = new StringBuilder();

        builder.Append('(').Append(name);
        foreach (var expression in expressions)
        {
            builder.Append(' ');
            builder.Append(expression.Accept(this));
        }
        builder.Append(')');
        return builder.ToString();
    }
}