﻿using LoxSharp.Expressions;

namespace LoxSharp.Syntax.Statements;

public abstract record Statement
{
    public abstract void Accept(IStatementVisitor visitor);
}

public record ClassStatement(Token Name, VarExpression? SuperClass, IEnumerable<FunctionStatement> Methods) : Statement
{
    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitClassStatement(this);
    }
}

public record ReturnStatement(Token ReturnKeyword, Expression? Value) : Statement
{
    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitReturnStatement(this);
    }
}

public record FunctionStatement(Token Name,IEnumerable<Token> Params,IEnumerable<Statement> Body) : Statement
{
    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitFunctionStatement(this);
    }
}

public record WhileStatement(Expression Condition, Statement Body) : Statement
{
    public override void Accept(IStatementVisitor visitor)
    {
        visitor.VisitWhileStatement(this);
    }
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