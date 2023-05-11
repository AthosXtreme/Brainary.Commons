using System.Reflection;
using Brainary.Commons.Data.Annotations;
using Brainary.Commons.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brainary.Commons.Data
{
    public static class Extensions
    {
        /// <summary>
        /// Register all public types derived from base type from a given assembly 
        /// </summary>
        /// <typeparam name="T">Entity base type</typeparam>
        /// <param name="modelBuilder">Model builder instance</param>
        /// <param name="fromAssembly">Assembly to look for</param>
        public static void AutoEntity<T>(this ModelBuilder modelBuilder, Assembly fromAssembly)
        {
            var types = fromAssembly.GetExportedTypes().Where(w => w.IsSubclassOf(typeof(T)));
            var entityMethod = modelBuilder.GetType().GetMethod("Entity", 1, Array.Empty<Type>());
            foreach (var t in types)
            {
                var entityGeneric = entityMethod?.MakeGenericMethod(t);
                var entityDelegate = (Func<EntityTypeBuilder>)Delegate.CreateDelegate(typeof(Func<EntityTypeBuilder>), modelBuilder, entityGeneric!);
                var entityTypeBuilder = entityDelegate();

                var options = t.GetTypeInfo().GetCustomAttribute<EntityOptionsAttribute>() ?? new EntityOptionsAttribute();
                if (!options.IdentityId)
                {
                    var propertyMethod = entityTypeBuilder?.GetType().GetMethod("Property", 0, new Type[] { typeof(string) });
                    var propertyDelegate = (Func<string, PropertyBuilder>)Delegate.CreateDelegate(typeof(Func<string, PropertyBuilder>), entityTypeBuilder, propertyMethod!);
                    var propertyBuilder = propertyDelegate("Id");

                    var vgnMethod = propertyBuilder?.GetType().GetMethod("ValueGeneratedNever", 0, Array.Empty<Type>());
                    var vgnDelegate = (Func<PropertyBuilder>)Delegate.CreateDelegate(typeof(Func<PropertyBuilder>), propertyBuilder, vgnMethod!);
                    vgnDelegate();
                }
            }
        }

        /// <summary>
        /// Uses entity name as table name
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        public static void RemovePluralizingTableName(this ModelBuilder modelBuilder)
        {
            var maxLen = modelBuilder.Model.GetMaxIdentifierLength();
            var idCount = 1;
            var types = modelBuilder.Model.GetEntityTypes();
            var strLen = types.Count().ToString().Length;
            var digits = strLen > 2 ? strLen : 2;
            foreach (IMutableEntityType entityType in types)
            {
                var idName = entityType.ShortName();
                idName = idName.Length > maxLen ? $"{idName[..(maxLen - digits)]}{idCount.ToString($"D{digits}")}" : idName;
                entityType.SetTableName(idName);
                idCount++;
            }
        }

        /// <summary>
        /// Add a prefix to table names
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        /// <param name="prefix">The prefix</param>
        public static void AddTableNamePrefix(this ModelBuilder modelBuilder, string prefix)
        {
            var maxLen = modelBuilder.Model.GetMaxIdentifierLength();
            var idCount = 1;
            var types = modelBuilder.Model.GetEntityTypes();
            var strLen = types.Count().ToString().Length;
            var digits = strLen > 2 ? strLen : 2;
            foreach (var entityType in types)
            {
                var idName = $"{prefix}{entityType.GetTableName()}";
                idName = idName.Length > maxLen ? $"{idName[..(maxLen - digits)]}{idCount.ToString($"D{digits}")}" : idName;
                entityType.SetTableName(idName);
                idCount++;
            }
        }

        /// <summary>
        /// Set short PK names
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        public static void UseShortNamePk(this ModelBuilder modelBuilder)
        {
            var maxLen = modelBuilder.Model.GetMaxIdentifierLength();
            var idCount = 1;
            var types = modelBuilder.Model.GetEntityTypes().SelectMany(s => s.GetKeys().Where(w => w.IsPrimaryKey()).Select(e => (s, e)));
            var strLen = types.Count().ToString().Length;
            var digits = strLen > 2 ? strLen : 2;
            foreach (var (entityType, key) in types)
            {
                var idName = $"PK_{entityType.ShortName()}";
                idName = idName.Length > maxLen ? $"{idName[..(maxLen - digits)]}{idCount.ToString($"D{digits}")}" : idName;
                key.SetName(idName);
                idCount++;
            }
        }

        /// <summary>
        /// Set short FK names
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        public static void UseShortNameFk(this ModelBuilder modelBuilder)
        {
            var maxLen = modelBuilder.Model.GetMaxIdentifierLength();
            var idCount = 1;
            var types = modelBuilder.Model.GetEntityTypes().SelectMany(entityType => entityType.GetForeignKeys()
            .GroupBy(k => k.PrincipalEntityType.ShortName(), (k, v) => new { Key = k, Many = v.Count() > 1, Values = v })
            .SelectMany(group => group.Values.Select((s, i) => new { Index = i, Value = s }).Select(fk => (entityType, group, fk))));
            var strLen = types.Count().ToString().Length;
            var digits = strLen > 2 ? strLen : 2;
            foreach (var tuple in types)
            {
                var entityType = tuple.entityType;
                var group = tuple.group;
                var fk = tuple.fk;
                var index = group.Many ? $"_{fk.Index:D2}" : string.Empty;
                var idName = $"FK_{entityType.ShortName()}_{fk.Value.PrincipalEntityType.ShortName()}{index}";
                idName = idName.Length > maxLen ? $"{idName[..(maxLen - digits)]}{idCount.ToString($"D{digits}")}" : idName;
                RelationalForeignKeyExtensions.SetConstraintName(fk.Value, idName);
                idCount++;
            }
        }

        /// <summary>
        /// Set short IX names
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        public static void UseShortNameIx(this ModelBuilder modelBuilder)
        {
            var maxLen = modelBuilder.Model.GetMaxIdentifierLength();
            var idCount = 1;
            var types = modelBuilder.Model.GetEntityTypes().SelectMany(entityType => entityType.GetIndexes().Select((s, i) => (entityType, new { Index = i, Value = s })));
            var strLen = types.Count().ToString().Length;
            var digits = strLen > 2 ? strLen : 2;
            foreach (var (entityType, ix) in types)
            {
                var idName = $"IX_{ix.Index + 1:D2}_{entityType.ShortName()}";
                idName = idName.Length > maxLen ? $"{idName[..(maxLen - digits)]}{idCount.ToString($"D{digits}")}" : idName;
                ix.Value.SetDatabaseName(idName);
                idCount++;
            }
        }

        /// <summary>
        /// Set string value for enums
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        public static void UseStringEnumValues(this ModelBuilder modelBuilder)
        {
            foreach (var (prop, enumType) in modelBuilder.Model.GetEntityTypes().SelectMany(entityType => entityType.GetProperties().Select(s => (s, Nullable.GetUnderlyingType(s.ClrType) ?? s.ClrType))).Where(w => w.Item2.IsEnum))
            {
                var converterType = typeof(EnumToStringConverter<>).MakeGenericType(enumType);
                prop.SetValueConverter((ValueConverter)Activator.CreateInstance(converterType)!);
            }
        }

        /// <summary>
        /// Applies known custom atrtibutes for model
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        public static void ApplyCustomAttributes(this ModelBuilder modelBuilder)
        {
            foreach (var tuple in modelBuilder.Model.GetEntityTypes().SelectMany(entityType => entityType.GetProperties().Select(prop => (entityType, prop))))
            {
                var entityType = tuple.entityType;
                var prop = tuple.prop;
                var memberInfo = (MemberInfo?)prop.PropertyInfo ?? prop.FieldInfo;
                if (memberInfo != null)
                {
                    var customAttributes = Attribute.GetCustomAttributes(memberInfo, inherit: false);

                    var computedColumnSqlAttribute = customAttributes.OfType<ComputedColumnSqlAttribute>().FirstOrDefault();
                    if (computedColumnSqlAttribute != null)
                        RelationalPropertyExtensions.SetComputedColumnSql(prop, computedColumnSqlAttribute.Statement);

                    var defaultValueSqlAttribute = customAttributes.OfType<DefaultValueSqlAttribute>().FirstOrDefault();
                    if (defaultValueSqlAttribute != null)
                        RelationalPropertyExtensions.SetDefaultValueSql(prop, defaultValueSqlAttribute.Statement);
                }
            }
        }
    }
}