namespace Brainary.Commons.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class ExpressionParser
    {
        private const string KeywordIt = "it";

        private const string KeywordIif = "iif";

        private const string KeywordNew = "new";

        private static readonly Type[] PredefinedTypes = 
                        {
                            typeof(object), typeof(bool), typeof(char), typeof(string), typeof(sbyte), 
                            typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), 
                            typeof(ulong), typeof(float), typeof(double), typeof(decimal), typeof(DateTime), 
                            typeof(TimeSpan), typeof(Guid), typeof(Math), typeof(Convert)
                        };

        private static readonly Expression TrueLiteral = Expression.Constant(true);

        private static readonly Expression FalseLiteral = Expression.Constant(false);

        private static readonly Expression NullLiteral = Expression.Constant(null);

        private static Dictionary<string, object> keywords;

        private readonly Dictionary<string, object> symbols;

        private readonly Dictionary<Expression, string> literals;

        private readonly string text;

        private readonly int textLen;

        private IDictionary<string, object> externals;

        private ParameterExpression it;

        private int textPos;

        private char ch;

        private Token token;

        private Type interfaceType;

        private Type baseType;

        public ExpressionParser(ParameterExpression[] parameters, string expression, object[] values)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (keywords == null)
            {
                keywords = CreateKeywords();
            }

            symbols = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            literals = new Dictionary<Expression, string>();
            if (parameters != null)
            {
                ProcessParameters(parameters);
            }

            if (values != null)
            {
                ProcessValues(values);
            }

            text = expression;
            textLen = text.Length;
            SetTextPos(0);
            NextToken();
        }

        private enum TokenId
        {
            Unknown, 

            End, 

            Identifier, 

            StringLiteral, 

            IntegerLiteral, 

            RealLiteral, 

            Exclamation, 

            Percent, 

            Amphersand, 

            OpenParen, 

            CloseParen, 

            Asterisk, 

            Plus, 

            Comma, 

            Minus, 

            Dot, 

            Slash, 

            Colon, 

            LessThan, 

            Equal, 

            GreaterThan, 

            Question, 

            OpenBracket, 

            CloseBracket, 

            Bar, 

            ExclamationEqual, 

            DoubleAmphersand, 

            LessThanEqual, 

            LessGreater, 

            DoubleEqual, 

            GreaterThanEqual, 

            DoubleBar
        }

        private interface ILogicalSignatures
        {
            #region Public Methods and Operators

            void F(bool x, bool y);

            void F(bool? x, bool? y);

            #endregion
        }

        private interface IArithmeticSignatures
        {
            #region Public Methods and Operators

            void F(int x, int y);

            void F(uint x, uint y);

            void F(long x, long y);

            void F(ulong x, ulong y);

            void F(float x, float y);

            void F(double x, double y);

            void F(decimal x, decimal y);

            void F(int? x, int? y);

            void F(uint? x, uint? y);

            void F(long? x, long? y);

            void F(ulong? x, ulong? y);

            void F(float? x, float? y);

            void F(double? x, double? y);

            void F(decimal? x, decimal? y);

            #endregion
        }

        private interface IRelationalSignatures : IArithmeticSignatures
        {
            #region Public Methods and Operators

            void F(string x, string y);

            void F(char x, char y);

            void F(DateTime x, DateTime y);

            void F(DateTimeOffset x, DateTimeOffset y);

            void F(TimeSpan x, TimeSpan y);

            void F(char? x, char? y);

            void F(DateTime? x, DateTime? y);

            void F(DateTimeOffset? x, DateTimeOffset? y);

            void F(TimeSpan? x, TimeSpan? y);

            #endregion
        }

        private interface IEqualitySignatures : IRelationalSignatures
        {
            #region Public Methods and Operators

            void F(bool x, bool y);

            void F(bool? x, bool? y);

            void F(Guid x, Guid y);

            void F(Guid? x, Guid? y);

            #endregion
        }

        private interface IAddSignatures : IArithmeticSignatures
        {
            #region Public Methods and Operators

            void F(DateTime x, TimeSpan y);

            void F(TimeSpan x, TimeSpan y);

            void F(DateTime? x, TimeSpan? y);

            void F(TimeSpan? x, TimeSpan? y);

            #endregion
        }

        private interface ISubtractSignatures : IAddSignatures
        {
            #region Public Methods and Operators

            void F(DateTime x, DateTime y);

            void F(DateTime? x, DateTime? y);

            #endregion
        }

        private interface INegationSignatures
        {
            #region Public Methods and Operators

            void F(int x);

            void F(long x);

            void F(float x);

            void F(double x);

            void F(decimal x);

            void F(int? x);

            void F(long? x);

            void F(float? x);

            void F(double? x);

            void F(decimal? x);

            #endregion
        }

        private interface INotSignatures
        {
            #region Public Methods and Operators

            void F(bool x);

            void F(bool? x);

            #endregion
        }

        private interface IEnumerableSignatures
        {
            #region Public Methods and Operators

            void All(bool predicate);

            void Any();

            void Any(bool predicate);

            void Average(int selector);

            void Average(int? selector);

            void Average(long selector);

            void Average(long? selector);

            void Average(float selector);

            void Average(float? selector);

            void Average(double selector);

            void Average(double? selector);

            void Average(decimal selector);

            void Average(decimal? selector);

            void Count();

            void Count(bool predicate);

            void Max(object selector);

            void Min(object selector);

            void Sum(int selector);

            void Sum(int? selector);

            void Sum(long selector);

            void Sum(long? selector);

            void Sum(float selector);

            void Sum(float? selector);

            void Sum(double selector);

            void Sum(double? selector);

            void Sum(decimal selector);

            void Sum(decimal? selector);

            void Where(bool predicate);

            #endregion
        }

        public static object ParseNumber(string text, TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.SByte:
                    sbyte sb;
                    if (sbyte.TryParse(text, out sb))
                    {
                        return sb;
                    }

                    break;
                case TypeCode.Byte:
                    byte b;
                    if (byte.TryParse(text, out b))
                    {
                        return b;
                    }

                    break;
                case TypeCode.Int16:
                    short s;
                    if (short.TryParse(text, out s))
                    {
                        return s;
                    }

                    break;
                case TypeCode.UInt16:
                    ushort us;
                    if (ushort.TryParse(text, out us))
                    {
                        return us;
                    }

                    break;
                case TypeCode.Int32:
                    int i;
                    if (int.TryParse(text, out i))
                    {
                        return i;
                    }

                    break;
                case TypeCode.UInt32:
                    uint ui;
                    if (uint.TryParse(text, out ui))
                    {
                        return ui;
                    }

                    break;
                case TypeCode.Int64:
                    long l;
                    if (long.TryParse(text, out l))
                    {
                        return l;
                    }

                    break;
                case TypeCode.UInt64:
                    ulong ul;
                    if (ulong.TryParse(text, out ul))
                    {
                        return ul;
                    }

                    break;
                case TypeCode.Single:
                    float f;
                    if (float.TryParse(text, out f))
                    {
                        return f;
                    }

                    break;
                case TypeCode.Double:
                    double d;
                    if (double.TryParse(text, out d))
                    {
                        return d;
                    }

                    break;
                case TypeCode.Decimal:
                    decimal e;
                    if (decimal.TryParse(text, out e))
                    {
                        return e;
                    }

                    break;
            }

            return null;
        }

        public static object ParseNumber(string text, Type type)
        {
            return ParseNumber(text, Type.GetTypeCode(GetNonNullableType(type)));
        }

        public Expression Parse(Type resultType, Type baseType = null)
        {
            // Update base resultType
            interfaceType = resultType;
            this.baseType = baseType;

            int exprPos = token.Pos;
            Expression expr = ParseExpression(resultType);
            if (resultType != null)
            {
                if ((expr = PromoteExpression(expr, resultType, true)) == null)
                {
                    throw ParseError(exprPos, Messages.ExpressionTypeMismatch, GetTypeName(resultType));
                }
            }

            ValidateToken(TokenId.End, Messages.SyntaxError);
            return expr;
        }

#pragma warning disable 0219

        public IEnumerable<DynamicOrdering> ParseOrdering()
        {
            var orderings = new List<DynamicOrdering>();
            while (true)
            {
                Expression expr = ParseExpression();
                bool ascending = true;
                if (TokenIdentifierIs("asc") || TokenIdentifierIs("ascending"))
                {
                    NextToken();
                }
                else if (TokenIdentifierIs("desc") || TokenIdentifierIs("descending"))
                {
                    NextToken();
                    ascending = false;
                }

                orderings.Add(new DynamicOrdering { Selector = expr, Ascending = ascending });
                if (token.Id != TokenId.Comma)
                {
                    break;
                }

                NextToken();
            }

            ValidateToken(TokenId.End, Messages.SyntaxError);
            return orderings;
        }

#pragma warning restore 0219

        private static Type FindGenericType(Type generic, Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == generic)
                {
                    return type;
                }

                if (generic.IsInterface)
                {
                    foreach (Type found in type.GetInterfaces().Select(intfType => FindGenericType(generic, intfType)).Where(found => found != null))
                    {
                        return found;
                    }
                }

                type = type.BaseType;
            }

            return null;
        }

        private static bool IsPredefinedType(Type type)
        {
            return PredefinedTypes.Any(t => t == type);
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static Type GetNonNullableType(Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        private static string GetTypeName(Type type)
        {
            Type baseType = GetNonNullableType(type);
            string s = baseType.Name;
            if (type != baseType)
            {
                s += '?';
            }

            return s;
        }

        private static bool IsNumericType(Type type)
        {
            return GetNumericTypeKind(type) != 0;
        }

        private static bool IsSignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 2;
        }

        private static bool IsUnsignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 3;
        }

        private static int GetNumericTypeKind(Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum)
            {
                return 0;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return 1;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return 2;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return 3;
                default:
                    return 0;
            }
        }

        private static bool IsEnumType(Type type)
        {
            return GetNonNullableType(type).IsEnum;
        }

        private static IEnumerable<Type> SelfAndBaseTypes(Type type)
        {
            if (type.IsInterface)
            {
                var types = new List<Type>();
                AddInterface(types, type);
                return types;
            }

            return SelfAndBaseClasses(type);
        }

        private static IEnumerable<Type> SelfAndBaseClasses(Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        private static void AddInterface(List<Type> types, Type type)
        {
            if (!types.Contains(type))
            {
                types.Add(type);
                foreach (Type t in type.GetInterfaces())
                {
                    AddInterface(types, t);
                }
            }
        }

        private static Dictionary<string, object> CreateKeywords()
        {
            var d = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                        {
                            { "true", TrueLiteral },
                            { "false", FalseLiteral },
                            { "null", NullLiteral },
                            { KeywordIt, KeywordIt },
                            { KeywordIif, KeywordIif },
                            { KeywordNew, KeywordNew }
                        };
            foreach (Type type in PredefinedTypes)
            {
                d.Add(type.Name, type);
            }

            return d;
        }

        private static object ParseEnum(string name, Type type)
        {
            if (type.IsEnum)
            {
                MemberInfo[] memberInfos = type.FindMembers(
                    MemberTypes.Field,
                    BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static,
                    Type.FilterNameIgnoreCase,
                    name);
                if (memberInfos.Length != 0)
                {
                    return ((FieldInfo)memberInfos[0]).GetValue(null);
                }
            }

            return null;
        }

        private static bool IsCompatibleWith(Type source, Type target)
        {
            if (source == target)
            {
                return true;
            }

            if (!target.IsValueType)
            {
                return target.IsAssignableFrom(source);
            }

            Type st = GetNonNullableType(source);
            Type tt = GetNonNullableType(target);
            if (st != source && tt == target)
            {
                return false;
            }

            TypeCode sc = st.IsEnum ? TypeCode.Object : Type.GetTypeCode(st);
            TypeCode tc = tt.IsEnum ? TypeCode.Object : Type.GetTypeCode(tt);
            switch (sc)
            {
                case TypeCode.SByte:
                    switch (tc)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Byte:
                    switch (tc)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Int16:
                    switch (tc)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.UInt16:
                    switch (tc)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Int32:
                    switch (tc)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.UInt32:
                    switch (tc)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Int64:
                    switch (tc)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.UInt64:
                    switch (tc)
                    {
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Single:
                    switch (tc)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }

                    break;
                default:
                    if (st == tt)
                    {
                        return true;
                    }

                    break;
            }

            return false;
        }

        private static bool IsBetterThan(Expression[] args, MethodData m1, MethodData m2)
        {
            bool better = false;
            for (int i = 0; i < args.Length; i++)
            {
                int c = CompareConversions(args[i].Type, m1.Parameters[i].ParameterType, m2.Parameters[i].ParameterType);
                if (c < 0)
                {
                    return false;
                }

                if (c > 0)
                {
                    better = true;
                }
            }

            return better;
        }

        // Return 1 if s -> t1 is a better conversion than s -> t2
        // Return -1 if s -> t2 is a better conversion than s -> t1
        // Return 0 if neither conversion is better
        private static int CompareConversions(Type s, Type t1, Type t2)
        {
            if (t1 == t2)
            {
                return 0;
            }

            if (s == t1)
            {
                return 1;
            }

            if (s == t2)
            {
                return -1;
            }

            bool t1t2 = IsCompatibleWith(t1, t2);
            bool t2t1 = IsCompatibleWith(t2, t1);
            if (t1t2 && !t2t1)
            {
                return 1;
            }

            if (t2t1 && !t1t2)
            {
                return -1;
            }

            if (IsSignedIntegralType(t1) && IsUnsignedIntegralType(t2))
            {
                return 1;
            }

            if (IsSignedIntegralType(t2) && IsUnsignedIntegralType(t1))
            {
                return -1;
            }

            return 0;
        }

        private void ProcessParameters(ParameterExpression[] parameters)
        {
            foreach (ParameterExpression pe in parameters.Where(pe => !string.IsNullOrEmpty(pe.Name)))
            {
                AddSymbol(pe.Name, pe);
            }

            if (parameters.Length == 1 && string.IsNullOrEmpty(parameters[0].Name))
            {
                it = parameters[0];
            }
        }

        private void ProcessValues(object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                object value = values[i];
                if (i == values.Length - 1 && value is IDictionary<string, object>)
                {
                    externals = (IDictionary<string, object>)value;
                }
                else
                {
                    AddSymbol("@" + i.ToString(CultureInfo.InvariantCulture), value);
                }
            }
        }

        private void AddSymbol(string name, object value)
        {
            if (symbols.ContainsKey(name))
            {
                throw ParseError(Messages.DuplicateIdentifier, name);
            }

            symbols.Add(name, value);
        }

        // ?: operator
        private Expression ParseExpression(Type resultType = null)
        {
            int errorPos = token.Pos;
            Expression expr = ParseLogicalOr(resultType);
            if (token.Id == TokenId.Question)
            {
                NextToken();
                Expression expr1 = ParseExpression(resultType);
                ValidateToken(TokenId.Colon, Messages.ColonExpected);
                NextToken();
                Expression expr2 = ParseExpression(resultType);
                expr = GenerateConditional(expr, expr1, expr2, errorPos);
            }

            return expr;
        }

        // ||, or operator
        private Expression ParseLogicalOr(Type resultType = null)
        {
            Expression left = ParseLogicalAnd(resultType);
            while (token.Id == TokenId.DoubleBar || TokenIdentifierIs("or"))
            {
                Token op = token;
                NextToken();
                Expression right = ParseLogicalAnd(resultType);
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.Text, ref left, ref right, op.Pos);
                left = Expression.OrElse(left, right);
            }

            return left;
        }

        // &&, and operator
        private Expression ParseLogicalAnd(Type resultType = null)
        {
            Expression left = ParseComparison(resultType);
            while (token.Id == TokenId.DoubleAmphersand || TokenIdentifierIs("and"))
            {
                Token op = token;
                NextToken();
                Expression right = ParseComparison(resultType);
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.Text, ref left, ref right, op.Pos);
                left = Expression.AndAlso(left, right);
            }

            return left;
        }

        // =, ==, !=, <>, >, >=, <, <= operators
        private Expression ParseComparison(Type resultType = null)
        {
            Expression left = ParseAdditive(resultType);
            while (token.Id == TokenId.Equal || token.Id == TokenId.DoubleEqual || token.Id == TokenId.ExclamationEqual
                   || token.Id == TokenId.LessGreater || token.Id == TokenId.GreaterThan
                   || token.Id == TokenId.GreaterThanEqual || token.Id == TokenId.LessThan
                   || token.Id == TokenId.LessThanEqual)
            {
                Token op = token;
                NextToken();
                Expression right = ParseAdditive(resultType);
                bool isEquality = op.Id == TokenId.Equal || op.Id == TokenId.DoubleEqual
                                  || op.Id == TokenId.ExclamationEqual || op.Id == TokenId.LessGreater;
                if (isEquality && !left.Type.IsValueType && !right.Type.IsValueType)
                {
                    if (left.Type != right.Type)
                    {
                        if (left.Type.IsAssignableFrom(right.Type))
                        {
                            right = Expression.Convert(right, left.Type);
                        }
                        else if (right.Type.IsAssignableFrom(left.Type))
                        {
                            left = Expression.Convert(left, right.Type);
                        }
                        else
                        {
                            throw IncompatibleOperandsError(op.Text, left, right, op.Pos);
                        }
                    }
                }
                else if (IsEnumType(left.Type) || IsEnumType(right.Type))
                {
                    if (left.Type != right.Type)
                    {
                        Expression e;
                        if ((e = PromoteExpression(right, left.Type, true)) != null)
                        {
                            right = e;
                        }
                        else if ((e = PromoteExpression(left, right.Type, true)) != null)
                        {
                            left = e;
                        }
                        else
                        {
                            throw IncompatibleOperandsError(op.Text, left, right, op.Pos);
                        }
                    }
                }
                else
                {
                    CheckAndPromoteOperands(
                        isEquality ? typeof(IEqualitySignatures) : typeof(IRelationalSignatures), 
                        op.Text, 
                        ref left, 
                        ref right, 
                        op.Pos);
                }

                switch (op.Id)
                {
                    case TokenId.Equal:
                    case TokenId.DoubleEqual:
                        left = GenerateEqual(left, right);
                        break;
                    case TokenId.ExclamationEqual:
                    case TokenId.LessGreater:
                        left = GenerateNotEqual(left, right);
                        break;
                    case TokenId.GreaterThan:
                        left = GenerateGreaterThan(left, right);
                        break;
                    case TokenId.GreaterThanEqual:
                        left = GenerateGreaterThanEqual(left, right);
                        break;
                    case TokenId.LessThan:
                        left = GenerateLessThan(left, right);
                        break;
                    case TokenId.LessThanEqual:
                        left = GenerateLessThanEqual(left, right);
                        break;
                }
            }

            return left;
        }

        // +, -, & operators
        private Expression ParseAdditive(Type resultType = null)
        {
            Expression left = ParseMultiplicative(resultType);
            while (token.Id == TokenId.Plus || token.Id == TokenId.Minus || token.Id == TokenId.Amphersand)
            {
                Token op = token;
                NextToken();
                Expression right = ParseMultiplicative(resultType);
                switch (op.Id)
                {
                    case TokenId.Plus:
                        if (left.Type == typeof(string) || right.Type == typeof(string))
                        {
                            goto case TokenId.Amphersand;
                        }

                        CheckAndPromoteOperands(typeof(IAddSignatures), op.Text, ref left, ref right, op.Pos);
                        left = GenerateAdd(left, right);
                        break;
                    case TokenId.Minus:
                        CheckAndPromoteOperands(typeof(ISubtractSignatures), op.Text, ref left, ref right, op.Pos);
                        left = GenerateSubtract(left, right);
                        break;
                    case TokenId.Amphersand:
                        left = GenerateStringConcat(left, right);
                        break;
                }
            }

            return left;
        }

        // *, /, %, mod operators
        private Expression ParseMultiplicative(Type resultType = null)
        {
            Expression left = ParseUnary(resultType);
            while (token.Id == TokenId.Asterisk || token.Id == TokenId.Slash || token.Id == TokenId.Percent
                   || TokenIdentifierIs("mod"))
            {
                Token op = token;
                NextToken();
                Expression right = ParseUnary(resultType);
                CheckAndPromoteOperands(typeof(IArithmeticSignatures), op.Text, ref left, ref right, op.Pos);
                switch (op.Id)
                {
                    case TokenId.Asterisk:
                        left = Expression.Multiply(left, right);
                        break;
                    case TokenId.Slash:
                        left = Expression.Divide(left, right);
                        break;
                    case TokenId.Percent:
                    case TokenId.Identifier:
                        left = Expression.Modulo(left, right);
                        break;
                }
            }

            return left;
        }

        // -, !, not unary operators
        private Expression ParseUnary(Type resultType = null)
        {
            if (token.Id == TokenId.Minus || token.Id == TokenId.Exclamation || TokenIdentifierIs("not"))
            {
                Token op = token;
                NextToken();
                if (op.Id == TokenId.Minus && (token.Id == TokenId.IntegerLiteral || token.Id == TokenId.RealLiteral))
                {
                    token.Text = "-" + token.Text;
                    token.Pos = op.Pos;
                    return ParsePrimary(resultType);
                }

                Expression expr = ParseUnary(resultType);
                if (op.Id == TokenId.Minus)
                {
                    CheckAndPromoteOperand(typeof(INegationSignatures), op.Text, ref expr, op.Pos);
                    expr = Expression.Negate(expr);
                }
                else
                {
                    CheckAndPromoteOperand(typeof(INotSignatures), op.Text, ref expr, op.Pos);
                    expr = Expression.Not(expr);
                }

                return expr;
            }

            return ParsePrimary(resultType);
        }

        private Expression ParsePrimary(Type resultType = null)
        {
            Expression expr = ParsePrimaryStart(resultType);
            while (true)
            {
                if (token.Id == TokenId.Dot)
                {
                    NextToken();
                    expr = ParseMemberAccess(null, expr);
                }
                else if (token.Id == TokenId.OpenBracket)
                {
                    expr = ParseElementAccess(expr);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private Expression ParsePrimaryStart(Type resultType = null)
        {
            switch (token.Id)
            {
                case TokenId.Identifier:
                    return ParseIdentifier(resultType);
                case TokenId.StringLiteral:
                    return ParseStringLiteral(resultType);
                case TokenId.IntegerLiteral:
                    return ParseIntegerLiteral(resultType);
                case TokenId.RealLiteral:
                    return ParseRealLiteral(resultType);
                case TokenId.OpenParen:
                    return ParseParenExpression(resultType);
                default:
                    throw ParseError(Messages.ExpressionExpected);
            }
        }

        private Expression ParseStringLiteral(Type resultType = null)
        {
            ValidateToken(TokenId.StringLiteral);
            char quote = token.Text[0];
            string s = token.Text.Substring(1, token.Text.Length - 2);
            int start = 0;
            while (true)
            {
                int i = s.IndexOf(quote, start);
                if (i < 0)
                {
                    break;
                }

                s = s.Remove(i, 1);
                start = i + 1;
            }

            if (quote == '\'')
            {
                if (s.Length != 1)
                {
                    throw ParseError(Messages.InvalidCharacterLiteral);
                }

                NextToken();
                return CreateLiteral(s[0], s);
            }

            NextToken();
            return CreateLiteral(s, s);
        }

        private Expression ParseIntegerLiteral(Type resultType = null)
        {
            ValidateToken(TokenId.IntegerLiteral);
            string args = token.Text;
            if (args[0] != '-')
            {
                ulong value;
                if (!ulong.TryParse(args, out value))
                {
                    throw ParseError(Messages.InvalidIntegerLiteral, args);
                }

                NextToken();
                if (value <= int.MaxValue)
                {
                    return CreateLiteral((int)value, args);
                }

                if (value <= uint.MaxValue)
                {
                    return CreateLiteral((uint)value, args);
                }

                if (value <= long.MaxValue)
                {
                    return CreateLiteral((long)value, args);
                }

                return CreateLiteral(value, args);
            }
            else
            {
                long value;
                if (!long.TryParse(args, out value))
                {
                    throw ParseError(Messages.InvalidIntegerLiteral, args);
                }

                NextToken();
                if (value >= int.MinValue && value <= int.MaxValue)
                {
                    return CreateLiteral((int)value, args);
                }

                return CreateLiteral(value, args);
            }
        }

        private Expression ParseRealLiteral(Type resultType = null)
        {
            ValidateToken(TokenId.RealLiteral);
            string args = token.Text;
            object value = null;
            char last = args[args.Length - 1];
            if (last == 'F' || last == 'f')
            {
                float f;
                if (float.TryParse(args.Substring(0, args.Length - 1), out f))
                {
                    value = f;
                }
            }
            else
            {
                double d;
                if (double.TryParse(args, out d))
                {
                    value = d;
                }
            }

            if (value == null)
            {
                throw ParseError(Messages.InvalidRealLiteral, args);
            }

            NextToken();
            return CreateLiteral(value, args);
        }

        private Expression CreateLiteral(object value, string txt)
        {
            ConstantExpression expr = Expression.Constant(value);
            literals.Add(expr, txt);
            return expr;
        }

        private Expression ParseParenExpression(Type resultType = null)
        {
            ValidateToken(TokenId.OpenParen, Messages.OpenParenExpected);
            NextToken();
            Expression e = ParseExpression();
            ValidateToken(TokenId.CloseParen, Messages.CloseParenOrOperatorExpected);
            NextToken();
            return e;
        }

        private Expression ParseIdentifier(Type resultType = null)
        {
            ValidateToken(TokenId.Identifier);
            object value;
            if (keywords.TryGetValue(token.Text, out value))
            {
                if (value is Type)
                {
                    return ParseTypeAccess((Type)value);
                }

                if ((string)value == KeywordIt)
                {
                    return ParseIt(resultType);
                }

                if ((string)value == KeywordIif)
                {
                    return ParseIif(resultType);
                }

                if ((string)value == KeywordNew)
                {
                    return ParseNew(resultType);
                }

                NextToken();
                return (Expression)value;
            }

            if (symbols.TryGetValue(token.Text, out value) || (externals != null && externals.TryGetValue(token.Text, out value)))
            {
                var expr = value as Expression;
                if (expr == null)
                {
                    expr = Expression.Constant(value);
                }
                else
                {
                    var lambda = expr as LambdaExpression;
                    if (lambda != null)
                    {
                        return ParseLambdaInvocation(lambda);
                    }
                }

                NextToken();
                return expr;
            }

            if (it != null)
            {
                return ParseMemberAccess(null, it);
            }

            throw ParseError(Messages.UnknownIdentifier, token.Text);
        }

        private Expression ParseIt(Type resultType = null)
        {
            if (it == null)
            {
                throw ParseError(Messages.NoItInScope);
            }

            NextToken();
            return it;
        }

        private Expression ParseIif(Type resultType = null)
        {
            int errorPos = token.Pos;
            NextToken();
            Expression[] args = ParseArgumentList();
            if (args.Length != 3)
            {
                throw ParseError(errorPos, Messages.IifRequiresThreeArgs);
            }

            return GenerateConditional(args[0], args[1], args[2], errorPos);
        }

        private Expression GenerateConditional(Expression test, Expression expr1, Expression expr2, int errorPos)
        {
            if (test.Type != typeof(bool))
            {
                throw ParseError(errorPos, Messages.FirstExprMustBeBool);
            }

            if (expr1.Type != expr2.Type)
            {
                Expression expr1as2 = expr2 != NullLiteral ? PromoteExpression(expr1, expr2.Type, true) : null;
                Expression expr2as1 = expr1 != NullLiteral ? PromoteExpression(expr2, expr1.Type, true) : null;
                if (expr1as2 != null && expr2as1 == null)
                {
                    expr1 = expr1as2;
                }
                else if (expr2as1 != null && expr1as2 == null)
                {
                    expr2 = expr2as1;
                }
                else
                {
                    string type1 = expr1 != NullLiteral ? expr1.Type.Name : "null";
                    string type2 = expr2 != NullLiteral ? expr2.Type.Name : "null";
                    if (expr1as2 != null && expr2as1 != null)
                    {
                        throw ParseError(errorPos, Messages.BothTypesConvertToOther, type1, type2);
                    }

                    throw ParseError(errorPos, Messages.NeitherTypeConvertsToOther, type1, type2);
                }
            }

            return Expression.Condition(test, expr1, expr2);
        }

        private Expression ParseNew(Type resultType = null)
        {
            NextToken();
            ValidateToken(TokenId.OpenParen, Messages.OpenParenExpected);
            NextToken();
            var properties = new List<DynamicProperty>();
            var expressions = new List<Expression>();
            while (true)
            {
                int exprPos = token.Pos;
                Expression expr = ParseExpression();
                string propName;
                if (TokenIdentifierIs("as"))
                {
                    NextToken();
                    propName = GetIdentifier();
                    NextToken();
                }
                else
                {
                    var me = expr as MemberExpression;
                    if (me == null)
                    {
                        throw ParseError(exprPos, Messages.MissingAsClause);
                    }

                    propName = me.Member.Name;
                }

                expressions.Add(expr);
                properties.Add(new DynamicProperty(propName, expr.Type));
                if (token.Id != TokenId.Comma)
                {
                    break;
                }

                NextToken();
            }

            ValidateToken(TokenId.CloseParen, Messages.CloseParenOrCommaExpected);
            NextToken();

            Type type = interfaceType != null
                            ? DynamicExpression.CreateClass(properties, interfaceType, baseType)
                            : DynamicExpression.CreateClass(properties);

            var bindings = new MemberBinding[properties.Count];
            for (int i = 0; i < bindings.Length; i++)
            {
                bindings[i] = Expression.Bind(type.GetProperty(properties[i].Name), expressions[i]);
            }

            return Expression.MemberInit(Expression.New(type), bindings);
        }

        private Expression ParseLambdaInvocation(LambdaExpression lambda)
        {
            int errorPos = token.Pos;
            NextToken();
            Expression[] args = ParseArgumentList();
            MethodBase method;
            if (FindMethod(lambda.Type, "Invoke", false, args, out method) != 1)
            {
                throw ParseError(errorPos, Messages.ArgsIncompatibleWithLambda);
            }

            return Expression.Invoke(lambda, args);
        }

        private Expression ParseTypeAccess(Type type)
        {
            int errorPos = token.Pos;
            NextToken();
            if (token.Id == TokenId.Question)
            {
                if (!type.IsValueType || IsNullableType(type))
                {
                    throw ParseError(errorPos, Messages.TypeHasNoNullableForm, GetTypeName(type));
                }

                type = typeof(Nullable<>).MakeGenericType(type);
                NextToken();
            }

            if (token.Id == TokenId.OpenParen)
            {
                Expression[] args = ParseArgumentList();
                MethodBase method;
                switch (FindBestMethod(type.GetConstructors(), args, out method))
                {
                    case 0:
                        if (args.Length == 1)
                        {
                            return GenerateConversion(args[0], type, errorPos);
                        }

                        throw ParseError(errorPos, Messages.NoMatchingConstructor, GetTypeName(type));
                    case 1:
                        return Expression.New((ConstructorInfo)method, args);
                    default:
                        throw ParseError(errorPos, Messages.AmbiguousConstructorInvocation, GetTypeName(type));
                }
            }

            ValidateToken(TokenId.Dot, Messages.DotOrOpenParenExpected);
            NextToken();
            return ParseMemberAccess(type, null);
        }

        private Expression GenerateConversion(Expression expr, Type type, int errorPos)
        {
            Type exprType = expr.Type;
            if (exprType == type)
            {
                return expr;
            }

            if (exprType.IsValueType && type.IsValueType)
            {
                if ((IsNullableType(exprType) || IsNullableType(type))
                    && GetNonNullableType(exprType) == GetNonNullableType(type))
                {
                    return Expression.Convert(expr, type);
                }

                if ((IsNumericType(exprType) || IsEnumType(exprType)) && (IsNumericType(type) || IsEnumType(type)))
                {
                    return Expression.ConvertChecked(expr, type);
                }
            }

            if (exprType.IsAssignableFrom(type) || type.IsAssignableFrom(exprType) || exprType.IsInterface
                || type.IsInterface)
            {
                return Expression.Convert(expr, type);
            }

            throw ParseError(errorPos, Messages.CannotConvertValue, GetTypeName(exprType), GetTypeName(type));
        }

        private Expression ParseMemberAccess(Type type, Expression instance)
        {
            if (instance != null)
            {
                type = instance.Type;
            }

            int errorPos = token.Pos;
            string id = GetIdentifier();
            NextToken();
            if (token.Id == TokenId.OpenParen)
            {
                if (instance != null && type != typeof(string))
                {
                    Type enumerableType = FindGenericType(typeof(IEnumerable<>), type);
                    if (enumerableType != null)
                    {
                        Type elementType = enumerableType.GetGenericArguments()[0];
                        return ParseAggregate(instance, elementType, id, errorPos);
                    }
                }

                Expression[] args = ParseArgumentList();
                MethodBase mb;
                switch (FindMethod(type, id, instance == null, args, out mb))
                {
                    case 0:
                        throw ParseError(errorPos, Messages.NoApplicableMethod, id, GetTypeName(type));
                    case 1:
                        var method = (MethodInfo)mb;

                        // HM removed this as it seemed to be breaking stuff
                        ////if (!IsPredefinedType(method.DeclaringType))
                        ////    throw ParseError(errorPos, Messages.MethodsAreInaccessible, GetTypeName(method.DeclaringType));
                        if (method.ReturnType == typeof(void))
                        {
                            throw ParseError(errorPos, Messages.MethodIsVoid, id, GetTypeName(method.DeclaringType));
                        }

                        return Expression.Call(instance, method, args);
                    default:
                        throw ParseError(errorPos, Messages.AmbiguousMethodInvocation, id, GetTypeName(type));
                }
            }

            MemberInfo member = FindPropertyOrField(type, id, instance == null);
            if (member == null)
            {
                throw ParseError(errorPos, Messages.UnknownPropertyOrField, id, GetTypeName(type));
            }

            return member is PropertyInfo
                       ? Expression.Property(instance, (PropertyInfo)member)
                       : Expression.Field(instance, (FieldInfo)member);
        }

        private Expression ParseAggregate(Expression instance, Type elementType, string methodName, int errorPos)
        {
            ParameterExpression outerIt = it;
            ParameterExpression innerIt = Expression.Parameter(elementType, string.Empty);
            it = innerIt;
            Expression[] args = ParseArgumentList();
            it = outerIt;
            MethodBase signature;
            if (FindMethod(typeof(IEnumerableSignatures), methodName, false, args, out signature) != 1)
            {
                throw ParseError(errorPos, Messages.NoApplicableAggregate, methodName);
            }

            Type[] typeArgs;
            if (signature.Name == "Min" || signature.Name == "Max")
            {
                typeArgs = new[] { elementType, args[0].Type };
            }
            else
            {
                typeArgs = new[] { elementType };
            }

            args = args.Length == 0 ? new[] { instance } : new[] { instance, Expression.Lambda(args[0], innerIt) };

            return Expression.Call(typeof(Enumerable), signature.Name, typeArgs, args);
        }

        private Expression[] ParseArgumentList()
        {
            ValidateToken(TokenId.OpenParen, Messages.OpenParenExpected);
            NextToken();
            Expression[] args = token.Id != TokenId.CloseParen ? ParseArguments() : new Expression[0];
            ValidateToken(TokenId.CloseParen, Messages.CloseParenOrCommaExpected);
            NextToken();
            return args;
        }

        private Expression[] ParseArguments()
        {
            var argList = new List<Expression>();
            while (true)
            {
                argList.Add(ParseExpression());
                if (token.Id != TokenId.Comma)
                {
                    break;
                }

                NextToken();
            }

            return argList.ToArray();
        }

        private Expression ParseElementAccess(Expression expr)
        {
            int errorPos = token.Pos;
            ValidateToken(TokenId.OpenBracket, Messages.OpenParenExpected);
            NextToken();
            Expression[] args = ParseArguments();
            ValidateToken(TokenId.CloseBracket, Messages.CloseBracketOrCommaExpected);
            NextToken();
            if (expr.Type.IsArray)
            {
                if (expr.Type.GetArrayRank() != 1 || args.Length != 1)
                {
                    throw ParseError(errorPos, Messages.CannotIndexMultiDimArray);
                }

                Expression index = PromoteExpression(args[0], typeof(int), true);
                if (index == null)
                {
                    throw ParseError(errorPos, Messages.InvalidIndex);
                }

                return Expression.ArrayIndex(expr, index);
            }

            MethodBase mb;
            switch (FindIndexer(expr.Type, args, out mb))
            {
                case 0:
                    throw ParseError(errorPos, Messages.NoApplicableIndexer, GetTypeName(expr.Type));
                case 1:
                    return Expression.Call(expr, (MethodInfo)mb, args);
                default:
                    throw ParseError(errorPos, Messages.AmbiguousIndexerInvocation, GetTypeName(expr.Type));
            }
        }

        private void CheckAndPromoteOperand(Type signatures, string optName, ref Expression expr, int errorPos)
        {
            Expression[] args = { expr };
            MethodBase method;
            if (FindMethod(signatures, "F", false, args, out method) != 1)
            {
                throw ParseError(errorPos, Messages.IncompatibleOperand, optName, GetTypeName(args[0].Type));
            }

            expr = args[0];
        }

        private void CheckAndPromoteOperands(
            Type signatures, 
            string optName, 
            ref Expression left, 
            ref Expression right, 
            int errorPos)
        {
            Expression[] args = { left, right };
            MethodBase method;
            if (FindMethod(signatures, "F", false, args, out method) != 1)
            {
                throw IncompatibleOperandsError(optName, left, right, errorPos);
            }

            left = args[0];
            right = args[1];
        }

        private Exception IncompatibleOperandsError(string optName, Expression left, Expression right, int pos)
        {
            return ParseError(pos, Messages.IncompatibleOperands, optName, GetTypeName(left.Type), GetTypeName(right.Type));
        }

        private MemberInfo FindPropertyOrField(Type type, string memberName, bool staticAccess)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly
                                 | (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            return (from t in SelfAndBaseTypes(type) select t.FindMembers(MemberTypes.Property | MemberTypes.Field, flags, Type.FilterNameIgnoreCase, memberName) into members where members.Length != 0 select members[0]).FirstOrDefault();
        }

        private int FindMethod(
            Type type, 
            string methodName, 
            bool staticAccess, 
            Expression[] args, 
            out MethodBase method)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly
                                 | (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            foreach (Type t in SelfAndBaseTypes(type))
            {
                MemberInfo[] members = t.FindMembers(MemberTypes.Method, flags, Type.FilterNameIgnoreCase, methodName);
                int count = FindBestMethod(members.Cast<MethodBase>(), args, out method);
                if (count != 0)
                {
                    return count;
                }
            }

            method = null;
            return 0;
        }

        private int FindIndexer(Type type, Expression[] args, out MethodBase method)
        {
            foreach (IEnumerable<MethodBase> methods in from t in SelfAndBaseTypes(type) select t.GetDefaultMembers() into members where members.Length != 0 select members.OfType<PropertyInfo>().Select(p => (MethodBase)p.GetGetMethod()).Where(m => m != null))
            {
                int count = FindBestMethod(methods, args, out method);
                if (count != 0)
                {
                    return count;
                }
            }

            method = null;
            return 0;
        }

        private int FindBestMethod(IEnumerable<MethodBase> methods, Expression[] args, out MethodBase method)
        {
            MethodData[] applicable =
                methods.Select(m => new MethodData { MethodBase = m, Parameters = m.GetParameters() })
                    .Where(m => IsApplicable(m, args))
                    .ToArray();
            if (applicable.Length > 1)
            {
                applicable = applicable.Where(m => applicable.All(n => m == n || IsBetterThan(args, m, n))).ToArray();
            }

            if (applicable.Length == 1)
            {
                MethodData md = applicable[0];
                for (int i = 0; i < args.Length; i++)
                {
                    args[i] = md.Args[i];
                }

                method = md.MethodBase;
            }
            else
            {
                method = null;
            }

            return applicable.Length;
        }

        private bool IsApplicable(MethodData method, Expression[] args)
        {
            if (method.Parameters.Length != args.Length)
            {
                return false;
            }

            var promotedArgs = new Expression[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                ParameterInfo pi = method.Parameters[i];
                if (pi.IsOut)
                {
                    return false;
                }

                Expression promoted = PromoteExpression(args[i], pi.ParameterType, false);
                if (promoted == null)
                {
                    return false;
                }

                promotedArgs[i] = promoted;
            }

            method.Args = promotedArgs;
            return true;
        }

        private Expression PromoteExpression(Expression expr, Type type, bool exact)
        {
            if (expr.Type == type)
            {
                return expr;
            }

            if (expr is ConstantExpression)
            {
                var ce = (ConstantExpression)expr;
                if (ce == NullLiteral)
                {
                    if (!type.IsValueType || IsNullableType(type))
                    {
                        return Expression.Constant(null, type);
                    }
                }
                else
                {
                    string txt;
                    if (literals.TryGetValue(ce, out txt))
                    {
                        Type target = GetNonNullableType(type);
                        object value = null;
                        switch (Type.GetTypeCode(ce.Type))
                        {
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                                value = ParseNumber(txt, target);
                                break;
                            case TypeCode.Double:
                                if (target == typeof(decimal))
                                {
                                    value = ParseNumber(txt, target);
                                }

                                break;
                            case TypeCode.String:
                                value = ParseEnum(txt, target);
                                break;
                        }

                        if (value != null)
                        {
                            return Expression.Constant(value, type);
                        }
                    }
                }
            }

            if (IsCompatibleWith(expr.Type, type))
            {
                if (type.IsValueType || exact)
                {
                    return Expression.Convert(expr, type);
                }

                return expr;
            }

            return null;
        }

        private Expression GenerateEqual(Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }

        private Expression GenerateNotEqual(Expression left, Expression right)
        {
            return Expression.NotEqual(left, right);
        }

        private Expression GenerateGreaterThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThan(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }

            return Expression.GreaterThan(left, right);
        }

        private Expression GenerateGreaterThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThanOrEqual(
                    GenerateStaticMethodCall("Compare", left, right), 
                    Expression.Constant(0));
            }

            return Expression.GreaterThanOrEqual(left, right);
        }

        private Expression GenerateLessThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThan(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }

            return Expression.LessThan(left, right);
        }

        private Expression GenerateLessThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThanOrEqual(
                    GenerateStaticMethodCall("Compare", left, right), 
                    Expression.Constant(0));
            }

            return Expression.LessThanOrEqual(left, right);
        }

        private Expression GenerateAdd(Expression left, Expression right)
        {
            if (left.Type == typeof(string) && right.Type == typeof(string))
            {
                return GenerateStaticMethodCall("Concat", left, right);
            }

            return Expression.Add(left, right);
        }

        private Expression GenerateSubtract(Expression left, Expression right)
        {
            return Expression.Subtract(left, right);
        }

        private Expression GenerateStringConcat(Expression left, Expression right)
        {
            return Expression.Call(
                null, 
                typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) }), 
                new[] { left, right });
        }

        private MethodInfo GetStaticMethod(string methodName, Expression left, Expression right)
        {
            return left.Type.GetMethod(methodName, new[] { left.Type, right.Type });
        }

        private Expression GenerateStaticMethodCall(string methodName, Expression left, Expression right)
        {
            return Expression.Call(null, GetStaticMethod(methodName, left, right), new[] { left, right });
        }

        private void SetTextPos(int pos)
        {
            textPos = pos;
            ch = textPos < textLen ? text[textPos] : '\0';
        }

        private void NextChar()
        {
            if (textPos < textLen)
            {
                textPos++;
            }

            ch = textPos < textLen ? text[textPos] : '\0';
        }

        private void NextToken()
        {
            while (char.IsWhiteSpace(ch))
            {
                NextChar();
            }

            TokenId t;
            int tokenPos = textPos;
            switch (ch)
            {
                case '!':
                    NextChar();
                    if (ch == '=')
                    {
                        NextChar();
                        t = TokenId.ExclamationEqual;
                    }
                    else
                    {
                        t = TokenId.Exclamation;
                    }

                    break;
                case '%':
                    NextChar();
                    t = TokenId.Percent;
                    break;
                case '&':
                    NextChar();
                    if (ch == '&')
                    {
                        NextChar();
                        t = TokenId.DoubleAmphersand;
                    }
                    else
                    {
                        t = TokenId.Amphersand;
                    }

                    break;
                case '(':
                    NextChar();
                    t = TokenId.OpenParen;
                    break;
                case ')':
                    NextChar();
                    t = TokenId.CloseParen;
                    break;
                case '*':
                    NextChar();
                    t = TokenId.Asterisk;
                    break;
                case '+':
                    NextChar();
                    t = TokenId.Plus;
                    break;
                case ',':
                    NextChar();
                    t = TokenId.Comma;
                    break;
                case '-':
                    NextChar();
                    t = TokenId.Minus;
                    break;
                case '.':
                    NextChar();
                    t = TokenId.Dot;
                    break;
                case '/':
                    NextChar();
                    t = TokenId.Slash;
                    break;
                case ':':
                    NextChar();
                    t = TokenId.Colon;
                    break;
                case '<':
                    NextChar();
                    if (ch == '=')
                    {
                        NextChar();
                        t = TokenId.LessThanEqual;
                    }
                    else if (ch == '>')
                    {
                        NextChar();
                        t = TokenId.LessGreater;
                    }
                    else
                    {
                        t = TokenId.LessThan;
                    }

                    break;
                case '=':
                    NextChar();
                    if (ch == '=')
                    {
                        NextChar();
                        t = TokenId.DoubleEqual;
                    }
                    else
                    {
                        t = TokenId.Equal;
                    }

                    break;
                case '>':
                    NextChar();
                    if (ch == '=')
                    {
                        NextChar();
                        t = TokenId.GreaterThanEqual;
                    }
                    else
                    {
                        t = TokenId.GreaterThan;
                    }

                    break;
                case '?':
                    NextChar();
                    t = TokenId.Question;
                    break;
                case '[':
                    NextChar();
                    t = TokenId.OpenBracket;
                    break;
                case ']':
                    NextChar();
                    t = TokenId.CloseBracket;
                    break;
                case '|':
                    NextChar();
                    if (ch == '|')
                    {
                        NextChar();
                        t = TokenId.DoubleBar;
                    }
                    else
                    {
                        t = TokenId.Bar;
                    }

                    break;
                case '"':
                case '\'':
                    char quote = ch;
                    do
                    {
                        NextChar();
                        while (textPos < textLen && ch != quote)
                        {
                            NextChar();
                        }

                        if (textPos == textLen)
                        {
                            throw ParseError(textPos, Messages.UnterminatedStringLiteral);
                        }

                        NextChar();
                    }
                    while (ch == quote);
                    t = TokenId.StringLiteral;
                    break;
                default:
                    if (char.IsLetter(ch) || ch == '@' || ch == '_')
                    {
                        do
                        {
                            NextChar();
                        }
                        while (char.IsLetterOrDigit(ch) || ch == '_');
                        t = TokenId.Identifier;
                        break;
                    }

                    if (char.IsDigit(ch))
                    {
                        t = TokenId.IntegerLiteral;
                        do
                        {
                            NextChar();
                        }
                        while (char.IsDigit(ch));
                        if (ch == '.')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            }
                            while (char.IsDigit(ch));
                        }

                        if (ch == 'E' || ch == 'e')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            if (ch == '+' || ch == '-')
                            {
                                NextChar();
                            }

                            ValidateDigit();
                            do
                            {
                                NextChar();
                            }
                            while (char.IsDigit(ch));
                        }

                        if (ch == 'F' || ch == 'f')
                        {
                            NextChar();
                        }

                        break;
                    }

                    if (textPos == textLen)
                    {
                        t = TokenId.End;
                        break;
                    }

                    throw ParseError(textPos, Messages.InvalidCharacter, ch);
            }

            token.Id = t;
            token.Text = text.Substring(tokenPos, textPos - tokenPos);
            token.Pos = tokenPos;
        }

        private bool TokenIdentifierIs(string id)
        {
            return token.Id == TokenId.Identifier && string.Equals(id, token.Text, StringComparison.OrdinalIgnoreCase);
        }

        private string GetIdentifier()
        {
            ValidateToken(TokenId.Identifier, Messages.IdentifierExpected);
            string id = token.Text;
            if (id.Length > 1 && id[0] == '@')
            {
                id = id.Substring(1);
            }

            return id;
        }

        private void ValidateDigit()
        {
            if (!char.IsDigit(ch))
            {
                throw ParseError(textPos, Messages.DigitExpected);
            }
        }

        private void ValidateToken(TokenId t, string errorMessage)
        {
            if (token.Id != t)
            {
                throw ParseError(errorMessage);
            }
        }

        private void ValidateToken(TokenId t)
        {
            if (token.Id != t)
            {
                throw ParseError(Messages.SyntaxError);
            }
        }

        private Exception ParseError(string format, params object[] args)
        {
            return ParseError(token.Pos, format, args);
        }

        private Exception ParseError(int pos, string format, params object[] args)
        {
            return new ParseException(string.Format(CultureInfo.CurrentCulture, format, args), pos);
        }

        private struct Token
        {
            #region Fields

            public TokenId Id;

            public int Pos;

            public string Text;

            #endregion
        }

        private class MethodData
        {
            #region Fields

            public Expression[] Args { get; set; }

            public MethodBase MethodBase { get; set; }

            public ParameterInfo[] Parameters { get; set; }

            #endregion
        }
    }
}