using FluentAssertions;

namespace LoxSharp.Tests.InterpreterTests;

public class FunctionsTests
{
    [Fact]
    public void ClosedVariablesShouldBeAccessible()
    {
        const string closure = """
                               fun makeCounter() {
                                   var i = 0;
                                   fun count() {
                                     i = i + 1;
                                     print i;
                                   }
                               
                                   return count;
                               }
                               var counter = makeCounter();
                               counter(); 
                               counter();
                               """;

        Lox.ExecuteLoxCode(closure);
        Lox.CaughtRuntimeException.Should().Be(null);
    }
}