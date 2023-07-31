using LoxSharp.Interpreting.Exceptions;
using LoxSharp.Syntax.Statements;

namespace LoxSharp.Interpreting.RuntimeContainers;

public class LoxFunction : ICallable
{
    private readonly FunctionStatement statement;
    private readonly LoxEnvironment closure;

    public LoxFunction(FunctionStatement statement, LoxEnvironment closure)
    {
        this.statement = statement;
        this.closure = closure;
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
            return returnException.Value;
        }
        return null;
    }

    public int Arity()
    {
        return statement.Params.Count();
    }
}