# LoxSharp
This is a C# interpreter for the Lox language, following the great and detailed guide and book [*Crafting Interpreters*](https://craftinginterpreters.com/) By Robert Nystrom.

# How to run it

To run the interpreter:
- insure you have [dotnet 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) installed, verify using the version command

```
dotnet --version
```
- Navigate to the root folder containing the solution file and run the command:
```
dotnet run --project ./LoxSharp/LoxSharp.csproj
```

This will run the interpreter in REPL mode and you will be able to execute statements directly, the recording below has some example interaction with the REPL.

[![asciicast](https://asciinema.org/a/mXYIFs62LqEqoCKPehXakjOEH.svg)](https://asciinema.org/a/mXYIFs62LqEqoCKPehXakjOEH) 

### Passing Code Files

you can pass lox file to intepreter by passing a path to it as command argument, below is an example using on of the included Lox example files

```
dotnet run --project .\LoxSharp\LoxSharp.csproj .\Examples\HelloWorld.txt
```
