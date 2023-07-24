namespace LoxSharp.Expressions;

public interface IVisitor<out TR>
{
    TR VisitBinaryExpression(BinaryExpression expression);
    TR VisitGroupingExpression(Grouping expression);
    TR VisitLiteralExpression(Literal expression);
    TR VisitUnaryExpression(Unary expression);
    TR VisitVarExpression(VarExpression expression);
    TR VisitAssignExpression(AssignExpression expression);
}