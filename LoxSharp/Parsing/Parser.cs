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


    public IEnumerable<Statement> Parse()
    {
        var statements = new List<Statement>();
        while (!IsAtEnd())
        {
            statements.Add(Declaration());
        }

        return statements;
    }
    private Expression Expression()
    {
        return Assignment();
    }

    private Expression Assignment()
    {
        var expression = Or();
        if (Match(EQUAL))
        {
            var token = Previous();
            var value = Assignment();

            switch (expression)
            {
                case VarExpression varExpression:
                    return new AssignExpression(varExpression.Name, value);
                case GetExpression getExpression:
                    return new SetExpression(getExpression.Object, getExpression.Name, value);
                default:
                    Error(token, "Invalid assignment target");
                    break;
            }
        }

        return expression;
    }

    private Expression Or()
    {
        var expression = And();

        while (Match(OR)) {
            var @operator = Previous();
            var right = And();
            expression = new LogicalExpression(expression, @operator, right);
        }

        return expression;
    }
    
    private Expression And() {
        var expression = Equality();

        while (Match(AND)) {
            var @operator = Previous();
            var right = Equality();
            expression = new LogicalExpression(expression, @operator, right);
        }

        return expression;
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
        if (Match(FUN))
            return Function("function");
        if (Match(PRINT))
            return PrintStatement();
        if (Match(LEFT_BRACE))
            return new BlockStatement(Block());
        if (Match(IF))
            return IfStatement();
        if (Match(WHILE))
            return WhileStatement();
        if (Match(FOR))
            return ForStatement();
        if (Match(RETURN))
            return ReturnStatement();
        if (Match(CLASS))
            return ClassDeclaration();
        return ExpressionStatement();

        PrintStatement PrintStatement()
        {
            var expression = Expression();
            Consume(SEMICOLON, "Expect ';' after value.");
            return new PrintStatement(expression);
        }
    }

    private Statement ClassDeclaration()
    {
        var name = Consume(IDENTIFIER, "Expect class name.");
        Consume(LEFT_BRACE, "Expect '{' before class body.");

        var methods = new List<FunctionStatement>();
        while (!Check(RIGHT_BRACE) && !IsAtEnd()) {
            methods.Add(Function("method"));
        }

        Consume(RIGHT_BRACE, "Expect '}' after class body.");

        return new ClassStatement(name, methods);
    }

    private Statement ReturnStatement()
    {
        var keyword = Previous();
        Expression? value = null;
        if (!Check(SEMICOLON)) {
            value = Expression();
        }

        Consume(SEMICOLON, "Expect ';' after return value.");
        return new ReturnStatement(keyword, value);
    }

    private FunctionStatement Function(string kind)
    {
        var name = Consume(IDENTIFIER, "Expect " + kind + " name.");
        Consume(LEFT_PAREN, "Expect '(' after " + kind + " name.");
        var parameters = new List<Token>();
        if (!Check(RIGHT_PAREN))
        {
            do
            {
                if (parameters.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 parameters.");
                }

                parameters.Add(
                    Consume(IDENTIFIER, "Expect parameter name."));
            } while (Match(COMMA));
        }

        Consume(RIGHT_PAREN, "Expect ')' after parameters.");
        Consume(LEFT_BRACE, "Expect '{' before " + kind + " body.");
        var body = Block();
        return new FunctionStatement(name, parameters, body);
    }

    private Statement ExpressionStatement()
    {
        var expr = Expression();
        Consume(SEMICOLON, "Expect ';' after expression.");
        return new ExpressionStatement(expr);
    }

    private Statement ForStatement()
    {
        Consume(LEFT_PAREN, "Expect '(' after 'for'.");
        
        Statement? initializer = null;
        if (Match(SEMICOLON))
        {
            initializer = null;
        }
        else if (Match(VAR))
        {
            initializer = VarDeclaration();
        }
        else
        {
            initializer = ExpressionStatement();
        }
        
        Expression? condition = null;
        if (!Check(SEMICOLON)) {
            condition = Expression();
        }
        Consume(SEMICOLON, "Expect ';' after loop condition.");
        
        Expression? increment = null;
        if (!Check(RIGHT_PAREN)) {
            increment = Expression();
        }
        Consume(RIGHT_PAREN, "Expect ')' after for clauses.");

        var body = Statement();

        if (increment is not null)
        {
            body = new BlockStatement(new[] { body, new ExpressionStatement(increment) });
        }

        condition ??= new Literal(true);

        body = new WhileStatement(condition, body);

        if (initializer is not null)
        {
            body = new BlockStatement(new[] { initializer, body });
        }

        return body;
    }

    private Statement WhileStatement()
    {
        Consume(LEFT_PAREN, "Expect '(' after 'while'.");
        var condition = Expression();
        Consume(RIGHT_PAREN, "Expect ')' after condition.");
        var body = Statement();

        return new WhileStatement(condition, body);
    }

    private Statement IfStatement()
    {
        Consume(LEFT_PAREN, "Expect '(' after 'if'.");
        var condition = Expression();
        Consume(RIGHT_PAREN, "Expect ')' after if condition."); 

        var thenBranch = Statement();
        Statement? elseBranch = null;
        if (Match(ELSE)) {
            elseBranch = Statement();
        }

        return new IfStatement(condition, thenBranch, elseBranch);
    }
    
    private IEnumerable<Statement> Block() {
        var statements = new List<Statement>();

        while (!Check(RIGHT_BRACE) && !IsAtEnd()) {
            statements.Add(Declaration());
        }

        Consume(RIGHT_BRACE, "Expect '}' after block.");
        return statements;
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
        var expr = Unary();

        while (Match(SLASH, STAR))
        {
            var @operator = Previous();
            var right = Unary();
            expr = new BinaryExpression(expr, @operator, right);
        }

        return expr;
    }

    private Expression Unary()
    {
        if (Match(BANG, MINUS))
        {
            var @operator = Previous();
            var right = Unary();
            return new Unary(@operator, right);
        }

        return Call();
    }

    private Expression Call()
    {
        var expression = Primary();

        while (true)
        {
            if (Match(LEFT_PAREN))
            {
                expression = FinishCall(expression);
            }
            else if(Match(DOT))
            {
                var token = Consume(IDENTIFIER, "Expected property name after '.' .");
                expression = new GetExpression(expression, token);
            }
            else
            {
                break;
            }
        }

        return expression;
    }

    private Expression FinishCall(Expression callee)
    {
        var arguments = new List<Expression>();
        if (!Check(RIGHT_PAREN))
        {
            do
            {
                arguments.Add(Expression());
                if (arguments.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 arguments.");
                }
            } while (Match(COMMA));
        }

        var paren = Consume(RIGHT_PAREN, "Expect ')' after arguments.");
        return new CallExpression(callee, paren, arguments);
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