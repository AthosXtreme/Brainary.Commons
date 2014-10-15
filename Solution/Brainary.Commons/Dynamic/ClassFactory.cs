namespace Brainary.Commons.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;

    public class ClassFactory : Singleton<ClassFactory>
    {
        #region Fields

        private readonly Dictionary<Signature, Type> classes;

        private readonly ModuleBuilder module;

        private readonly ReaderWriterLock rwrLock;

        private int classCount;

        #endregion

        #region "Singleton Implementation"
        // Deny constructor
        private ClassFactory()
        {
            var name = new AssemblyName("DynamicClasses");
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
#if ENABLE_LINQ_PARTIAL_TRUST
            new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
            try
            {
                module = assembly.DefineDynamicModule("Module");
            }
            finally
            {
#if ENABLE_LINQ_PARTIAL_TRUST
                PermissionSet.RevertAssert();
#endif
            }

            classes = new Dictionary<Signature, Type>();
            rwrLock = new ReaderWriterLock();
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ClassFactory Instance
        {
            get
            {
                if (!Initialised)
                {
                    Init(new ClassFactory());
                }

                return UniqueInstance;
            }
        }

        #endregion

        #region Public Methods and Operators

        public Type GetDynamicClass(IEnumerable<DynamicProperty> properties)
        {
            rwrLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                var signature = new Signature(properties);
                Type type;
                if (!classes.TryGetValue(signature, out type))
                {
                    type = CreateDynamicClass(signature.Properties);
                    classes.Add(signature, type);
                }

                return type;
            }
            finally
            {
                rwrLock.ReleaseReaderLock();
            }
        }

        public Type GetDynamicClass(
            IEnumerable<DynamicProperty> properties, 
            Type resultType = null, 
            Type baseType = null)
        {
            rwrLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                var signature = new Signature(properties);
                Type type;
                if (!classes.TryGetValue(signature, out type))
                {
                    type = resultType == null
                               ? CreateDynamicClass(signature.Properties)
                               : CreateDynamicClass(signature.Properties, resultType, baseType);
                    classes.Add(signature, type);
                }

                return type;
            }
            finally
            {
                rwrLock.ReleaseReaderLock();
            }
        }

        #endregion

        #region Methods

        private static void GenerateEquals(TypeBuilder tb, IEnumerable<FieldInfo> fields)
        {
            MethodBuilder mb = tb.DefineMethod(
                "Equals",
                MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(bool), 
                new[] { typeof(object) });
            ILGenerator gen = mb.GetILGenerator();
            LocalBuilder other = gen.DeclareLocal(tb);
            Label next = gen.DefineLabel();
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Isinst, tb);
            gen.Emit(OpCodes.Stloc, other);
            gen.Emit(OpCodes.Ldloc, other);
            gen.Emit(OpCodes.Brtrue_S, next);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ret);
            gen.MarkLabel(next);

            foreach (FieldInfo field in fields)
            {
                Type ft = field.FieldType;
                Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                next = gen.DefineLabel();
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.Emit(OpCodes.Ldloc, other);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("Equals", new[] { ft, ft }), null);
                gen.Emit(OpCodes.Brtrue_S, next);
                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ret);
                gen.MarkLabel(next);
            }

            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Ret);
        }

        private static void GenerateGetHashCode(TypeBuilder tb, IEnumerable<FieldInfo> fields)
        {
            MethodBuilder mb = tb.DefineMethod(
                "GetHashCode", 
                MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig, 
                typeof(int), 
                Type.EmptyTypes);
            ILGenerator gen = mb.GetILGenerator();
            gen.Emit(OpCodes.Ldc_I4_0);
            foreach (FieldInfo field in fields)
            {
                Type ft = field.FieldType;
                Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("GetHashCode", new[] { ft }), null);
                gen.Emit(OpCodes.Xor);
            }

            gen.Emit(OpCodes.Ret);
        }

        private static FieldInfo[] GenerateProperties(
            TypeBuilder tb, 
            DynamicProperty[] properties, 
            Type returnType = null)
        {
            var fields = new FieldBuilder[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                DynamicProperty dp = properties[i];
                PrintPropertyInfo(dp);
                FieldBuilder fb = tb.DefineField("_" + dp.Name, dp.Type, FieldAttributes.Private);
                PropertyBuilder pb = tb.DefineProperty(dp.Name, PropertyAttributes.HasDefault, dp.Type, null);
                if (dp.CustomAttributeBuilder != null)
                {
                    pb.SetCustomAttribute(dp.CustomAttributeBuilder);
                }

                MethodBuilder mbuGet = tb.DefineMethod(
                    "get_" + dp.Name, 
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual, 
                    dp.Type, 
                    Type.EmptyTypes);
                ILGenerator genGet = mbuGet.GetILGenerator();
                genGet.Emit(OpCodes.Ldarg_0);
                genGet.Emit(OpCodes.Ldfld, fb);
                genGet.Emit(OpCodes.Ret);

                if (returnType != null)
                {
                    MethodInfo returnTypeMi = returnType.GetMethod(mbuGet.Name);
                    if (returnTypeMi != null)
                    {
                        tb.DefineMethodOverride(mbuGet, returnTypeMi);
                    }
                }

                MethodBuilder mbuSet = tb.DefineMethod(
                    "set_" + dp.Name, 
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual, 
                    null, 
                    new[] { dp.Type });
                ILGenerator genSet = mbuSet.GetILGenerator();
                genSet.Emit(OpCodes.Ldarg_0);
                genSet.Emit(OpCodes.Ldarg_1);
                genSet.Emit(OpCodes.Stfld, fb);
                genSet.Emit(OpCodes.Ret);

                if (returnType != null)
                {
                    MethodInfo returnTypeMi = returnType.GetMethod(mbuSet.Name);

                    if (returnTypeMi != null)
                    {
                        tb.DefineMethodOverride(mbuSet, returnTypeMi);
                    }
                }

                pb.SetGetMethod(mbuGet);
                pb.SetSetMethod(mbuSet);

                fields[i] = fb;
            }

            return fields;
        }

        [Conditional("DEBUG")]
        private static void PrintPropertyInfo(DynamicProperty dp)
        {
            Debug.WriteLine("Building Property {0} of Type {1}", dp.Name, dp.Type.Name);
        }

        private Type CreateDynamicClass(DynamicProperty[] properties)
        {
            LockCookie cookie = rwrLock.UpgradeToWriterLock(Timeout.Infinite);
            try
            {
                string typeName = "DynamicClass" + (classCount + 1);
#if ENABLE_LINQ_PARTIAL_TRUST
                new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
                try
                {
                    TypeBuilder tb = module.DefineType(
                        typeName, 
                        TypeAttributes.Class | TypeAttributes.Public, 
                        typeof(object));

                    FieldInfo[] fields = GenerateProperties(tb, properties);
                    GenerateEquals(tb, fields);
                    GenerateGetHashCode(tb, fields);
                    Type result = tb.CreateType();
                    return result;
                }
                finally
                {
#if ENABLE_LINQ_PARTIAL_TRUST
                    PermissionSet.RevertAssert();
#endif
                }
            }
            finally
            {
                classCount++;
                rwrLock.DowngradeFromWriterLock(ref cookie);
            }
        }

        private Type CreateDynamicClass(DynamicProperty[] properties, Type resultType, Type baseType = null)
        {
            LockCookie cookie = rwrLock.UpgradeToWriterLock(Timeout.Infinite);
            try
            {
                string typeName = "DynamicClass" + (classCount + 1);
#if ENABLE_LINQ_PARTIAL_TRUST
                new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
                try
                {
                    TypeBuilder tb = module.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public, baseType ?? typeof(object));

                    tb.AddInterfaceImplementation(resultType);
                    FieldInfo[] fields = GenerateProperties(tb, properties, resultType);

                    GenerateEquals(tb, fields);
                    GenerateGetHashCode(tb, fields);

                    Type result = tb.CreateType();
                    return result;
                }
                finally
                {
#if ENABLE_LINQ_PARTIAL_TRUST
                    PermissionSet.RevertAssert();
#endif
                }
            }
            finally
            {
                classCount++;
                rwrLock.DowngradeFromWriterLock(ref cookie);
            }
        }

        #endregion
    }
}