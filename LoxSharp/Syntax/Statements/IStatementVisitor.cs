namespace LoxSharp.Syntax.Statements;

public interface IStatementVisitor
{
    void VisitExpressionStatement(ExpressionStatement statement);
    void VisitPrintStatement(PrintStatement statement);
    void VisitVarStatement(VarStatement statement);
    void VisitBlockStatement(BlockStatement statement);
    void VisitIfStatement(IfStatement statement);
    void VisitWhileStatement(WhileStatement whileStatement);
    void VisitFunctionStatement(FunctionStatement functionStatement);
    void VisitReturnStatement(ReturnStatement returnStatement);
    void VisitClassStatement(ClassStatement classStatement);
}