﻿using LoxSharp.Interpreting.Exceptions;
using LoxSharp.Syntax.Statements;

namespace LoxSharp.Interpreting.RuntimeContainers;

public class LoxFunction : ICallable
{
    private readonly FunctionStatement statement;
    private readonly LoxEnvironment closure;
    private readonly bool isInitializer;

    public LoxFunction(FunctionStatement statement, LoxEnvironment closure, bool isInitializer)
    {
        this.statement = statement;
        this.closure = closure;
        this.isInitializer = isInitializer;
    }
    public object? Call(Interpreter interpreter, IEnumerable<object> arguments)
    {
        var functionEnvironment = new LoxEnvironment(closure);

        var argparams = arguments.Zip(statement.Params);

        foreach (var (arg, param) in argparams)
        {
            functionEnvironment.Define(param.Lexeme,arg);
        }
        
        try
        {
            interpreter.ExecuteBlock(statement.Body, functionEnvironment);
        }
        catch (ReturnException returnException)
        {
            return isInitializer ? GetThis() : returnException.Value;
        }

        return isInitializer ? GetThis() : null;

        object? GetThis()
        {
            return closure.GetAt(0, "this");
        }
    }

    public LoxFunction Bind(LoxInstance instance)
    {
        var environment = new LoxEnvironment(closure);
        environment.Define("this", instance);
        return new LoxFunction(statement, environment,isInitializer);
    }
    
    public int Arity()
    {
        return statement.Params.Count();
    }
    
}