using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

public static class Utility
{
    public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
    {
        // build parameter map (from parameters of second to parameters of first)
        var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);


        // replace parameters in the second lambda expression with parameters from the first
        var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);


        // apply composition of lambda expression bodies to parameters from the first expression 

        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }


    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.And);
    }


    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.Or);
    }

    public static Expression RemoveFalseOr(Expression e)
    {
        return (new ParameterRebinder()).Visit(e);
    }
}

public class ParameterRebinder : ExpressionVisitor
{
    private readonly Dictionary<ParameterExpression, ParameterExpression> map;

    public ParameterRebinder()
    {

    }

    public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
    {
        this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
    }

    public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
    {
        return new ParameterRebinder(map).Visit(exp);
    }
    protected override Expression VisitParameter(ParameterExpression p)
    {
        ParameterExpression replacement;
        if (map.TryGetValue(p, out replacement))
        {
            p = replacement;
        }
        return base.VisitParameter(p);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        if (node.NodeType == ExpressionType.Or || node.NodeType == ExpressionType.OrElse)
        {
            if (IsConstantFalse(node.Left))
            {
                // false Or right - just return right directly.
                return this.Visit(node.Right);
            }
            else if (IsConstantFalse(node.Right))
            {
                // left Or false - just return left directly.
                return this.Visit(node.Left);
            }
        }

        // In all other cases just use the expression as is.
        return base.VisitBinary(node);
    }

    private static bool IsConstantFalse(Expression expr)
    {
        return expr.NodeType == ExpressionType.Constant && expr.Type == typeof(bool) && (bool)((ConstantExpression)expr).Value == false;
    }

}
