namespace LoxSharp;
using static TokenType;
public class Scanner
{
    private readonly string code;
    private readonly IList<Token> tokens = new List<Token>();
    private int start = 0;
    private int current = 0;
    private int line = 1;
    private readonly int _codeLength;

    public Scanner(string code)
    {
        this.code = code;
        _codeLength = code.Length;
    }

    public IEnumerable<Token> GetTokens()
    {
        while (!AtEnd())
        {
            start = current;
            GenerateToken();
        }
        
        return tokens;
    }

    private void GenerateToken()
    {
        var c = Advance().ToString();
        switch (c)
        {
            case "(":
                AddToken(LEFT_PAREN);
                break;
            case ")":
                AddToken(RIGHT_PAREN);
                break;
            case "{":
                AddToken(LEFT_BRACE);
                break;
            case "}":
                AddToken(RIGHT_BRACE);
                break;
            case ",":
                AddToken(COMMA);
                break;
            case ".":
                AddToken(DOT);
                break;
            case "-":
                AddToken(MINUS);
                break;
            case "+":
                AddToken(PLUS);
                break;
            case ";":
                AddToken(SEMICOLON);
                break;
            case "*":
                AddToken(STAR);
                break;
            case "!":
                AddToken(Match("=") ? BANG_EQUAL : BANG);
                break;
            case "=":
                AddToken(Match("=") ? EQUAL_EQUAL : EQUAL);
                break;
            case "<":
                AddToken(Match("=") ? LESS_EQUAL : LESS);
                break;
            case ">":
                AddToken(Match("=") ? GREATER_EQUAL : GREATER);
                break;
            case "/":
                if (Match("/"))
                {
                    // A comment goes until the end of the line.
                    while (Peek() != Environment.NewLine && !AtEnd())
                    {
                        Advance();
                    }
                }
                else
                {
                    AddToken(SLASH);
                }

                break;
            case " ":
            case "\r":
            case "\t":
                // Ignore whitespace.
                break;
            case "\n":
            case "\r\n":
                line++;
                break;
            case "\"":
                StringToken();
                break;
            default:
                if (char.IsDigit(c[0]))
                {
                    NumberToken();
                }
                else
                {
                    Lox.Error(line, "Unexpected character");   
                }
                break;
        }
    }

    private void NumberToken()
    {
        while (char.IsDigit(Peek()[0]))
        {
            Advance();
        }

        if (Peek() == "." && char.IsDigit(PeekNext()[0]))
        {
            // Consume the "."
            Advance();
        }
        while (char.IsDigit(Peek()[0]))
        {
            Advance();
        }
        var value = code[start..current];
        AddToken(NUMBER, double.Parse(value));
    }

    private void StringToken()
    {
        while (Peek() != "\"" && !AtEnd()) 
        {
            if (Peek() == Environment.NewLine) line++;
            Advance();
        }

        if (AtEnd()) {
            Lox.Error(line, "Unterminated string.");
            return;
        }

        // The closing ".
        Advance();

        // Trim the surrounding quotes.
        var value = code[(start+1)..(current-1)];
        AddToken(STRING, value);
    }

    private string Peek()
    {
        return AtEnd() ? "\0" : code[current].ToString();
    }

    private char Advance()
    {
        return code[current++];
    }
    
    private void AddToken(TokenType tokenType, object? literal = null)
    {
        var lexeme = code.Substring(start, current);
        tokens.Add(new Token(tokenType, lexeme,literal, line));
    }

    private bool Match(string expected) {
        if (AtEnd())
        {
            return false;
        }
        if (code[current].ToString() != expected)
        {
            return false;
        }

        current++;
        return true;
    }

    private bool AtEnd()
    {
        return current >= _codeLength;
    }
    
    private string PeekNext()
    {
        return current + 1 >= code.Length ? "\0" : code[current + 1].ToString();
    }
}