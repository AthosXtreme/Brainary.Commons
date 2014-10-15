namespace Brainary.Commons.Dynamic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public static partial class Extensions
    {
        #region Public Methods and Operators

        public static bool Any(this IQueryable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return
                (bool)
                source.Provider.Execute(
                    Expression.Call(typeof(Queryable), "Any", new[] { source.ElementType }, source.Expression));
        }

        public static int Count(this IQueryable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return
                (int)
                source.Provider.Execute(
                    Expression.Call(typeof(Queryable), "Count", new[] { source.ElementType }, source.Expression));
        }

        public static IQueryable GroupBy(
            this IQueryable source, 
            string keySelector, 
            string elementSelector, 
            params object[] values)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            if (elementSelector == null)
            {
                throw new ArgumentNullException("elementSelector");
            }

            LambdaExpression keyLambda = DynamicExpression.ParseLambda(
                source.ElementType, 
                null, 
                keySelector, 
                null, 
                values);
            LambdaExpression elementLambda = DynamicExpression.ParseLambda(
                source.ElementType, 
                null, 
                elementSelector, 
                null, 
                values);
            return
                source.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable), 
                        "GroupBy", 
                        new[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type }, 
                        source.Expression, 
                        Expression.Quote(keyLambda), 
                        Expression.Quote(elementLambda)));
        }

        public static IQueryable Join(
            this IQueryable outer, 
            IEnumerable inner, 
            string outerSelector, 
            string innerSelector, 
            string resultsSelector, 
            params object[] values)
        {
            if (inner == null)
            {
                throw new ArgumentNullException("inner");
            }

            if (outerSelector == null)
            {
                throw new ArgumentNullException("outerSelector");
            }

            if (innerSelector == null)
            {
                throw new ArgumentNullException("innerSelector");
            }

            if (resultsSelector == null)
            {
                throw new ArgumentNullException("resultsSelector");
            }

            LambdaExpression outerSelectorLambda = DynamicExpression.ParseLambda(
                outer.ElementType, 
                null, 
                outerSelector, 
                null, 
                values);
            LambdaExpression innerSelectorLambda = DynamicExpression.ParseLambda(
                inner.AsQueryable().ElementType, 
                null, 
                innerSelector, 
                null, 
                values);
            var parameters = new[]
                                 {
                                     Expression.Parameter(outer.ElementType, "outer"), 
                                     Expression.Parameter(inner.AsQueryable().ElementType, "inner")
                                 };
            LambdaExpression resultsSelectorLambda = DynamicExpression.ParseLambda(
                parameters, 
                baseType: null, 
                resultType: null, 
                expression: resultsSelector, 
                values: values);
            return
                outer.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable), 
                        "Join", 
                        new[] { outer.ElementType, inner.AsQueryable().ElementType, outerSelectorLambda.Body.Type,  resultsSelectorLambda.Body.Type }, 
                        outer.Expression, 
                        inner.AsQueryable().Expression, 
                        Expression.Quote(outerSelectorLambda), 
                        Expression.Quote(innerSelectorLambda), 
                        Expression.Quote(resultsSelectorLambda)));
        }
   
        public static IQueryable<T> Join<T>(
            this IQueryable<T> outer, 
            IEnumerable<T> inner, 
            string outerSelector, 
            string innerSelector, 
            string resultsSelector, 
            params object[] values)
        {
            return (IQueryable<T>)Join(outer, (IEnumerable)inner, outerSelector, innerSelector, resultsSelector, values);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] values)
        {
            return (IQueryable<T>)OrderBy((IQueryable)source, ordering, values);
        }

        public static IQueryable OrderBy(this IQueryable source, string ordering, params object[] values)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (ordering == null)
            {
                throw new ArgumentNullException("ordering");
            }

            var parameters = new[] { Expression.Parameter(source.ElementType, string.Empty) };
            var parser = new ExpressionParser(parameters, ordering, values);
            IEnumerable<DynamicOrdering> orderings = parser.ParseOrdering();
            Expression queryExpr = source.Expression;
            string methodAsc = "OrderBy";
            string methodDesc = "OrderByDescending";
            foreach (DynamicOrdering o in orderings)
            {
                queryExpr = Expression.Call(
                    typeof(Queryable), 
                    o.Ascending ? methodAsc : methodDesc, 
                    new[] { source.ElementType, o.Selector.Type }, 
                    queryExpr, 
                    Expression.Quote(Expression.Lambda(o.Selector, parameters)));
                methodAsc = "ThenBy";
                methodDesc = "ThenByDescending";
            }

            return source.Provider.CreateQuery(queryExpr);
        }

        public static IQueryable Select(this IQueryable source, string selector, params object[] values)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            LambdaExpression lambda = DynamicExpression.ParseLambda(source.ElementType, null, selector, null, values);
            return
                source.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable), 
                        "Select", 
                        new[] { source.ElementType, lambda.Body.Type }, 
                        source.Expression, 
                        Expression.Quote(lambda)));
        }

        public static IQueryable Skip(this IQueryable source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return
                source.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable), 
                        "Skip", 
                        new[] { source.ElementType }, 
                        source.Expression, 
                        Expression.Constant(count)));
        }

        public static IQueryable Take(this IQueryable source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return
                source.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable), 
                        "Take", 
                        new[] { source.ElementType }, 
                        source.Expression, 
                        Expression.Constant(count)));
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, params object[] values)
        {
            return (IQueryable<T>)Where((IQueryable)source, predicate, values);
        }

        public static IQueryable Where(this IQueryable source, string predicate, params object[] values)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            LambdaExpression lambda = DynamicExpression.ParseLambda(
                source.ElementType, 
                typeof(bool), 
                predicate, 
                null, 
                values);
            return
                source.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable), 
                        "Where", 
                        new[] { source.ElementType }, 
                        source.Expression, 
                        Expression.Quote(lambda)));
        }

        #endregion
    }
}