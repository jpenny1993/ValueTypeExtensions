namespace ValueType.Extensions.Expressions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ExpressionExtensions
    {
        /// <summary>
        /// Compiles and runs the expression.
        /// </summary>
        /// <typeparam name="TEntity">Parameter type</typeparam>
        /// <param name="expression">Expression to run</param>
        /// <param name="arg">Expression parameter</param>
        public static void Execute<TEntity>(this Expression<Action<TEntity>> expression, TEntity arg)
        {
            expression.Compile()(arg);
        }

        /// <summary>
        /// Compiles and runs the expression.
        /// </summary>
        /// <typeparam name="TEntity">Parameter type</typeparam>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="expression">Expression to run</param>
        /// <param name="arg">Expression parameter</param>
        /// <returns>The result of the expression</returns>
        public static TResult Execute<TEntity, TResult>(this Expression<Func<TEntity, TResult>> expression, TEntity arg)
        {
            return expression.Compile()(arg);
        }

        /// <summary>
        /// Gets the property info of the property on the declaring type.
        /// </summary>
        /// <param name="propertyExpression">Expression that selects a property</param>
        /// <returns>The selected property info</returns>
        public static PropertyInfo GetPropertyInfo(Expression propertyExpression)
        {
            if (propertyExpression.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException("Selector must be lambda expression", nameof(propertyExpression));
            }

            var lambda = (LambdaExpression)propertyExpression;

            var memberExpression = ExtractMemberExpression(lambda.Body);

            if (memberExpression == null)
            {
                throw new ArgumentException("Selector must be member access expression", nameof(propertyExpression));
            }

            if (memberExpression.Member.DeclaringType == null)
            {
                throw new InvalidOperationException("Property does not have declaring type");
            }

            return memberExpression.Member.DeclaringType.GetProperty(memberExpression.Member.Name);
        }

        /// <summary>
        /// Gets the property info of the property on the declaring type.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TEntity>(this TEntity obj,
            Expression<Func<TEntity, object>> propertyExpression)
        {
            //Overloaded so allow object specific property access and allow implicit type argument
            return GetPropertyInfo(propertyExpression);
        }

        private static MemberExpression ExtractMemberExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expression);
                case ExpressionType.Convert:
                    var operand = ((UnaryExpression)expression).Operand;
                    return ExtractMemberExpression(operand);
                default:
                    return null;
            }
        }
    }
}
