namespace LoxSharp.Expressions;

public interface IVisitor<out TR>
{
    TR VisitBinaryExpression(BinaryExpression expression);
    TR VisitGroupingExpression(Grouping expression);
    TR VisitLiteralExpression(Literal expression);
    TR VisitUnaryExpression(Unary expression);
    TR VisitVarExpression(VarExpression expression);
    TR VisitAssignExpression(AssignExpression expression);
    TR VisitLogicalExpression(LogicalExpression logicalExpression);
    TR VisitCallExpression(CallExpression callExpression);
    TR VisitGetExpression(GetExpression getExpression);
    TR VisitSetExpression(SetExpression setExpression);
    TR VisitThisExpression(ThisExpression thisExpression);
}