namespace ValueType.Extensions.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;

    public static class ExpressionOperatorExtensions
    {
        public static Expression<Func<TTarget, TResult>> ConvertGenericTypeArgument<TSource, TTarget, TResult>(
            this Expression<Func<TSource, TResult>> root)
        {
            var visitor = new ParameterReplacer(typeof(TSource), typeof(TTarget));
            return visitor.Visit(root) as Expression<Func<TTarget, TResult>>;
        }

        public static LambdaExpression ConvertGenericTypeArgument<TSource, TResult>(
            this Expression<Func<TSource, TResult>> root, Type targetType)
        {
            var visitor = new ParameterReplacer(typeof(TSource), targetType);
            var expression = visitor.Visit(root);
            return expression as LambdaExpression;
        }

        /// <summary>
        /// Combines the first predicate with the second using the logical "and".
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        /// Combines the first predicate with the second using the logical "or".
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>
        /// Negates the predicate.
        /// </summary>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }

        /// <summary>
        /// Composes a new expression from two expressions.
        /// </summary>
        /// <param name="first">First predicate</param>
        /// <param name="second">Second Predicate</param>
        /// <param name="merge">A BinaryExpression to join the predicates.</param>
        public static Expression<TFunc> Compose<TFunc>(this Expression<TFunc> first, Expression<TFunc> second,
            Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)
            var map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with the parameters in the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression
            return Expression.Lambda<TFunc>(merge(first.Body, secondBody), first.Parameters);
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            private readonly IDictionary<ParameterExpression, ParameterExpression> _map;

            private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map,
                Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                if (_map.TryGetValue(p, out var replacement))
                {
                    p = replacement;
                }

                return base.VisitParameter(p);
            }
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private ReadOnlyCollection<ParameterExpression> _parameters;
            private readonly Type _source;
            private readonly Type _target;

            public ParameterReplacer(Type source, Type target)
            {
                _source = source;
                _target = target;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return _parameters?.FirstOrDefault(p => p.Name == node.Name) ??
                       (node.Type == _source ? Expression.Parameter(_target, node.Name) : node);
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                _parameters = VisitAndConvert(node.Parameters, nameof(VisitLambda));
                return Expression.Lambda(Visit(node.Body), _parameters);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                return node.Member.DeclaringType == _source
                    ? Expression.Property(Visit(node.Expression), node.Member.Name)
                    : base.VisitMember(node);
            }
        }
    }
}
