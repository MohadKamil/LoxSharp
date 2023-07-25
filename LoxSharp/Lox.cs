using LoxSharp.Expressions;
using LoxSharp.Interpreting;
using LoxSharp.Interpreting.Exceptions;
using LoxSharp.Parsing;

namespace LoxSharp;

public class Lox
{
    private static bool _hadSyntaxError;
    private static bool hadRuntimeError;
    private static readonly Interpreter interpreter = new Interpreter();

    internal static void RunFile(string filePath)
    {
        var code = File.ReadAllText(filePath);
        ExecuteLoxCode(code);
        
        if(_hadSyntaxError) 
            Environment.Exit(65);
        if (hadRuntimeError) 
            Environment.Exit(70);
    }

    internal static void RunPrompt()
    {
        Console.WriteLine("Running Lox in REPL mode, Type 'exit' to close the session");
        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();

            switch (input)
            {
                case null:
                case "exit":
                    return;
                default:
                    ExecuteLoxCode(input);
                    _hadSyntaxError = false;
                    break;
            }
        }
    }

    private static void ExecuteLoxCode(string code)
    {
        var scanner = new Scanner(code);

        var tokens = scanner.GetTokens().ToList();

        var parser = new Parser(tokens.ToList());

        var statements = parser.Parse().ToList();

        if (_hadSyntaxError) return;
        
        interpreter.Interpret(statements);
    }
    
    internal static void RuntimeError(RuntimeException exception) {
        
        Console.WriteLine(exception.Message + Environment.NewLine + $"[Line {exception.Token.Line}]");
        hadRuntimeError = true;
    }

    public static void Error(int line, string message) {
        Report(line, string.Empty, message);
    }
    
    public static void Error(Token token, string message) {
        if (token.TokenType == TokenType.EOF) {
            Report(token.Line, " at end", message);
        } else {
            Report(token.Line, " at '" + token.Lexeme + "'", message);
        }
    }

    private static void Report(int line, string where, string message) {
        Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
        _hadSyntaxError = true;
    }
    
}