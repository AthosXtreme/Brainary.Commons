using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Brainary.Commons.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Simplify an expresion translating all variables to constant values
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <returns>Simplified expression</returns>
        public static Expression Simplify(this Expression expression)
        {
            var searcher = new ParameterlessExpressionSearcher();
            searcher.Visit(expression);
            return new ParameterlessExpressionEvaluator(searcher.ParameterlessExpressions).Visit(expression);
        }

        /// <summary>
        /// Simplify a typed expresion translating all variables to constant values
        /// </summary>
        /// <typeparam name="T">Expression type</typeparam>
        /// <param name="expression">The typed expression</param>
        /// <returns>Simplified typed expression</returns>
        public static Expression<T> Simplify<T>(this Expression<T> expression)
        {
            return (Expression<T>)Simplify((Expression)expression);
        }

        #region EXPRESSION TRANSLATORS

        private class ParameterlessExpressionSearcher : ExpressionVisitor
        {
            public HashSet<Expression> ParameterlessExpressions { get; } = new HashSet<Expression>();
            private bool containsParameter = false;

            [return: NotNullIfNotNull("node")]
            public override Expression? Visit(Expression? node)
            {
                bool originalContainsParameter = containsParameter;
                containsParameter = false;
                base.Visit(node);
                if (!containsParameter)
                {
                    if (node?.NodeType == ExpressionType.Parameter)
                        containsParameter = true;
                    else
                        ParameterlessExpressions.Add(node!);
                }
                containsParameter |= originalContainsParameter;

                return node;
            }
        }

        private class ParameterlessExpressionEvaluator : ExpressionVisitor
        {
            private readonly HashSet<Expression> parameterlessExpressions;
            public ParameterlessExpressionEvaluator(HashSet<Expression> parameterlessExpressions)
            {
                this.parameterlessExpressions = parameterlessExpressions;
            }

            [return: NotNullIfNotNull("node")]
            public override Expression? Visit(Expression? node)
            {
                if (node != null && parameterlessExpressions.Contains(node))
                    return Evaluate(node);
                else
                    return base.Visit(node);
            }

            private static Expression Evaluate(Expression node)
            {
                if (node.NodeType == ExpressionType.Constant)
                    return node;

                object value = Expression.Lambda(node).Compile().DynamicInvoke()!;
                return Expression.Constant(value, node.Type);
            }
        }

        #endregion
    }
}