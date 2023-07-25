namespace LoxSharp.Interpreting.Exceptions;

public class RuntimeException : Exception
{
    public Token Token { get; }

    public RuntimeException(Token token, string message) : base(message)
    {
        this.Token = token;
    }
}