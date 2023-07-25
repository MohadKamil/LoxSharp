using LoxSharp.Syntax.Statements;

namespace LoxSharp.Interpreting;

public class LoxFunction : ICallable
{
    private readonly FunctionStatement statement;

    public LoxFunction(FunctionStatement statement)
    {
        this.statement = statement;
    }
    public object? Call(Interpreter interpreter, IEnumerable<object> arguments)
    {
        var functionEnvironment = new LoxEnvironment(Interpreter.Global);

        var argparams = arguments.Zip(statement.Params);

        foreach (var (arg, param) in argparams)
        {
            functionEnvironment.Define(param.Lexeme,arg);
        }
        
        interpreter.ExecuteBlock(statement.Body, functionEnvironment);
        return null;
    }

    public int Arity()
    {
        return statement.Params.Count();
    }
}