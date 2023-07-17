using FluentAssertions;
using LoxSharp.Expressions;

namespace LoxSharp.Tests;

public class ASTPrinterTests
{
    [Fact]
    public void BinaryExpressionShouldBePrinted()
    {
        var astPrinter = new ASTPrinter();

        var binaryExpression = new BinaryExpression(
            new Literal(15),
            new Token(TokenType.PLUS, "+", null, 1),
            new Literal(20)
        );


        var print = astPrinter.Print(binaryExpression);

        print.Should().Be("(+ 15 20)");
    }
}