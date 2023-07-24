using LoxSharp.Expressions;
using LoxSharp.Syntax.Statements;
using static LoxSharp.TokenType;

namespace LoxSharp.Parsing;

public class Parser
{
    private readonly IList<Token> tokens;
    private int current = 0;

    public Parser(IList<Token> tokens)
    {
        this.tokens = tokens;
    }
    

    internal IEnumerable<Statement> Parse()
    {
        var statements = new List<Statement>();
        try
        {
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
        }
        catch (ParseError)
        {
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return statements;
    }
    private Expression Expression()
    {
        return Equality();
    }

    private Statement? Declaration()
    {
        try
        {
            return Match(VAR) 
                ? VarDeclaration() 
                : Statement();
        }
        catch (ParseError)
        {
            Synchronize();
            return null;
        }
    }

    private Statement VarDeclaration()
    {
        var name = Consume(IDENTIFIER, "Expect variable name.");

        Expression? initializer = null;
        if (Match(EQUAL)) {
            initializer = Expression();
        }

        Consume(SEMICOLON, "Expect ';' after variable declaration.");
        return new VarStatement(name, initializer);
    }

    private Statement Statement()
    {
        return Match(PRINT) ? PrintStatement() : ExpressionStatement();

        PrintStatement PrintStatement()
        {
            var expression = Expression();
            Consume(SEMICOLON, "Expect ';' after value.");
            return new PrintStatement(expression);
        }
        
        Statement ExpressionStatement() {
            var expr = Expression();
            Consume(SEMICOLON, "Expect ';' after expression.");
            return new ExpressionStatement(expr);
        }
    }
    
    
    private Expression Equality()
    {
        var expr = Comparison();

        while (Match(BANG_EQUAL, EQUAL_EQUAL))
        {
            var @operator = Previous();
            var right = Comparison();
            expr = new BinaryExpression(expr, @operator, right);
        }

        return expr;
    }

    private Expression Comparison()
    {
        var expr = Term();

        while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
        {
            var @operator = Previous();
            var right = Term();
            expr = new BinaryExpression(expr, @operator, right);
        }

        return expr;
    }

    private Expression Term()
    {
        var expr = Factor();

        while (Match(MINUS, PLUS))
        {
            var @operator = Previous();
            var right = Factor();
            expr = new BinaryExpression(expr, @operator, right);
        }

        return expr;
    }

    private Expression Factor()
    {
        var expr = unary();

        while (Match(SLASH, STAR))
        {
            var @operator = Previous();
            var right = unary();
            expr = new BinaryExpression(expr, @operator, right);
        }

        return expr;
    }

    private Expression unary()
    {
        if (Match(BANG, MINUS))
        {
            var @operator = Previous();
            var right = unary();
            return new Unary(@operator, right);
        }

        return Primary();
    }

    private Expression Primary()
    {
        if (Match(FALSE)) return new Literal(false);
        if (Match(TRUE)) return new Literal(true);
        if (Match(NIL)) return new Literal(null);

        if (Match(NUMBER, STRING))
        {
            return new Literal(Previous().Literal);
        }

        if (Match(IDENTIFIER))
        {
            return new VarExpression(Previous());
        }
        if (Match(LEFT_PAREN))
        {
            var expr = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after expression.");
            return new Grouping(expr);
        }

        throw Error(Peek(), "Expect expression.");
    }


    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();

        throw Error(Peek(), message);
    }

    private void Synchronize()
    {
        Advance();
        while (!IsAtEnd())
        {
            if (Previous().TokenType == SEMICOLON) return;

            switch (Peek().TokenType)
            {
                case CLASS:
                case FUN:
                case VAR:
                case FOR:
                case IF:
                case WHILE:
                case PRINT:
                case RETURN:
                    return;
            }

            Advance();
        }
    }

    private ParseError Error(Token token, string message)
    {
        Lox.Error(token, message);
        return new ParseError();
    }

    private bool Match(params TokenType[] types)
    {
        if (types.Any(Check))
        {
            Advance();
            return true;
        }

        return false;
    }

    private class ParseError : Exception
    {
    }


    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().TokenType == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) current++;
        return Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().TokenType == EOF;
    }

    private Token Peek()
    {
        return tokens[current];
    }

    private Token Previous()
    {
        return tokens[current - 1];
    }
}