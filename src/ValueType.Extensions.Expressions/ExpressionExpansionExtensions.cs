namespace ValueType.Extensions.Expressions
{
    using System;
    using System.Linq.Expressions;

    public static class ExpressionExpansionExtensions
    {
        public static Expression<TFunc> Expand<TFunc>(this Expression<TFunc> expr)
        {
            return new ReduceFinder().Visit(expr) as Expression<TFunc>;
        }

        private class ReduceFinder : ExpressionVisitor
        {
            public override Expression Visit(Expression node)
            {
                if (node != null && node.CanReduce)
                {
                    var reduced = node.Reduce();
                    Console.WriteLine("Found expression to reduce!");
                    Console.WriteLine("Before: {0}: {1}", node.GetType().Name, node);
                    Console.WriteLine("After: {0}: {1}", reduced.GetType().Name, reduced);
                }
                return base.Visit(node);
            }
        }
    }
}
