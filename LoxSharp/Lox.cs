using LoxSharp.Expressions;
using LoxSharp.Parsing;

namespace LoxSharp;

public class Lox
{
    private static bool _hadError;
    
    internal static void RunFile(string filePath)
    {
        var code = File.ReadAllText(filePath);
        ExecuteLoxCode(code);
        
        if(_hadError) 
            Environment.Exit(65);
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
                    _hadError = false;
                    break;
            }
        }
    }

    private static void ExecuteLoxCode(string code)
    {
        var scanner = new Scanner(code);

        var tokens = scanner.GetTokens().ToList();

        var parser = new Parser(tokens.ToList());

        var expression = parser.Parse();

        if (expression == null) return;
        
        Console.WriteLine(new ASTPrinter().Print(expression));
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
        _hadError = true;
    }
    
}