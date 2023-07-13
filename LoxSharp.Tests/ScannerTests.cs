using FluentAssertions;

namespace LoxSharp.Tests;

public class ScannerTests
{
    [Fact]
    public void ForValidStringLiteral_StringTokenShouldBeCreated()
    {
        var scanner = new Scanner("\"Hello World\"");

        var tokens = scanner.GetTokens();

        tokens.Should().HaveCount(1);

        var stringToken = tokens.First();

        stringToken.TokenType.Should().Be(TokenType.STRING);
        stringToken.Literal.Should().Be("Hello World");
    }
    
    [Fact]
    public void ForValidNumberLiteral_NumberTokenShouldBeCreated()
    {
        var scanner = new Scanner("1234");

        var tokens = scanner.GetTokens();

        tokens.Should().HaveCount(1);

        var stringToken = tokens.First();

        stringToken.TokenType.Should().Be(TokenType.NUMBER);
        stringToken.Literal.Should().Be(1234);
    }
}