namespace LoxSharp.Syntax.Statements;

public interface IStatementVisitor
{
    void VisitExpressionStatement(ExpressionStatement statement);
    void VisitPrintStatement(PrintStatement statement);
    void VisitVarStatement(VarStatement statement);
    void VisitBlockStatement(BlockStatement statement);
}