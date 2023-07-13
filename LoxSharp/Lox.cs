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

        var tokens = scanner.GetTokens();

        foreach (var t in tokens)
        {
            Console.WriteLine(t);
        }
    }

    public static void Error(int line, string message) {
        Report(line, string.Empty, message);
    }

    private static void Report(int line, string where, string message) {
        Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
        _hadError = true;
    }
    
}