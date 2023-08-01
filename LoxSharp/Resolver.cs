using LoxSharp.Expressions;
using LoxSharp.Interpreting;
using LoxSharp.Syntax.Statements;

namespace LoxSharp;

public class Resolver : IStatementVisitor, IVisitor<object?>
{
    enum ClassType
    {
        None = 0,
        Class
    }
    private ClassType currentClass = ClassType.None;
    enum FunctionType
    {
        None = 0,
        Function,
        Initializer,
        Method
    }
    private FunctionType currentFunction = FunctionType.None;
    private readonly Stack<IDictionary<string, bool>> scopes = new();
    private readonly Interpreter interpreter;

    public Resolver(Interpreter interpreter)
    {
        this.interpreter = interpreter;
    }

    public void VisitExpressionStatement(ExpressionStatement statement)
    {
        Resolve(statement.Expression);
    }

    public void VisitPrintStatement(PrintStatement statement)
    {
        Resolve(statement.Expression);
    }

    public void VisitVarStatement(VarStatement statement)
    {
        Declare(statement.Identifier);
        if (statement.Initializer is not null) {
            Resolve(statement.Initializer);
        }
        Define(statement.Identifier);
    }

    private void Declare(Token identifier)
    {
        if (!scopes.Any())
        {
            return;
        }

        var scope = scopes.Peek();
        scope[identifier.Lexeme] = false;
    }
    
    private void Define(Token identifier)
    {
        if (!scopes.Any()) return;
        scopes.Peek()[identifier.Lexeme] = true;
    }

    public void VisitBlockStatement(BlockStatement statement)
    {
        BeginScope();
        Resolve(statement.Statements.ToArray());
        EndScope();
    }

    private void EndScope()
    {
        scopes.Pop();
    }

    private void BeginScope()
    {
        scopes.Push(new Dictionary<string, bool>());
    }

    public void Resolve(params Statement[] statements) {
        foreach (var statement in statements)
        {
            statement.Accept(this);
        }
    }
    
    private void Resolve(Expression expr) {
        expr.Accept(this);
    }
    public void VisitIfStatement(IfStatement statement)
    {
        Resolve(statement.Condition);
        Resolve(statement.ThenBranch);
        if (statement.ElseBranch is not null)
        {
            Resolve(statement.ThenBranch);
        }
    }

    public void VisitWhileStatement(WhileStatement whileStatement)
    {
        Resolve(whileStatement.Condition);
        Resolve(whileStatement.Body);
    }

    public void VisitFunctionStatement(FunctionStatement functionStatement)
    {
        Declare(functionStatement.Name);
        Define(functionStatement.Name);

        ResolveFunction(functionStatement,FunctionType.Function);
    }
    
    private void ResolveFunction(FunctionStatement function, FunctionType functionType)
    {
        var enclosingFunction = currentFunction;
        currentFunction = functionType;
        BeginScope();
        foreach (var param in function.Params)
        {
            Declare(param);
            Define(param);
        }
        Resolve(function.Body.ToArray());
        EndScope();
        currentFunction = enclosingFunction;
    }

    public void VisitReturnStatement(ReturnStatement returnStatement)
    {
        if (returnStatement.Value != null)
        {
            if (currentFunction == FunctionType.Initializer)
            {
                Lox.Error(returnStatement.ReturnKeyword,"Can't return a value from an initializer.");
            }
            Resolve(returnStatement.Value);
        }
    }

    public void VisitClassStatement(ClassStatement classStatement)
    {
        var enclosingClass = currentClass;
        currentClass = ClassType.Class;
        Declare(classStatement.Name);
        Define(classStatement.Name);
        
        if (classStatement.SuperClass != null)
        {
            if (classStatement.SuperClass.Name.Lexeme == classStatement.Name.Lexeme)
            {
                Lox.Error(classStatement.SuperClass.Name,
                    "A class can't inherit from itself.");
            }
            Resolve(classStatement.SuperClass);
            BeginScope();
            scopes.Peek()["super"] = true;
        }
        BeginScope();
        scopes.Peek()["this"] = true;

        foreach (var method in classStatement.Methods)
        {
            var functionType = FunctionType.Method;
            if (method.Name.Lexeme == "init")
            {
                functionType = FunctionType.Initializer;
            }
            ResolveFunction(method,functionType);
        }

        if (classStatement.SuperClass != null)
        {
            EndScope();
        }
        EndScope();
        currentClass = enclosingClass;
    }

    public object? VisitBinaryExpression(BinaryExpression expression)
    {
        Resolve(expression.Left);
        Resolve(expression.Right);
        return null;
    }

    public object? VisitGroupingExpression(Grouping expression)
    {
        Resolve(expression.Expression);
        return null;
    }

    public object? VisitLiteralExpression(Literal expression)
    {
        return null;
    }

    public object? VisitUnaryExpression(Unary expression)
    {
        Resolve(expression.Right);
        return null;
    }

    public object? VisitVarExpression(VarExpression expression)
    {
        if (scopes.Any())
        {
            scopes.Peek().TryGetValue(expression.Name.Lexeme, out var found);
            if (!found)
            {
                Lox.Error(expression.Name,"Can't read local variable in its own initializer.");   
            }
        }

        ResolveLocal(expression, expression.Name);
        return null;
    }

    private void ResolveLocal(Expression expression, Token name)
    {
        for (var i = scopes.Count - 1; i >= 0; i--) {
            if (scopes.ToArray()[i].ContainsKey(name.Lexeme))
            {
                interpreter.Resolve(expression, scopes.Count - 1 - i);
            }
        }
    }

    public object? VisitAssignExpression(AssignExpression expression)
    {
        Resolve(expression.Value);
        ResolveLocal(expression, expression.Name);
        return null;
    }

    public object? VisitLogicalExpression(LogicalExpression logicalExpression)
    {
        Resolve(logicalExpression.Left);
        Resolve(logicalExpression.Right);
        return null;
    }

    public object? VisitCallExpression(CallExpression callExpression)
    {
        Resolve(callExpression.Callee);
        foreach (var arg in callExpression.Arguments)
        {
            Resolve(arg);
        }

        return null;
    }

    public object? VisitGetExpression(GetExpression getExpression)
    {
        Resolve(getExpression.Object);
        return null;
    }

    public object? VisitSetExpression(SetExpression setExpression)
    {
        Resolve(setExpression.Value);
        Resolve(setExpression.Object);
        return null;
    }

    public object? VisitThisExpression(ThisExpression thisExpression)
    {
        if (currentClass != ClassType.Class)
        {
            Lox.Error(thisExpression.Keyword,"Can't use 'this' outside of a class.");
            return null;
        }
        ResolveLocal(thisExpression,thisExpression.Keyword);
        return null;
    }

    public object? VisitSuperExpression(SuperExpression superExpression)
    {
        ResolveLocal(superExpression,superExpression.Keyword);
        return null;
    }
}