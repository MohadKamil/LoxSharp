using LoxSharp;

switch (args.Length)
{
    case > 1:
        Console.WriteLine("Incorrect invocation");
        break;
    case 1:
        Lox.RunFile(args[0]);
        break;
    default:
        Lox.RunPrompt();
        break;
}