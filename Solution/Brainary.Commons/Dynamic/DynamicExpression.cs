namespace Brainary.Commons.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public static class DynamicExpression
    {
        #region Public Methods and Operators

        public static Type CreateClass(params DynamicProperty[] properties)
        {
            return ClassFactory.Instance.GetDynamicClass(properties);
        }

        public static Type CreateClass(IEnumerable<DynamicProperty> properties)
        {
            return ClassFactory.Instance.GetDynamicClass(properties);
        }

        public static Type CreateClass(
            Type resultType = null, 
            Type baseType = null, 
            params DynamicProperty[] properties)
        {
            return ClassFactory.Instance.GetDynamicClass(properties, resultType, baseType);
        }

        public static Type CreateClass(
            IEnumerable<DynamicProperty> properties, 
            Type resultType = null, 
            Type baseType = null)
        {
            return ClassFactory.Instance.GetDynamicClass(properties, resultType, baseType);
        }

        public static Expression Parse(Type resultType, string expression, params object[] values)
        {
            var parser = new ExpressionParser(null, expression, values);
            return parser.Parse(resultType);
        }

        public static LambdaExpression ParseLambda(
            Type itsType, 
            string expression, 
            Type baseType = null, 
            params object[] values)
        {
            return ParseLambda(
                new[] { Expression.Parameter(itsType, string.Empty) }, 
                null, 
                expression, 
                baseType, 
                values);
        }

        public static LambdaExpression ParseLambda(
            Type itsType, 
            Type resultType, 
            string expression, 
            Type baseType = null, 
            params object[] values)
        {
            return ParseLambda(
                new[] { Expression.Parameter(itsType, string.Empty) }, 
                resultType, 
                expression, 
                baseType, 
                values);
        }

        public static LambdaExpression ParseLambda(
            ParameterExpression[] parameters, 
            Type resultType, 
            string expression, 
            Type baseType = null, 
            params object[] values)
        {
            var parser = new ExpressionParser(parameters, expression, values);
            return Expression.Lambda(parser.Parse(resultType, baseType), parameters);
        }

        public static Expression<Func<T, S>> ParseLambda<T, S>(string expression, params object[] values)
        {
            return (Expression<Func<T, S>>)ParseLambda(typeof(T), typeof(S), expression, null, values);
        }

        #endregion
    }
}