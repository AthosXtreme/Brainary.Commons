using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Brainary.Commons.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Get an ordered collection of properties from type based on <see cref="DisplayAttribute"/> Order property
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Property collection</returns>
        public static IEnumerable<PropertyInfo> GetSortedProperties(this Type type)
        {
            return type.GetProperties()
                .Select(pi => new
                {
                    pi,
                    da = (DisplayAttribute?)pi.GetCustomAttributes(typeof(DisplayAttribute), false).SingleOrDefault()
                })
                .Select(@t1 => new
                {
                    @t1,
                    order = (@t1.da != null && @t1.da.GetOrder() != null && @t1.da.GetOrder() >= 0) ? @t1.da.Order : int.MaxValue
                })
                .OrderBy(@t1 => @t1.order).Select(@t1 => @t1.@t1.pi);
        }

        /// <summary>
        /// Determine if a type is anonymous
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Boolean</returns>
        public static bool IsAnonymousType(this Type type)
        {
            var hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
            var nameContainsAnonymousType = (type.FullName ?? string.Empty).Contains("AnonymousType");
            var isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;
            return isAnonymousType;
        }

        /// <summary>
        /// Determine if a enum is nullable
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Boolean</returns>
        public static bool IsNullableEnum(this Type type)
        {
            var utype = Nullable.GetUnderlyingType(type);
            return (utype != null) && utype.IsEnum;
        }

        /// <summary>
        /// Determine if a primitive type is nullable
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Boolean</returns>
        public static bool IsNullablePrimitive(this Type type)
        {
            var utype = Nullable.GetUnderlyingType(type);
            return (utype != null) && utype.IsPrimitive;
        }

        /// <summary>
        /// Determine if a value type is nullable
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Boolean</returns>
        public static bool IsNullableValueType(this Type type)
        {
            var utype = Nullable.GetUnderlyingType(type);
            return (utype != null) && utype.IsValueType;
        }

        /// <summary>
        /// Determine if type is subclass of raw generic type
        /// </summary>
        /// <param name="toCheck">Type</param>
        /// <param name="generic">Raw generic type</param>
        /// <returns></returns>
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            Type? test = toCheck; 
            while (test != null && test != typeof(object))
            {
                Type type = test.GetTypeInfo().IsGenericType ? test.GetGenericTypeDefinition() : test;
                if (generic == type) return true;
                test = test.GetTypeInfo().BaseType;
            }
            return false;
        }

        /// <summary>
        /// Get method info from expression
        /// </summary>
        /// <param name="method">Expression method</param>
        /// <returns>Method info</returns>
        public static MethodInfo MethodInfo(this Expression method)
        {
            if (method is not LambdaExpression lambda)
                throw new ArgumentNullException(nameof(method));

            MethodCallExpression? methodExpr = null;
            if (lambda.Body.NodeType == ExpressionType.Call)
                methodExpr = lambda.Body as MethodCallExpression;

            if (methodExpr == null)
                throw new ArgumentNullException(nameof(method));

            return methodExpr.Method;
        }

        /// <summary>
        /// Get custom attributes of specified type from field info at specified target level
        /// </summary>
        /// <typeparam name="T">Attribute type expected</typeparam>
        /// <param name="info">Field info</param>
        /// <param name="level">Target level</param>
        /// <returns>Attribute collection</returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this FieldInfo info, AttributeTargets level)
        {
            return GetCustomAttributes<T>((MemberInfo)info, level);
        }

        /// <summary>
        /// Get custom attributes of specified type from property info at specified target level
        /// </summary>
        /// <typeparam name="T">Attribute type expected</typeparam>
        /// <param name="info">Property info</param>
        /// <param name="level">Target level</param>
        /// <returns>Attribute collection</returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this PropertyInfo info, AttributeTargets level)
        {
            return GetCustomAttributes<T>((MemberInfo)info, level);
        }

        /// <summary>
        /// Get custom attributes of specified type from method info at specified target level
        /// </summary>
        /// <typeparam name="T">Attribute type expected</typeparam>
        /// <param name="info">Method info</param>
        /// <param name="level">Target level</param>
        /// <returns>Attribute collection</returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this MethodInfo info, AttributeTargets level)
        {
            return GetCustomAttributes<T>((MemberInfo)info, level);
        }

        /// <summary>
        /// Obtain a name/value dictionary from public scalar properties of an object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Dictionary</returns>
        public static IDictionary<string, object?> PropertiesToDictionary(this object obj)
        {
            return obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(prop => prop.PropertyType.IsValueType).ToDictionary(k => k.Name, v => v.GetValue(obj, null));
        }

        /// <summary>
        /// Custom attributes evaluator
        /// </summary>
        private static IEnumerable<T> GetCustomAttributes<T>(MemberInfo info, AttributeTargets level)
        {
            var list = new List<T>();
            switch (level)
            {
                case AttributeTargets.Assembly:
                    if (info.DeclaringType != null)
                        list.AddRange(info.DeclaringType.Assembly.GetCustomAttributes(typeof(T), false).Cast<T>());
                    goto case AttributeTargets.Class;
                case AttributeTargets.Class:
                case AttributeTargets.Struct:
                    if (info.DeclaringType != null)
                        list.AddRange(info.DeclaringType.GetCustomAttributes(typeof(T), false).Cast<T>());
                    goto case AttributeTargets.Field;
                case AttributeTargets.Field:
                case AttributeTargets.Method:
                case AttributeTargets.Property:
                    list.AddRange(info.GetCustomAttributes(typeof(T), false).Cast<T>());
                    break;
            }

            return list;
        }
    }
}